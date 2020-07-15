//using System;
//using System.Collections.Generic;
//using Alphaleonis.Win32.Filesystem;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using Excel = Microsoft.Office.Interop.Excel;

//namespace InazumaSearch.Core
//{
//    public class ExcelManipulator
//    {
//        [DllImport("user32.dll")]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        static extern bool SetForegroundWindow(IntPtr hWnd);

//        public static void OpenFile(string path)
//        {

//            UsingApp((xlApp) =>
//            {
//                SetForegroundWindow((IntPtr)xlApp.Hwnd);

//                Using(xlApp.Workbooks, (xlWorkbooks) =>
//                {
//                    Using(xlWorkbooks.Open(path), (xlOpenedBook) =>
//                    {
//                        xlOpenedBook.Activate();
//                        Using((Excel.Worksheet)xlOpenedBook.Sheets[1], (xlSheet) =>
//                        {
//                            xlSheet.Activate();
//                            Using((Excel.Range)xlSheet.Cells[1, 1], (xlCell) =>
//                            {
//                                xlCell.Select();
//                            });
//                        });
//                    });

//                });

//            });
//        }

//        public static void UsingApp(Action<Excel.Application> act)
//        {
//            Excel.Application xlApp;
//            try
//            {
//                // すでに起動しているExcelがあれば取得
//                xlApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
//            }
//            catch (Exception)
//            {
//                // なければ新しいExcelを表示状態で起動
//                xlApp = new Excel.Application();
//                xlApp.Visible = true;
//            }

//            Using(xlApp, (xlAppInner) =>
//            {
//                act.Invoke(xlApp);
//            });
//        }

//        public static void Using<T>(T comObject, Action<T> act){

//            if (!Marshal.IsComObject(comObject))
//            {
//                throw new ArgumentException("UsingCom accepts COM object only.");
//            }

//            try
//            {
//                act.Invoke(comObject);

//            } finally {
//                // COMオブジェクトを解放 (参照カウントを1減らす)
//                Marshal.ReleaseComObject(comObject);
//            }
//        }

//    }
//}
