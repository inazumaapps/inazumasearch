using System.Collections.Generic;
using System.Text;
using InazumaSearchLib.Groonga.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InazumaSearchLib.Groonga
{
    public partial class Manager
    {
        protected object Locker = new object();

        /// <summary>
        /// Groongaに対してコマンドを発行し、結果を文字列で取得 (optsをobjectで指定)
        /// </summary>
        public string ExecuteCommandAsString(string commandName, object opts = null, string input = null)
        {
            // objectをDictionaryに変換
            var optDict = new Dictionary<string, object>();
            if (opts != null)
            {
                foreach (var prop in opts.GetType().GetProperties())
                {
                    optDict[prop.Name] = prop.GetValue(opts);
                }
            }

            return ExecuteCommandAsStringByDict(commandName, optDict, input);
        }

        /// <summary>
        /// Groongaに対してコマンドを発行し、結果を文字列で取得
        /// </summary>
        public string ExecuteCommandAsStringByDict(string commandName, IDictionary<string, object> opts = null, string input = null)
        {
            var tokens = new List<string>
            {
                commandName
            };
            if (opts != null)
            {
                foreach (var optName in opts.Keys)
                {
                    if (opts[optName] != null)
                    {
                        var strVal = opts[optName].ToString();
                        tokens.Add($"--{optName} {strVal}");
                    }
                }
            }
            Logger.Debug(string.Join(" ", tokens));
            var cmdStr = string.Join(" ", tokens);


            return ExecuteCommandLineAsString(cmdStr, input);
        }

        /// <summary>
        /// Groongaに対してコマンドを発行し、結果をJSONからデシリアライズしたオブジェクトとして返す (optsをobjectで指定)
        /// </summary>
        public JToken ExecuteCommand(string commandName, object opts = null, string input = null)
        {
            // objectをDictionaryに変換
            var optDict = new Dictionary<string, object>();
            if (opts != null)
            {
                foreach (var prop in opts.GetType().GetProperties())
                {
                    optDict[prop.Name] = prop.GetValue(opts);
                }
            }

            return ExecuteCommandByDict(commandName, optDict, input);
        }

        /// <summary>
        /// Groongaに対してコマンドを発行し、結果をJSONからデシリアライズしたオブジェクトとして返す
        /// </summary>
        public JToken ExecuteCommandByDict(string commandName, IDictionary<string, object> opts = null, string input = null)
        {
            var res = ExecuteCommandAsStringByDict(commandName, opts, input);
            var parsedRes = (JArray)JsonConvert.DeserializeObject(res);

            if (res.Length > 1000)
            {
                Logger.Debug(res.Substring(0, 1000) + " ...");
            }
            else
            {
                Logger.Debug(res);
            }

            var header = (JArray)parsedRes[0];
            JToken body = null;
            if (parsedRes.Count >= 2)
            {
                body = parsedRes[1];
            }

            // 結果の解析
            var returnCode = (int)header[0];
            var unitTimeWhenCommandIsStarted = header[1];
            var elapsedTime = header[2];

            if (returnCode == 0) // 成功
            {
                return body;
            }
            else
            {
                var errorMessage = (string)header[3];
                if (parsedRes.Count >= 5)
                {
                    var errorLocation = header[4];
                }
                // 失敗
                throw new GroongaCommandError(returnCode, errorMessage);
            }

        }

        /// <summary>
        /// Groongaに対して文字列形式で渡したコマンドを発行し、結果を文字列で取得
        /// </summary>
        public string ExecuteCommandLineAsString(string commandLine, string input = null)
        {
            var groongaEnc = Encoding.UTF8;
            var cmdBytes = groongaEnc.GetBytes(commandLine);

            // コマンドの発行は、プロセスの標準入出力を使用するため、クリティカルセクション内で実行
            // (他スレッドからのアクセスを避ける)

            string res = null;
            lock (Locker)
            {
                Proc.StandardInput.BaseStream.Write(cmdBytes, 0, cmdBytes.Length);
                Proc.StandardInput.WriteLine();

                if (input != null)
                {
                    if (input.Length > 500)
                    {
                        Logger.Debug(input.Substring(0, 500) + "..");
                    }
                    else
                    {
                        Logger.Debug(input.Replace("\\", ""));
                    }
                    var bytes = groongaEnc.GetBytes(input);
                    Proc.StandardInput.BaseStream.Write(bytes, 0, bytes.Length);
                    Proc.StandardInput.WriteLine();
                }

                res = Proc.StandardOutput.ReadLine();
            }

            return res;
        }


        public bool TableCreate(
              string name
            , string[] flags = null
            , string keyType = null
            , string valueType = null
            , string defaultTokenizer = null
            , string normalizer = null
            , string tokenFilters = null
        )
        {
            var opts = new
            {
                flags = (flags == null ? null : string.Join("|", flags))
                ,
                name = name
                ,
                key_type = keyType
                ,
                value_type = valueType
                ,
                default_tokenizer = defaultTokenizer
                ,
                normalizer = normalizer
                ,
                token_filters = tokenFilters
            };
            return (bool)ExecuteCommand("table_create", opts);
        }

        public bool TableRename(
              string name
            , string newName
        )
        {
            var opts = new
            {
                name,
                new_name = newName
            };
            return (bool)ExecuteCommand("table_rename", opts);
        }
        public bool TableRemove(
              string name
            , bool? dependent = null // 未実装
        )
        {
            var opts = new
            {
                name
            };
            return (bool)ExecuteCommand("table_remove", opts);
        }

        public bool TableCopy(
              string fromName
            , string toName
        )
        {
            var opts = new
            {
                from_name = fromName,
                to_name = toName
            };
            return (bool)ExecuteCommand("table_copy", opts);
        }

        public bool Truncate(
              string targetName
        )
        {
            var opts = new
            {
                target_name = targetName
            };
            return (bool)ExecuteCommand("truncate", opts);
        }


        public bool ColumnCreate(
              string table
            , string name
            , string[] flags
            , string type
            , string source = null
        )
        {
            var opts = new
            {
                table = table
                ,
                name = name
                ,
                flags = (flags == null ? null : string.Join("|", flags))
                ,
                type = type
                ,
                source = source
            };
            return (bool)ExecuteCommand("column_create", opts);
        }

        public bool ColumnRename(
              string table
            , string name
            , string newName
        )
        {
            var opts = new
            {
                table = table
                ,
                name = name
                ,
                new_name = newName
            };
            return (bool)ExecuteCommand("column_rename", opts);
        }

        public bool ColumnCopy(
              string fromTable
            , string fromName
            , string toTable
            , string toName
        )
        {
            var opts = new
            {
                from_table = fromTable
                ,
                from_name = fromName
                ,
                to_table = toTable
                ,
                to_name = toName
            };
            return (bool)ExecuteCommand("column_copy", opts);
        }

        public bool ColumnRemove(
              string table
            , string name
        )
        {
            var opts = new
            {
                table = table
                ,
                name = name
            };
            return (bool)ExecuteCommand("column_remove", opts);
        }


        public bool ObjectExist(
              string name
        )
        {
            var opts = new
            {
                name = name
            };
            return (bool)ExecuteCommand("object_exist", opts);
        }

        /// <summary>
        /// プラグインを登録する
        /// </summary>
        public bool PluginRegister(
            string name
            )
        {
            var opts = new
            {
                name = name
            };
            return (bool)ExecuteCommand("plugin_register", opts);
        }


        /// <summary>
        /// プラグインの登録を解除する
        /// </summary>
        public bool PluginUnregister(
            string name
            )
        {
            var opts = new
            {
                name = name
            };
            return (bool)ExecuteCommand("plugin_unregister", opts);
        }

        /// <summary>
        /// レコードの登録/更新 (Dictionary指定形式)
        /// </summary>
        public int Load(
              IEnumerable<IDictionary<string, object>> values
            , string table
            , string each = null
            , string ifExists = null
        )
        {
            var opts = new
            {
                table = table
                ,
                each = FormatForParameter(each)
                ,
                ifexists = ifExists
            };
            var obj = ExecuteCommand("load", opts, JsonConvert.SerializeObject(values));
            return (int)obj;

        }

        /// <summary>
        /// レコードの登録/更新 (配列とcolumns指定形式)
        /// </summary>
        public int Load(
              IEnumerable<IEnumerable<object>> values
            , string table
            , string[] columns = null
            , string ifExists = null
        )
        {
            var opts = new
            {
                table = table
                ,
                columns = (columns == null ? null : string.Join(",", columns))
                ,
                ifexists = ifExists
            };
            var obj = ExecuteCommand("load", opts, JsonConvert.SerializeObject(values));
            return (int)obj;

        }

        public bool Delete(
              string table
            , string key = null
            , int? id = null
            , string filter = null

        )
        {
            var opts = new
            {
                table = table
                ,
                key = FormatForParameter(key)
                ,
                id = id
                ,
                filter = FormatForParameter(filter)
            };
            return (bool)ExecuteCommand("delete", opts);
        }

        public class ObjectListResult
        {
            public long id { get; set; }
            public string name { get; set; }
            public string opened { get; set; }
            public long? value_size { get; set; }
            public long n_elements { get; set; }
            public Type type { get; set; }
            public Flag flags { get; set; }
            public string path { get; set; }

            public class Type
            {
                public long id { get; set; }
                public string name { get; set; }
            }

            public class Flag
            {
                public string names { get; set; }
                public long value { get; set; }
            }
        }

        public IDictionary<string, ObjectListResult> ObjectList(
            )
        {
            var resJToken = (JObject)ExecuteCommand("object_list");


            return resJToken.ToObject<IDictionary<string, ObjectListResult>>();
        }
        public RecordSet Suggest(
              string[] types
            , string table
            , string column
            , string query
            , string sortBy = null
            , string[] outputColumns = null
            , long? offset = null
            , long? limit = null
            , long? frequencyThreshold = null
            , double? conditionalProbabilityThreshold = null
            , string prefixSearch = null
            , string similarSearch = null
        )
        {
            var opts = new Dictionary<string, object>
            {
                ["types"] = string.Join("|", types),
                ["table"] = table,
                ["column"] = column,
                ["query"] = FormatForParameter(query),
                ["sortby"] = sortBy,
                ["output_columns"] = (outputColumns == null ? null : string.Join(",", outputColumns)),
                ["offset"] = offset,
                ["limit"] = limit,
                ["frequency_threshold"] = frequencyThreshold,
                ["conditional_probability_threshold"] = conditionalProbabilityThreshold,
                ["prefix_search"] = prefixSearch,
                ["similarSearch"] = similarSearch
            };

            var resJToken = (JObject)ExecuteCommandByDict("suggest", opts);
            return SelectResultDataToRecordSet((JArray)resJToken["complete"]);
        }



        public SelectResult Select(
              string table
            , string[] matchColumns = null
            , string query = null
            , string filter = null
            , string scorer = null
            , string[] outputColumns = null
            , int? offset = null
            , int? limit = null
            , string[] drilldown = null
            , string[] drilldownOutputColumns = null
            , int? drilldownOffset = null
            , int? drilldownLimit = null
            , bool? cache = null
            , int? matchEscalationThreshold = null
            , string queryExpansion = null
            , string[] queryFlags = null
            , string queryExpander = null
            , string adjuster = null
            , string[] drilldownCalcTypes = null
            , string drilldownCalcTarget = null
            , string drilldownFilter = null
            , string[] sortKeys = null
            , string[] drilldownSortKeys = null
            , IEnumerable<DynamicColumn> columns = null
        )
        {
            var opts = new Dictionary<string, object>
            {
                ["table"] = table,
                ["match_columns"] = (matchColumns == null ? null : FormatForParameter(string.Join(" || ", matchColumns))),
                ["output_columns"] = (outputColumns == null ? null : FormatForParameter(string.Join(",", outputColumns))),
                ["offset"] = offset,
                ["query"] = FormatForParameter(query),
                ["filter"] = FormatForParameter(filter),
                ["limit"] = limit,
                ["drilldown"] = (drilldown == null ? null : FormatForParameter(string.Join(",", drilldown))),
                ["drilldown_limit"] = drilldownLimit,
                ["drilldown_output_columns"] = (drilldownOutputColumns == null ? null : FormatForParameter(string.Join(",", drilldownOutputColumns))),
                ["drilldown_sort_keys"] = (drilldownSortKeys == null ? null : FormatForParameter(string.Join(",", drilldownSortKeys))),
                ["sort_keys"] = (sortKeys == null ? null : FormatForParameter(string.Join(",", sortKeys))),
                ["scorer"] = FormatForParameter(scorer)
            };

            if (columns != null)
            {
                foreach (var col in columns)
                {
                    opts[$"columns[{col.Name}].stage"] = col.Stage;
                    opts[$"columns[{col.Name}].type"] = col.Type;
                    opts[$"columns[{col.Name}].value"] = FormatForParameter(col.Value);
                    opts[$"columns[{col.Name}].flags"] = (col.Flags == null ? null : FormatForParameter(string.Join(",", col.Flags)));
                }
            }

            var selectRes = ExecuteCommandByDict("select", opts);
            var array = (JArray)selectRes;

            // メイン検索結果を格納
            var res = new SelectResult
            {
                SearchResult = SelectResultDataToRecordSet((JArray)array[0], outputColumns),

                // ドリルダウン結果があれば、それも格納
                DrilldownResults = new List<RecordSet>()
            };
            if (drilldown != null && drilldown.Length >= 1)
            {
                for (var i = 0; i < drilldown.Length; i++)
                {
                    var recSet = SelectResultDataToRecordSet((JArray)array[i + 1], drilldownOutputColumns);
                    res.DrilldownResults.Add(recSet);
                }
            }

            return res;
        }

        /// <summary>
        /// JSONから取得した検索結果データをもとに、RecordSetクラスのオブジェクトを構築
        /// </summary>
        /// <param name="resultData"></param>
        /// <param name="outputColumns"></param>
        /// <returns></returns>
        protected RecordSet SelectResultDataToRecordSet(JArray resultData, string[] outputColumns = null)
        {
            var recSet = new RecordSet
            {
                NHits = (long)resultData[0][0],
                Columns = new List<RecordSet.Column>()
            };
            foreach (var columnData in resultData[1])
            {
                recSet.Columns.Add(new RecordSet.Column() { Name = (string)columnData[0], Type = (string)columnData[1] });
            }

            recSet.Records = new List<RecordSet.Record>();
            for (var i = 2; i < resultData.Count; i++)
            {
                var values = (JArray)resultData[i];
                recSet.Records.Add(new RecordSet.Record(outputColumns, recSet.Columns, values));
            }

            return recSet;
        }

        /// <summary>
        /// パラメータの値を整形する
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected string FormatForParameter(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return null;
            }
            else
            {
                return "'" + (v ?? "").Replace(@"\", @"\\").Replace(@"'", @"\'") + "'";
            }
        }

    }
}
