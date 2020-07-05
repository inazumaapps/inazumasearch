using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InazumaSearch.Core;
using InazumaSearch.Forms;

namespace InazumaSearch
{
    public partial class MainComponent : Component
    {
        public Core.Application App { get; set; }
        public MainComponent()
        {
            InitializeComponent();

            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;

        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            NotifyIcon.Dispose();
        }

        public MainComponent(Core.Application app) : this()
        {
            App = app;
        }

        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            StartBrowser();
        }

        public void StartBrowser()
        {
            try
            {
                var f = new BrowserForm(App.HtmlDirPath)
                {
                    App = App
                };
                f.Show();
            }
            catch (FileNotFoundException ex)
            {
                App.Logger.Error(ex);
                Util.ShowErrorMessage("ブラウザコントロールの初期化に失敗しました。");
                Core.Application.Quit();
            }
        }

        protected void MenuItem_Quit_Click(object sender, EventArgs e)
        {
            Core.Application.Quit();
        }


        private void TaskBarContextMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void MenuItem_WindowOpen_Click(object sender, EventArgs e)
        {
            StartBrowser();
        }
    }
}
