using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using InazumaSearch.Core;

namespace InazumaSearch.Forms
{
    public partial class ProcessStatusForm : Form
    {
        public ProcessStatusForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var proc = Process.GetCurrentProcess();
            proc.Refresh();

            var buf = new StringBuilder();
            buf.AppendLine($"[Current]");
            buf.AppendLine($"Virtual memory size      : {Util.FormatFileSizeByMB(proc.VirtualMemorySize64).PadLeft(20)}");
            buf.AppendLine($"  Paged memory size      : {Util.FormatFileSizeByMB(proc.PagedMemorySize64).PadLeft(20)}");
            buf.AppendLine($"  Working set            : {Util.FormatFileSizeByMB(proc.WorkingSet64).PadLeft(20)}");
            buf.AppendLine($"    Private memory size  : {Util.FormatFileSizeByMB(proc.PrivateMemorySize64).PadLeft(20)}");
            //buf.AppendLine($"Paged system memory size     : {Util.FormatFileSizeByMB(proc.PagedSystemMemorySize64)}");
            //buf.AppendLine($"Non-paged system memory size : {Util.FormatFileSizeByMB(proc.NonpagedSystemMemorySize64)}");
            buf.AppendLine();
            buf.AppendLine($"[Peak]");
            buf.AppendLine($"Virtual memory size      : {Util.FormatFileSizeByMB(proc.PeakVirtualMemorySize64).PadLeft(20)}");
            buf.AppendLine($"  Paged memory size      : {Util.FormatFileSizeByMB(proc.PeakPagedMemorySize64).PadLeft(20)}");
            buf.AppendLine($"  Working set            : {Util.FormatFileSizeByMB(proc.PeakWorkingSet64).PadLeft(20)}");
            buf.AppendLine();
            buf.AppendLine($"[Perf]");
            buf.AppendLine($"CPU Time    : {proc.TotalProcessorTime}");

            mainBox.Text = buf.ToString();
        }
    }
}
