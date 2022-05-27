namespace InazumaSearch.Groonga
{
    public class Function
    {
        public static string SnippetHtml(string column)
        {
            return string.Format("snippet_html({0})", column);
        }

        public static string HighlightFull(string column, string normalizerName, bool useHtmlEscape, string keyword1, string startTag1, string endTag1)
        {
            return $"highlight_full({column}, \"{normalizerName}\", {(useHtmlEscape ? "true" : "false")}, \"{keyword1}\", \"{startTag1}\", \"{endTag1}\")";
        }
    }
}
