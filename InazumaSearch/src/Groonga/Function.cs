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
