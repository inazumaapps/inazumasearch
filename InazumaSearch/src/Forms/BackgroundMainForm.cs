using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InazumaSearch.src.Forms
{
    public partial class BackgroundMainForm : Form
    {
        private MainComponent _mainComponent;

        private static readonly int WM_SHOW_BROWSER = RegisterWindowMessage("InazumaSearch_ShowBrowser");

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int RegisterWindowMessage(string lpString);

        /// <summary>
        /// 二重起動時の通知先として FindWindow で検索するためのウィンドウタイトル
        /// </summary>
        public const string WindowTitle = "InazumaSearch_BackgroundMainForm";

        public BackgroundMainForm()
        {
            InitializeComponent();
            Text = WindowTitle;
        }

        /// <summary>
        /// ブラウザ起動などの操作を行うメインコンポーネントを設定します。
        /// </summary>
        /// <param name="mainComponent">設定するメインコンポーネント</param>
        public void SetMainComponent(MainComponent mainComponent)
        {
            _mainComponent = mainComponent;
        }

        /// <summary>
        /// ウィンドウメッセージを処理します。
        /// 二重起動時に他プロセスから WM_SHOW_BROWSER メッセージを受信した場合、新しいブラウザ画面を起動します。
        /// </summary>
        /// <inheritdoc />
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SHOW_BROWSER)
            {
                if (_mainComponent != null)
                {
                    _mainComponent.StartBrowser();
                }
            }

            base.WndProc(ref m);
        }
    }
}
