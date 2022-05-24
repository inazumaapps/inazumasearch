using System.Collections.Generic;
using System.Linq;

namespace InazumaSearch.Core
{
    public class Format
    {

        #region 定数 (スタティック変数)

        public static readonly Format EXCEL =
            new Format("excel", "Excel", new[] { "xls", "xlsx", "xlsm", "xlsb" });
        public static readonly Format WORD =
            new Format("word", "Word", new[] { "doc", "docx", "docm", "docxb" });
        public static readonly Format POWERPOINT =
            new Format("powerpoint", "PowerPoint", new[] { "ppt", "pptx", "pptm" });

        public static readonly Format OPENOFFICE_WRITER =
            new Format("openoffice-writer", "OpenOffice Writer", new[] { "sxw" });
        public static readonly Format OPENOFFICE_CALC =
            new Format("openoffice-writer", "OpenOffice Calc", new[] { "sxx" });
        public static readonly Format OPENOFFICE_IMPRESS =
            new Format("openoffice-impress", "OpenOffice Impress", new[] { "sxi" });
        public static readonly Format OPENOFFICE_DRAW =
            new Format("openoffice-writer", "OpenOffice Draw", new[] { "sxd" });


        public static readonly Format ODF_WRITER =
            new Format("odf-writer", "OpenDocument Writer", new[] { "odt" });
        public static readonly Format ODF_SHEET =
            new Format("odf-sheet", "OpenDocument Sheet", new[] { "ods" });
        public static readonly Format ODF_PRESENTATION =
            new Format("odf-presentation", "OpenDocument Presentation", new[] { "odp" });
        public static readonly Format ODF_DRAW =
            new Format("odf-draw", "OpenDocument Draw", new[] { "odg" });


        public static readonly Format ICHITARO =
            new Format("ichitaro", "一太郎", new[] { "jaw", "jbw", "jfw", "jtd" });

        public static readonly Format HTML =
            new Format("html", "HTML", new[] { "html", "htm", "shtml", "mht", "xhtml" });
        public static readonly Format PDF =
            new Format("pdf", "PDF", new[] { "pdf" });
        public static readonly Format RICH_TEXT =
            new Format("richtext", "リッチテキスト", new[] { "rtf" });

        public static readonly Format JAVASCRIPT = new Format("javascript", "JavaScript", new[] { "js", "mjs", "cjs" });
        public static readonly Format TYPESCRIPT = new Format("typescript", "Typecript", new[] { "ts", "mts", "cts" });

        public static readonly Format EML =
           new Format("eml", "メール", new[] { "eml" });

        public static readonly Format[] ALL_DEFAULT_FORMATS =
            new[] {
                  EXCEL, WORD, POWERPOINT
                , OPENOFFICE_WRITER, OPENOFFICE_CALC, OPENOFFICE_IMPRESS, OPENOFFICE_DRAW
                , ODF_WRITER, ODF_SHEET, ODF_PRESENTATION, ODF_DRAW
                , ICHITARO
                , HTML, PDF, EML

                , JAVASCRIPT
                , TYPESCRIPT
            };

        public static ISet<string> SOURCE_CODE_EXTENSIONS = new HashSet<string>();

        #endregion

        #region スタティックコンストラクタ

        static Format()
        {
            foreach (var format in ALL_DEFAULT_FORMATS.Where(f => f.IsSourceCode))
            {
                foreach (var ext in format.Extensions)
                {
                    SOURCE_CODE_EXTENSIONS.Add(ext);
                }
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// フォーマット固有の名前
        /// </summary>
        public string Name { get; set; }

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
        public Format(string name, string caption, IEnumerable<string> extensions, bool sourceCode = false)
        {
            Name = name;
            Label = caption;
            Extensions = new List<string>(extensions);
            IsSourceCode = sourceCode;
        }


    }
}