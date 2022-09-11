using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Shell;
using MimeKit;

namespace InazumaSearch.Forms
{
    public partial class DebugForm : Form
    {
        public Core.Application Application { get; set; }

        public DebugForm()
        {
            InitializeComponent();
        }
        public DebugForm(Core.Application app)
        {
            InitializeComponent();
            Application = app;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Application.GM.Shutdown();
            CleanDBFiles();

            Application.GM.Boot();
        }


        public void CleanDBFiles()
        {
            if (!Directory.Exists(Application.GM.DBDirPath))
            {
                Directory.CreateDirectory(Application.GM.DBDirPath);
            }

            if (File.Exists(Application.GM.DBPath))
            {
                foreach (var path in Directory.GetFiles(Application.GM.DBDirPath))
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    else
                    {
                        File.Delete(path);
                    }
                }
            }
        }

        private void TxtIconFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtGetThumbnail_Click(object sender, EventArgs e)
        {
            LoadImage(TxtPath.Text);

        }

        private void LoadImage(string path)
        {
            // OSからサムネイル画像を取得
            var sh = ShellObject.FromParsingName(path);
            //sh.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
            Bitmap bmp = null;
            switch (cmbFormat.SelectedIndex)
            {
                case 0:
                    bmp = sh.Thumbnail.SmallBitmap;
                    break;
                case 1:
                    bmp = sh.Thumbnail.MediumBitmap;
                    break;
                case 2:
                    bmp = sh.Thumbnail.LargeBitmap;
                    break;
                case 3:
                    bmp = sh.Thumbnail.ExtraLargeBitmap;
                    break;
            }
            bmp.Save("out.png", ImageFormat.Png);

            if (ChkTransparent.Checked)
            {
                bmp.MakeTransparent(Color.Black);
            }

            pictureBox1.Image = bmp;

            //byte[] bytes;
            //using (var ms = new MemoryStream())
            //{
            //    bmp.Save(ms, ImageFormat.Png);
            //    bytes = ms.GetBuffer();
            //}
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            cmbFormat.SelectedIndex = 0;

            var objects = Application.GM.ObjectList();
            LsvObjectList.Items.Clear();
            foreach (var objPair in objects)
            {
                if (objPair.Value.path == null) continue;
                var fullPath = Path.Combine(Application.DBDirPath, objPair.Value.path);
                if (!File.Exists(fullPath)) continue;

                var size = new FileInfo(fullPath).Length;
                LsvObjectList.Items.Add(new ListViewItem(new string[] { objPair.Key, Util.FormatFileSizeByMB(size) }));
            }

        }

        private void BtnGetExcelText_Click(object sender, EventArgs e)
        {
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();

            sw1.Start();
            var text1 = XDoc2TxtApi.Extract(TxtPath.Text);
            sw1.Stop();
            sw2.Start();
            //var text2 = new ExcelTextExtractor().ExtractToString(TxtPath.Text);
            //sw2.Stop();

            File.WriteAllText("out_xdoc2txt.txt", text1);
            //File.WriteAllText("out_original.txt", text2);

            Debug.Print("xdoc2txt -> {0}", sw1.Elapsed);
            //Debug.Print("original -> {0}", sw2.Elapsed);
        }

        private void BtnDBBrowser_Click(object sender, EventArgs e)
        {
            var f = new DBBrowserForm(Application);
            f.Show(this);
        }

        private void BtnGetSmallIco_Click(object sender, EventArgs e)
        {
            var ico = IconFetcher.GetFileIconFromExtension(TxtIconPath.Text);
            pictureBox1.Image = ico.ToBitmap();
        }

