using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;
using Semver;

namespace InazumaSearch.Core
{
    /// <summary>
    /// ユーザー設定
    /// </summary>
    public class UserSetting
    {
        public const int CurrentSettingVersion = 3;

        public static string LastLoadedUserUuid { get; set; }

        public class TargetFolderType
        {
            public const string DocumentFile = "document_file";
            public const string Mail = "mail";
        }

        public class TargetFolder
        {
            public virtual string Type { get; set; }
            public virtual string Path { get; set; }
            public virtual string Label { get; set; }

            /// <summary>
            /// そのフォルダに対する無視設定（行のコレクション）
            /// </summary>
            public virtual List<string> IgnoreSettingLines { get; set; } = new List<string>();
        }

        public class Extension
        {
            public virtual string ExtName { get; set; }
            public virtual string Label { get; set; }
        }

        public class PlainData
        {
            public virtual string UserUuid { get; set; }
            public virtual int SettingVersion { get; set; } = CurrentSettingVersion;
            public virtual List<TargetFolder> TargetFolders { get; set; } = new List<TargetFolder>();
            public virtual DateTime? LastCrawlTime { get; set; } = null;
            public virtual bool AlwaysCrawlMode { get; set; } = false;
            public virtual bool StartUp { get; set; } = false;
            public virtual bool KeywordAutoComplete { get; set; } = true;
            public virtual string LastBootVersion { get; set; } = null;
            public virtual Dictionary<string, int> LastLoadedPluginVersionNumbers { get; set; } = new Dictionary<string, int>();
            public virtual List<Extension> TextExtensions { get; set; } = new List<Extension>();
            public virtual string DocumentDBDirPath { get; set; } = null;
            public virtual List<string> LastExcludingDirPaths { get; set; } = null;
            public virtual int DisplayPageSizeForNormalView { get; set; } = 20;
            public virtual int DisplayPageSizeForListView { get; set; } = 100;
        }

        public class Store
        {
            public string SettingFilePath { get; protected set; }

            public PlainData PlainData { get; protected set; } = new PlainData();

            public Store(string filePath)
            {
                SettingFilePath = filePath;
            }

