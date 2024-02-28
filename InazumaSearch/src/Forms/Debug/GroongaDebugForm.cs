using System;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace InazumaSearch.Forms
{
    public partial class GroongaDebugForm : Form
    {
        public string Body { get; set; }
        public InazumaSearchLib.Groonga.Manager GM { get; set; }

        public GroongaDebugForm()
        {
            InitializeComponent();
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var res = GM.ExecuteCommandLineAsString(TxtCommand.Text);
            dynamic parsedJson = JsonConvert.DeserializeObject(res);
            TxtResult.Text = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
