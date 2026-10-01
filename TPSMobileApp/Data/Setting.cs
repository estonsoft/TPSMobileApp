using Realms;

namespace TPSMobileApp
{
    public class Setting : RealmObject
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
