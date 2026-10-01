using Realms;

namespace TPSMobileApp
{
    public class Banner : RealmObject
    {
        [PrimaryKey]
        public string BannerName { get; set; }
        public string BannerURL { get; set; }
    }
}
