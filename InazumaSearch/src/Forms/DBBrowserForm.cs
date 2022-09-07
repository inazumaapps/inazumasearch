using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InazumaSearch.Forms
{
    public partial class DBBrowserForm : Form
    {
        public Core.Application Application { get; set; }
        public IList<Groonga.RecordSet.Record> DocumentRecords { get; set; }

        public DBBrowserForm()
        {
            InitializeComponent();
        }
        public DBBrowserForm(Core.Application app)
        {
            InitializeComponent();
            Application = app;
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DBBrowserForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ツリー表示を更新
        /// </summary>
        protected virtual void UpdateFolderTree()
        {
            // ツリーを一度初期化
            TreeFolder.Nodes.Clear();
            var systemImgListHandle = IntPtr.Zero;

            // 全文書のファイルパス一覧を取得
            var selectRes = Application.GM.Select(
                table: Table.Documents
                , outputColumns: new[] { Column.Documents.KEY, Column.Documents.FILE_PATH, Column.Documents.FILE_UPDATED_AT, Column.Documents.SIZE }
                , sortKeys: new[] { Column.Documents.FILE_PATH }
                , limit: -1
            );
            DocumentRecords = selectRes.SearchResult.Records.Where(r => !string.IsNullOrWhiteSpace(r.GetTextValue(Column.Documents.FILE_PATH)))
                                                                 .ToList();

            // 文書1件ごとに処理
            foreach (var rec in DocumentRecords)
            {
                Application.Logger.Trace("[DBBrowser] dirEach: {0}", rec.GetTextValue(Column.Documents.FILE_PATH));

                string dirPath = null;

                try
                {
                    dirPath = Path.GetDirectoryName(rec.GetTextValue(Column.Documents.FILE_PATH));
                }
                catch (Exception ex)
                {
                    Application.Logger.Debug(ex);
                    Application.Logger.Debug(ex.ToString());
                }

                if (string.IsNullOrEmpty(dirPath)) continue;

                var addTargetNodes = TreeFolder.Nodes;
                string currentPath = null;
                foreach (var pathItem in dirPath.Split(new char[] { '\\' }))
                {
                    var nodeAddFlag = true;
                    currentPath = (currentPath == null ? pathItem : currentPath += (@"\" + pathItem));

                    // 現在の対象ノードの子に、同じ名前を持つノードがいるかどうかを探索
                    foreach (TreeNode node in addTargetNodes)
                    {
                        if (node.Text == pathItem)
                        {
                            // 同じ名前を持つノードがいれば、新ノードの追加はしない
                            addTargetNodes = node.Nodes;
                            nodeAddFlag = false;
                        }

                    }

                    // 新ノードの追加を行う場合
                    if (nodeAddFlag)
                    {
                        var newNode = new TreeNode(pathItem)
                        {
                            Tag = currentPath
                        };
                        int iconImageIndex;
                        addTargetNodes.Add(newNode);
                        addTargetNodes = newNode.Nodes;

                        if (IconFetcher.GetSystemImageListInfo(currentPath, out systemImgListHandle, out iconImageIndex))
                        {
                            newNode.ImageIndex = iconImageIndex;
                            newNode.SelectedImageIndex = iconImageIndex;
                        };
                    }
                }
            }

            IconFetcher.SetImageListToTreeView(TreeFolder, systemImgListHandle);
        }

        private void delayTimer_Tick(object sender, EventArgs e)
        {
            UpdateFolderTree();
            delayTimer.Stop();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            UpdateFolderTree();
        }

        private void TreeFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedFolderPath = (string)e.Node.Tag;
            var systemImgListHandle = IntPtr.Zero;

            var recs = DocumentRecords.Where(rec => selectedFolderPath == Path.GetDirectoryName(rec.GetTextValue(Column.Documents.FILE_PATH)));
            LstFile.Items.Clear();

            // 0件の場合は中断
            if (!recs.Any()) return;

            foreach (var rec in recs)
            {
                var path = rec.GetTextValue(Column.Documents.FILE_PATH);
                var fileName = Path.GetFileName(path);
                var size = rec.GetIntValue(Column.Documents.SIZE);
                var updated = rec.GetTimeValue(Column.Documents.FILE_UPDATED_AT);

                var item = new ListViewItem(new string[] { fileName, updated.Value.ToString("yyyy/MM/dd HH:mm"), Util.FormatFileSizeByKB(size.Value) });
                int iconImageIndex;
                if (IconFetcher.GetSystemImageListInfo(path, out systemImgListHandle, out iconImageIndex))
                {
                    item.ImageIndex = iconImageIndex;
                };

                item.Tag = rec.Key;

                LstFile.Items.Add(item);
            }

            IconFetcher.SetImageListToListView(LstFile, systemImgListHandle);

        }

        /// <summary>
        /// リストダブルクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstFile_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (LstFile.SelectedItems.Count >= 1)
            {
                var item = LstFile.SelectedItems[0];
                var documentKey = (string)item.Tag;

                var res = Application.GM.Select(
                      table: Table.Documents
                    , outputColumns: new[] { Column.Documents.KEY, Column.Documents.BODY }
                    , query: string.Format("{0}:{1}", Column.Documents.KEY, Groonga.Util.EscapeForQuery(documentKey))
                );

                var body = res.SearchResult.Records[0].GetTextValue(Column.Documents.BODY);
                var f = new FileBodyViewDialog
                {
                    Body = body
                };
                f.ShowDialog(this);
            }
        }
    }
}
