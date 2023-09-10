using System.Windows.Forms;

namespace Tools
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void BtnDummyDocumentGenerator_Click(object sender, System.EventArgs e)
        {
            var f = new DummyDocumentGenerator();
            f.Show(this);
        }
    }
}
