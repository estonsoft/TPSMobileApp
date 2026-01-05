using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace TPSMobileApp
{
    public class SuspendItem
    {
        public string CustNo { get; set; }
        public int ItemNo { get; set; }
        public int QtyOrder { get; set; }
        public int QtyCredit { get; set; }
        public int QtyLabel { get; set; }
        public string ServerURL { get; set; }
        public int LineNo { get; set; }
    }
}
