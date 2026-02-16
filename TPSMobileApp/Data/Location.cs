using SQLite;

namespace TPSMobileApp
{
    public class Location
    {
        [PrimaryKey]
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CityStateZip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string HoursMonThruFri { get; set; }
        public string HoursSat { get; set; }
        public string HoursSun { get; set; }

        public void Refresh()
        {
        }
    }
}
