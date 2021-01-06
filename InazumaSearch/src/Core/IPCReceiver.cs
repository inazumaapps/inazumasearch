using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace InazumaSearch.Core
{
    /// <summary>
    /// IPC（プロセス間通信）での操作を受け取るオブジェクト。二重起動時に使用される
    /// </summary>
    public class IPCReceiver : MarshalByRefObject
    {
        protected MainComponent mainComponent;

        /// <summary>
        /// URI上のオブジェクト名
        /// </summary>
        public const string UriName = "receiver";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IPCReceiver(MainComponent mainComponent)
        {
            this.mainComponent = mainComponent;
        }

        /// <summary>
        /// 二重起動時に呼び出される処理
        /// </summary>
        public void OnDoubleBoot()
        {
            // UI側スレッドで処理実行（IPCでリモートからメソッドを呼び出した場合、別スレッドで処理が立ち上がるため）
            Application.BootingBrowserForms.First().Invoke((MethodInvoker)delegate
            {
                // 新しいブラウザ画面を起動
                mainComponent.StartBrowser();
            });
        }

        /// <summary>
        /// IPCポート名を取得 (ログイン中のWindowsユーザーによって異なるポート名となる)
        /// </summary>
        public static string GetIPCPortName()
        {
            // ドメイン名とユーザー名から、一意なハッシュを生成し、左側から8桁を取得
            var hashProvider = new SHA1CryptoServiceProvider();
            var userHash8 = Util.HexDigest(hashProvider, $@"{Environment.UserDomainName}\{Environment.UserName}").Substring(0, 8);

            // ポート名を生成して返す
            return $"{userHash8}.inazumasearch.inazumaapps.info";
        }
    }
}