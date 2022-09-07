using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using InazumaSearch.PluginSDK.V1;
using System;
using System.Collections.Generic;

namespace InazumaSearch.Plugin
{
    public class Manager
    {
        protected NLog.Logger Logger { get; set; }
        protected API Api { get; set; }
        protected IList<IPlugin> Plugins { get; set; }

        public Manager()
        {
            Api = new API();
            Plugins = new List<IPlugin>();
            Logger = NLog.LogManager.GetLogger(LoggerName.PluginManager);
        }

        /// <summary>
        /// プラグインを読み込む
        /// </summary>
        /// <returns>読み込んだプラグイン名(完全修飾名)とバージョンを格納したDictionary</returns>
        /// <remarks>from https://dobon.net/vb/dotnet/programing/plugin.html </remarks>
        public virtual IDictionary<string, int> LoadPlugins()
        {
            //バージョン記録用のDictionary
            var loadedPluginVersionNumbers = new Dictionary<string, int>();

            //IPlugin型の完全修飾名を取得
            var interfaceName = typeof(IPlugin).FullName;

            //プラグインフォルダ内を探索
            var pluginDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plugins");
            foreach (var subDir in Directory.GetDirectories(pluginDir))
            {
                // サブフォルダの中にdllファイルがあったら、プラグインとみなして読み込み
                var dllFiles = Directory.GetFiles(subDir, "*.dll");
                foreach (var dllPath in dllFiles)
                {
                    try
                    {
                        //アセンブリとして読み込む
                        var asm = System.Reflection.Assembly.LoadFrom(dllPath);
                        foreach (var t in asm.GetTypes())
                        {
                            //アセンブリ内のすべての型について、
                            //プラグインとして有効か調べる
                            if (t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(interfaceName) != null)
                            {
                                //プラグインインスタンスを生成し、APIを設定した上でリストに追加
                                var plugin = (IPlugin)asm.CreateInstance(t.FullName);
                                plugin.Api = Api;
                                Plugins.Add(plugin);

                                // ロードメソッドを呼び出す
                                plugin.Load();

                                // バージョンを保持
                                loadedPluginVersionNumbers[t.FullName] = plugin.VersionNumber;

                                Logger.Info("register plugin: {0} (TypeName = {1})", dllPath, t.FullName);
                            }
                        }
                    }
                    catch (System.Reflection.ReflectionTypeLoadException ex)
                    {
                        Logger.Warn(ex.ToString());
                        foreach (var ex2 in ex.LoaderExceptions)
                        {
                            Logger.Warn(ex2.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.ToString());
                    }
                }
            }

            // 読み込んだプラグインとバージョン番号のDictionaryを返却
            return loadedPluginVersionNumbers;
        }

        public virtual IDictionary<string, string> GetTextExtNameToLabelMap()
        {
            return Api.TextExtNameToLabelMap;
        }


        public virtual string ExtractText(string path)
        {
            var handler = Api.TextExtractHandlers[Path.GetExtension(path).TrimStart('.')];
            return handler.Invoke(path);
        }
    }
}
