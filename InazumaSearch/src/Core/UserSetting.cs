using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace InazumaSearch.Core
{
    public class UserSetting
    {
        public const int CurrentSettingVersion = 3;

        public static string LastLoadedUserUuid { get; set; }

        public class TargetFolderType
        {
            public const string DocumentFile = "document_file";
            public const string Mail = "mail";
        }


        public class TargetFolder
        {
            public virtual string Type { get; set; }
            public virtual string Path { get; set; }
            public virtual string Label { get; set; }
        }

        public class Extension
        {
            public virtual string ExtName { get; set; }
            public virtual string Label { get; set; }
        }

        public class PlainData
        {
            public virtual string UserUuid { get; set; }
            public virtual int SettingVersion { get; set; }
            public virtual List<TargetFolder> TargetFolders { get; set; }
            public virtual DateTime? LastCrawlTime { get; set; }
            public virtual bool AlwaysCrawlMode { get; set; }
            public virtual bool StartUp { get; set; }
            public virtual Dictionary<string, int> LastLoadedPluginVersionNumbers { get; set; }
            public virtual List<Extension> TextExtensions { get; set; }

            public PlainData()
            {
                SettingVersion = CurrentSettingVersion;
                TargetFolders = new List<TargetFolder>();
                LastCrawlTime = null;
                AlwaysCrawlMode = false;
                StartUp = false;
                LastLoadedPluginVersionNumbers = new Dictionary<string, int>();
                TextExtensions = new List<Extension>();
            }
        }

        public class Store
        {
            public string SettingFilePath { get; protected set; }
            public PlainData PlainData { get; protected set; }

            public Store(string filePath)
            {
                PlainData = new PlainData();

                SettingFilePath = filePath;
            }

            public void Save()
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingFilePath));
                File.WriteAllText(SettingFilePath, JsonConvert.SerializeObject(PlainData));
                NLog.LogManager.GetCurrentClassLogger().Info("設定ファイル保存 - {0}", JsonConvert.SerializeObject(PlainData));
            }

            public void Load()
            {
                PlainData = JsonConvert.DeserializeObject<PlainData>(File.ReadAllText(SettingFilePath));

                // 補正
                foreach (var f in TargetFolders)
                {
                    if (f.Type == null) f.Type = TargetFolderType.DocumentFile;
                }

                // 設定ファイルバージョンが2以下であれば、テキスト拡張子を設定
                if (PlainData.SettingVersion <= 2)
                {
                    PlainData.TextExtensions = new List<Extension>(new[] { new Extension() { ExtName = "txt", Label = "テキスト" } });
                }

                // UUIDをスタティック変数に設定 (エラー情報送信用)
                UserSetting.LastLoadedUserUuid = UserUuid;

                // 設定オブジェクトのバージョンを更新
                PlainData.SettingVersion = CurrentSettingVersion;
            }

            public static Store Setup(string settingFilePath)
            {
                var store = new Store(Path.GetFullPath(settingFilePath));

                // ファイルが存在しない場合は、ユーザーIDを生成して新規ファイルを作成
                if (!File.Exists(store.SettingFilePath))
                {
                    store.PlainData.UserUuid = Guid.NewGuid().ToString();
                    store.Save();
                }

                // 設定ファイルの読み込み
                store.Load();

                // 設定オブジェクトを返す
                return store;
            }

            /// <summary>
            /// 文書データのキーを受け取り、その値を元に「その文書を含んでいるすべての対象フォルダ」を取得する
            /// </summary>
            /// <param name="key"></param>
            public virtual IEnumerable<TargetFolder> FindTargetFoldersFromDocumentKey(string key)
            {
                var folders = TargetFolders.Where((f) => key.StartsWith(Util.MakeDocumentFileKey(f.Path)));
                return folders;
            }


            #region 個別のユーザー設定値プロパティ

            /// <summary>
            /// 検索対象フォルダリスト
            /// </summary>
            public List<TargetFolder> TargetFolders { get { return PlainData.TargetFolders; } }

            /// <summary>
            /// 検索対象フォルダリストの設定
            /// </summary>
            public void SaveTargetFolders(List<TargetFolder> folders)
            {
                PlainData.TargetFolders = folders;
                Save();
            }

            /// <summary>
            /// 最終クロール日時
            /// </summary>
            public DateTime? LastCrawlTime { get { return PlainData.LastCrawlTime; } }


            /// <summary>
            /// 最終クロール日時の設定
            /// </summary>
            public void SaveLastCrawlTime(DateTime t)
            {
                PlainData.LastCrawlTime = t;
                Save();
            }

            /// <summary>
            /// 常駐クロールモード
            /// </summary>
            public bool AlwaysCrawlMode { get { return PlainData.AlwaysCrawlMode; } }

            /// <summary>
            /// 常駐クロールモードの設定
            /// </summary>
            public void SaveAlwaysCrawlMode(bool value)
            {
                PlainData.AlwaysCrawlMode = value;
                Save();
            }

            /// <summary>
            /// スタートアップ起動
            /// </summary>
            public bool StartUp { get { return PlainData.StartUp; } }

            /// <summary>
            /// スタートアップ起動の設定
            /// </summary>
            public void SaveStartUp(bool value)
            {
                PlainData.StartUp = value;
                Save();
            }

            /// <summary>
            /// 最後にロードしたプラグインのバージョン番号
            /// </summary>
            public Dictionary<string, int> LastLoadedPluginVersionNumbers { get { return PlainData.LastLoadedPluginVersionNumbers; } }

            /// <summary>
            /// 最後にロードしたプラグインのバージョン番号の設定
            /// </summary>
            public void SaveLastLoadedPluginVersionNumbers(Dictionary<string, int> value)
            {
                PlainData.LastLoadedPluginVersionNumbers = value;
                Save();
            }

            /// <summary>
            /// テキストファイルとして登録する拡張子
            /// </summary>
            public List<Extension> TextExtensions { get { return PlainData.TextExtensions; } }

            /// <summary>
            /// テキストファイルとして登録する拡張子の設定
            /// </summary>
            public void SaveTextExtNames(List<Extension> value)
            {
                PlainData.TextExtensions = value;
                Save();
            }


            /// <summary>
            /// ユーザー識別用ID
            /// </summary>
            public string UserUuid { get { return PlainData.UserUuid; } }

            #endregion
        }
    }
}
