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
        /// <summary>
        /// 選択モードの値
        /// </summary>
        public enum SelectMode
        {
            /// <summary>
            /// 検索条件の入力時に選択
            /// </summary>
            CONDITION = 1,

            /// <summary>
            /// ドリルダウン選択（検索結果に対する絞り込みを行う）
            /// </summary>
            DRILLDOWN = 2,
        }

        public Core.Application Application { get; set; }
        public string InputFolderPath { get; set; }
        public IList<string> DirPaths { get; set; }
        public SearchEngine.Condition SearchCondition { get; set; }

        /// <summary>
        /// 選択モード
        /// </summary>
        public SelectMode Mode { get; set; }

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

                if (Mode == SelectMode.CONDITION)
                {
                    // 検索条件入力時は、フォルダパス入力済であればその値を初期値とする
                    // 入力済みでなければ前回選択したパスを使う
                    if (!string.IsNullOrWhiteSpace(InputFolderPath))
                    {
                        return InputFolderPath;
                    }
                    else
                    {
                        return Application.UserSettings.LastSelectedSearchTargetDirPath;
                    }
                }
                else
                {
                    // ドリルダウン時は、現在絞り込みに使用しているパスを使用
                    return SearchCondition.FolderPath;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SearchFolderSelectDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ（検索条件入力時用）
        /// </summary>
        /// <param name="mode">選択モード</param>
        /// <param name="app">アプリケーションインスタンス</param>
        /// <param name="inputFolderPath">現在入力されているフォルダパス</param>
        public SearchFolderSelectDialog(Core.Application app, string inputFolderPath)
        {
            InitializeComponent();
            Mode = SelectMode.CONDITION;
            Application = app;
            InputFolderPath = inputFolderPath;
        }

        /// <summary>
        /// コンストラクタ（ドリルダウン用）
        /// </summary>
        /// <param name="mode">選択モード</param>
        /// <param name="app">アプリケーションインスタンス</param>
        /// <param name="cond">検索条件オブジェクト</param>
        public SearchFolderSelectDialog(
              Core.Application app
            , SearchEngine.Condition cond
        )
        {
            InitializeComponent();
            Mode = SelectMode.DRILLDOWN;
            Application = app;
            SearchCondition = cond;
        }

        /// <summary>
        /// 確定ボタンクリック
        /// </summary>
        private void BtnDecide_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (Mode == SelectMode.CONDITION)
            {
                // 検索条件入力時は、選択したパスを記憶
                Application.UserSettings.SaveLastSelectedSearchTargetDirPath(SelectedFolderPath);
            }
            Close();
        }

        /// <summary>
        /// 閉じるボタンクリック
        /// </summary>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// ツリー表示を更新
        /// </summary>
        protected virtual void UpdateFolderTree()
        {
            // ツリーを一度初期化
            TreeFolder.Nodes.Clear();
            var allNodes = new List<TreeNode>();
            var rootNodes = new List<TreeNode>();
            var systemImgListHandle = IntPtr.Zero;

            // 検索条件に合致するフォルダパス情報を取得
            // ただし、この時フォルダパスの絞り込みは行わない
            var searchEngine = new SearchEngine(Application);
            SearchEngine.Condition usingCond = null;
            if (SearchCondition != null)
            {
                usingCond = SearchCondition.Clone();
                usingCond.FolderPath = null;
            }
            var folderPaths = searchEngine.SearchAllFolderPath(usingCond);

            // フォルダパス1件ごとに処理
            foreach (var pair in folderPaths)
            {
                var folderPath = pair.Key;
                var docCount = pair.Value;
                if (string.IsNullOrEmpty(folderPath)) continue;

                var addTargetNodes = TreeFolder.Nodes;
                string currentPath = null;

                // パス要素を分割
                // 「\\」から始まる共有パスは特別扱いする
                string[] pathItems = null;
                if (folderPath.StartsWith(@"\\"))
                {
                    pathItems = folderPath.TrimStart('\\').TrimEnd('\\').Split(new char[] { '\\' });
                    pathItems[0] = $@"\\{pathItems[0]}";
                }
                else
                {
                    pathItems = folderPath.TrimEnd('\\').Split(new char[] { '\\' });
                }

                // 経路上にあるノード（親→子の順で追加）
                var nodesOnPathRoute = new List<TreeNode>();

                // パス要素1つごとに処理
                var isRootNode = true;
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

                        if (isRootNode)
                        {
                            rootNodes.Add(newNode);
                        }
                    }

                    // 次からはルートノードでない
                    isRootNode = false;
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
                if (Mode == SelectMode.DRILLDOWN)
                {
                    node.Text = $"{tag.FolderName} ({tag.DocumentCount})";
                }
                else
                {
                    node.Text = $"{tag.FolderName}";
                }

                // 初期パスと一致すれば選択・展開
                if (defaultFolderPath != null && tag.Path == defaultFolderPath.TrimEnd('\\'))
                {
                    TreeFolder.SelectedNode = node;
                    TreeFolder.SelectedNode.Expand();
                }
            }

            // どのノードも選択していない場合は、自動的に選択できる（1つしか選択肢がない）階層まで選択する
            if (TreeFolder.SelectedNode == null)
            {
                // ルートノードが1つの場合のみ有効
                if (rootNodes.Count == 1)
                {
                    var targetNode = rootNodes[0];

                    // 1階層下をたどり、その下のノードが1つしか存在しないなら、さらに下を探索
                    while (targetNode.Nodes.Count == 1)
                    {
                        var child = targetNode.Nodes[0];

                        // 文書数が現在ノードと子ノードで異なる場合は、子を選択しない（この場合は現在フォルダの直下にも文書がある）
                        if (((FolderNodeTag)targetNode.Tag).DocumentCount != ((FolderNodeTag)child.Tag).DocumentCount)
                        {
                            break;
                        }

                        targetNode = child;
                    }

                    // 対象ノードを選択・展開
                    TreeFolder.SelectedNode = targetNode;
                    TreeFolder.SelectedNode.Expand();
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