        private void BtnCheckUpdate_Click(object sender, EventArgs e)
        {
            //// Get a local pointer to the UpdateManager instance
            //var updManager = UpdateManager.Instance;
            //updManager.UpdateSource = new SimpleWebSource(@"http://scl.littlestar.jp/temp/inazumasearch/update/napp-update-feed.xml");
            //updManager.CheckForUpdates();



            //if (updManager.UpdatesAvailable >= 1)
            //{
            //    if (Util.Confirm(this, "Inazuma Searchの更新が見つかりました。アップグレードしますか？\n(管理者権限が必要となる場合があります)"))
            //    {
            //        var t = Task.Run(() => {
            //            try
            //            {
            //                updManager.PrepareUpdates();
            //                updManager.ApplyUpdates();
            //            } catch(Exception) {
            //                updManager.RollbackUpdates();
            //                throw;
            //            }
            //        });
            //        var f = new ProgressForm(t, "Inazuma Searchを更新しています...");
            //        f.ShowDialog(this);
            //        Util.ShowInformationMessage(this, "更新が完了しました。");

            //        return;
            //    }

            //}
            //else
            //{
            //    Util.ShowInformationMessage(this, "更新はありません。");
            //}
            //updManager.CleanUp();
        }



        private void BtnRestart_Click(object sender, EventArgs e)
        {
            Core.Application.Restart();
        }

        private void BtnRaise_Click(object sender, EventArgs e)
        {
            throw new Exception("test exception.");
        }


        private void BtnGetSmallSystemIco_Click(object sender, EventArgs e)
        {
            IntPtr imgListHandle;
            int iconImageIndex;
            IconFetcher.GetSystemImageListInfo(TxtIconPath.Text, out imgListHandle, out iconImageIndex);
            IconFetcher.SetImageListToListView(LstIcons, imgListHandle);
            LstIcons.Items.Add(new ListViewItem(TxtIconPath.Text, iconImageIndex));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Beep.Play();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Question.Play();
        }

        private void BtnOpenExcelFile_Click(object sender, EventArgs e)
        {
            //ExcelManipulator.OpenFile(TxtPath.Text);
        }

        private void BtnAutoUpdate_Click(object sender, EventArgs e)
        {


        }

        private void BtnUpdateDocumentFolderLabels_Click(object sender, EventArgs e)
        {
            var t = Task.Run(() =>
            {
                Application.UpdateDocumentFolderLabels();
            });

            var pf = new ProgressForm(t, "DBのラベル情報を更新中...");
            pf.ShowDialog(this);
        }

        private void BtnShowProcessStatus_Click(object sender, EventArgs e)
        {
            var procStatusForm = new ProcessStatusForm();
            procStatusForm.Show();
        }

        private void BtnParseMBox_Click(object sender, EventArgs e)
        {
            using (var stream = File.OpenRead(TxtPath.Text))
            {
                // パーサを生成
                var parser = new MimeParser(stream, MimeFormat.Mbox);
                while (!parser.IsEndOfStream)
                {
                    // メッセージをパースする
                    var message = parser.ParseMessage();

                    // メッセージを使って何かする
                    Console.WriteLine("[From]");
                    Console.WriteLine(string.Join(Environment.NewLine, message.From.Select(a => a.ToString())));
                    Console.WriteLine("[To]");
                    Console.WriteLine(string.Join(Environment.NewLine, message.To.Select(a => a.ToString())));
                    Console.WriteLine("[Subject]");
                    Console.WriteLine(message.Subject);
                    Console.WriteLine("[TextBody]");
                    Console.WriteLine(message.TextBody);
                    Console.WriteLine("[HtmlBody]");
                    Console.WriteLine(message.HtmlBody);
                    Console.WriteLine("[Attachments]");
                    Console.WriteLine(string.Join(Environment.NewLine, message.Attachments.Select(a => a.ContentDisposition)));
                    Console.WriteLine();
                }
            }
        }

        private void BtnDBDefrag_Click(object sender, EventArgs e)
        {
            long oldSize = 0;
            long newSize = 0;
            var t = Task.Run(() =>
            {
                oldSize = Application.GM.GetDBDiskUsage();
                Application.GM.ExecuteCommand("defrag");
                newSize = Application.GM.GetDBDiskUsage();
            });
            var f = new ProgressForm(t, "データベースをデフラグしています...");
            f.ShowDialog();

            Util.ShowInformationMessage($"ファイルサイズ: {Util.FormatFileSize(oldSize)} -> {Util.FormatFileSize(newSize)} (差分: {Util.FormatFileSize(oldSize - newSize)})");

        }
    }
}
