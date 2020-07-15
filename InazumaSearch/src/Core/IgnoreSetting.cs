using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            /// マッチ用の正規表現
            /// </summary>
            public virtual Regex Regex { get; set; }
        }

        /// <summary>
        /// .inazumaignore が存在するフォルダのパス。比較のために小文字に変換されている
        /// </summary>
        public virtual string DirPathLower { get; set; }

        /// <summary>
        /// 無視パターンのリスト
        /// </summary>
        public virtual IList<Pattern> Patterns { get; set; } = new List<Pattern>();

        /// <summary>
        /// .inazumaignore ファイルの内容を読み込んでインスタンスを生成
        /// </summary>
        /// <param name="path">.inazumaignore ファイルのパス</param>
        public static IgnoreSetting Load(string path)
        {
            var setting = new IgnoreSetting();
            setting.LoadInternal(path);
            return setting;
        }

        /// <summary>
        /// .inazumaignore ファイルの内容を読み込む
        /// </summary>
        /// <param name="path">.inazumaignore ファイルのパス</param>
        protected virtual void LoadInternal(string path)
        {
            DirPathLower = Path.GetDirectoryName(path).ToLower();
            Patterns.Clear();

            // ファイルの内容を1行ずつ読み込む
            var lines = File.ReadAllLines(path, encoding: Encoding.UTF8);
            foreach (var line in lines)
            {
                // 空行、コメント行はスキップ
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("#")) continue;

                // 正規表現に変換したうえで格納
                // 最初が "/" で始まるならば、.inazumasetting 直下からの相対パスとして適用
                // 最初が "/" で始まらないならば、 サブフォルダ内も含めた全ファイルに適用
                var relative = false;
                string pat;
                if (line.StartsWith("/"))
                {
                    pat = line.Substring(1).Trim();
                    relative = true;
                }
                else
                {
                    pat = line.Trim();
                }


                var regexPattern = Regex.Replace(
                pat,
                ".",
                m =>
                {
                    var s = m.Value;
                    if (s.Equals("?"))
                    {
                        //?は任意の1文字を示す正規表現(.)に変換
                        return ".";
                    }
                    else if (s.Equals("*"))
                    {
                        //*は0文字以上の任意の文字列を示す正規表現(.*)に変換
                        return ".*";
                    }
                    else if (s.Equals("/"))
                    {
                        //スラッシュは\マークと同じ扱い
                        return Regex.Escape("\\");
                    }
                    else
                    {
                        //上記以外はエスケープする
                        return Regex.Escape(s);
                    }
                }
                );
                var regex = new Regex($"^{regexPattern}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                Patterns.Add(new Pattern { Regex = regex, Relative = relative });
            }
        }

        /// <summary>
        /// 無視設定パターンにマッチするかどうかチェック
        /// </summary>
        public virtual bool IsMatch(string filePath)
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
                    if (pattern.Relative)
                    {
                        if (pattern.Regex.IsMatch(relPath))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (pattern.Regex.IsMatch(fileName))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
