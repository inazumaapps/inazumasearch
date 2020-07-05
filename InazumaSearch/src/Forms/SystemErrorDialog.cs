using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Shell;

namespace InazumaSearch.Forms
{
    public partial class SystemErrorDialog : Form
    {
        public Exception Exception { get; set; }
        public string ExceptionString { get; set; }
        public DateTime RaisedTime { get; set; }

        public SystemErrorDialog()
        {
            InitializeComponent();
        }
        public SystemErrorDialog(Exception ex)
        {
            InitializeComponent();
            Exception = ex;
            ExceptionString = ex.ToString();
            RaisedTime = DateTime.Now;
        }

        private void ChkErrorReportSend_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            BtnShowReportBody.Enabled = checkBox.Checked;
            TxtErrorComment.Enabled = checkBox.Checked;
        }

        private void BtnShowReportBody_Click(object sender, EventArgs e)
        {
            var f = new SystemErrorReportBodyDialog(GetReportBody());
            f.ShowDialog(this);
        }


        private void SystemErrorDialog_Load(object sender, EventArgs e)
        {
            //描画先とするImageオブジェクトを作成する
            var canvas = new Bitmap(PicErrorIcon.Width, PicErrorIcon.Height);

            //ImageオブジェクトのGraphicsオブジェクトを作成し、システムのエラーアイコン(WIN32: IDI_ERROR)を描画
            var g = Graphics.FromImage(canvas);
            g.DrawIcon(SystemIcons.Error, 0, 0);
            g.Dispose();

            //PictureBox1に表示する
            PicErrorIcon.Image = canvas;
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            if (ChkErrorReportSend.Checked) ReportSend();
            Close();
        }

        private void BtnQuitAndRestart_Click(object sender, EventArgs e)
        {
            if (ChkErrorReportSend.Checked) ReportSend();
            Core.Application.Restart();
        }

        private void SystemErrorDialog_Shown(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
        }

        protected virtual string GetReportBody()
        {
            var body = new StringBuilder();
            body.AppendLine(ExceptionString);
            body.AppendLine();
            body.AppendLine(new string('-', 80));
            if (!string.IsNullOrWhiteSpace(TxtErrorComment.Text))
            {
                body.AppendLine(TxtErrorComment.Text);
                body.AppendLine(new string('-', 80));

            }
            body.AppendLine(string.Format("バージョン: Inazuma Search {0}", Util.GetVersion().ToString()));
            body.AppendLine(string.Format("発生日時: {0}", RaisedTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if (UserSetting.LastLoadedUserUuid != null)
            {
                body.AppendLine(string.Format("UUID: {0}", UserSetting.LastLoadedUserUuid));
            }


            return body.ToString();
        }

        private void ReportSend()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var parameters = new Dictionary<string, string>
                    {
                        ["body"] = GetReportBody()
                    };

                    var content = new FormUrlEncodedContent(parameters);
                    var t = httpClient.PostAsync("https://tranquil-river-59548.herokuapp.com/inazumasearch/report", content);
                    var f = new ProgressForm(t, "エラー情報を送信しています...");
                    f.ShowDialog(this);
                }
            }
            catch (Exception)
            {
                Util.ShowErrorMessage(this, "エラー情報の送信に失敗しました。");
            }
        }

        private void LblWebsiteUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(LblWebsiteUrl.Text);
        }
    }
}
