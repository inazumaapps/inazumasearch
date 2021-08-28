using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using InazumaSearch.Core;

namespace InazumaSearch.Forms
{
    public partial class SystemErrorDialog : Form
    {
        public Exception Exception { get; set; }
        public string ExceptionString { get; set; }
        public DateTime RaisedTime { get; set; }

        public long? ProcessWorkingSet { get; set; } = null;
        public long? ProcessPeakWorkingSet { get; set; } = null;
        public long? ProcessPagedMemorySize { get; set; } = null;
        public long? ProcessPeakPagedMemorySize { get; set; } = null;
        public string OSCaption { get; set; } = null;
        public bool? OSIs64Bit { get; set; } = null;

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

            try
            {
                // プロセス情報を取得
                var proc = Process.GetCurrentProcess();
                proc.Refresh(); // キャッシュクリア

                // 物理RAM使用量と、ページングファイル使用量を取得
                ProcessWorkingSet = proc.WorkingSet64;
                ProcessPagedMemorySize = proc.PagedMemorySize64;
                ProcessPeakWorkingSet = proc.PeakWorkingSet64;
                ProcessPeakPagedMemorySize = proc.PeakPagedMemorySize64;

 
            }
            catch (Exception)
            {
            }

            try
            {
                // OS情報を取得
                var os = System.Environment.OSVersion;
                OSIs64Bit = System.Environment.Is64BitOperatingSystem;

                // WMI情報を取得
                using (var mc = new System.Management.ManagementClass("Win32_OperatingSystem"))
                {
                    using (var moc = mc.GetInstances())
                    {
                        foreach (System.Management.ManagementObject mo in moc)
                        {
                            //簡単な説明（Windows 8.1では「Microsoft Windows 8.1 Pro」等）
                            OSCaption = (string)mo["Caption"];
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

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
                body.AppendLine("<補足>:" + TxtErrorComment.Text);
                body.AppendLine(new string('-', 80));

            }
            body.AppendLine($"バージョン: Inazuma Search {Util.GetVersion().ToString()}" + (Util.GetPlatform() == "x86" ? " (32ビットバージョン)" : ""));
            body.AppendLine($"発生日時: {RaisedTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            if (UserSetting.LastLoadedUserUuid != null)
            {
                body.AppendLine($"UUID: {UserSetting.LastLoadedUserUuid}");
            }
            body.AppendLine();
            body.AppendLine($"[OS]");
            var bitCaption = (OSIs64Bit == null ? "ビット数不明" : OSIs64Bit.Value ? "64ビット" : "32ビット");
            body.AppendLine($"{OSCaption ?? "不明"} {bitCaption}");
            body.AppendLine($"[メモリ使用量]");
            body.AppendLine($"物理RAM            : {FormatMemorySize(ProcessWorkingSet)}  (ピーク: {FormatMemorySize(ProcessPeakWorkingSet)})");
            body.AppendLine($"ページングファイル : {FormatMemorySize(ProcessPagedMemorySize)}  (ピーク: {FormatMemorySize(ProcessPeakPagedMemorySize)})");


            return body.ToString();
        }

        protected virtual string FormatMemorySize(long? size)
        {
            if (size != null)
            {
                return Util.FormatFileSizeByMB(size.Value).PadLeft(10);
            }
            else
            {
                return "(不明)";
            }
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
