namespace MuziekApp.Models
{
    public class SearchResultItem
    {
        public string VideoId { get; set; }     // van je PHP API output
        public string Title { get; set; }
        public string Channel { get; set; }
        public string Thumbnail { get; set; }

        // Handige property om direct een bruikbare YouTube-URL te hebben
        public string Url => $"https://www.youtube.com/watch?v={VideoId}";
    }
}