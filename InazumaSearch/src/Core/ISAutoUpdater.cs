using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AutoUpdaterDotNET;
using InazumaSearch.Core;

namespace InazumaSearch
{
    /// <summary>
    /// 自動更新モジュール
    /// </summary>
    public static class ISAutoUpdater
    {
        /// <summary>
        /// 初期化済みかどうか
        /// </summary>
        private static bool IsInitialized { get; set; } = false;

        /// <summary>
        /// 更新が見つかった時に実行する処理
        /// </summary>
        private static Action<UpdateInfoEventArgs> UpdateFoundProc;

        /// <summary>
        /// 最後に更新を確認したときの更新情報引数
        /// </summary>
        private static UpdateInfoEventArgs LastUpdateInfoEventArgs { get; set; }

        /// <summary>
        /// 更新チェックが完了したかどうか
        /// </summary>
        public static bool UpdateCheckFinished { get; set; }

        /// <summary>
        /// staticコンストラクタ
        /// </summary>
        static ISAutoUpdater()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (!IsInitialized)
            {
                AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                AutoUpdater.ShowRemindLaterButton = false;
                AutoUpdater.ShowSkipButton = false;
                AutoUpdater.PersistenceProvider = new JsonFilePersistenceProvider("autoupdate-setting.json");
#if PORTABLE
                AutoUpdater.RunUpdateAsAdmin = false;
#endif
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 更新をチェック
        /// </summary>
        /// <param name="isPortable">ポータブル版かどうか</param>
        /// <param name="updateFoundProc">更新が見つかった時の処理</param>
        public static void Check(bool isPortable, Action<UpdateInfoEventArgs> updateFoundProc)
        {
            UpdateFoundProc = updateFoundProc;

            var edition = (isPortable ? "portable" : "standard");
            var platform = Util.GetPlatform();
            var url = $"https://inazumaapps.info/inazumasearch/autoupdate/AutoUpdate-{edition}-{platform}.xml";

            AutoUpdater.Start(url);
        }

        /// <summary>
        /// 更新ダイアログを表示
        /// </summary>
        public static void ShowUpdateForm()
        {
            AutoUpdater.ShowUpdateForm(LastUpdateInfoEventArgs);
        }

        /// <summary>
        /// 更新確認時の処理
        /// </summary>
        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    LastUpdateInfoEventArgs = args;
                    UpdateFoundProc.Invoke(args);
                }

                UpdateCheckFinished = true;
            }
            else
            {
                // サーバーに接続できなかった
            }
        }
    }
}
