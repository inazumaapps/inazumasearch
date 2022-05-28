using System.Collections.Generic;
using System.Linq;

namespace InazumaSearch.Core.Format
{
    public class SourceCodeFormat
    {

        #region 定数 (スタティック変数)

        public static readonly SourceCodeFormat JAVASCRIPT = new SourceCodeFormat("javascript", "javascript", "JavaScript", new[] { "js", "mjs", "cjs" });
        public static readonly SourceCodeFormat TYPESCRIPT = new SourceCodeFormat("typescript", "typescript", "TypeScript", new[] { "ts", "mts", "cts" });
        public static readonly SourceCodeFormat VUE = new SourceCodeFormat("vue", "markup", "vue", new[] { "vue" });
        public static readonly SourceCodeFormat CSS = new SourceCodeFormat("css", "css", "CSS", new[] { "css" });
        public static readonly SourceCodeFormat SCSS = new SourceCodeFormat("scss", "scss", "SCSS", new[] { "scss" });

        public static readonly SourceCodeFormat[] ALL_DEFAULT_FORMATS =
            new[] {
                JAVASCRIPT
                , TYPESCRIPT
                , VUE
                , CSS, SCSS
            };

        #endregion

        #region プロパティ

        /// <summary>
        /// フォーマット固有の名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// prism.jsでシンタックスハイライトを行う際に使用する言語名
        /// </summary>
        public string PrismJSLanguageName { get; set; }

        /// <summary>
        /// 表示文字列
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 含まれる拡張子一覧
        /// </summary>
        public IList<string> Extensions { get; set; }

        /// <summary>
        /// ソースコードフラグ
        /// </summary>
        public bool IsSourceCode { get; set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caption"></param>
        /// <param name="extensions"></param>
        public SourceCodeFormat(string name, string prismJSLanguageName, string caption, IEnumerable<string> extensions)
        {
            Name = name;
            PrismJSLanguageName = prismJSLanguageName;
            Label = caption;
            Extensions = new List<string>(extensions);
        }
    }
}