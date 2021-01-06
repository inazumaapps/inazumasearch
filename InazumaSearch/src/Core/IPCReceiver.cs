using System;
using System.Linq;
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
        /// IPCポート名
        /// </summary>
        public const string IPCPortName = "inazumasearch.inazumaapps.info";

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
    }
}