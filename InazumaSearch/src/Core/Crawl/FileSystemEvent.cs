
namespace InazumaSearch.Core.Crawl
{
    public class FileSystemEvent
    {
        public FileSystemEventType Type { get; set; }
        public FileSystemEventTargetType? TargetType { get; set; }
        public string Path { get; set; }
    }

    public enum FileSystemEventType
    {
        UPDATE
        , DELETE
    }

    public enum FileSystemEventTargetType
    {
        FILE
        , FOLDER
    }
}
