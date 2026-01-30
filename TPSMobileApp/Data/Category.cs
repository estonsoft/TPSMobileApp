using SQLite;

namespace TPSMobileApp
{
    public class Category
    {
        [PrimaryKey]
        public string Code { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int HomePage { get; set; }
        public int Rank { get; set; }
        public string ImageBase64 { get; set; }
    }
}
