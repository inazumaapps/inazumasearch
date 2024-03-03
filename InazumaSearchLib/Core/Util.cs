﻿using System;
using System.Security.Cryptography;
using System.Text;
using Alphaleonis.Win32.Filesystem;

namespace InazumaSearchLib.Core
{
    public class Util
    {
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

        #region 文字列フォーマット

        /// <summary>
        /// ファイルサイズをフォーマット　※値に応じて、B / KB / MB / GBのうち適切な単位を自動選択する
        /// </summary>
        public static string FormatFileSize(long size)
        {
            if (size >= 1024 * 1024 * 1024)
            {
                // 1GB以上
                return FormatFileSizeByGB(size);
            }
            else if (size >= 1024 * 1024)
            {
                // 1MB以上
                return FormatFileSizeByMB(size);
            }
            else if (size >= 1024)
            {
                // 1KB以上
                return FormatFileSizeByKB(size);
            }
            else
            {
                // 上記以外
                return string.Format("{0:#,0} B", size);
            }
        }

        /// <summary>
        /// ファイルサイズをKB単位でフォーマット
        /// </summary>
        public static string FormatFileSizeByKB(long size)
        {
            // KB単位で表記 (小数点以下切り上げ)
            var byKB = Math.Ceiling((decimal)size / (decimal)1024);

            return string.Format("{0:#,0} KB", byKB);
        }

        /// <summary>
        /// ファイルサイズをMB単位でフォーマット
        /// </summary>
        public static string FormatFileSizeByMB(long size)
        {
            // MB単位で表記 (小数点以下切り上げ)
            var byMB = Math.Ceiling((decimal)size / (decimal)1024 / (decimal)1024);

            return string.Format("{0:#,0} MB", byMB);
        }

        /// <summary>
        /// ファイルサイズをGB単位でフォーマット
        /// </summary>
        public static string FormatFileSizeByGB(long size)
        {
            // GB単位で表記 (小数点第二位まで)
            var byGB = Math.Round((decimal)size / (decimal)1024 / (decimal)1024 / (decimal)1024, 2);

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