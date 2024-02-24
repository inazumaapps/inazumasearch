using System;

namespace InazumaSearch.Groonga
{
    public class Util
    {
        public static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static double ToUnixTime(DateTime targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();
            // UNIXエポックからの経過時間を取得
            var elapsedTime = targetTime - UNIX_EPOCH;
            // 経過秒数に変換
            return (double)(elapsedTime.TotalMilliseconds / 1000);
        }

        public static DateTime FromUnixTime(double unixTime)
        {
            // UNIXエポックからの経過秒数で得られるローカル日付
            return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
        }

        public static string ToExprTimeFormat(DateTime targetTime)
        {
            return targetTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
        }

        /// <summary>
        /// Groongaのクエリー構文の値用にエスケープ処理を行う　※検索キーワードには使用しない
        /// </summary>
        public static string EscapeForQueryValue(string src)
        {
            // 先頭・末尾をダブルクォートで囲い、中に含まれるバックスラッシュとダブルクォートをエスケープする
            return "\"" + src.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\"";
        }

        /// <summary>
        /// Groongaのスクリプト構文の文字列値用にエスケープ処理を行う
        /// </summary>
        public static string EscapeForScriptStringValue(string src)
        {
            // クエリー構文と同実装
            return EscapeForQueryValue(src);
        }

    }
}
