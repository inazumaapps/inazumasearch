﻿using System;
using Alphaleonis.Win32.Filesystem;
using Semver;

namespace InazumaSearch.Core
{
    /// <summary>
    /// アプリケーションに関連する環境情報の取得処理を行うクラス
    /// </summary>
    public static class ApplicationEnvironment
    {
        /// <summary>
        /// 文書DBディレクトリの初期パス
        /// </summary>
        public static string DefaultDBDirPath
        {
            get
            {
                if (IsPortableMode())
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data\db");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\db");
                }
            }
        }

        /// <summary>
        /// ログディレクトリのパス
        /// </summary>
        public static string LogDirPath
        {
            get
            {
                if (IsPortableMode())
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data\db");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\db");
                }
            }
        }

        /// <summary>
        /// ユーザー設定ファイルディレクトリのパス
        /// </summary>
        public static string UserSettingDirPath
        {
            get
            {
                if (IsPortableMode())
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data\db");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\db");
                }
            }
        }

        /// <summary>
        /// ユーザー設定ファイルのパス
        /// </summary>
        public static string UserSettingPath
        {
            get
            {
                return Path.Combine(UserSettingDirPath, @"UserSettings.json");
            }
        }

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
        /// バージョン表記文字列を取得
        /// </summary>
        /// <returns></returns>
        public static string GetVersionCaption()
        {
            return $"{GetVersion()}" + (IsPortableMode() ? " ポータブル版" : "") + (GetPlatform() == "x86" ? " (32ビットバージョン)" : "");
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
            return (System.Environment.Is64BitProcess ? "x64" : "x86");
        }
    }
}
