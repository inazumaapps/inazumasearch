using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InazumaSearch.Core;

namespace InazumaSearch.Forms
{
    public partial class SearchFolderSelectForm : Form
    {
        public Core.Application Application { get; set; }
        public string QueryKeyword { get; set; }
        public string QueryFileName { get; set; }
        public string QueryBody { get; set; }
        public string QueryUpdated { get; set; }
        public string SelectedFormat { get; set; }
        public string SelectedFolderPath { get; set; }
        public string SelectedFolderLabel { get; set; }
        public IList<string> DirPaths { get; set; }

        public SearchFolderSelectForm()
        {
            InitializeComponent();
        }
        public SearchFolderSelectForm(
              Core.Application app
            , string queryKeyword
            , string queryFileName
            , string queryBody
            , string queryUpdated
            , string selectedFormat
            , string selectedFolderPath
            , string selectedFolderLabel)
        {
            InitializeComponent();
            Application = app;
            QueryKeyword = queryKeyword;
            QueryFileName = queryFileName;
            QueryBody = queryBody;
            QueryUpdated = queryUpdated;
            SelectedFormat = selectedFormat;
            SelectedFolderPath = selectedFolderPath;
            SelectedFolderLabel = selectedFolderLabel;
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
            var allNodes = new List<TreeNode>();
            var systemImgListHandle = IntPtr.Zero;

            // 検索条件に合致するフォルダパス情報を取得
            var searchEngine = new SearchEngine(Application);
            var folderPaths = searchEngine.SearchAllFolderPath(QueryKeyword, QueryFileName, QueryBody, QueryUpdated, SelectedFormat, SelectedFolderPath, SelectedFolderLabel);

            // フォルダパス1件ごとに処理
            foreach (var pair in folderPaths)
            {
                var folderPath = pair.Key;
                var docCount = pair.Value;
                if (string.IsNullOrEmpty(folderPath)) continue;

                var addTargetNodes = TreeFolder.Nodes;
                string currentPath = null;
                var pathItems = folderPath.Split(new char[] { '\\' });

                // 経路上にあるノード（親→子の順で追加）
                var nodesOnPathRoute = new List<TreeNode>();

                // パス要素1つごとに処理
                foreach (var pathItem in pathItems)
                {
                    var nodeAddFlag = true;
                    currentPath = (currentPath == null ? pathItem : currentPath += (@"\" + pathItem));

                    // 現在の対象ノードの子に、同じ名前を持つノードがいるかどうかを探索
                    foreach (TreeNode node in addTargetNodes)
                    {
                        var tag = (FolderNodeTag)node.Tag;
                        if (tag.FolderName == pathItem)
                        {
                            // 同じ名前を持つノードがいれば、新ノードの追加はしない
                            addTargetNodes = node.Nodes;
                            nodeAddFlag = false;
                            nodesOnPathRoute.Add(node);
                        }
                    }

                    // 新ノードの追加を行う場合
                    if (nodeAddFlag)
                    {
                        var newNode = new TreeNode()
                        {
                            Tag = new FolderNodeTag() { DocumentCount = 0, Path = folderPath, FolderName = pathItem }
                        };
                        int iconImageIndex;
                        addTargetNodes.Add(newNode);
                        addTargetNodes = newNode.Nodes;

                        if (IconFetcher.GetSystemImageListInfo(currentPath, out systemImgListHandle, out iconImageIndex))
                        {
                            newNode.ImageIndex = iconImageIndex;
                            newNode.SelectedImageIndex = iconImageIndex;
                        };

                        nodesOnPathRoute.Add(newNode);
                        allNodes.Add(newNode);
                    }
                }

                // 経路上の全ノードに対して、文書カウントを追加
                foreach (var node in nodesOnPathRoute)
                {
                    var tag = (FolderNodeTag)node.Tag;
                    tag.DocumentCount += docCount;
                }

            }

            // 表示文字列の指定
            foreach (TreeNode node in allNodes)
            {
                var tag = (FolderNodeTag)node.Tag;
                node.Text = $"{tag.FolderName} ({tag.DocumentCount})";
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
            var folderNodeTag = (FolderNodeTag)TreeFolder.SelectedNode.Tag;

            TxtTarget.Text = TxtTarget.Text.TrimEnd() + "\r\n" + folderNodeTag.Path + @"\";
        }
    }

    public class FolderNodeTag
    {
        public string Path { get; set; }
        public string FolderName { get; set; }
        public long DocumentCount { get; set; }
    }
}
