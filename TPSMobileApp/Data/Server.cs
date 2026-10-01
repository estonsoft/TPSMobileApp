using Realms;

namespace TPSMobileApp
{
    public class Server : RealmObject
    {
        [PrimaryKey]
        public string ServerURL { get; set; }
    }
}
