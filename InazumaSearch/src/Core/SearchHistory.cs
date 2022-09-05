using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace InazumaSearch.Core
{
    /// <summary>
    /// 検索履歴
    /// </summary>
    public class SearchHistory
    {
        /// <summary>
        /// JSON化するための生データ
        /// </summary>
        public class PlainData
        {
            /// <summary>
            /// クエリ情報Dictionary。キーにクエリ文字列、値に検索結果を格納する
            /// </summary>
            public Dictionary<string, QueryData> Queries { get; set; } = new Dictionary<string, QueryData>();

            /// <summary>
            /// クエリの検索履歴情報
            /// </summary>
            public class QueryData
            {
                /// <summary>
                /// 最後に検索した時刻 (UNIX Time)
                /// </summary>
                public long LastSearchAt { get; set; }

                /// <summary>
                /// 累計検索回数
                /// </summary>
                public long SearchCount { get; set; }
            }
        }

        /// <summary>
        /// 検索履歴の保存・読み込みを管理するオブジェクト
        /// </summary>
        public class Store
        {
            private NLog.Logger _logger;

            /// <summary>
            /// ログ出力用オブジェクト
            /// </summary>
            protected NLog.Logger Logger
            {
                get
                {
                    if (_logger == null)
                    {
                        _logger = NLog.LogManager.GetCurrentClassLogger();
                    }
                    return _logger;
                }
            }

            /// <summary>
            /// Base64エンコード/デコードを行う際のエンコーディング
            /// </summary>
            protected Encoding Base64Encoding { get { return Encoding.UTF8; } }

            /// <summary>
            /// ファイルパス
            /// </summary>
            public string FilePath { get; protected set; }

            /// <summary>
            /// 保存用の生データ
            /// </summary>
            public PlainData PlainData { get; protected set; } = new PlainData();

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="filePath">履歴データファイルのパス</param>
            public Store(string filePath)
            {
                FilePath = filePath;
            }

            /// <summary>
            /// 指定したファイルパスの検索履歴データファイルを読み込む。まだ存在しない場合は新規作成
            /// </summary>
            public static Store Setup(string filePath)
            {
                var store = new Store(Path.GetFullPath(filePath));

                // ファイルが存在しない場合は、新規ファイルを作成
                if (!File.Exists(store.FilePath))
                {
                    store.Save();
                }

                // 設定ファイルの読み込み
                store.Load();

                // 設定オブジェクトを返す
                return store;
            }

            /// <summary>
            /// ファイルに保存（排他制御あり）
            /// </summary>
            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Save()
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(PlainData));
                Logger.Trace("検索履歴ファイル保存 (パス: {0})", FilePath);
            }

            /// <summary>
            /// ファイルから読み込む
            /// </summary>
            public void Load()
            {
                PlainData = JsonConvert.DeserializeObject<PlainData>(File.ReadAllText(FilePath));
                Logger.Trace("検索履歴ファイル読込 (パス: {0})", FilePath);
            }

            /// <summary>
            /// 検索実行時の保存処理
            /// </summary>
            /// <param name="query">検索クエリ</param>
            public void SaveOnSearch(string query)
            {
                // クエリが0文字の場合か、30文字を超えている場合は記録しない
                if (query.Length == 0 || query.Length > 30)
                {
                    return;
                }

                // クエリをBase64エンコード
                var encodedQuery = Convert.ToBase64String(Base64Encoding.GetBytes(query));

                // UNIX Timeで現在時刻を取得
                var now = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // クエリ情報記録
                if (PlainData.Queries.ContainsKey(encodedQuery))
                {
                    PlainData.Queries[encodedQuery].SearchCount++;
                    PlainData.Queries[encodedQuery].LastSearchAt = now;
                }
                else
                {
                    PlainData.Queries[encodedQuery] = new PlainData.QueryData() { SearchCount = 1, LastSearchAt = now };
                }

                // 保存
                Save();
            }

            /// <summary>
            /// 検索実行時の保存処理（非同期実行）
            /// </summary>
            /// <param name="query">検索クエリ</param>
            public async void SaveOnSearchAsync(string query)
            {
                await Task.Run(() =>
                {
                    SaveOnSearch(query);
                });
            }


            /// <summary>
            /// 入力欄に表示するための検索候補を取得
            /// </summary>
            public List<string> GetCandidates()
            {
                var res = new List<string>();
                foreach (var queryData in PlainData.Queries)
                {
                    var decodedQuery = Base64Encoding.GetString(Convert.FromBase64String(queryData.Key));
                    res.Add(decodedQuery);
                }
                return res;
            }
        }
    }
}
