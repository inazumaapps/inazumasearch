using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace InazumaSearch.Groonga
{
    public class SelectResult
    {
        public RecordSet SearchResult { get; set; }
        public IList<RecordSet> DrilldownResults { get; set; }
    }

    public class RecordSet
    {
        public class Column
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
        public class Record
        {
            protected IList<Column> Columns { get; set; }
            protected IList<JToken> OriginalValues { get; set; }
            protected IDictionary<string, object> ValueMap { get; set; }

            public object this[string name] { get { return ValueMap[name]; } }

            public bool? GetBoolValue(string columnName)
            {
                return (bool?)this[columnName];
            }
            public long? GetIntValue(string columnName)
            {
                return (long?)this[columnName];
            }
            public string GetTextValue(string columnName)
            {
                return (string)this[columnName];
            }
            public DateTime? GetTimeValue(string columnName)
            {
                var unixTime = (double?)this[columnName];
                if (unixTime != null)
                {
                    return Util.FromUnixTime(unixTime.Value);
                }
                else
                {
                    return null;
                }
            }

            public long Id { get { return (long)ValueMap[Groonga.VColumn.ID]; } }
            public object Key { get { return ValueMap[Groonga.VColumn.KEY]; } }
            public long NSubRecs { get { return (long)ValueMap[Groonga.VColumn.NSUBRECS]; } }

            public Record(IList<string> paramOutputColumns, IList<Column> columns, IList<JToken> values)
            {
                Columns = columns;
                OriginalValues = values;

                ValueMap = new Dictionary<string, object>();
                for (var i = 0; i < Columns.Count; i++)
                {
                    var colName = (paramOutputColumns != null ? paramOutputColumns[i] : columns[i].Name);
                    var value = values[i];

                    if (value is JArray)
                    {
                        ValueMap[colName] = ((JArray)value).Select(v => ((JValue)v).Value).ToArray();
                    }
                    else
                    {
                        ValueMap[colName] = ((JValue)value).Value;
                    }

                }
            }

        }
        public long NHits { get; set; }
        public IList<Column> Columns { get; set; }
        public IList<Record> Records { get; set; }
    }


}
