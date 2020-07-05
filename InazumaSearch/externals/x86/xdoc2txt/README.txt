��xd2txlib.dll�ɂ���

xdoc2txt��DLL�łł��B
���쌠�E���p�����́Axdoc2txt�ɏ����܂��B


���z�z�t�@�C��

xd2txlib.dll	xdoc2txt DLL��
Sample1/*	C# �T���v���v���O����(DllImport�ŁF�񐄏�)
Sample2/*	VB.Net �T���v���v���O����(DllImport�ŁF�񐄏�)
Sample3/*	C++ �T���v���v���O����
Sample4/*	C# �T���v���v���O����(LoadLibrary�ŁF����)
Sample5/*	VB.Net �T���v���v���O����(LoadLibrary�ŁF�����FVisual Studio2010�p)
FileFind/*	C# �T���v���v���O�����i�t�@�C�������N���C�A���g�̗�j

�����ӎ���

�Exd2txlib.dll ��32bit�Ȃ̂ŁAxd2txlib.dll���Ăяo���A�v���P�[�V������Windows 64bit�ł�
�@�J������Ƃ��́A�^�[�Q�b�gCPU��x86�ɂ���K�v������܂��B
	C#	�v���p�e�B���r���h���v���b�g�t�H�[���^�[�Q�b�g(G): AnyCPU��x86
	VB.Net	�v���p�e�B���R���p�C�����ڍ׃R���p�C���I�v�V����(A)���^�[�Q�b�gCPU(U):AnyCPU��x86

�Exd2txlib.dll��COM�I�u�W�F�N�g�ł͂���܂���̂ŁADllImport�܂���LoadLibrary�ŌĂяo���K�v������܂��B
�@��̓I�Ȏg�p���@�̓T���v���v���O�������Q�Ƃ��Ă��������B

�E�y�d�v�zxd2txlib.dll�̃��[�h���@�́ADllImport�܂���LoadLibrary�̕��@������܂����A
�@DllImport�ł̓A�v���P�[�V�������I������܂Ń��������������Ȃ����߁A
�@�\�����Ȃ����̓t�@�C���ɂ��N���b�V�����N�����ꍇ�ɃA�v���P�[�V�������̂��ُ�I������\��������܂��B
  �A�����ăe�L�X�g���o���s���ꍇ�́A1�t�@�C���̒��o����LoadLibrary & FreeLibrary��
�@����xd2txlib.dll�����[�h�E������邱�Ƃ������߂��܂��BSample4,Sample5���Q�Ƃ��Ă��������B

�Exd2txlib.dll(xdoc2txt)�̓�������Ńt�@�C���̓W�J����уG���R�[�h���s�����߁A���������𒴂��鋐��ȃt�@�C���͈����܂���B
�Exd2txlib.dll�̓T�|�[�g���Ă���g���q�ȊO�̓e�L�X�g�t�@�C���ƌ��Ȃ��ăG���R�[�h�����݂܂��B�o�C�i���t�@�C���ł͎g�p���Ȃ��ł��������B


��`�F
int ExtractText(BSTR lpFileName,	// ���̓t�@�C����
		bool bProp,		// T:�v���p�e�B�̒��o F:�{���e�L�X�g�̒��o
		BSTR *lpFileText)	// ���o�����e�L�X�g

int ExtractTextEx(BSTR lpFileName,	// ���̓t�@�C����
		bool bProp,		// T:�v���p�e�B�̒��o F:�{���e�L�X�g�̒��o
		BSTR lpOptions,		// �R�}���h���C���I�v�V����(-r -o -g -x �̂ݗL��)
		BSTR *lpFileText)	// ���o�����e�L�X�g


	�e�L�X�g�̒��o���ʂ�UTF16�ł��B


C#�̗�(1)�FDllImport��

       [DllImport("xd2txlib.dll", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl)]
       public static extern int ExtractText(
       [MarshalAs(UnmanagedType.BStr)] String lpFileName,
       bool bProp,
       [MarshalAs(UnmanagedType.BStr)] ref String lpFileText);

	string fileName = "sample.doc";
	string fileText = "";
	int fileLength = ExtractText( fileName, false, ref fileText );

C#�̗�(2)�F���I���[�h�Łi�����j

        // LoadLibrary�AFreeLibrary�AGetProcAddress���C���|�[�g
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // �֐���delegate�Ő錾����
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ExtractText(
          [MarshalAs(UnmanagedType.BStr)] String lpFileName,
          bool bProp,
          [MarshalAs(UnmanagedType.BStr)] ref String lpFileText);


	string fileName = "sample.doc";
        string fileText = "";
        // ���I��dll�����[�h���A�g�p��ɉ��
        IntPtr handle = LoadLibrary("xd2txlib.dll");
        IntPtr funcPtr = GetProcAddress(handle, "ExtractText");

        ExtractText extractText = (ExtractText)Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(ExtractText));
        int fileLength = extractText(fileName, false, ref fileText);
        FreeLibrary(handle);


VB.Net�̗�(1)�FDllImport��

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

VB.Net�̗�(2)�F���I���[�h�Łi�����j

    '********************************************************
    '*  LoadLibrary,FreeLibrary,GetProcAddress �̐錾
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
    '* ExtractText�̐錾
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

	' xd2txlib.dll ��LoadLibrary�o�R�Ń��[�h
	Dim TaragetDll As String = "xd2txlib.dll"
	Dim TaragetFunc As String = "ExtractText"
	Dim hModule As UInt32 = LoadLibrary(TaragetDll)
	If hModule = 0 Then
	TextBox1.Text = "xd2txlib.dll not found"
	Exit Sub
	End If

	' �ϊ��̎��s
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


	' DLL�̉��
	Call FreeLibrary(hModule)


���Ăяo������

ExtractText  �̌Ăяo��������stdcall�ł͂Ȃ�cdecl�ł��̂ŁA�Ăяo�����ɖ������Ă��������B
�i�T���v���v���O�����Q�Ɓj
srdcall�ŌĂяo���ƁA�X�^�b�N���s����ɂȂ�\��������܂��B


�� ����

2012/11/15	�����̕��т�COM�R���|�[�l���g�łƓ����ɂ���
2012/11/13	xdoc2txt 2.0��DLL�łƂ��Ĕz�z
2014/06/14	���I���[�h�̃T���v��(Sample4,Sample5)��ǉ�
2015/03/11	�Ăяo���K��cdecl�𖾎�
