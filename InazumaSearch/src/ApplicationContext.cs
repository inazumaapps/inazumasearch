﻿using System;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using InazumaSearch.src.Forms;

namespace InazumaSearch
{
    public class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationContext(
              string htmlDirPath
            , bool showBrowser = true
            , bool appDebugMode = false
        ) : base()
        {
            // アプリケーション生成
            var app = new Core.Application
            {
                DebugMode = appDebugMode,
                HtmlDirPath = htmlDirPath
            };

            // ロガー用にログディレクトリパスを設定
            NLog.GlobalDiagnosticsContext.Set("LogDirPath", app.LogDirPath);
            // ログパスを設定
            var nlogConfPath =
                Path.Combine(System.Windows.Forms.Application.StartupPath, "nlogconf", (app.DebugMode ? @"Debug.config" : @"Release.config"));
            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(nlogConfPath);

            // Inazuma Searchアプリケーションクラスの起動
            var bootSuccess = app.Boot();
            if (!bootSuccess)
            {
                Environment.Exit(0);
            }

            // メインコンポーネントの生成
            var comp = new MainComponent(app);

            // 通知アイコンをアプリケーションのstaticプロパティに設定
            Core.Application.NotifyIcon = comp.NotifyIcon;

            // 常駐クロールモードであれば、通知アイコンを表示する
            if (app.UserSettings.AlwaysCrawlMode)
            {
                Core.Application.NotifyIcon.Visible = true;
            }

            // メインフォームの生成（画面には表示しない）
            var mainForm = new BackgroundMainForm();
            mainForm.Show();
            mainForm.Hide();

            // ブラウザの立ち上げ
            if (showBrowser)
            {
                comp.StartBrowser();
            }
        }
    }
}
