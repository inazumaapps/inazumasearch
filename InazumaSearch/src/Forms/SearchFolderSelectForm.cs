using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;

namespace InazumaSearch.Forms
{
    public partial class SearchFolderSelectForm : Form
    {
        public Core.Application Application { get; set; }
        public IList<string> DirPaths { get; set; }

        public SearchFolderSelectForm()
        {
            InitializeComponent();
        }
        public SearchFolderSelectForm(Core.Application app)
        {
            InitializeComponent();
            Application = app;
        }

        private void BtnDecide_Click(object sender, EventArgs e)
        {
            Close();

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SearchFolderSelectForm_Load(object sender, EventArgs e)
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
            DirPaths = selectRes.SearchResult.Records.Where(r => !string.IsNullOrWhiteSpace(r.GetTextValue(Column.Documents.FILE_PATH)))
                                                                 .Select(r => Path.GetDirectoryName(r.GetTextValue(Column.Documents.FILE_PATH)))
                                                                 .Distinct().ToList();

            // フォルダパス1件ごとに処理
            foreach (var dirPath in DirPaths)
            {
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

        private void TreeFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void TreeFolder_DoubleClick(object sender, EventArgs e)
        {
            var selectedFolderPath = (string)TreeFolder.SelectedNode.Tag;

            TxtTarget.Text = TxtTarget.Text.TrimEnd() + "\r\n" + selectedFolderPath + @"\";
        }

        private void ChkCrawlFolderOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFolderTree();
        }
    }
}
