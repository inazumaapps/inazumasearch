using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            return new SemVersion(asmVer.Major, asmVer.Minor, asmVer.Build);
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
        /// 16進数の文字列形式でハッシュを算出。使用にはCryptProviderが必要
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HexDigest(SHA1CryptoServiceProvider cryptProvider, string source)
        {
            var digest = cryptProvider.ComputeHash(Encoding.UTF8.GetBytes(source));
            return BitConverter.ToString(digest).ToLower().Replace("-", "");
        }

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

        /// <summary>
        /// 現在日を元に、指定した日付の分類名を取得（最終更新日の絞り込みなどで使用）
        /// </summary>
        public static string FormatTimeClass(DateTime timeClass, DateTime now)
        {
            if (Groonga.Util.ToUnixTime(timeClass) == 0)
            {
                return $"{now.Year - SystemConst.GROUPING_YEAR_RANGE - 1}年以前";
            }
            else if (now.Date == timeClass.Date)
            {
                return "今日";
            }
            else if (now.Date.AddDays(-1) == timeClass.Date)
            {
                return "昨日";
            }
            else if (now.Year == timeClass.Year && now.Month == timeClass.Month)
            {
                return "今月";
            }
            else if (now.Year == timeClass.Year)
            {
                return $"{timeClass.Month}月";
            }
            else
            {
                return $"{timeClass.Year}年";
            }
        }

    }
}
