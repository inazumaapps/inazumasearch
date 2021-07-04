﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace InazumaSearch.Forms
{
    public partial class DipswForm : Form
    {
        public Core.Application Application { get; set; }

        public DipswForm()
        {
            InitializeComponent();
        }
        public DipswForm(Core.Application app)
        {
            InitializeComponent();
            Application = app;
        }
        private void BtnClearCrawledData_Click(object sender, EventArgs e)
        {
            if (Util.Confirm(this, "クロール時に収集した文書データと、文書のサムネイル画像をクリアします。\nよろしいですか？", defaultNo: true))
            {
                Application.InvokeAfterSuspendingCrawl(this, "文書データをクリアしています...", () =>
                {
                    Application.GM.Truncate(Table.Documents);
                    Application.GM.Truncate(Table.DocumentsIndex);
                    Application.DeleteAllThumbnailFiles();

                });
                UpdateLabels();
            }
        }

        private void BtnClearAllData_Click(object sender, EventArgs e)
        {
            if (Util.Confirm(this, "ユーザー設定を含む全てのデータを初期化します。\nこの操作は取り消せません。\n\nよろしいですか？", defaultNo: true))
            {
                var t = Task.Run(async () =>
                {
                    // 常駐クロール実行中の場合、停止
                    await Application.Crawler.StopAlwaysCrawlIfRunningAsync();

                    // Groongaを停止し、データをクリアして再起動
                    Application.GM.Shutdown();
                    CleanDBFiles();
                    Application.GM.Boot();
                    var dummy = false;
                    Application.GM.SetupSchema(0, out dummy);
                    Application.DeleteAllThumbnailFiles();

                    // ユーザー設定を初期化
                    var newSetting = new UserSetting.Store(Application.UserSettings.SettingFilePath);
                    newSetting.Save();
                    Application.UserSettings.Load();
                });
                var f = new ProgressForm(t, "全てのデータを初期化しています...");
                f.ShowDialog();
                UpdateLabels();
            }
        }

        /// <summary>
        /// データベースファイルをすべて削除
        /// </summary>
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

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void DipswForm_Load(object sender, EventArgs e)
        {
            UpdateLabels();
            lblVersion.Text = Application.GetVersionCaption();
            tblDebug.Visible = Application.DebugMode;
            lnkOpenDataFolder.Visible = Application.DebugMode;
            UpdateExtensionList();

            // 「初期設定へ戻す」ボタンの押下可否を更新
            RefreshBtnResetDocumentDBDirPathEnabled();

        }

        protected void UpdateLabels()
        {
            TxtDocumentDBDirPath.Text = Path.GetFullPath(Application.GM.DBDirPath);
            lblDocumentDBSize.Text = Util.FormatFileSizeByMB(Application.GM.GetDBFileSizeTotal());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = Task.Run(() =>
            {
                Application.GM.ExecuteCommand("defrag");
            });
            var f = new ProgressForm(t, "データベースをデフラグしています...");
            f.ShowDialog();
            UpdateLabels();
        }

        private void BtnAddTextExt_Click(object sender, EventArgs e)
        {
            var f = new ExtNameAddDialog();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                var newExtensions = new List<UserSetting.Extension>(Application.UserSettings.TextExtensions);
                foreach (var extName in f.ExtNames)
                {
                    newExtensions.Add(new UserSetting.Extension() { ExtName = extName.TrimStart('.').ToLower(), Label = f.ExtLabel });
                }

                // 設定ファイルに保存
                Application.UserSettings.SaveTextExtNames(newExtensions.Distinct().OrderBy(ext => ext.ExtName).ToList());

                // フォーマット一覧の更新
                Application.RefreshFormats();

                // リストボックスの表示を更新
                UpdateExtensionList();

                // 常駐クロール実行中であれば再起動
                Application.RestartAlwaysCrawlIfRunning(this);
            }
        }

        protected virtual void UpdateExtensionList()
        {
            lsvTextExtensions.Items.Clear();
            foreach (var ext in Application.UserSettings.TextExtensions)
            {
                lsvTextExtensions.Items.Add(new ListViewItem(new[] { "." + ext.ExtName, ext.Label }));
            }
        }

        private void lsvTextExtensions_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnDeleteTextExt.Enabled = (lsvTextExtensions.SelectedIndices.Count >= 1);
        }

        private void BtnDeleteTextExt_Click(object sender, EventArgs e)
        {
            var selectedExtName = lsvTextExtensions.SelectedItems[0].Text.TrimStart('.').ToLower();
            lsvTextExtensions.SelectedItems[0].Selected = false;

            // 設定ファイルに保存
            var remIndex = Application.UserSettings.TextExtensions.FindIndex(ext => ext.ExtName == selectedExtName);
            Application.UserSettings.TextExtensions.RemoveAt(remIndex);
            Application.UserSettings.Save();

            // フォーマット一覧の更新
            Application.RefreshFormats();

            // リストボックスの表示を更新
            UpdateExtensionList();

            // 常駐クロール実行中であれば再起動
            Application.RestartAlwaysCrawlIfRunning(this);
        }

        private void lnkOpenDataFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", $"\"{Application.UserSettingDirPath}\"");
        }

        /// <summary>
        /// 文書DBフォルダパスの変更ボタンを押下
        /// </summary>
        private void BtnChangeDocumentDBDirPath_Click(object sender, EventArgs e)
        {
            var msg = $@"
文書DBフォルダは、高速な検索のために最適化するため
数百MB～数GBのファイルサイズになる可能性があります。
ネットワーク上やUSBメモリ上に保存先を変更する場合は、十分にご注意ください。
（現在のファイルサイズ：{Util.FormatFileSizeByMB(1646)}）

保存先フォルダを変更してよろしいですか？
";
            if (Util.Confirm(msg, defaultNo: true))
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;
                    dialog.InitialDirectory = TxtDocumentDBDirPath.Text;

                    var ret = dialog.ShowDialog();

                    if (ret == CommonFileDialogResult.Ok)
                    {
                        // メイン処理へ移行
                        ChangeDocumentDBDirPathMain(dialog.FileName);
                    }
                }
            }
        }

        /// <summary>
        /// 文書DBフォルダパスのリセットボタンを押下
        /// </summary>
        private void BtnResetDocumentDBDirPath_Click(object sender, EventArgs e)
        {
            // メイン処理へ移行
            ChangeDocumentDBDirPathMain(Application.DefaultDBDirPath);

        }

        /// <summary>
        /// 文書DBフォルダパス変更メイン処理
        /// </summary>
        /// <param name="newDirPath"></param>
        protected virtual void ChangeDocumentDBDirPathMain(string newDirPath)
        {
            // 空フォルダでなければ警告
            string confirmMsg;
            if (Directory.GetFiles(newDirPath).Any())
            {
                confirmMsg = $"指定したフォルダは空ではありません。\n本当に文書DBの保存先を下記フォルダに変更してよろしいですか？\n{newDirPath}";
            }
            else
            {
                confirmMsg = $"文書DBの保存先を下記フォルダに変更します。\nよろしいですか？\n{newDirPath}";
            }

            if (!Util.Confirm(confirmMsg, defaultNo: true))
            {
                // キャンセルした場合は中断
                return;
            }

            // フォルダ変更処理
            var t = Task.Run(() =>
            {
                string errorMessage;
                if (!Application.ChangeDocumentDBDir(newDirPath, out errorMessage))
                {
                    Util.ShowErrorMessage(errorMessage);
                }
            });
            var f = new ProgressForm(t, "文書DBフォルダを変更しています...");
            f.ShowDialog();

            // 表示されているパスを置き換え
            TxtDocumentDBDirPath.Text = newDirPath;

            // 「初期設定へ戻す」ボタンの押下可否を更新
            RefreshBtnResetDocumentDBDirPathEnabled();
        }

        /// <summary>
        /// 「初期設定へ戻す」ボタンの押下可否更新
        /// </summary>
        protected void RefreshBtnResetDocumentDBDirPathEnabled()
        {
            BtnResetDocumentDBDirPath.Enabled = (TxtDocumentDBDirPath.Text.ToLower() != Application.DefaultDBDirPath.ToLower());
        }
    }
}
