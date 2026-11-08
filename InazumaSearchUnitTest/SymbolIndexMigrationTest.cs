using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using InazumaSearch.Core;
using InazumaSearch.Groonga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InazumaSearchUnitTest
{
    /// <summary>実際のGroongaを使った索引更新・中断復旧のテスト</summary>
    [TestClass]
    public class SymbolIndexMigrationTest
    {
        /// <summary>テストごとに隔離するDBディレクトリ</summary>
        private string _directory;

        /// <summary>同梱エンジンを直接起動するテスト用マネージャー</summary>
        private FixtureManager _manager;

        /// <summary>従来のバージョン4のDBと文書を作成する</summary>
        [TestInitialize]
        public void Initialize()
        {
            _directory = Path.Combine(Path.GetTempPath(), "InazumaSymbolIndexTest-" + Guid.NewGuid());
            Directory.CreateDirectory(_directory);
            _manager = new FixtureManager(_directory);
            _manager.Start();
            _manager.CreateSchemaV1();
            // サジェスト用外部ツールを必要としない、検索索引と文書データのフィクスチャ
            _manager.UpgradeSchemaTo(3);
            _manager.UpgradeSchemaTo(4);
            var records = new List<IDictionary<string, object>>();
            foreach (var symbol in new[] { "▽", "◎", "□" })
            {
                foreach (var prefix in new[] { "", "第1章（", "第1章（ " })
                {
                    records.Add(new Dictionary<string, object>
                    {
                        { "_key", symbol + prefix }, { "body", prefix + symbol + "業務概要）に記載の通り～" },
                        { "title", prefix + symbol + "業務概要" }, { "file_name", prefix + symbol + "業務概要.txt" },
                        { "file_path", "C:\\documents\\" + symbol + prefix + ".txt" },
                        { "folder_path", "C:\\documents\\" }, { "folder_labels", new[] { "▽" } }
                    });
                }
            }
            _manager.Load(records, Table.Documents);
        }

        /// <summary>プロセスを停止し、自分が作った一時DBだけを削除する</summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (_manager != null && _manager.Booting) _manager.Shutdown();
            if (_directory != null && Directory.Exists(_directory)) Directory.Delete(_directory, true);
        }

        /// <summary>自動セットアップでは、同意が必要な更新を適用しない</summary>
        [TestMethod]
        public void SetupPreservesLegacyIndex()
        {
            bool reCrawlRequired;
            _manager.SetupSchema(4, out reCrawlRequired);
            Assert.AreEqual(4L, _manager.GetSchemaVersion());
            Assert.IsFalse(reCrawlRequired);
            Assert.AreEqual(2L, Hits("▽業務概要"));
        }

        /// <summary>再クロールせず全索引を更新し、文書情報とIDを保持する</summary>
        [TestMethod]
        public void MigrationPreservesDocumentsAndFixesSymbolSearch()
        {
            var before = _manager.Select(Table.Documents, limit: -1, sortKeys: new[] { "_id" }).SearchResult;
            _manager.MigrateSymbolIndex();
            Assert.AreEqual(5L, _manager.GetSchemaVersion());
            foreach (var symbol in new[] { "▽", "◎", "□" })
            {
                foreach (var column in new[] { "body", "title", "file_name" })
                    Assert.AreEqual(3L, Hits(symbol + "業務概要", column));
            }
            var after = _manager.Select(Table.Documents, limit: -1, sortKeys: new[] { "_id" }).SearchResult;
            Assert.AreEqual(before.NHits, after.NHits);
            for (var i = 0; i < before.Records.Count; i++)
            {
                Assert.AreEqual(before.Records[i].Id, after.Records[i].Id);
                Assert.AreEqual(before.Records[i].Key, after.Records[i].Key);
                Assert.AreEqual(before.Records[i].GetTextValue("body"), after.Records[i].GetTextValue("body"));
            }
            Assert.AreEqual(9L, _manager.Select(Table.Documents, filter: "folder_path == \"C:\\\\documents\\\\\"", limit: 0).SearchResult.NHits);
            Assert.AreEqual(9L, _manager.Select(Table.Documents, filter: "folder_labels @ \"▽\"", limit: 0).SearchResult.NHits);
            Assert.IsFalse(_manager.ObjectExist(Manager.SymbolIndexBackup));
            Assert.IsFalse(_manager.ObjectExist(Manager.SymbolIndexBuilding));
            _manager.MigrateSymbolIndex();
            Assert.AreEqual(9L, _manager.Select(Table.Documents, limit: 0).SearchResult.NHits);
        }

        /// <summary>空DBでも更新できる</summary>
        [TestMethod]
        public void EmptyDatabaseCanMigrate()
        {
            _manager.Truncate(Table.Documents);
            _manager.MigrateSymbolIndex();
            Assert.AreEqual(5L, _manager.GetSchemaVersion());
            Assert.AreEqual(0L, _manager.Select(Table.Documents, limit: 0).SearchResult.NHits);
        }

        /// <summary>記号列の部分一致への変更と通常の日本語検索を確認する</summary>
        [TestMethod]
        public void SymbolSubstringSearchKeepsJapaneseSearch()
        {
            _manager.Load(new[]
            {
                new Dictionary<string, object> { { "_key", "cpp" }, { "body", "C++" } },
                new Dictionary<string, object> { { "_key", "cp" }, { "body", "C+" } },
                new Dictionary<string, object> { { "_key", "cs" }, { "body", "C#" } }
            }, Table.Documents);
            Assert.AreEqual(1L, Hits("\"C+\""));
            _manager.MigrateSymbolIndex();
            Assert.AreEqual(2L, Hits("\"C+\""));
            Assert.AreEqual(1L, Hits("\"C++\""));
            Assert.AreEqual(1L, Hits("\"C#\""));
            Assert.AreEqual(9L, Hits("業務概要"));
        }

        /// <summary>索引構築失敗時も旧索引を保持し、再試行できる</summary>
        [TestMethod]
        public void BuildFailureRestoresLegacyIndex()
        {
            _manager.ColumnRemove(Table.Documents, "title");
            Assert.ThrowsException<InvalidOperationException>(() => _manager.MigrateSymbolIndex());
            Assert.AreEqual(4L, _manager.GetSchemaVersion());
            Assert.AreEqual(2L, Hits("▽業務概要"));
            Assert.IsFalse(_manager.ObjectExist(Manager.SymbolIndexBuilding));
            _manager.ColumnCreate(Table.Documents, "title", new[] { ColumnCreateFlag.COLUMN_SCALAR }, DataType.Text);
            _manager.MigrateSymbolIndex();
            Assert.AreEqual(5L, _manager.GetSchemaVersion());
        }

        /// <summary>各切り替え段階でプロセスが終了しても、確定した構成へ復旧する</summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void InterruptedSwitchRecovers(int stage)
        {
            _manager.TableCreate(Manager.SymbolIndexBuilding, flags: new[] { TableCreateFlag.TABLE_PAT_KEY },
                keyType: DataType.ShortText, defaultTokenizer: Tokenizer.TokenBigramSplitSymbol, normalizer: Normalizer.NormalizerAuto);
            _manager.ColumnCreate(Manager.SymbolIndexBuilding, "documents_body",
                new[] { ColumnCreateFlag.COLUMN_INDEX, ColumnCreateFlag.WITH_POSITION }, Table.Documents, "body");
            if (stage >= 1) _manager.TableRename(Table.DocumentsIndex, Manager.SymbolIndexBackup);
            if (stage >= 2) _manager.TableRename(Manager.SymbolIndexBuilding, Table.DocumentsIndex);
            if (stage >= 3) _manager.Load(new[] { new Dictionary<string, object> { { "_id", 1 }, { "schema_version", 5 } } }, Table.MetaData);
            _manager.ExecuteCommand("io_flush");
            _manager.Interrupt();
            _manager.Start();
            _manager.RecoverSymbolIndexMigration();
            Assert.AreEqual(stage == 3 ? 5L : 4L, _manager.GetSchemaVersion());
            Assert.AreEqual(stage == 3 ? 3L : 2L, Hits("▽業務概要"));
            Assert.AreEqual(9L, _manager.Select(Table.Documents, limit: 0).SearchResult.NHits);
            Assert.IsFalse(_manager.ObjectExist(Manager.SymbolIndexBackup));
            Assert.IsFalse(_manager.ObjectExist(Manager.SymbolIndexBuilding));
            _manager.RecoverSymbolIndexMigration();
        }

        /// <summary>指定した列への検索件数を取得する</summary>
        private long Hits(string query, string column = "body")
        {
            return _manager.Select(Table.Documents, query: query, matchColumns: new[] { column }, limit: 0).SearchResult.NHits;
        }

        /// <summary>テストホストの配置に依存せず、リポジトリ内のGroongaを利用する</summary>
        private class FixtureManager : Manager
        {
            /// <summary>テスト用DBの保存先を設定する</summary>
            public FixtureManager(string directory) : base(directory, directory, false) { }

            /// <summary>同梱のエンジンを検索して、隔離したDBを開く</summary>
            public void Start()
            {
                var root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (root != null && !File.Exists(Path.Combine(root.FullName, "InazumaSearch.sln"))) root = root.Parent;
                Assert.IsNotNull(root, "リポジトリ内でテストを実行してください。");
                var exe = Path.Combine(root.FullName, "InazumaSearch", "externals", ApplicationEnvironment.GetPlatform(), "groonga", "bin", "groonga.exe");
                Proc = Process.Start(new ProcessStartInfo(exe, (File.Exists(DBPath) ? "" : "-n ") + "\"" + DBPath + "\"")
                {
                    UseShellExecute = false, CreateNoWindow = true, RedirectStandardInput = true,
                    RedirectStandardOutput = true, StandardOutputEncoding = Encoding.UTF8
                });
                Booting = true;
            }

            /// <summary>永続化後の強制終了を再現する</summary>
            public void Interrupt()
            {
                Proc.Kill();
                Proc.WaitForExit();
                Proc.Dispose();
                Booting = false;
            }
        }
    }
}
