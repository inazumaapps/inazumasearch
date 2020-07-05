using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Groonga
{
    public class Function
    {
        public static string SnippetHtml(string column)
        {
            return string.Format("snippet_html({0})", column);
        }
    }
}
