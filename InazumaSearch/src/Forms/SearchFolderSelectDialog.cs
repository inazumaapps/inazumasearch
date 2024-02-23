using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InazumaSearch.Core;

namespace InazumaSearch.Forms
{
    /// <summary>
    /// 検索対象を選択するダイアログ
    /// </summary>
    public partial class SearchFolderSelectDialog : Form
    {
        public Core.Application Application { get; set; }
        public string InputFolderPath { get; set; }
        public IList<string> DirPaths { get; set; }

        /// <summary>
        /// 選択されたフォルダのパス。外部からの取得時に使用
        /// </summary>
        public string SelectedFolderPath { get; set; }

        /// <summary>
        /// 初期選択フォルダ
        /// </summary>
        protected string DefaultSelectedFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InputFolderPath))
                {
                    return Application.UserSettings.LastSelectedSearchTargetDirPath;
                }
                else
                {
                    return InputFolderPath;
                }
            }
        }

        public SearchFolderSelectDialog()
        {
            InitializeComponent();
        }

        public SearchFolderSelectDialog(Core.Application app, string inputFolderPath)
        {
            InitializeComponent();
            Application = app;
            InputFolderPath = inputFolderPath;
        }

        private void BtnDecide_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Application.UserSettings.SaveLastSelectedSearchTargetDirPath(SelectedFolderPath);
            Close();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SearchFolderSelectDialog_Load(object sender, EventArgs e)
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
            var folderPaths = searchEngine.GetAllFolderPath();

            // フォルダパス1件ごとに処理
            foreach (var pair in folderPaths)
            {
                var folderPath = pair.Key;
                var docCount = pair.Value;
                if (string.IsNullOrEmpty(folderPath)) continue;

                var addTargetNodes = TreeFolder.Nodes;
                string currentPath = null;
                var pathItems = folderPath.TrimEnd('\\').Split(new char[] { '\\' });

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
                            Tag = new FolderNodeTag() { DocumentCount = 0, Path = currentPath, FolderName = pathItem }
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

            // 初期選択フォルダパスを取得
            var defaultFolderPath = DefaultSelectedFolderPath;

            // 全ノード処理
            foreach (TreeNode node in allNodes)
            {
                var tag = (FolderNodeTag)node.Tag;

                // 表示文字列の指定
                node.Text = $"{tag.FolderName}";

                // 入力パスと同じであれば選択・展開
                if (defaultFolderPath != null && tag.Path == defaultFolderPath.TrimEnd('\\'))
                {
                    node.Expand();
                    TreeFolder.SelectedNode = node;
                }
            }

            // ツリービューを選択（これを行わないと、選択ノードが反転表示にならない）
            TreeFolder.Focus();

            IconFetcher.SetImageListToTreeView(TreeFolder, systemImgListHandle);
        }

        private void delayTimer_Tick(object sender, EventArgs e)
        {
            UpdateFolderTree();
            delayTimer.Stop();
        }

        private void TreeFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = (FolderNodeTag)e.Node.Tag;
            SelectedFolderPath = tag.Path;
        }

        public class FolderNodeTag
        {
            public string Path { get; set; }
            public string FolderName { get; set; }
            public long DocumentCount { get; set; }
        }
    }

}
