using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Groonga
{
    /// <summary>
    /// 動的カラム。Selectコマンドで使用
    /// </summary>
    public class DynamicColumn
    {
        public string Name { get; set; }
        public string Stage { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string[] Flags { get; set; }

        public DynamicColumn(string name, string stage, string type, string value, string[] flags = null)
        {
            Name = name;
            Stage = stage;
            Type = type;
            Value = value;
            Flags = flags;
        }

    }
}
