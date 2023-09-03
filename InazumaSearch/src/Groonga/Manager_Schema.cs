using System.Collections.Generic;
using System.Diagnostics;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;

namespace InazumaSearch.Groonga
{
    public partial class Manager
    {
        /// <summary>
        /// アプリケーションが要求するスキーマバージョン
        /// </summary>
        public const int AppSchemaVersion = 5;

        /// <summary>
        /// スキーマのセットアップ(新規作成 or アップグレード)を実行
        /// </summary>
        /// <param name="currentSchemaVer"></param>
        public void SetupSchema(long currentSchemaVer, out bool reCrawlRequired)
        {
            var newMode = (currentSchemaVer == 0); // DBを新規作成するかどうかの判定

            // out引数の初期値を設定
            reCrawlRequired = false;

            // スキーマバージョンが0（DBが存在しない）の場合は、バージョン1のスキーマを作成
            if (currentSchemaVer == 0)
            {
                CreateSchemaV1();
                currentSchemaVer = 1;
            }

            // スキーマを更新する
            for (var nextVer = currentSchemaVer + 1; nextVer <= AppSchemaVersion; nextVer++)
            {
                UpgradeSchemaTo(nextVer);

                // 大きく変更が入る場合、再クロールが必要 (DBの新規作成時は除く)
                if (!newMode && (nextVer == 2 || nextVer == 3))
                {
                    reCrawlRequired = true;
                }
            }

            return;
        }

        /// <summary>
        /// DBの現在のスキーマバージョンを取得
        /// </summary>
        /// <returns>スキーマバージョン。DBが存在しない場合(初期化が必要な場合)は0</returns>
        public long GetSchemaVersion()
        {
            if (ObjectExist(Table.MetaData))
            {
                var res = Select(Table.MetaData);
                if (res.SearchResult.NHits >= 1)
                {
                    return (long)res.SearchResult.Records[0][Column.MetaData.SCHEMA_VERSION];
                }
            }

            // DBが存在しない場合は0
            return 0;
        }

        public void CreateSchemaV1()
        {
            TableCreate(Table.MetaData, flags: new[] { TableCreateFlag.TABLE_NO_KEY, TableCreateFlag.TABLE_PAT_KEY });

            ColumnCreate(
                  Table.MetaData
                , Column.MetaData.SCHEMA_VERSION
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.Int32
            );


            TableCreate(Table.Documents, keyType: DataType.ShortText);
            ColumnCreate(
                  Table.Documents
                , Column.Documents.FILE_NAME
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.ShortText
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.FILE_PATH
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.ShortText
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.FILE_UPDATED_AT
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.Time
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.FILE_UPDATED_YEAR
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.Int16
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.SIZE
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.UInt64
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.EXT
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.ShortText
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.TITLE
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.Text
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.BODY
                , new[] { ColumnCreateFlag.COLUMN_SCALAR, ColumnCreateFlag.COMPRESS_ZSTD }
                , DataType.LongText
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.FOLDER_LABELS
                , new[] { ColumnCreateFlag.COLUMN_VECTOR }
                , DataType.ShortText
            );

            ColumnCreate(
                  Table.Documents
                , Column.Documents.RESULT_SELECTED_AT
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.Time
            );
            ColumnCreate(
                  Table.Documents
                , Column.Documents.UPDATED_AT
                , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                , DataType.Time
            );

            // フォルダラベル列と同じテーブルに、フォルダラベル列のインデックスを追加
            // このインデックスを追加することで、ベクターカラムに対する一致検索を行えるようになる
            // (別テーブルへのインデックス追加ではうまくいかない。全文検索用のインデックスではないため？)
            ColumnCreate(
                  Table.Documents
                , "folder_labels_index"
                , new[] { ColumnCreateFlag.COLUMN_INDEX }
                , type: Table.Documents
                , source: Column.Documents.FOLDER_LABELS
            );


            TableCreate(Table.DocumentsIndex
                , flags: new[] { TableCreateFlag.TABLE_PAT_KEY }
                , keyType: DataType.ShortText
                , defaultTokenizer: Tokenizer.TokenBigram
                , normalizer: Normalizer.NormalizerAuto
            );
            ColumnCreate(
                  Table.DocumentsIndex
                , "documents_title"
                , new[] { ColumnCreateFlag.COLUMN_INDEX, ColumnCreateFlag.WITH_POSITION }
                , type: Table.Documents
                , source: Column.Documents.TITLE
            );
            ColumnCreate(
                  Table.DocumentsIndex
                , "documents_file_name"
                , new[] { ColumnCreateFlag.COLUMN_INDEX, ColumnCreateFlag.WITH_POSITION }
                , type: Table.Documents
                , source: Column.Documents.FILE_NAME
            );
            ColumnCreate(
                  Table.DocumentsIndex
                , "documents_body"
                , new[] { ColumnCreateFlag.COLUMN_INDEX, ColumnCreateFlag.WITH_POSITION }
                , type: Table.Documents
                , source: Column.Documents.BODY
            );

            var meta = new Dictionary<string, object> { { Groonga.VColumn.ID, 1 }, { Column.MetaData.SCHEMA_VERSION, 1 } };
            Load(new[] { meta }, table: Table.MetaData);


        }

