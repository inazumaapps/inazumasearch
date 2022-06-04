namespace InazumaSearch.PluginSDK.V1
{
    public delegate string TextExtractHandler(string path);

    /// <summary>
    /// プラグインの機能にアクセスするためのAPI
    /// </summary>
    public interface IPluginAPI
    {
        /// <summary>
        /// APIのバージョンを表す番号
        /// </summary>
        int VersionNumber { get; }

        /// <summary>
        /// プラグインdllが存在するディレクトリのフルパス
        /// </summary>
        string BaseDirectoryPath { get; }

        /// <summary>
        /// 指定した拡張子のファイルに対するテキスト抽出処理を追加
        /// </summary>
        /// <param name="formatTitle">ファイルのフォーマット名 (検索画面で表示される)。通常は"Excel", "Word"のような名前を指定</param>
        /// <param name="extensions">対応する拡張子。"txt" 形式で指定 (ピリオドなし)<param>
        /// <param name="handler">テキスト抽出処理。ファイルパス(string)を受け取り、テキストの抽出結果(string)を返すメソッドもしくはラムダ式を渡す</param>
        void RegisterDocumentTextExtractor(string formatTitle, string[] extensions, TextExtractHandler handler);

        ///// <summary>
        ///// 検索を実行する
        ///// </summary>
        ///// <param name="formatTitle"></param>
        ///// <param name="extensions"></param>
        ///// <param name="handler"></param>
        //SearchResult Search(string keyword);

        ///// <summary>
        ///// 検索を非同期に実行する
        ///// </summary>
        ///// <param name="formatTitle"></param>
        ///// <param name="extensions"></param>
        ///// <param name="handler"></param>
        //Task<SearchResult> SearchAsync(string keyword);
    }
}
