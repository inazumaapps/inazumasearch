namespace InazumaSearch.Groonga
{
    public class VColumn
    {
        public const string ID = "_id";
        public const string KEY = "_key";
        public const string VALUE = "_value";
        public const string SCORE = "_score";
        public const string NSUBRECS = "_nsubrecs";
    }

    public class DataType
    {
        public const string Bool = "Bool";
        public const string Int8 = "Int8";
        public const string UInt8 = "UInt8";
        public const string Int16 = "Int16";
        public const string UInt16 = "UInt16";
        public const string Int32 = "Int32";
        public const string UInt32 = "UInt32";
        public const string Int64 = "Int64";
        public const string UInt64 = "UInt64";
        public const string Float = "Float";
        public const string Time = "Time";
        public const string ShortText = "ShortText";
        public const string Text = "Text";
        public const string LongText = "LongText";
        public const string TokyoGeoPoint = "TokyoGeoPoint";
        public const string WGS84GeoPoint = "WGS84GeoPoint";
    }

    public class DataTypeMaxLength
    {
        /// <summary>
        /// LongText型の最大サイズ
        /// </summary>
        public const long LongText = 2147483647;
    }

    public class ColumnCreateFlag
    {
        public const string COLUMN_SCALAR = "COLUMN_SCALAR";
        public const string COLUMN_VECTOR = "COLUMN_VECTOR";
        public const string COLUMN_INDEX = "COLUMN_INDEX";
        public const string COMPRESS_ZLIB = "COMPRESS_ZLIB";
        public const string COMPRESS_LZ4 = "COMPRESS_LZ4";
        public const string COMPRESS_ZSTD = "COMPRESS_ZSTD"; // ref: https://qiita.com/kenhys/items/1fa7306be6772765b017
        public const string WITH_SECTION = "WITH_SECTION";
        public const string WITH_WEIGHT = "WITH_WEIGHT";
        public const string WITH_POSITION = "WITH_POSITION";
        public const string INDEX_SMALL = "INDEX_SMALL";
        public const string INDEX_MEDIUM = "INDEX_MEDIUM";
    }

    public class TableCreateFlag
    {
        public const string TABLE_NO_KEY = "TABLE_NO_KEY";
        public const string TABLE_HASH_KEY = "TABLE_HASH_KEY";
        public const string TABLE_PAT_KEY = "TABLE_PAT_KEY";
        public const string TABLE_DAT_KEY = "TABLE_DAT_KEY";
        public const string KEY_WITH_SIS = "KEY_WITH_SIS";
        public const string KEY_LARGE = "KEY_LARGE";
    }
    public class Tokenizer
    {
        public const string TokenBigram = "TokenBigram";
    }
    public class Normalizer
    {
        public const string NormalizerAuto = "NormalizerAuto";
    }

    public class Stage
    {
        /// <summary>
        /// 最初に動的カラムを作成
        /// </summary>
        public const string INITIAL = "initial";

        /// <summary>
        /// queryとfilterの評価後に動的カラムを作成
        /// </summary>
        public const string FILTERED = "filtered";

        /// <summary>
        /// output_columnsの評価前に動的カラムを作成
        /// </summary>
        public const string OUTPUT = "output";
    }

    public class CommandReturnCode
    {
        public const int GRN_SUCCESS = 0;
        public const int GRN_INVALID_ARGUMENT = -22;
        public const int GRN_SYNTAX_ERROR = -63;
    }

}