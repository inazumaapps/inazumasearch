using System.Collections.Generic;
using InazumaSearch.PluginSDK.V1;

namespace InazumaSearch.Plugin
{
    public class API : IPluginAPI
    {
        public virtual IDictionary<string, string> TextExtNameToLabelMap { get; set; }
        public virtual IDictionary<string, TextExtractHandler> DocumentTextExtractHandlers { get; set; }

        public API()
        {
            TextExtNameToLabelMap = new Dictionary<string, string>();
            DocumentTextExtractHandlers = new Dictionary<string, TextExtractHandler>();
        }

        /// <summary>
        /// APIのバージョンを表す番号
        /// </summary>
        public int VersionNumber { get { return 1; } }

        /// <summary>
        /// プラグインdllが存在するディレクトリのフルパス
        /// </summary>
        public string BaseDirectoryPath { get; set; }

        /// <summary>
        /// 指定した拡張子のファイルに対するテキスト抽出処理を追加
        /// </summary>
        /// <param name="formatTitle">ファイルのフォーマット名 (検索画面で表示される)</param>
        /// <param name="extensions">対応する拡張子。"txt" 形式で指定 (ピリオドなし)<param>
        /// <param name="handler">テキスト抽出処理。ファイルパス(string)を受け取り、抽出したテキスト(string)を返すメソッドもしくはラムダ式を渡す</param>
        public virtual void RegisterDocumentTextExtractor(string formatTitle, string[] extensions, TextExtractHandler handler)
        {
            foreach (var ext in extensions)
            {
                TextExtNameToLabelMap[ext] = formatTitle;
                DocumentTextExtractHandlers[ext] = handler;
            }
        }
    }
}
