using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Core
{
    public class XDoc2TxtApi
    {
        // LoadLibrary、FreeLibrary、GetProcAddressをインポート (xdoc2txtのサンプルを参考)
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // 関数をdelegateで宣言する
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ExtractText(
          [MarshalAs(UnmanagedType.BStr)] string lpFileName,
          bool bProp,
          [MarshalAs(UnmanagedType.BStr)] ref string lpFileText);

        public static string Extract(string path)
        {
            int l;

            var fileText = "";
            // 動的にdllをロードし、使用後に解放
            var handle = LoadLibrary($"externals/{Util.GetPlatform()}/xdoc2txt/xd2txlib.dll");
            var funcPtr = GetProcAddress(handle, "ExtractText");

            var extractText = (ExtractText)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(ExtractText));
            l = extractText(path, false, ref fileText);
            FreeLibrary(handle);

            return fileText;
        }
    }


}
