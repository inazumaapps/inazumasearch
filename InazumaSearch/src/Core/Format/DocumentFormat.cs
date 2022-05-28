using System.Collections.Generic;
using System.Linq;

namespace InazumaSearch.Core.Format
{
    public class DocumentFormat
    {

        #region 定数 (スタティック変数)

        public static readonly DocumentFormat EXCEL =
            new DocumentFormat("excel", "Excel", new[] { "xls", "xlsx", "xlsm", "xlsb" });
        public static readonly DocumentFormat WORD =
            new DocumentFormat("word", "Word", new[] { "doc", "docx", "docm", "docxb" });
        public static readonly DocumentFormat POWERPOINT =
            new DocumentFormat("powerpoint", "PowerPoint", new[] { "ppt", "pptx", "pptm" });

        public static readonly DocumentFormat OPENOFFICE_WRITER =
            new DocumentFormat("openoffice-writer", "OpenOffice Writer", new[] { "sxw" });
        public static readonly DocumentFormat OPENOFFICE_CALC =
            new DocumentFormat("openoffice-writer", "OpenOffice Calc", new[] { "sxx" });
        public static readonly DocumentFormat OPENOFFICE_IMPRESS =
            new DocumentFormat("openoffice-impress", "OpenOffice Impress", new[] { "sxi" });
        public static readonly DocumentFormat OPENOFFICE_DRAW =
            new DocumentFormat("openoffice-writer", "OpenOffice Draw", new[] { "sxd" });


        public static readonly DocumentFormat ODF_WRITER =
            new DocumentFormat("odf-writer", "OpenDocument Writer", new[] { "odt" });
        public static readonly DocumentFormat ODF_SHEET =
            new DocumentFormat("odf-sheet", "OpenDocument Sheet", new[] { "ods" });
        public static readonly DocumentFormat ODF_PRESENTATION =
            new DocumentFormat("odf-presentation", "OpenDocument Presentation", new[] { "odp" });
        public static readonly DocumentFormat ODF_DRAW =
            new DocumentFormat("odf-draw", "OpenDocument Draw", new[] { "odg" });


        public static readonly DocumentFormat ICHITARO =
            new DocumentFormat("ichitaro", "一太郎", new[] { "jaw", "jbw", "jfw", "jtd" });

        public static readonly DocumentFormat HTML =
            new DocumentFormat("html", "HTML", new[] { "html", "htm", "shtml", "mht", "xhtml" });
        public static readonly DocumentFormat PDF =
            new DocumentFormat("pdf", "PDF", new[] { "pdf" });
        public static readonly DocumentFormat RICH_TEXT =
            new DocumentFormat("richtext", "リッチテキスト", new[] { "rtf" });

        public static readonly DocumentFormat EML =
           new DocumentFormat("eml", "メール", new[] { "eml" });

        public static readonly DocumentFormat[] ALL_DEFAULT_FORMATS =
            new[] {
                  EXCEL, WORD, POWERPOINT
                , OPENOFFICE_WRITER, OPENOFFICE_CALC, OPENOFFICE_IMPRESS, OPENOFFICE_DRAW
                , ODF_WRITER, ODF_SHEET, ODF_PRESENTATION, ODF_DRAW
                , ICHITARO
                , HTML, PDF, EML
            };

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

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caption"></param>
        /// <param name="extensions"></param>
        public DocumentFormat(string name, string caption, IEnumerable<string> extensions)
        {
            Name = name;
            Label = caption;
            Extensions = new List<string>(extensions);
        }
    }
}