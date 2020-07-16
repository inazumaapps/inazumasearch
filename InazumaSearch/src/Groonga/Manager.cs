using System;
using System.Collections.Generic;
using System.Diagnostics;
using Alphaleonis.Win32.Filesystem;
using System.Text;
using InazumaSearch.Core;

namespace InazumaSearch.Groonga
{
    public partial class Manager
    {
        /// <summary>
        /// ログ出力用オブジェクト
        /// </summary>
        public NLog.Logger Logger { get; protected set; }

        /// <summary>
        /// 起動中フラグ
        /// </summary>
        public bool Booting { get; set; }

        /// <summary>
        /// Groonga.exeのパス
        /// </summary>
        public string GroongaExePath { get { return Path.Combine(System.Windows.Forms.Application.StartupPath, $"externals/{Core.Util.GetPlatform()}/groonga/bin/groonga.exe"); } }

        /// <summary>
        /// Groonga.exeが存在するディレクトリのパス
        /// </summary>
        public string GroongaDirPath { get { return Path.GetDirectoryName(GroongaExePath); } }

        public string LogDirPath { get; protected set; }
        public string DBDirPath { get; protected set; }
        public string DBPath { get { return Path.Combine(DBDirPath, "InazumaSearch.db"); } }

        public Process Proc { get; protected set; }

        public Manager(string dbDirPath, string logDirPath)
        {
            Booting = false;

            DBDirPath = dbDirPath;
            LogDirPath = logDirPath;

            Logger = NLog.LogManager.GetLogger(Core.LoggerName.Groonga);
        }

        /// <summary>
        /// Managerの起動 (Groongaの子プロセスを立ち上げる)
        /// </summary>
        public void Boot()
        {
            if (Booting)
            {
                throw new InvalidOperationException("Groonga.Managerはすでに起動しています。");
            }

            // DBファイルがすでに存在しているかを調べる
            var newMode = !File.Exists(DBPath);
            // 存在していなければフォルダを作成
            if (newMode)
            {
                Directory.CreateDirectory(DBDirPath);
            }

            // ログフォルダが存在していなければログフォルダも作成
            if (!Directory.Exists(LogDirPath))
            {
                Directory.CreateDirectory(LogDirPath);
            }

            Proc = new Process();

            Proc.StartInfo.FileName = GroongaExePath;
            Proc.StartInfo.Arguments = string.Format("--log-level debug --log-path \"{0}\" --log-level info --query-log-path \"{1}\" {2} \"{3}\""
                                                   , Path.Combine(LogDirPath, "groonga.log")
                                                   , Path.Combine(LogDirPath, "groonga_query.log")
                                                   , (newMode ? "-n" : "")
                                                   , DBPath);
            Proc.StartInfo.UseShellExecute = false;
            Proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            Proc.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            Proc.StartInfo.RedirectStandardOutput = true;
            Proc.StartInfo.RedirectStandardInput = true;
            Proc.StartInfo.RedirectStandardError = true;
            Proc.StartInfo.CreateNoWindow = true;

            //Proc.OutputDataReceived += Proc_OutputDataReceived;
            Proc.ErrorDataReceived += Proc_ErrorDataReceived;

            Proc.Start();
            //Proc.BeginOutputReadLine();
            Proc.BeginErrorReadLine();

            // 起動フラグON
            Booting = true;
        }

        /// <summary>
        /// Managerの停止
        /// </summary>
        public void Shutdown()
        {
            if (!Booting)
            {
                throw new InvalidOperationException("Groonga.Managerが起動していない状態で停止しようとしています。");
            }

            // Groongaプロセスを終了
            ExecuteCommandAsString("quit");
            Proc.WaitForExit();
            Proc.Close();
            Proc.Dispose();

            // 起動フラグOFF
            Booting = false;
        }

        /// <summary>
        /// Managerの再起動
        /// </summary>
        public void Reboot()
        {
            Shutdown();
            Boot();
        }

        private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                throw new Exception(e.Data);
            }
        }


        /// <summary>
        /// データベースのファイルサイズ合計を取得
        /// </summary>
        /// <returns></returns>
        public virtual long GetDBFileSizeTotal()
        {
            if (Directory.Exists(DBDirPath))
            {
                long size = 0;
                foreach (var path in Directory.GetFiles(DBDirPath))
                {
                    size += new FileInfo(path).Length;
                }
                return size;
            }
            else
            {
                return 0;
            }
        }


    }
}
