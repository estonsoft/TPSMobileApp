using Realms;

namespace TPSMobileApp
{
    class DiscontinuedItem : RealmObject
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
    }
}
