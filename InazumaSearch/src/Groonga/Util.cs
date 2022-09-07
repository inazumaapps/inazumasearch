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


        public static string EscapeForQuery(string src)
        {
            return "\"" + src.Replace(@"\", @"\\") + "\"";
        }

        public static string EscapeForScript(string src)
        {
            return "\"" + src.Replace("\"", "\"\"").Replace(@"\", @"\\") + "\"";
        }

    }
}
