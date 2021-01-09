using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Semver;

namespace InazumaSearch.Core
{
    public class Util
    {
        /// <summary>
        /// バージョンを取得 (セマンティックバージョニング準拠)
        /// </summary>
        public static SemVersion GetVersion()
        {
            // アセンブリバージョンを取得
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmVer = asm.GetName().Version;

            // アセンブリバージョンをSemVer形式とする
            return new SemVersion(asmVer.Major, asmVer.Minor, asmVer.Build, SystemConst.PreReleaseVersion);
        }

        /// <summary>
        /// ポータブル版かどうかを判定
        /// </summary>
        /// <returns></returns>
        public static bool IsPortableMode()
        {
#if PORTABLE
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// プラットフォーム文字列を取得 "x86" or "x64"
        /// </summary>
        /// <returns></returns>
        public static string GetPlatform()
        {
            return (Environment.Is64BitProcess ? "x64" : "x86");
        }

        /// <summary>
        /// ファイルのフルパスからDocumentsテーブルのキー文字列を生成
        /// </summary>
        public static string MakeDocumentFileKey(string fullPath)
        {
            return "f:" + fullPath.ToLower();
        }

        /// <summary>
        /// フォルダのフルパスからDocumentsテーブルを検索するためのキー文字列プレフィックスを生成
        /// このプレフィックスで前方一致検索を行うことで、「指定したフォルダ内の全文書ファイル」を取得可能
        /// </summary>
        public static string MakeDocumentDirKeyPrefix(string dirPath)
        {
            // 名前が部分一致する別のフォルダを誤って検索対象としないように、フォルダパスの最後に\を付ける
            // （ただし、ドライブ直下の場合はすでに「C:\」のようなパスとなっているため、それも考慮）
            var lowerPath = dirPath.ToLower();
            if (!lowerPath.EndsWith(@"\")) lowerPath = lowerPath + @"\";
            return "f:" + lowerPath;
        }

        /// <summary>
        /// 16進数の文字列形式でハッシュを算出
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HexDigest(HashAlgorithm cryptProvider, string source)
        {
            var digest = cryptProvider.ComputeHash(Encoding.UTF8.GetBytes(source));
            return BitConverter.ToString(digest).ToLower().Replace("-", "");
        }

        #region ダイアログボックス表示

        public static void ShowInformationMessage(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ShowErrorMessage(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool Confirm(string message, bool defaultNo = false)
        {
            var res = MessageBox.Show(
                  message
                , "確認"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Warning
                , (defaultNo ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1));

            return (res == DialogResult.Yes);
        }

        public static bool Confirm(IWin32Window owner, string message, bool defaultNo = false)
        {
            var res = MessageBox.Show(owner
                , message
                , "確認"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Warning
                , (defaultNo ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1));

            return (res == DialogResult.Yes);
        }

        #endregion

        #region 文字列フォーマット

        public static string FormatFileSizeByKB(long size)
        {
            // KB単位で表記 (小数点以下切り上げ)
            var byKB = Math.Ceiling(size / (decimal)1024);

            return string.Format("{0:#,0} KB", byKB);
        }

        public static string FormatFileSizeByMB(long size)
        {
            // MB単位で表記 (小数点以下切り上げ)
            var byMB = Math.Ceiling(size / (decimal)1024 / 1024);

            return string.Format("{0:#,0} MB", byMB);
        }

        public static string FormatFileSizeByGB(long size)
        {
            // GB単位で表記 (小数点第二位まで)
            var byGB = Math.Round(size / (decimal)1024 / 1024 / 1024, 2);

            return string.Format("{0:#,0.00} GB", byGB);
        }

        #endregion

        #region ファイル・フォルダに対する一括操作

        /// <summary>
        /// フォルダ内直下のファイルすべてに対して処理 (ファイル一覧の取得時や、ファイルへの処理時に例外が発生した場合は無視)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        public static void ApplyFiles(NLog.Logger logger, string folder, Action<string> fileAction)
        {
            try
            {
                foreach (var path in Directory.GetFiles(folder))
                {
                    try
                    {
                        fileAction(path);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // キャンセル操作の場合は外に投げる
                        throw (ex);
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.ToString());
                        logger.Debug($"Skip file action - {path}");
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.ToString());
                logger.Debug($"Ignore dir - {folder}");
            }
        }

        /// <summary>
        /// 直下の子ディレクトリすべてに対して処理 (フォルダ一覧の取得時や、フォルダへの処理時に例外が発生した場合は無視)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        public static void ApplySubDirectories(NLog.Logger logger, string folder, Action<string> dirAction)
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(folder))
                {
                    try
                    {
                        dirAction(dir);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // キャンセル操作の場合は外に投げる
                        throw (ex);
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.ToString());
                        logger.Debug($"Skip dir action - {dir}");
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.ToString());
                logger.Debug($"Ignore dir - {folder}");
            }
        }

        /// <summary>
        /// 子ディレクトリすべてに対して処理 (サブディレクトリを再帰的に検索し、例外が発生したファイルは無視する)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        /// <remarks>https://stackoverflow.com/questions/172544/ignore-folders-files-when-directory-getfiles-is-denied-access</remarks>
        public static void ApplyAllDirectories(NLog.Logger logger, string folder, Action<string> dirAction)
        {
            try
            {
                foreach (var dirPath in Directory.GetDirectories(folder))
                {
                    try
                    {
                        dirAction(dirPath);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // キャンセル操作の場合は外に投げる
                        throw (ex);
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.ToString());
                        logger.Debug($"Skip dir action - {dirPath}");
                    }
                }
                foreach (var dirPath in Directory.GetDirectories(folder))
                {
                    ApplyAllDirectories(logger, dirPath, dirAction);
                }
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.ToString());
                logger.Debug($"Ignore dir - {folder}");
            }
        }

        /// <summary>
        /// 子ファイルすべてに対して処理 (サブディレクトリを再帰的に検索し、例外が発生したファイルは無視する)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        /// <remarks>https://stackoverflow.com/questions/172544/ignore-folders-files-when-directory-getfiles-is-denied-access</remarks>
        public static void ApplyAllFiles(NLog.Logger logger, string dirPath, Action<string> fileAction)
        {
            try
            {
                foreach (var filePath in Directory.GetFiles(dirPath))
                {
                    try
                    {
                        fileAction(filePath);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // キャンセル操作の場合は外に投げる
                        throw (ex);
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.ToString());
                        logger.Debug($"Skip file action - {filePath}");
                    }
                }
                foreach (var subDir in Directory.GetDirectories(dirPath))
                {
                    ApplyAllFiles(logger, subDir, fileAction);
                }
            }
            catch (OperationCanceledException ex)
            {
                // キャンセル操作の場合は外に投げる
                throw (ex);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.ToString());
                logger.Debug($"Ignore dir - {dirPath}");
            }
        }

        #endregion

    }
}
