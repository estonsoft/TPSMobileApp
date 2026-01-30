using SQLite;

namespace TPSMobileApp
{
    public class Banner
    {
        [PrimaryKey]
        public string BannerName { get; set; }
        public string BannerURL { get; set; }
    }
}
