using SQLite;

namespace TPSMobileApp
{
    public class Setting
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
