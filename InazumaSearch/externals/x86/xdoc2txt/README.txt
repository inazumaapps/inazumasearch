□xd2txlib.dllについて

xdoc2txtのDLL版です。
著作権・利用条件は、xdoc2txtに準じます。


□配布ファイル

xd2txlib.dll	xdoc2txt DLL版
Sample1/*	C# サンプルプログラム(DllImport版：非推奨)
Sample2/*	VB.Net サンプルプログラム(DllImport版：非推奨)
Sample3/*	C++ サンプルプログラム
Sample4/*	C# サンプルプログラム(LoadLibrary版：推奨)
Sample5/*	VB.Net サンプルプログラム(LoadLibrary版：推奨：Visual Studio2010用)
FileFind/*	C# サンプルプログラム（ファイル検索クライアントの例）

□注意事項

・xd2txlib.dll は32bitなので、xd2txlib.dllを呼び出すアプリケーションをWindows 64bit版で
　開発するときは、ターゲットCPUをx86にする必要があります。
	C#	プロパティ→ビルド→プラットフォームターゲット(G): AnyCPU→x86
	VB.Net	プロパティ→コンパイル→詳細コンパイルオプション(A)→ターゲットCPU(U):AnyCPU→x86

・xd2txlib.dllはCOMオブジェクトではありませんので、DllImportまたはLoadLibraryで呼び出す必要があります。
　具体的な使用方法はサンプルプログラムを参照してください。

・【重要】xd2txlib.dllのロード方法は、DllImportまたはLoadLibraryの方法がありますが、
　DllImport版はアプリケーションが終了するまでメモリが解放されないため、
　予期しない入力ファイルによるクラッシュが起きた場合にアプリケーション自体が異常終了する可能性があります。
  連続してテキスト抽出を行う場合は、1ファイルの抽出毎にLoadLibrary & FreeLibraryで
　毎回xd2txlib.dllをロード・解放することをお勧めします。Sample4,Sample5を参照してください。

・xd2txlib.dll(xdoc2txt)はメモリ上でファイルの展開およびエンコードを行うため、実メモリを超える巨大なファイルは扱えません。
・xd2txlib.dllはサポートしている拡張子以外はテキストファイルと見なしてエンコードを試みます。バイナリファイルでは使用しないでください。


定義：
int ExtractText(BSTR lpFileName,	// 入力ファイル名
		bool bProp,		// T:プロパティの抽出 F:本文テキストの抽出
		BSTR *lpFileText)	// 抽出したテキスト

int ExtractTextEx(BSTR lpFileName,	// 入力ファイル名
		bool bProp,		// T:プロパティの抽出 F:本文テキストの抽出
		BSTR lpOptions,		// コマンドラインオプション(-r -o -g -x のみ有効)
		BSTR *lpFileText)	// 抽出したテキスト


	テキストの抽出結果はUTF16です。


C#の例(1)：DllImport版

       [DllImport("xd2txlib.dll", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl)]
       public static extern int ExtractText(
       [MarshalAs(UnmanagedType.BStr)] String lpFileName,
       bool bProp,
       [MarshalAs(UnmanagedType.BStr)] ref String lpFileText);

	string fileName = "sample.doc";
	string fileText = "";
	int fileLength = ExtractText( fileName, false, ref fileText );

C#の例(2)：動的ロード版（推奨）

        // LoadLibrary、FreeLibrary、GetProcAddressをインポート
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // 関数をdelegateで宣言する
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ExtractText(
          [MarshalAs(UnmanagedType.BStr)] String lpFileName,
          bool bProp,
          [MarshalAs(UnmanagedType.BStr)] ref String lpFileText);


	string fileName = "sample.doc";
        string fileText = "";
        // 動的にdllをロードし、使用後に解放
        IntPtr handle = LoadLibrary("xd2txlib.dll");
        IntPtr funcPtr = GetProcAddress(handle, "ExtractText");

        ExtractText extractText = (ExtractText)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(ExtractText));
        int fileLength = extractText(fileName, false, ref fileText);
        FreeLibrary(handle);


VB.Netの例(1)：DllImport版

    <DllImport("xd2txlib.dll", SetLastError:=True, CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.Cdecl)> _
    Public Shared Function _
        ExtractText( _
        <MarshalAs(UnmanagedType.BStr)> ByVal lpFileName As String, _
         ByVal bProp As Boolean, _
         <MarshalAs(UnmanagedType.BStr)> ByRef lpFileText As String _
        ) As Integer
    End Function


    Dim fileName As String
    Dim fileText As String

    fileName = "sample.doc"
    fileText = ""

    ExtractText( fileName, False, fileText)

VB.Netの例(2)：動的ロード版（推奨）

    '********************************************************
    '*  LoadLibrary,FreeLibrary,GetProcAddress の宣言
    '********************************************************
    <DllImport("kernel32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function LoadLibrary(ByVal lpFileName As String) As IntPtr
    End Function

    <DllImport("kernel32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function FreeLibrary(ByVal hModule As IntPtr) As Boolean
    End Function

    <DllImport("kernel32", CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Private Shared Function GetProcAddress(ByVal hModule As IntPtr, ByVal lpProcName As String) As IntPtr
    End Function

    '********************************************************
    '* ExtractTextの宣言
    '********************************************************
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Delegate Sub ExtractText( _
        <MarshalAs(UnmanagedType.BStr)> ByVal lpFileName As String, _
        ByVal bProp As Boolean, _
        <MarshalAs(UnmanagedType.BStr)> ByRef lpFileText As String _
        )

	Dim fileText As String
	Dim fileName As String
	fileText = ""
	fileName = "sample.doc"

	' xd2txlib.dll をLoadLibrary経由でロード
	Dim TaragetDll As String = "xd2txlib.dll"
	Dim TaragetFunc As String = "ExtractText"
	Dim hModule As UInt32 = LoadLibrary(TaragetDll)
	If hModule = 0 Then
	TextBox1.Text = "xd2txlib.dll not found"
	Exit Sub
	End If

	' 変換の実行
	Dim ptr As IntPtr
	ptr = GetProcAddress(hModule, "ExtractText")

	If ptr <> IntPtr.Zero Then
		Dim dllFunc As ExtractText = _
		Marshal.GetDelegateForFunctionPointer( _
		ptr, _
		GetType(ExtractText) _
		)

		Call dllFunc(fileName, False, fileText)

	End If


	' DLLの解放
	Call FreeLibrary(hModule)


□呼び出し方式

ExtractText  の呼び出し方式はstdcallではなくcdeclですので、呼び出し時に明示してください。
（サンプルプログラム参照）
srdcallで呼び出すと、スタックが不安定になる可能性があります。


□ 履歴

2012/11/15	引数の並びをCOMコンポーネント版と同じにした
2012/11/13	xdoc2txt 2.0のDLL版として配布
2014/06/14	動的ロードのサンプル(Sample4,Sample5)を追加
2015/03/11	呼び出し規約cdeclを明示
