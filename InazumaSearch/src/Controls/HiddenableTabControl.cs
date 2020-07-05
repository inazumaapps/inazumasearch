using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PhalanXware
{
    /// <summary>
    /// thanks to https://qiita.com/PharaohKJ/items/cdce408078c560b6acbc
    /// </summary>
    public partial class HiddenableTabControl : TabControl
    {

        [Browsable(true)]
        [Category("Appearance")]
        public bool HiddenTab { set; get; }

        protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !DesignMode && HiddenTab == true)
                m.Result = (IntPtr)1;
            else
                base.WndProc(ref m);
        }
    }
}
