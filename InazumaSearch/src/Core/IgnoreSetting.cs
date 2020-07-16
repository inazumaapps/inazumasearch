using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using CefSharp;

namespace InazumaSearch
{
    /// <summary>
    /// .inazumaignore で指定された無視設定
    /// </summary>
    public class IgnoreSetting
    {
        /// <summary>
        /// パターンオブジェクト
        /// </summary>
        public class Pattern
        {
            /// <summary>
            /// 相対パスとして判定するかどうか (falseの場合は全サブディレクトリ内のファイルを検索)
            /// </summary>
            public virtual bool Relative { get; set; }

            /// <summary>
            /// ディレクトリのみを対象とするかどうか
            /// </summary>
            public virtual bool DirectoryOnly { get; set; }

            /// <summary>
            /// ファイルマッチ用の正規表現
            /// </summary>
            public virtual Regex FileRegex { get; set; }

            /// <summary>
            /// ディレクトリマッチ用の正規表現
            /// </summary>
            public virtual Regex DirRegex { get; set; }
        }

        /// <summary>
        /// .inazumaignore が存在するフォルダのパス。比較のために小文字に変換されている
        /// </summary>
        public virtual string DirPathLower { get; protected set; }

        /// <summary>
        /// 無視パターンのリスト
        /// </summary>
        protected virtual IList<Pattern> Patterns { get; set; } = new List<Pattern>();

        public IgnoreSetting(string dirPath)
        {
            DirPathLower = dirPath.ToLower().Replace('/', '\\').TrimEnd('\\'); // 正規化のため、/マークは\に置換。また、最後に\マークがついていれば取り除く
        }

        /// <summary>
        /// .inazumaignore ファイルの内容を読み込んでインスタンスを生成
        /// </summary>
        /// <param name="path">.inazumaignore ファイルのパス</param>
        public static IgnoreSetting Load(string path)
        {
            var setting = new IgnoreSetting(Path.GetDirectoryName(path));
            setting.LoadInternal(path);
            return setting;
        }

        /// <summary>
        /// .inazumaignore ファイルの内容を読み込む
        /// </summary>
        /// <param name="path">.inazumaignore ファイルのパス</param>
        protected virtual void LoadInternal(string path)
        {
            Patterns.Clear();

            // ファイルの内容を1行ずつ読み込む
            var lines = File.ReadAllLines(path, encoding: Encoding.UTF8);
            foreach (var line in lines)
            {
                // 空行、コメント行はスキップ
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("#")) continue;

                // 無視設定を追加
                AddPattern(line);
            }
        }

        /// <summary>
        /// 無視パターンの追加
        /// </summary>
        /// <param name="patternStr"></param>
        public virtual void AddPattern(string patternStr)
        {
            var pattern = new Pattern();

            // 正規化
            patternStr = patternStr.Replace('/', '\\');

            // パターンの末尾が "\" で終わるならば、ディレクトリのみを対象とする
            if (patternStr.EndsWith("\\"))
            {
                pattern.DirectoryOnly = true;
                patternStr = patternStr.TrimEnd('\\');
            }

            // パターンが末尾以外に "\" を含むならば、.inazumasetting 直下からの相対パスとして適用
            // 含まないならば、 サブフォルダ内も含めた全ファイルに適用
            if (patternStr.Contains("\\"))
            {
                pattern.Relative = true;
            }

            // 先頭の "\\" は削除 (ルートパスとして扱わない)
            if (patternStr.StartsWith("\\"))
            {
                patternStr = patternStr.Substring(1);
            }

            // パターン指定を正規表現に変換
            var regexPattern = Regex.Replace(
            patternStr,
            ".",
            m =>
            {
                var s = m.Value;
                if (s.Equals("?"))
                {
                    //?は、任意の1文字（\マーク除く）を示す正規表現に変換
                    return @"[^\\]";
                }
                else if (s.Equals("*"))
                {
                    //*は、0文字以上の任意の文字列（\マーク除く）を示す正規表現に変換
                    return @"[^\\]*";
                }
                else
                {
                    //上記以外はエスケープする
                    return Regex.Escape(s);
                }
            }
            );

            if (pattern.Relative)
            {
                // 相対パス指定の場合 (パス構成要素の前方一致)
                pattern.DirRegex = new Regex($@"^{regexPattern}(?:$|\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (pattern.DirectoryOnly)
                {
                    // フォルダを対象とするパターン（末尾が "\" ）の場合は、ファイルパスに対して末尾にマッチしてはならない
                    pattern.FileRegex = new Regex($@"^{regexPattern}\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                else
                {
                    pattern.FileRegex = pattern.DirRegex;
                }
            }
            else
            {
                // 相対パス指定でない場合 (パス構成要素のうち、どれか1つだけでもマッチするならOK)
                pattern.DirRegex = new Regex($@"(?:^|\\){regexPattern}(?:$|\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (pattern.DirectoryOnly)
                {
                    // フォルダを対象とするパターン（末尾が "\" ）の場合は、ファイルパスに対して末尾にマッチしてはならない
                    pattern.FileRegex = new Regex($@"(?:^|\\){regexPattern}\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                else
                {
                    pattern.FileRegex = pattern.DirRegex;
                }
            }

            // 追加
            Patterns.Add(pattern);
        }

        /// <summary>
        /// 無視設定パターンにマッチするかどうかチェック
        /// </summary>
        public virtual bool IsMatch(string filePath, bool isDirectory)
        {
            // 小文字に変換してから比較
            var pathLower = filePath.ToLower();
            if (pathLower.StartsWith(DirPathLower))
            {
                // 長さが同じなら同フォルダとみなしてスキップ
                if (pathLower.Length == DirPathLower.Length) return false;

                // 相対パスとファイル名取得
                var relPath = pathLower.Substring(DirPathLower.Length + 1);
                var fileName = Path.GetFileName(pathLower);

                // マッチするパターンが1つでもあるならtrue
                foreach (var pattern in Patterns)
                {
                    var regex = (isDirectory ? pattern.DirRegex : pattern.FileRegex);
                    if (regex.IsMatch(relPath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
