using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InazumaSearch.Forms
{
    public partial class ProgressForm : Form
    {
        protected DateTime StartingTime { get; set; }
        public Task MainTask { get; set; }
        public string Message { get; set; }

        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressForm(Task mainTask, string message) : this()
        {
            MainTask = mainTask;
            Message = message;
        }


        private void Form_Load(object sender, EventArgs e)
        {
            lblMessage.Text = Message;
        }


        private async void Form_Shown(object sender, EventArgs e)
        {
            timeCounter.Start();
            StartingTime = DateTime.Now;

            await MainTask;
            await Task.Run(() => Thread.Sleep(1000)); // 必ず1秒は待つ (ダイアログが一瞬で閉じることを避けるため
            Close();
        }

        private void timeCounter_Tick(object sender, EventArgs e)
        {
            var span = DateTime.Now - StartingTime;
            statusTimeCount.Text = string.Format("処理時間: {0}", span.ToString(@"mm\:ss"));
        }
    }
}
