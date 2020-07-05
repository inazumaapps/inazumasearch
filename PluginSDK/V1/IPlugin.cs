using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.PluginSDK.V1
{
    public interface IPlugin
    {
        /// <summary>
        /// プラグインAPIの機能へアクセスするためのオブジェクト
        /// </summary>
        IPluginAPI Api { get; set; }

        /// <summary>
        /// プラグインのバージョンを表す番号。機能追加や修正のたびに増加
        /// </summary>
        int VersionNumber { get; }

        /// <summary>
        /// プラグインの読み込み時処理
        /// </summary>
        void Load();
    }
}
