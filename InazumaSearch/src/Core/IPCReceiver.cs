using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using InazumaSearch.src.Forms;

namespace InazumaSearch.Core
{
    /// <summary>
    /// IPC（プロセス間通信）での操作を受け取るオブジェクト。二重起動時に使用される
    /// </summary>
    public class IPCReceiver : MarshalByRefObject
    {
        protected MainComponent mainComponent;
        protected BackgroundMainForm mainForm;

        /// <summary>
        /// URI上のオブジェクト名
        /// </summary>
        public const string UriName = "receiver";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IPCReceiver(MainComponent mainComponent, BackgroundMainForm mainForm)
        {
            this.mainComponent = mainComponent;
            this.mainForm = mainForm;
        }

        /// <summary>
        /// 二重起動時に呼び出される処理
        /// </summary>
        public void OnDoubleBoot()
        {
            // UI側スレッドで処理実行（IPCでリモートからメソッドを呼び出した場合、別スレッドで処理が立ち上がるため）
            mainForm.Invoke((MethodInvoker)delegate
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
            // ドメイン名とユーザー名、アセンブリの名前 (exe名) から、一意なハッシュを生成し、左側から8桁を取得
            var hashProvider = new SHA1CryptoServiceProvider();
            var idHash8 = Util.HexDigest(hashProvider, $@"{Environment.UserDomainName}\{Environment.UserName}-{AppDomain.CurrentDomain.FriendlyName}").Substring(0, 8);

            // ポート名を生成して返す
            return $"{idHash8}.inazumasearch.inazumaapps.info";
        }
    }
}