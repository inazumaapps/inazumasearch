﻿namespace InazumaSearch.Core
{
    public class Table
    {
        /// <summary>
        /// スキーマ全体の情報を格納するテーブル
        /// </summary>
        public const string MetaData = "MetaData";

        /// <summary>
        /// 文書情報テーブル
        /// </summary>
        public const string Documents = "Documents";

        /// <summary>
        /// 文書情報インデックス
        /// </summary>
        public const string DocumentsIndex = "DocumentsIndex";

    }

    public class Column
    {
        public class MetaData
        {
            /// <summary>
            /// 現在のスキーマバージョン
            /// </summary>
            public const string SCHEMA_VERSION = "schema_version";
        }

        public class Documents
        {
            /// <summary>
            /// 文書キー。Util.MakeFileDocumentKeyでファイルパスから生成する
            /// </summary>
            public const string KEY = "_key";

            /// <summary>
            /// 文書のタイトル
            /// </summary>
            public const string TITLE = "title";

            /// <summary>
            /// 文書の本文テキスト
            /// </summary>
            public const string BODY = "body";

            /// <summary>
            /// フォルダパス
            /// </summary>
            public const string FOLDER_PATH = "folder_path";

            /// <summary>
            /// ファイルパス
            /// </summary>
            public const string FILE_PATH = "file_path";

            /// <summary>
            /// ファイル名
            /// </summary>
            public const string FILE_NAME = "file_name";

            /// <summary>
            /// ファイルサイズ
            /// </summary>
            public const string SIZE = "size";

            /// <summary>
            /// 拡張子 ("txt" 形式で格納。ピリオドなし)
            /// </summary>
            public const string EXT = "ext";

            /// <summary>
            /// ファイル更新日時
            /// </summary>
            public const string FILE_UPDATED_AT = "file_updated_at";

            /// <summary>
            /// ファイル更新年
            /// </summary>
            public const string FILE_UPDATED_YEAR = "file_updated_year";

            /// <summary>
            /// 最後に検索結果を選択した日時
            /// </summary>
            public const string RESULT_SELECTED_AT = "result_selected_at";

            /// <summary>
            /// フォルダラベル
            /// </summary>
            public const string FOLDER_LABELS = "folder_labels";

            /// <summary>
            /// レコードの更新日時
            /// </summary>
            public const string UPDATED_AT = "updated_at";

            /// <summary>
            /// 更新時のアプリケーションのメジャーバージョン
            /// </summary>
            public const string APPLICATION_MAJOR_VERSION_ON_UPDATED = "application_major_version_on_updated";

            /// <summary>
            /// 更新時のアプリケーションのマイナーバージョン
            /// </summary>
            public const string APPLICATION_MINOR_VERSION_ON_UPDATED = "application_minor_version_on_updated";

            /// <summary>
            /// 更新時のアプリケーションのパッチバージョン
            /// </summary>
            public const string APPLICATION_PATCH_VERSION_ON_UPDATED = "application_patch_version_on_updated";
        }
    }
    public class LoggerName
    {
        public const string Application = "Application";
        public const string Groonga = "Groonga";
        public const string Crawler = "Crawler";
        public const string PluginManager = "PluginManager";
        public const string SearchEngine = "SearchEngine";
    }

    public class SystemConst
    {
        /// <summary>
        /// Windowsにおける通常の最大パス長 (このパスを超えると、他アプリケーションで正しく処理できない場合がある)
        /// </summary>
        public const int WindowsMaxPath = 260 - 1; // NULL文字

        /// <summary>
        /// プレリリースバージョン（通常リリースの場合はnull）
        /// </summary>
        public const string PreReleaseVersion = null;
    }
}