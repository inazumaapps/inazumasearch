using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InazumaSearch.Core
{
    public class IconFetcher
    {

        /// <summary>
        /// Windows API SHGetFileInfo
        /// </summary>
        /// <remarks>from http://www.pinvoke.net/default.aspx/shell32/SHGetFileInfo.html </remarks>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        protected static extern IntPtr SHGetFileInfo(
          string pszPath,
          uint dwFileAttributes,
          out SHFILEINFO psfi,
          uint cbfileInfo,
          SHGFI uFlags);

        /// <summary>
        /// Windows API SHGetFileInfo
        /// </summary>
        /// <remarks>from http://acha-ya.cocolog-nifty.com/blog/2010/11/1-d49b.html </remarks>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        protected static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);

        public const int LVSIL_NORMAL = 0;
        public const int LVSIL_SMALL = 1;
        public const int TVSIL_NORMAL = 0;
        public const int TVSIL_STATE = 2;
        public const int LVM_SETIMAGELIST = 0x1003;
        public const int TVM_SETIMAGELIST = 0x1109;

        [Flags]
        protected enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        // SHGetFileInfo関数で使用する構造体 (from http://www.pinvoke.net/default.aspx/Structures/SHFILEINFO.html)
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        protected struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        /// <summary>
        /// ".xls" 形式の拡張子から、拡張子に対応するファイルアイコンを取得
        /// </summary>
        /// <param name="extName"></param>
        /// <returns></returns>
        public static Icon GetFileIconFromExtension(string extName)
        {
            // アプリケーション・アイコンを取得
            var shinfo = new SHFILEINFO();
            var hSuccess = SHGetFileInfo("*" + extName, 0, out shinfo,
                (uint)Marshal.SizeOf(shinfo), SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes);
            if (hSuccess != IntPtr.Zero)
            {
                return Icon.FromHandle(shinfo.hIcon);
            }

            return null;
        }


        /// <summary>
        /// システムイメージリストのハンドルと、指定したパスのアイコンインデックスを取得
        /// </summary>
        /// <param name="extName"></param>
        /// <returns>取得に成功した場合はtrue、失敗した場合はfalse</returns>
        public static bool GetSystemImageListInfo(string path, out IntPtr imgListHandle, out int imageIndex)
        {
            var shFileInfo = new SHFILEINFO();
            imgListHandle = SHGetFileInfo(
                  path
                , 0
                , out shFileInfo
                , (uint)Marshal.SizeOf(shFileInfo)
                , SHGFI.SmallIcon | SHGFI.SysIconIndex
            );

            imageIndex = shFileInfo.iIcon;
            if (imgListHandle != IntPtr.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the image list for the ListView to the system image list.
        /// </summary>
        /// <remarks>from https://www.ipentec.com/document/document.aspx?page=csharp-create-system-imagelist</remarks>
        public static void SetImageListToListView(ListView targetListView, IntPtr imgListHandle)
        {
            var hRes = SendMessage(targetListView.Handle, LVM_SETIMAGELIST, LVSIL_SMALL, imgListHandle.ToInt32());
            if (hRes != 0)
                Marshal.ThrowExceptionForHR(hRes);
        }

        /// <summary>
        /// Sets the image list for the ListView to the system image list.
        /// </summary>
        /// <param name="tvHandle">The window handle of the TreeView control</param>
        public static void SetImageListToTreeView(TreeView targetTreeView, IntPtr imgListHandle)
        {
            var hRes = SendMessage(targetTreeView.Handle, TVM_SETIMAGELIST, TVSIL_NORMAL, imgListHandle.ToInt32());
            if (hRes != 0)
                Marshal.ThrowExceptionForHR(hRes);
        }
    }
}
