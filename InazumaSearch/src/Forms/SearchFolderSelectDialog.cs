using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InazumaSearch.Core;
using InazumaSearch.Forms.CustomControls;

namespace InazumaSearch.Forms
{
    /// <summary>
    /// 検索対象を選択するダイアログ
    /// </summary>
    public partial class SearchFolderSelectDialog : Form
    {
        /// <summary>
        /// ツリーノードに設定するフォルダ情報
        /// </summary>
        public class FolderNodeTag
        {
            public string Path { get; set; }
            public string FolderName { get; set; }
            public long DocumentCount { get; set; }
        }

        /// <summary>
        /// 仮想ノード。
        /// 最初にこの仮想ノードを用いてツリーを構築し、ツリー展開操作が行われたタイミングで実際のノードを生成する
        /// </summary>
        public class VirtualNode
        {
            /// <summary>
            /// 親ノード。最上位ノードの場合はnull
            /// </summary>
            public VirtualNode Parent = null;

            /// <summary>
            /// 子ノードのリスト（内部用・書き込み可能）
            /// </summary>
            protected List<VirtualNode> _nodes = new List<VirtualNode>();

            /// <summary>
            /// 子ノードのリスト（外部からのアクセス用・変更不可）
            /// </summary>
            public IReadOnlyList<VirtualNode> Nodes { get { return _nodes; } }

            /// <summary>
            /// 表示テキスト
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// タグ（フォルダ情報を設定）
            /// </summary>
            public FolderNodeTag Tag { get; set; }

            /// <summary>
            /// 実体化後のノード
            /// </summary>
            public TreeNodeEx RealNode { get; set; } = null;

            /// <summary>
            /// 子ノードを追加
            /// </summary>
            public void AddChild(VirtualNode child)
            {
                _nodes.Add(child);
                child.Parent = this;
            }
        }

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

        /// <summary>
        /// メインアプリケーションインスタンス
        /// </summary>
        public Core.Application Application { get; set; }

        /// <summary>
        /// 選択ダイアログを開く前の時点で入力済・選択済のフォルダパス
        /// </summary>
        public string InputFolderPath { get; set; }

        /// <summary>
        /// 検索条件（ドリルダウン時のみ設定）
        /// </summary>
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
        /// ルート（最上位）の仮想ノードリスト
        /// </summary>
        protected IList<VirtualNode> VirtualRootNodes { get; set; } = new List<VirtualNode>();

        /// <summary>
        /// フォルダパス→仮想ノードの対応付けDictionary
        /// </summary>
        protected IDictionary<string, VirtualNode> AllVirtualNodeDict = new Dictionary<string, VirtualNode>();

        /// <summary>
        /// フォルダツリーに、アイコン画像のイメージリストを設定したかどうか
        /// </summary>
        protected bool isSetTreeFolderIconImageList = false;

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

            // 仮想ノードツリーの構築を行う
            BuildVirtualNodeTree(folderPaths);

            // ルートノードの実体化を行う
            foreach (var rootVNode in VirtualRootNodes)
            {
                // ノード実体化
                var rootNode = RealizeVirtualNode(rootVNode);

                // ノード追加
                TreeFolder.Nodes.Add(rootNode);
            }

            // 初期パスと一致するノードがあれば、そのノードを展開
            if (DefaultSelectedFolderPath != null)
            {
                var targetPath = DefaultSelectedFolderPath.TrimEnd('\\');
                if (AllVirtualNodeDict.ContainsKey(targetPath))
                {
                    var vNode = AllVirtualNodeDict[targetPath];
                    ExpandTreeToVirtualNode(vNode);
                }
            }

            // どのノードも選択していない場合は、自動的に選択できる（1つしか選択肢がない）階層まで選択する
            if (TreeFolder.SelectedNode == null)
            {
                // ルートノードが1つの場合のみ有効
                if (VirtualRootNodes.Count == 1)
                {
                    var targetVNode = VirtualRootNodes[0];

                    // 1階層下をたどり、その下のノードが1つしか存在しないなら、さらに下を探索
                    while (targetVNode.Nodes.Count == 1)
                    {
                        var child = targetVNode.Nodes[0];

                        // 文書数が現在ノードと子ノードで異なる場合は、子を選択しない（この場合は現在フォルダの直下にも文書がある）
                        if (((FolderNodeTag)targetVNode.Tag).DocumentCount != ((FolderNodeTag)child.Tag).DocumentCount)
                        {
                            break;
                        }

                        targetVNode = child;
                    }

                    // 対象ノードを選択・展開
                    ExpandTreeToVirtualNode(targetVNode);
                }
            }

            // ツリービューを選択（これを行わないと、選択ノードが反転表示にならない）
            TreeFolder.Focus();
        }

        /// <summary>
        /// 仮想ノードツリーの構築を行う
        /// </summary>
        /// <param name="folderDocCounts">フォルダパスをキー、そのフォルダ内（直下）の文書件数を値として格納したDictionary</param>
        protected void BuildVirtualNodeTree(Dictionary<string, long> folderDocCounts)
        {
            // 現在の仮想ノードをクリア
            VirtualRootNodes.Clear();
            AllVirtualNodeDict.Clear();

            // フォルダパス1件ごとに処理
            foreach (var pair in folderDocCounts)
            {
                var folderPath = pair.Key;
                var docCount = pair.Value;
                if (string.IsNullOrEmpty(folderPath)) continue;

                VirtualNode currentNode = null; // 現在探索中のノード。初期時はnull
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
                var nodesOnPathRoute = new List<VirtualNode>();

                // パス要素1つごとに処理
                foreach (var pathItem in pathItems)
                {
                    var nodeAddFlag = true;
                    currentPath = (currentPath == null ? pathItem : currentPath += (@"\" + pathItem));

                    // 現在の対象ノードの子に、同じ名前を持つノードがいるかどうかを探索
                    var addTargetNodes = (currentNode == null ? (IReadOnlyList<VirtualNode>)VirtualRootNodes : currentNode.Nodes);
                    foreach (var node in addTargetNodes)
                    {
                        var tag = (FolderNodeTag)node.Tag;
                        if (tag.FolderName == pathItem)
                        {
                            // 同じ名前を持つノードがいれば、新ノードの追加はしない
                            currentNode = node;
                            nodeAddFlag = false;
                            nodesOnPathRoute.Add(node);
                        }
                    };

                    // 新ノードの追加を行う場合
                    if (nodeAddFlag)
                    {
                        var newNode = new VirtualNode()
                        {
                            Tag = new FolderNodeTag() { DocumentCount = 0, Path = currentPath, FolderName = pathItem }
                        };
                        if (currentNode == null)
                        {
                            // ルートノード追加
                            VirtualRootNodes.Add(newNode);
                        }
                        else
                        {
                            // 子ノード追加
                            currentNode.AddChild(newNode);
                        }

                        currentNode = newNode;
                        nodesOnPathRoute.Add(newNode);
                        AllVirtualNodeDict[currentPath] = newNode;
                    }
                }

                // 経路上の全ノードに対して、文書カウントを追加
                foreach (var node in nodesOnPathRoute)
                {
                    var tag = (FolderNodeTag)node.Tag;
                    tag.DocumentCount += docCount;
                }
            }

            // 全仮想ノード処理
            foreach (var vNode in AllVirtualNodeDict.Values)
            {
                var tag = (FolderNodeTag)vNode.Tag;

                // 表示文字列の指定
                if (Mode == SelectMode.DRILLDOWN)
                {
                    vNode.Text = $"{tag.FolderName} ({tag.DocumentCount})";
                }
                else
                {
                    vNode.Text = $"{tag.FolderName}";
                }
            }
        }

        /// <summary>
        /// 表示後少ししてから実行する処理
        /// </summary>
        private void delayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();
            UpdateFolderTree();
        }

        /// <summary>
        /// ノード選択時処理
        /// </summary>
        private void TreeFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = (FolderNodeTag)e.Node.Tag;
            SelectedFolderPath = tag.Path;
        }

        /// <summary>
        /// ツリーノード展開前の処理
        /// </summary>
        private void TreeFolder_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // 展開しようとしたノードの子ノードをまだ実体化していなければ、実体化する
            var node = (TreeNodeEx)e.Node;
            if (!node.SubFoldersAdded)
            {
                // ダミーノードをクリア
                node.Nodes.Clear();

                // 対応する仮想ノードを取得
                var tag = (FolderNodeTag)node.Tag;
                var vNode = AllVirtualNodeDict[tag.Path];

                // 子の仮想ノードを全て実体化
                foreach (var vChildNode in vNode.Nodes)
                {
                    var newNode = RealizeVirtualNode(vChildNode);
                    node.Nodes.Add(newNode);
                }

                node.SubFoldersAdded = true;
            }
        }

        /// <summary>
        /// 仮想ノードの実体化
        /// </summary>
        private TreeNodeEx RealizeVirtualNode(VirtualNode vNode)
        {
            var node = new TreeNodeEx(vNode.Text);
            node.Tag = vNode.Tag;

            // フォルダアイコンのイメージリストを取得して、そのノードにセット
            var systemImgListHandle = IntPtr.Zero;
            int iconImageIndex;
            if (IconFetcher.GetSystemImageListInfo(vNode.Tag.Path, out systemImgListHandle, out iconImageIndex))
            {
                node.ImageIndex = iconImageIndex;
                node.SelectedImageIndex = iconImageIndex;
            };

            // 子がいればダミーノード追加（展開可能とするため）
            if (vNode.Nodes.Count > 0)
            {
                node.Nodes.Add(new TreeNodeEx("[dummy]"));
            }

            // 初回実体化時のみ、ツリーにシステム画像のハンドルを設定
            if (!isSetTreeFolderIconImageList)
            {
                IconFetcher.SetImageListToTreeView(TreeFolder, systemImgListHandle);
                isSetTreeFolderIconImageList = true;
            }

            // 仮想ノードと実体ノードを紐付け
            vNode.RealNode = node;

            // 結果を返却
            return node;
        }

        /// <summary>
        /// ツリーを対象の仮想ノード部分まで展開する
        /// </summary>
        /// <param name="vNode">対象仮想ノード</param>
        private void ExpandTreeToVirtualNode(VirtualNode vNode)
        {
            var ancestorVirtualNodes = new List<VirtualNode>() { }; // 仮想ノードリスト。子→親の順で格納する

            // 親をたどる
            ancestorVirtualNodes.Add(vNode);
            var targetNode = vNode;
            while (targetNode.Parent != null)
            {
                ancestorVirtualNodes.Add(targetNode.Parent);
                targetNode = targetNode.Parent;
            }

            // ルートから子に向けて、1段ずつ実ノードを展開（このタイミングで実体化も行う）
            TreeNodeEx lastNode = null;
            foreach (var vNode2 in ((IEnumerable<VirtualNode>)ancestorVirtualNodes).Reverse())
            {
                vNode2.RealNode.Expand();
                lastNode = vNode2.RealNode;
            }

            // 対象ノードを選択
            TreeFolder.SelectedNode = lastNode;
        }
    }

}
