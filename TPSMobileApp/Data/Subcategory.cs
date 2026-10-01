using Realms;

namespace TPSMobileApp
{
    public class Subcategory : RealmObject
    {
        [PrimaryKey]
        public string Code { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public string ImageBase64 { get; set; }
    }
}
