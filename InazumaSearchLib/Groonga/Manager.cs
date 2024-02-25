using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Alphaleonis.Win32.Filesystem;

namespace InazumaSearchLib.Groonga
{
    public partial class Manager
    {
        /// <summary>
        /// DB名
        /// </summary>
        /// <remarks>
        /// 原則変更しないようにしてください。この値が変わると、以前のデータベースを引き継ぐことができません。
        /// </remarks>
        public const string DB_NAME = "InazumaSearch.db";
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
        public string GroongaExePath { get; protected set; }

        /// <summary>
        /// Groonga.exeが存在するディレクトリのパス
        /// </summary>
        public string GroongaDirPath { get { return Path.GetDirectoryName(GroongaExePath); } }

        public string LogDirPath { get; protected set; }
        public string DBDirPath { get; set; }
        public string DBPath { get { return Path.Combine(DBDirPath, DB_NAME); } }

        /// <summary>
        /// デバッグ起動フラグ（通常はアプリケーションの値を引き継ぐ）
        /// </summary>
        public bool DebugMode { get; protected set; }

        public Process Proc { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="groongaExePath">groonga.exeのパス</param>
        /// <param name="dbDirPath">DBディレクトリパス</param>
        /// <param name="logDirPath">ログディレクトリパス</param>
        /// <param name="debugMode">デバッグ起動フラグ（通常はアプリケーションの値を引き継ぐ）</param>
        public Manager(string groongaExePath, string dbDirPath, string logDirPath, bool debugMode)
        {
            Booting = false;

            GroongaExePath = groongaExePath;
            DBDirPath = dbDirPath;
            LogDirPath = logDirPath;
            DebugMode = debugMode;

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
            var logPath = Path.Combine(LogDirPath, "groonga.log");
            var queryLogPath = Path.Combine(LogDirPath, "groonga_query.log");
            var debugOnlyOptions = (DebugMode ? $"--log-level debug --log-path \"{logPath}\" --query-log-path \"{queryLogPath}\" " : "");
            Proc.StartInfo.Arguments = $"{debugOnlyOptions} {(newMode ? "-n" : "")} \"{DBPath}\"";
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
            ExecuteCommandAsStringByDict("quit");
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
            // コマンド実行時の入出力と同時に実行されないようにロック
            lock (Locker)
            {
                Shutdown();
                Boot();
            }
        }

        private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                throw new Exception(e.Data);
            }
        }

        /// <summary>
        /// データベースファイルの一覧を取得
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetDBFiles()
        {
            if (Directory.Exists(DBDirPath))
            {
                return Directory.GetFiles(DBDirPath)
                                .Where(p => Path.GetFileName(p).StartsWith(DB_NAME))
                                .ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// データベースのファイルサイズ合計を取得
        /// </summary>
        /// <returns></returns>
        public virtual long GetDBFileSizeTotal()
        {
            long size = 0;
            foreach (var path in GetDBFiles())
            {
                size += new FileInfo(path).Length;
            }
            return size;
        }


    }
}