        public void UpgradeSchemaTo(long nextSchemaVer)
        {
            #region 1 -> 2

            if (nextSchemaVer == 2)
            {

                // サジェスト用の学習用テーブルを初期化
                Shutdown();
                var psi = new ProcessStartInfo(Path.Combine(GroongaDirPath, "groonga-suggest-create-dataset.exe"), string.Format("\"{0}\" query", DBPath))
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
                Boot();

            }

            #endregion

            #region 2 -> 3

            if (nextSchemaVer == 3)
            {
                // アプリケーションバージョンの列を追加
                ColumnCreate(
                      Table.Documents
                    , Column.Documents.APPLICATION_MAJOR_VERSION_ON_UPDATED
                    , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                    , DataType.UInt8
                );
                ColumnCreate(
                      Table.Documents
                    , Column.Documents.APPLICATION_MINOR_VERSION_ON_UPDATED
                    , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                    , DataType.UInt8
                );
                ColumnCreate(
                      Table.Documents
                    , Column.Documents.APPLICATION_PATCH_VERSION_ON_UPDATED
                    , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                    , DataType.UInt8
                );
            }

            #endregion


            #region 3 -> 4

            if (nextSchemaVer == 4)
            {
                // フォルダパスの列を追加
                ColumnCreate(
                      Table.Documents
                    , Column.Documents.FOLDER_PATH
                    , new[] { ColumnCreateFlag.COLUMN_SCALAR }
                    , DataType.ShortText
                );

                UpdateFolderPaths();
            }

            #endregion

            // 更新したスキーマの値を設定
            var meta = new Dictionary<string, object> { { Groonga.VColumn.ID, 1 }, { Column.MetaData.SCHEMA_VERSION, nextSchemaVer } };
            Load(table: Table.MetaData, values: new[] { meta });
        }

        /// <summary>
        /// 文書データのフォルダパス情報を一括更新
        /// </summary>
        public virtual void UpdateFolderPaths()
        {
            var selectRes = Select(
                  Table.Documents
                , outputColumns: new[] { Column.Documents.KEY, Column.Documents.FILE_PATH }
                , limit: -1
                , sortKeys: new[] { Column.Documents.KEY }
            );

            var records = selectRes.SearchResult.Records;
            var values = new List<IDictionary<string, object>>();
            foreach (var record in records)
            {
                var key = (string)record.Key;
                var filePath = (string)record.GetTextValue(Column.Documents.FILE_PATH);

                var valueDict = new Dictionary<string, object>
                {
                    [Column.Documents.KEY] = key
                };

                if (!string.IsNullOrEmpty(filePath))
                {
                    valueDict[Column.Documents.FOLDER_PATH] = Path.GetDirectoryName(filePath);
                }

                values.Add(valueDict);
            }

            Load(table: Table.Documents, values: values);
        }

    }
}
