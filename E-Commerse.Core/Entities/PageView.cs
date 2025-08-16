namespace E_Commerce.Core.Entities
{
    public class PageView
    {
        public int Id { get; set; }
        public string Path { get; set; } = "";
        public string Referrer { get; set; } = "";
        public DateTime At { get; set; }
        public string? UserAgent { get; set; }
    }
}
