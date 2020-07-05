//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace InazumaSearch.Core
//{
//    /// <summary>
//    /// Excelのシート名と、シートごとのテキスト内容を抽出する独自実装
//    /// (xdoc2txtではシート位置や抽出できないため、NPOIを利用して独自実装)
//    /// </summary>
//    public class ExcelTextExtractor
//    {
//        #region クラス

//        public class Sheet
//        {
//            public string Name { get; set; }
//            public IList<Row> Rows { get; set; }

//            public Sheet()
//            {
//                Rows = new List<Row>();
//            }
//        }

//        public class Row
//        {
//            public string Text { get; set; }
//            public IList<string> Comments { get; set; }
//        }


//        #endregion

//        public ExcelTextExtractor()
//        {

//        }

//        /// <summary>
//        /// Excelファイルのシート一覧とテキスト内容を抽出する
//        /// </summary>
//        /// <param name="filePath"></param>
//        /// <returns></returns>
//        /// <remarks>from http://hensa40.cutegirl.jp/archives/4093 </remarks> 
//        public IList<Sheet> Extract(string filePath)
//        {
//            var ret = new List<Sheet>();

//            IWorkbook wb; // ワークブックオブジェクト

//            // エクセルファイルの内容を取得したら即クローズ
//            // FileShare.ReadWrite を設定していないと、他プロセスでファイルがオープン
//            // されている場合、例外が発生してしまう。
//            using (FileStream excelfile = new FileStream(filePath,
//                                                        FileMode.Open,
//                                                        FileAccess.Read,
//                                                        FileShare.ReadWrite))
//            {
//                // ワークブックオブジェクトの生成
//                // 第2パラメータで、テキストのみを情報を取得している
//                // style などの情報は取得されない
//                // すべての情報を取得する場合は、ImportOption.All を指定する
//                //wb = new XSSFWorkbook(excelfile);
//                wb = WorkbookFactory.Create(excelfile, ImportOption.All);
//            }

//            // シート数を取得
//            int sheetCount = wb.NumberOfSheets;

//            // シート数分ループ
//            for (int i = 0; i < sheetCount; i++)
//            {
//                // シート名を取得して、シートリストに保存
//                var npoiSheet = wb.GetSheetAt(i);
//                var sheet = new Sheet() { Name = npoiSheet.SheetName };
//                ret.Add(sheet);

//                for (var r = npoiSheet.FirstRowNum; r <= npoiSheet.LastRowNum; r++)
//                {
//                    var lineBuf = new StringBuilder();
//                    var row = new Row();
//                    row.Comments = new List<string>();
//                    var npoiRow = npoiSheet.GetRow(r);
//                    if (npoiRow != null)
//                    {
//                        foreach (var cell in npoiRow)
//                        {
//                            CellType valueType;
//                            if (cell.CellType == CellType.Formula)
//                            {
//                                valueType = cell.CachedFormulaResultType;
//                            } else
//                            {
//                                valueType = cell.CellType;
//                            }

//                            switch (valueType)
//                            {
//                                case CellType.String:
//                                    lineBuf.Append(cell.StringCellValue);
//                                    break;
//                                case CellType.Numeric:
//                                    lineBuf.Append(cell.NumericCellValue);
//                                    break;
//                                case CellType.Boolean:
//                                    lineBuf.Append(cell.BooleanCellValue);
//                                    break;

//                                case CellType.Error:
//                                    lineBuf.Append(FormulaError.ForInt(cell.ErrorCellValue).String);
//                                    break;
//                                default:
//                                    lineBuf.Append("[UNKNOWN]");
//                                    break;
//                            }
//                            lineBuf.Append("\t");

//                        }
//                    }

//                    row.Text = lineBuf.ToString().TrimEnd('\t');
//                    sheet.Rows.Add(row);


//                    // すべての図を取得
//                    var patriarch = npoiSheet.DrawingPatriarch;

//                    // xlsx / xlsmの場合
//                    if(patriarch is XSSFDrawing)
//                    {
//                        var shapes = ((XSSFDrawing)patriarch).GetShapes();
//                        foreach(var shape in shapes)
//                        {
//                            if(shape is XSSFSimpleShape)
//                            {
//                                var simpleShape = (XSSFSimpleShape)shape;
//                                foreach(var p in simpleShape.TextParagraphs)
//                                {
//                                }
//                            }
//                        }
//                    }

//                    // xlsの場合
//                    if (patriarch is HSSFPatriarch)
//                    {
//                        var shapes = ((HSSFPatriarch)patriarch).GetShapes();
//                        foreach (var shape in shapes)
//                        {
//                            if (shape is HSSFSimpleShape)
//                            {
//                                var simpleShape = (HSSFSimpleShape)shape;
//                            }
//                        }
//                    }
//                }

//            }
//            return ret;
//        }

//        /// <summary>
//        /// Excelファイルのシート一覧とテキスト内容を抽出する (xdoc2txtライクな文字列で出力)
//        /// </summary>
//        /// <param name="filePath"></param>
//        /// <returns></returns>
//        public string ExtractToString(string filePath)
//        {
//            var sheets = Extract(filePath);

//            var buf = new StringBuilder();
//            foreach (var sheet in sheets)
//            {
//                var sheetComments = new List<string>();

//                buf.AppendLine(string.Format("[{0}]", sheet.Name));
//                foreach (var row in sheet.Rows)
//                {
//                    if (!string.IsNullOrWhiteSpace(row.Text)) buf.AppendLine(row.Text);
//                    if (row.Comments.Count >= 1) sheetComments.AddRange(row.Comments);
//                }

//                foreach(var comment in sheetComments)
//                {
//                    buf.AppendLine(comment);
//                }
//            }

//            return buf.ToString();
//        }
//    }
//}
