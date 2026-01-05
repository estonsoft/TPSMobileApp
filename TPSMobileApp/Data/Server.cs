using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TPSMobileApp
{
    public class Server
    {
        [PrimaryKey]
        public string ServerURL { get; set; }
    }
}
