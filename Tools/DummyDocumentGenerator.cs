using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Tools
{
    public partial class DummyDocumentGenerator : Form
    {
        public DummyDocumentGenerator()
        {
            InitializeComponent();
        }

        private void DummyDocumentGenerator_Load(object sender, EventArgs e)
        {
            // TxtOutputFolderPath.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            var destDir = TxtOutputFolderPath.Text;
            if (ChkMakeTimestampSubFolder.Checked)
            {
                destDir = Path.Combine(destDir, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            }

            var res = MessageBox.Show($"以下フォルダに{NumDocCount.Value}件のランダム文書を生成します。よろしいですか？\n{destDir}", "確認", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                Generate(destDir);

                // 出力が完了したらエクスプローラで開く
                System.Diagnostics.Process.Start("explorer.exe", destDir);
            }
        }

        private void Generate(string destDir)
        {
            // ダミーファイルのリストを取得
            var baseDir = Path.GetDirectoryName(Application.ExecutablePath);
            var dummyFiles = new List<string>(Directory.GetFiles(Path.Combine(baseDir, "dummyText")));

            // 出力先フォルダを作る
            Directory.CreateDirectory(destDir);

            // 乱数オブジェクト生成
            var random = new Random(Decimal.ToInt32(NumSeed.Value));

            // 既定回数繰り返し
            var docCount = Decimal.ToInt32(NumDocCount.Value);
            for (var i = 0; i < docCount; i++)
            {
                // サブディレクトリの段数を決める
                var subDirDepth = random.Next(1, 3);

                // 段数分のサブディレクトリを掘る
                var docDestDir = destDir;
                for (var k = 0; k < subDirDepth; k++)
                {
                    docDestDir = Path.Combine(docDestDir, "dir_" + random.Next(1, 8 + 1)); // 1-8のランダムな番号を割り振る
                }

                // 出力先フォルダを作る
                Directory.CreateDirectory(docDestDir);

                // 出力するファイルを選択
                var dummyFile = dummyFiles[random.Next(0, dummyFiles.Count)];

                // 出力
                var destPath = Path.Combine(docDestDir, "file_" + i.ToString("000000") + Path.GetExtension(dummyFile));
                File.Copy(dummyFile, destPath, overwrite: true);
            }
        }
    }
}