            public void Save()
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingFilePath));
                File.WriteAllText(SettingFilePath, JsonConvert.SerializeObject(PlainData));
                NLog.LogManager.GetCurrentClassLogger().Trace("設定ファイル保存 - {0}", JsonConvert.SerializeObject(PlainData));
            }

            public void Load()
            {
                PlainData = JsonConvert.DeserializeObject<PlainData>(File.ReadAllText(SettingFilePath));

                // 補正
                foreach (var f in TargetFolders)
                {
                    if (f.Type == null) f.Type = TargetFolderType.DocumentFile;
                }

                // 設定ファイルバージョンが2以下であれば、テキスト拡張子を設定
                if (PlainData.SettingVersion <= 2)
                {
                    PlainData.TextExtensions = new List<Extension>(new[] { new Extension() { ExtName = "txt", Label = "テキスト" } });
                }

                // UUIDをスタティック変数に設定 (エラー情報送信用)
                UserSetting.LastLoadedUserUuid = UserUuid;

                // 設定オブジェクトのバージョンを更新
                PlainData.SettingVersion = CurrentSettingVersion;
            }

            public static Store Setup(string settingFilePath)
            {
                var store = new Store(Path.GetFullPath(settingFilePath));

                // ファイルが存在しない場合は、ユーザーIDを生成して新規ファイルを作成
                if (!File.Exists(store.SettingFilePath))
                {
                    store.PlainData.UserUuid = Guid.NewGuid().ToString();
                    store.Save();
                }

                // 設定ファイルの読み込み
                store.Load();

                // 設定オブジェクトを返す
                return store;
            }

            /// <summary>
            /// 文書データのキーを受け取り、その値を元に「その文書を含んでいるすべての対象フォルダ情報」を取得する
            /// </summary>
            /// <param name="key"></param>
            public virtual IEnumerable<TargetFolder> FindTargetFoldersFromDocumentKey(string key)
            {
                var folders = TargetFolders.Where((f) => key.StartsWith(Util.MakeDocumentDirKeyPrefix(f.Path)));
                return folders;
            }


            #region 個別のユーザー設定値プロパティ

            /// <summary>
            /// 前回起動した最終バージョン。0.17.0以降での起動時のみセットされている（それより前のバージョンではnull）
            /// </summary>
            public SemVersion LastBootVersion { get { return (PlainData.LastBootVersion == null ? null : SemVersion.Parse(PlainData.LastBootVersion)); } }

            /// <summary>
            /// 検索対象フォルダリスト
            /// </summary>
            public List<TargetFolder> TargetFolders { get { return PlainData.TargetFolders; } }

            /// <summary>
            /// 検索対象フォルダリストの設定
            /// </summary>
            public void SaveTargetFolders(List<TargetFolder> folders)
            {
                PlainData.TargetFolders = folders;
                Save();
            }

            /// <summary>
            /// 最終クロール日時
            /// </summary>
            public DateTime? LastCrawlTime { get { return PlainData.LastCrawlTime; } }


            /// <summary>
            /// 常駐クロールモード
            /// </summary>
            public bool AlwaysCrawlMode { get { return PlainData.AlwaysCrawlMode; } }

            /// <summary>
            /// 常駐クロールモードの設定
            /// </summary>
            public void SaveAlwaysCrawlMode(bool value)
            {
                PlainData.AlwaysCrawlMode = value;
                Save();
            }

            /// <summary>
            /// スタートアップ起動
            /// </summary>
            public bool StartUp { get { return PlainData.StartUp; } }

            /// <summary>
            /// スタートアップ起動の設定
            /// </summary>
            public void SaveStartUp(bool value)
            {
                PlainData.StartUp = value;
                Save();
            }

            /// <summary>
            /// 検索キーワードの自動補完
            /// </summary>
            public bool KeywordAutoComplete { get { return PlainData.KeywordAutoComplete; } }

            /// <summary>
            /// 検索キーワードの自動補完ON/OFFの設定
            /// </summary>
            public void SaveKeywordAutoComplete(bool value)
            {
                PlainData.KeywordAutoComplete = value;
                Save();
            }

            /// <summary>
            /// 最後にロードしたプラグインのバージョン番号
            /// </summary>
            public Dictionary<string, int> LastLoadedPluginVersionNumbers { get { return PlainData.LastLoadedPluginVersionNumbers; } }


            /// <summary>
            /// テキストファイルとして登録する拡張子
            /// </summary>
            public List<Extension> TextExtensions { get { return PlainData.TextExtensions; } }

            /// <summary>
            /// テキストファイルとして登録する拡張子の設定
            /// </summary>
            public void SaveTextExtNames(List<Extension> value)
            {
                PlainData.TextExtensions = value;
                Save();
            }

            /// <summary>
            /// 文書DB保存先パス（未設定時は初期フォルダへ保存）
            /// </summary>
            public string DocumentDBDirPath { get { return PlainData.DocumentDBDirPath; } }

            /// <summary>
            /// 文書DB保存先パスの設定
            /// </summary>
            public void SaveDocumentDBDirPath(string value)
            {
                PlainData.DocumentDBDirPath = value;
                Save();
            }


            /// <summary>
            /// ユーザー識別用ID
            /// </summary>
            public string UserUuid { get { return PlainData.UserUuid; } }

            /// <summary>
            /// 最後にクロールした時、選択から除外した検索対象フォルダ一覧。指定なし時（設定された検索対象フォルダが1件しかない場合含む）はnull
            /// </summary>
            public List<string> LastExcludingDirPaths { get { return PlainData.LastExcludingDirPaths; } }

            /// <summary>
            /// 一度に表示する検索結果件数（通常表示）
            /// </summary>
            public int DisplayPageSizeForNormalView { get { return PlainData.DisplayPageSizeForNormalView; } }

            /// <summary>
            /// 一度に表示する検索結果件数の設定（通常表示）
            /// </summary>
            public void SaveDisplayPageSizeForNormalView(int value)
            {
                PlainData.DisplayPageSizeForNormalView = value;
                Save();
            }

            /// <summary>
            /// 一度に表示する検索結果件数（一覧表示）
            /// </summary>
            public int DisplayPageSizeForListView { get { return PlainData.DisplayPageSizeForListView; } }

            /// <summary>
            /// 一度に表示する検索結果件数の設定（一覧表示）
            /// </summary>
            public void SaveDisplayPageSizeForListView(int value)
            {
                PlainData.DisplayPageSizeForListView = value;
                Save();
            }

            #endregion

            #region 一括更新処理

            /// <summary>
            /// 起動完了時の更新処理（最終起動バージョン、最後にロードしたプラグインのバージョン番号を更新する）
            /// </summary>
            public void SaveOnAfterBoot(Dictionary<string, int> lastLoadedPluginVersionNumbers)
            {
                PlainData.LastBootVersion = ApplicationEnvironment.GetVersion().ToString();
                PlainData.LastLoadedPluginVersionNumbers = lastLoadedPluginVersionNumbers;
                Save();
            }

            /// <summary>
            /// クロール実行時の更新
            /// </summary>
            /// <param name="crawlTime">クロール日時</param>
            /// <param name="selectedTargetDirPaths">検索対象として選択したフォルダパス一覧。指定なし時（設定された検索対象フォルダが1件しかない場合含む）はnull</param>
            public void SaveOnCrawl(DateTime crawlTime, IEnumerable<string> selectedTargetDirPaths = null)
            {
                PlainData.LastCrawlTime = crawlTime;
                if (selectedTargetDirPaths != null)
                {
                    // 「選択から除外されたフォルダパスの一覧」を記憶
                    var settingDirPaths = TargetFolders.Select(f => f.Path);
                    PlainData.LastExcludingDirPaths = settingDirPaths.Except(selectedTargetDirPaths).ToList();
                }
                else
                {
                    PlainData.LastExcludingDirPaths = null;
                }
                Save();
            }

            #endregion
        }
    }
}
