using Semver;

namespace InazumaSearchLib
{
    public class Environment
    {
        /// <summary>
        /// ライブラリバージョンを取得 (セマンティックバージョニング準拠)
        /// </summary>
        public static SemVersion GetLibraryVersion()
        {
            // アセンブリバージョンを取得
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmVer = asm.GetName().Version;

            // アセンブリバージョンをSemVer形式とする
            return new SemVersion(asmVer.Major, asmVer.Minor, asmVer.Build);
        }

        /// <summary>
        /// ポータブル版かどうかを判定
        /// </summary>
        /// <returns></returns>
        public static bool IsPortableMode()
        {
#if PORTABLE
            return true;
#else
            return false;
#endif
        }
    }
}
