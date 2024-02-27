namespace InazumaSearch.Forms.CustomControls
{
    /// <summary>
    /// サブノードを展開済かどうかを記憶可能なツリーノード
    /// </summary>
    /// <remarks>from https://atmarkit.itmedia.co.jp/fdotnet/dotnettips/260createtreeview/createtreeview.html </remarks>
    public class TreeNodeEx : System.Windows.Forms.TreeNode
    {
        public TreeNodeEx(string text) :
            base(text)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
        }

        public bool SubFoldersAdded { get; set; } = false;
    }
}
