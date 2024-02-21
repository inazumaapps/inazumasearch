using System;
using System.Text;
using System.Windows.Forms;
using InazumaSearch.Core;
using Newtonsoft.Json;

namespace InazumaSearch.src.Forms
{
    public partial class EventLogForm : Form
    {
        public Core.Application App { get; set; }

        /// <summary>
        /// コンストラクタ（引数なし）
        /// </summary>
        public EventLogForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="app"></param>
        public EventLogForm(Core.Application app) : this()
        {
            App = app;
        }

        /// <summary>
        /// フォーム初期化
        /// </summary>
        private void EventLogForm_Load(object sender, EventArgs e)
        {
            UpdateLogList();

            // 高DPI対応
            // ListView (Detailsスタイル) には高DPIに合わせた列幅の自動調整が行われない不具合があるため、手動で調整
            var widthScale = this.CurrentAutoScaleDimensions.Width / 96; // テキストズーム100%＝96dpi

            foreach (ColumnHeader col in lsvLogList.Columns)
            {
                col.Width = (int)Math.Round(col.Width * widthScale);
            }
        }

        /// <summary>
        /// ログリストの選択変更
        /// </summary>
        private void lsvLogList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvLogList.SelectedItems.Count >= 1)
            {
                var item = lsvLogList.SelectedItems[0];
                var log = (EventLog)item.Tag;

                TxtMessage.Text = log.Message;
                TxtTargetPath.Text = log.TargetPath;

                lnkOpenExplorer.Visible = !string.IsNullOrWhiteSpace(TxtTargetPath.Text);
            }
        }

        /// <summary>
        /// ログリスト表示
        /// </summary>
        private void UpdateLogList()
        {
            // 初期化
            lsvLogList.Items.Clear();

            // 常駐クロール側と同時に触らないようにロック
            lock (App.EventLogs)
            {
                // アイテム追加
                foreach (var log in App.EventLogs)
                {
                    var item = new ListViewItem(new string[] { log.Timestamp.ToString("yyyy/MM/dd HH:mm"), EventLog.LogTypeToCaption(log.Type), log.TargetPath, log.TargetFileSize != null ? Util.FormatFileSize(log.TargetFileSize.Value) : "" }); ;
                    item.Tag = log;
                    lsvLogList.Items.Add(item);
                }

                // 末尾のログを選択
                if (lsvLogList.Items.Count >= 1)
                {
                    lsvLogList.Items[lsvLogList.Items.Count - 1].Selected = true;
                }
                lsvLogList.Focus();

                // 出力ボタンの有効/無効切り替え
                BtnExport.Enabled = lsvLogList.Items.Count >= 1;
            }
        }

        /// <summary>
        /// 閉じるボタンクリック
        /// </summary>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            var res = dlgExport.ShowDialog();
            if (res == DialogResult.OK)
            {
                var path = dlgExport.FileName;

                var buf = new StringBuilder();
                foreach (var log in App.EventLogs)
                {
                    buf.AppendLine(JsonConvert.SerializeObject(log, Formatting.Indented));
                    buf.AppendLine("---");
                }

                System.IO.File.WriteAllText(path, buf.ToString());

                Util.ShowInformationMessage($"出力を完了しました。\n「OK」ボタンを押下すると、エクスプローラで出力先のフォルダを開きます。");

                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdateLogList();
        }

        private void lnkOpenExplorer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{TxtTargetPath.Text}\"");
        }
    }
}
