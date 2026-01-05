using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TPSMobileApp
{
    class DiscontinuedItem
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
    }
}
