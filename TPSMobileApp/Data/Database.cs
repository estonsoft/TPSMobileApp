using SQLite;

namespace TPSMobileApp
{
    public class Database
    {
        readonly SQLiteConnection _database;
        static object locker = new object();

        public Database()
        {
            //string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DBName);
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, Constants.DBName);
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Item>();
            _database.CreateTable<Customer>();
            _database.CreateTable<Banner>();
            _database.CreateTable<Category>();
            _database.CreateTable<Subcategory>();
            _database.CreateTable<Subsubcategory>();
            _database.CreateTable<Setting>();
            _database.CreateTable<PaymentToken>();
            _database.CreateTable<Location>();
            _database.CreateTable<OrderHeader>();
            _database.CreateTable<OrderDetail>();
            _database.CreateTable<ReorderItem>();
            _database.CreateTable<CartItem>();
            _database.CreateTable<DiscontinuedItem>();
            _database.CreateTable<SuspendItem>();
            _database.CreateTable<SalesCustomer>();
            _database.CreateTable<FlyerItem>();
            _database.CreateTable<Server>();

            _database.EnableWriteAheadLogging();
            _database.Execute("PRAGMA synchronous = NORMAL");
        }

        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _database.Commit();
        }

        public void RollbackTransaction()
        {
            _database.Rollback();
        }

        public List<Item> SearchItems(String sSearch, Category category, String sBarcode, Subcategory subcategory, Subsubcategory subsubcategory)
        {
            Decimal dItemNo = 0;

            String sBarcodeShort = sBarcode;
            if (sBarcode.Length > 11)
            {
                sBarcodeShort = sBarcodeShort.Substring(0, 11);
            }
            try
            {
                if ((sBarcode.Length <= 6) && (sBarcode != ""))
                {
                    dItemNo = Decimal.Parse(sBarcode);
                    sBarcodeShort = dItemNo.ToString();

                    category.Code = "";
                    subcategory.Code = "";
                    subsubcategory.Code = "";
                }
            }
            catch
            {
            }

            if (Decimal.TryParse(sSearch, out dItemNo))
            {
                dItemNo = Decimal.Parse(sSearch);
                sBarcodeShort = dItemNo.ToString();

                category.Code = "";
                subcategory.Code = "";
                subsubcategory.Code = "";
            }

            sSearch = sSearch.Replace("'", "");

            String sSearchShort = sSearch;
            if (sSearch.Length > 11)
            {
                sSearchShort = sSearchShort.Substring(0, 11);
            }

            String sQuery = "select * from [Item] where ";

            if (sBarcode != "")
            {
                sQuery += " (((([UPC_1] like '%" + sBarcode + "%' or [UPC_1] like '%" + sBarcodeShort + "') and [UPC_1] > '') or ";
                sQuery += " (([UPC_2] like '%" + sBarcode + "%' or [UPC_2] like '%" + sBarcodeShort + "') and [UPC_2] > '') or ";
                sQuery += " (([UPC_3] like '%" + sBarcode + "%' or [UPC_3] like '%" + sBarcodeShort + "') and [UPC_3] > '') or ";
                sQuery += " (([UPC_4] like '%" + sBarcode + "%' or [UPC_4] like '%" + sBarcodeShort + "') and [UPC_4] > '')) or ";
                sQuery += " (([ItemNoDisplay] = '" + sBarcode + "') or ([ItemNoDisplay] = '" + sBarcodeShort + "')) ";
            }
            else
            {
                sQuery += " ([Description] like '%" + sSearch + "%' or [ItemNoDisplay] like '%" + sSearch + "%' or ";
                if (dItemNo > 0)
                {
                    sQuery += " [ItemNoDisplay] like '%" + dItemNo.ToString() + "%' or ";
                }
                sQuery += " (([UPC_1] like '%" + sSearch + "%' or [UPC_1] like '%" + sSearchShort + "') and [UPC_1] > '') or ";
                sQuery += " (([UPC_2] like '%" + sSearch + "%' or [UPC_2] like '%" + sSearchShort + "') and [UPC_2] > '') or ";
                sQuery += " (([UPC_3] like '%" + sSearch + "%' or [UPC_3] like '%" + sSearchShort + "') and [UPC_3] > '') or ";
                sQuery += " (([UPC_4] like '%" + sSearch + "%' or [UPC_4] like '%" + sSearchShort + "') and [UPC_4] > '')) ";
            }

            if (category.Code != "")
            {
                sQuery += " and CategoryCode = '" + category.Code + "' ";
            }

            if (subcategory.Code != "")
            {
                sQuery += " and SubcategoryCode = '" + subcategory.Code + "' ";
            }

            if (subsubcategory.Code != "")
            {
                sQuery += " and SubsubcategoryCode = '" + subsubcategory.Code + "' ";
            }

            if ((subcategory.Code != "") && (subcategory.Code != "TOPSELLERS"))
            {
                //sQuery += " and SubcategoryCode = '" + subcategory.Code + "' ";
            }

            if (App.g_InStockOnly)
            {
                sQuery += " and QOH > 0 ";
            }

            sQuery += " and Status = 'A' ";

            if (App.g_IsTopSellers)
            {
                sQuery += " order by CategoryRank limit 25 ";
            }
            else
            {
                sQuery += " order by Description";
            }
            lock (locker)
            {
                return _database.Query<Item>(sQuery);
            }
        }

        public int InsertDiscontinuedItems()
        {
            lock (locker)
            {
                String sQuery = "delete from [DiscontinuedItem]";
                _database.Execute(sQuery);

                sQuery = "insert into [DiscontinuedItem] select ItemNo from [Item]";
                return _database.Execute(sQuery);
            }
        }

        public int DeleteDiscontinuedItem(string ItemNo)
        {
            lock (locker)
            {
                String sQuery = "delete from [DiscontinuedItem] where ItemNo = " + ItemNo;
                return _database.Execute(sQuery);
            }
        }

        public int UpdateDiscontinuedItems()
        {
            lock (locker)
            {
                String sQuery = "update [Item] set Status = 'D' where ItemNo in (select ItemNo from [DiscontinuedItem])";
                return _database.Execute(sQuery);
            }
        }

        public List<Item> GetCartItems()
        {
            lock (locker)
            {
                String sQuery = "select * from [Item] where QtyOrder > 0 or QtyCredit > 0 or QtyLabel > 0";
                return _database.Query<Item>(sQuery);
            }
        }

        public List<Item> GetOrderCartItems()
        {
            lock (locker)
            {
                String sQuery = "select * from [Item] where QtyOrder > 0 ";
                if (App.g_ShoppingCartSort == "F")
                {
                    sQuery += " order by LineNo";
                }
                else if (App.g_ShoppingCartSort == "L")
                {
                    sQuery += " order by LineNo desc";
                }
                else
                {
                    sQuery += " order by Description";
                }
                return _database.Query<Item>(sQuery);
            }
        }

        public List<Item> GetReturnCartItems()
        {
            lock (locker)
            {
                String sQuery = "select * from [Item] where QtyCredit > 0 order by Description";
                return _database.Query<Item>(sQuery);
            }
        }

        public List<Item> GetLabelCartItems()
        {
            lock (locker)
            {
                String sQuery = "select * from [Item] where QtyLabel > 0 order by Description";
                return _database.Query<Item>(sQuery);
            }
        }

        public int GetCartPieces()
        {
            lock (locker)
            {
                String sQuery = "select sum(QtyOrder) from [Item]";
                return _database.ExecuteScalar<int>(sQuery);
            }
        }

        public int ClearCartItems()
        {
            lock (locker)
            {
                String sQuery = "update [Item] set QtyOrder = 0, QtyCredit = 0, QtyLabel = 0, PriceOrder = 0, LineNo = 0";
                return _database.Execute(sQuery);
            }
        }

        public int ClearOrderCartItems()
        {
            lock (locker)
            {
                String sQuery = "update [Item] set QtyOrder = 0, PriceOrder = 0, LineNo = 0";
                return _database.Execute(sQuery);
            }
        }

        public int ClearReturnCartItems()
        {
            lock (locker)
            {
                String sQuery = "update [Item] set QtyCredit = 0";
                return _database.Execute(sQuery);
            }
        }

        public int ClearLabelCartItems()
        {
            lock (locker)
            {
                String sQuery = "update [Item] set QtyLabel = 0";
                return _database.Execute(sQuery);
            }
        }

        public int GetItemCount()
        {
            lock (locker)
            {
                String sQuery = "select count(*) from [Item]";
                return _database.ExecuteScalar<int>(sQuery);
            }
        }

        public Item FindItem(int item_no, string item_ref_no)
        {
            lock (locker)
            {
                if (App.g_IsRefNoLookup)
                {
                    return _database.Find<Item>(s => s.ItemRefNo == item_ref_no);
                }
                else
                {
                    return _database.Find<Item>(s => s.ItemNo == item_no);
                }
            }
        }

        public Item FindItemUPC_1(string UPC)
        {
            lock (locker)
            {
                return _database.Find<Item>(s => s.UPC_1 == UPC);
            }
        }

        public Item FindItemUPC_2(string UPC)
        {
            lock (locker)
            {
                return _database.Find<Item>(s => s.UPC_1 == UPC);
            }
        }

        public Item FindItemUPC_3(string UPC)
        {
            lock (locker)
            {
                return _database.Find<Item>(s => s.UPC_3 == UPC);
            }
        }

        public Item FindItemUPC_4(string UPC)
        {
            lock (locker)
            {
                return _database.Find<Item>(s => s.UPC_4 == UPC);
            }
        }

        public List<Item> SearchItemsQuickEntry(String sSearch)
        {
            lock (locker)
            {
                Decimal dItemNo = 0;
                try
                {
                    if ((sSearch.Length <= 6) && (sSearch != ""))
                    {
                        dItemNo = Decimal.Parse(sSearch);
                    }
                }
                catch
                {
                }

                String sSearch2 = "";

                sSearch = sSearch.Replace("'", "");
                if (sSearch.Length >= 6 && sSearch.Length <= 8)
                {
                    sSearch2 = sSearch;
                    string sUPCExpand = UPCExpand(sSearch);
                    if (sUPCExpand != "")
                    {
                        sSearch = sUPCExpand;
                    }
                }

                String sSearchShort = sSearch;
                String sSearchShort2 = sSearch;
                if (sSearch.Length == 13)
                {
                    sSearchShort = sSearchShort.Substring(2, 11);
                }
                else if (sSearch.Length > 11)
                {
                    sSearchShort2 = sSearchShort2.Substring(0, 11);
                }

                String sQuery = "select * from [Item] where ";

                sQuery += " ((([UPC_1] like '%" + sSearch + "%' or [UPC_1] like '%" + sSearchShort + "' or [UPC_1] like '%" + sSearchShort2 + "') and [UPC_1] > '') or ";
                sQuery += " (([UPC_2] like '%" + sSearch + "%' or [UPC_2] like '%" + sSearchShort + "' or [UPC_2] like '%" + sSearchShort2 + "') and [UPC_2] > '') or ";
                sQuery += " (([UPC_3] like '%" + sSearch + "%' or [UPC_3] like '%" + sSearchShort + "' or [UPC_3] like '%" + sSearchShort2 + "') and [UPC_3] > '') or ";
                sQuery += " (([UPC_4] like '%" + sSearch + "%' or [UPC_4] like '%" + sSearchShort + "' or [UPC_4] like '%" + sSearchShort2 + "') and [UPC_4] > '') ";

                if (sSearch2 != "")
                {
                    sQuery += " or ([UPC_1] = '" + sSearch2 + "' or [UPC_2] = '" + sSearch2 + "' or [UPC_3] = '" + sSearch2 + "' or [UPC_4] = '" + sSearch2 + "') ";
                }

                if (dItemNo > 0)
                {
                    sQuery += " or ([ItemNoDisplay] like '%" + dItemNo.ToString() + "%')";
                }

                sQuery += " ) and Status <> 'D' ";

                return _database.Query<Item>(sQuery);
            }
        }

        private string UPCExpand(string sUPC)
        {
            string sUPCExpand = "";

            if (sUPC.Length == 8)
            {
                //return UPC8Expand(sUPC);
                sUPC = sUPC.Substring(1, 6);
            }

            if (sUPC.Length == 6)
            {
                sUPC = "0" + sUPC;
            }

            string D1 = sUPC.Substring(0, 1);
            string D2 = sUPC.Substring(1, 1);
            string D3 = sUPC.Substring(2, 1);
            string D4 = sUPC.Substring(3, 1);
            string D5 = sUPC.Substring(4, 1);
            string D6 = sUPC.Substring(5, 1);
            string D7 = sUPC.Substring(6, 1);

            switch (D7)
            {
                case "0":
                    sUPCExpand = D1 + D2 + D3 + "00000" + D4 + D5 + D6;
                    break;

                case "1":
                    sUPCExpand = D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6;
                    break;

                case "2":
                    sUPCExpand = D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6;
                    break;

                case "3":
                    sUPCExpand = D1 + D2 + D3 + D4 + "00000" + D5 + D6;
                    break;

                case "4":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + "00000" + D6;
                    break;

                case "5":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "6":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "7":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "8":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "9":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                default:
                    sUPCExpand = "";
                    break;
            }

            return sUPCExpand;
        }

        private string UPC8Expand(string sUPC)
        {
            string sUPCExpand = "";

            string D7 = sUPC.Substring(6, 1);

            if (D7 == "3")
            {
                sUPCExpand = sUPC.Substring(0, 4) + "00000" + sUPC.Substring(4, 2) + sUPC.Substring(7, 1);
            }
            else // D7 should be 0
            {
                sUPCExpand = sUPC.Substring(0, 3) + "00000" + sUPC.Substring(3, 3) + sUPC.Substring(7, 1);
            }

            return sUPCExpand;
        }

        public List<Server> GetServers()
        {
            String sQuery = "select * from Server";
            return _database.Query<Server>(sQuery);
        }

        public int SaveServer(Server server)
        {
            return _database.InsertOrReplace(server);
        }

        public int DeleteServer(Server server)
        {
            return _database.Delete(server);
        }

        public int SaveItem(Item item)
        {
            lock (locker)
            {
                int i = _database.InsertOrReplace(item);
                return i;
            }
        }

        public int SaveItemReplace(Item item)
        {
            return _database.InsertOrReplace(item);
            /*
            Item _item = FindItem(item.ItemNo);

            if (_item == null)
            {
                return _database.Insert(item);
            }
            else
            {
                item.QtyOrder = _item.QtyOrder;
                _database.Update(item);

                UpdateItemPriceOrder(item.ItemNo);

                return 1;
            }
            */
        }

        public int UpdateItem(Item item)
        {
            lock (locker)
            {
                return _database.Update(item);
            }
        }

        public int DeleteItems()
        {
            lock (locker)
            {
                return _database.Execute("delete from Item");
            }
        }

        public List<Item> GetItems()
        {
            lock (locker)
            {
                String sQuery = "select * from [Item] ";
                return _database.Query<Item>(sQuery);
            }
        }

        public int UpdateItemQty(int iItem, int iQty)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyOrder = QtyOrder + " + iQty.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set LineNo = ifnull((select max(LineNo) from Item), 0) + 1 where LineNo = 0 and ItemNo = " + iItem.ToString());

                try
                {
                    Vibration.Vibrate(200);
                }
                catch (Exception e)
                {
                }

                return 1;
            }
        }

        public int UpdateItemCreditQty(int iItem, int iQty)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyCredit = QtyCredit + " + iQty.ToString() + " where ItemNo = " + iItem.ToString());

                try
                {
                    Vibration.Vibrate(200);
                }
                catch (Exception e)
                {
                }

                return 1;
            }
        }

        public int UpdateItemLabelQty(int iItem, int iQty)
        {

            lock (locker)
            {
                _database.Execute("update Item set QtyLabel = QtyLabel + " + iQty.ToString() + " where ItemNo = " + iItem.ToString());

                try
                {
                    Vibration.Vibrate(200);
                }
                catch (Exception e)
                {
                }

                return 1;
            }
        }

        public int UpdateItemQty(int iItem, int iQtyOrder, int iQtyCredit, int iQtyLabel)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyOrder = QtyOrder + " + iQtyOrder.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set QtyCredit = QtyCredit + " + iQtyCredit.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set QtyLabel = QtyLabel + " + iQtyLabel.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set LineNo = ifnull((select max(LineNo) from Item), 0) + 1 where LineNo = 0 and ItemNo = " + iItem.ToString());

                return 1;
            }
        }

        public int UpdateItemQtySet(int iItem, int iQty)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyOrder = " + iQty.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set LineNo = ifnull((select max(LineNo) from Item), 0) + 1 where LineNo = 0 and ItemNo = " + iItem.ToString());

                try
                {
                    Vibration.Vibrate(200);
                }
                catch (Exception e)
                {
                }

                return 1;
            }
        }

        public int UpdateItemCreditQtySet(int iItem, int iQty)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyCredit = " + iQty.ToString() + " where ItemNo = " + iItem.ToString());

                try
                {
                    Vibration.Vibrate(200);
                }
                catch (Exception e)
                {
                }

                return 1;
            }
        }

        public int UpdateItemLabelQtySet(int iItem, int iQty)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyLabel = " + iQty.ToString() + " where ItemNo = " + iItem.ToString());

                try
                {
                    Vibration.Vibrate(200);
                }
                catch (Exception e)
                {
                }

                return 1;
            }
        }

        public int UpdateItemQtySet(int iItem, int iQtyOrder, int iQtyCredit, int iQtyLabel, int iLineNo)
        {
            lock (locker)
            {
                _database.Execute("update Item set QtyOrder = " + iQtyOrder.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set QtyCredit = " + iQtyCredit.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set QtyLabel = " + iQtyLabel.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update Item set LineNo = " + iLineNo.ToString() + " where ItemNo = " + iItem.ToString());

                return 1;
            }
        }

        public int UpdateItemQOH(int iItem, int iQOH)
        {
            lock (locker)
            {
                _database.Execute("update Item set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update ReorderItem set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
                _database.Execute("update OrderDetail set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());

                return 1;
            }
        }

        public int GetItemQty(int iItem)
        {
            lock (locker)
            {
                return _database.ExecuteScalar<int>("select QtyOrder from Item where ItemNo = " + iItem.ToString());
            }
        }

        public int DeleteSalesCustomers()
        {
            lock (locker)
            {
                return _database.Execute("delete from [SalesCustomer]");
            }
        }

        public List<SalesCustomer> GetSalesCustomers()
        {
            String sQuery = "select * from [SalesCustomer] ";
            return _database.Query<SalesCustomer>(sQuery);
        }

        public List<SalesCustomer> GetSalesCustomers(string SearchCustomer)
        {
            lock (locker)
            {
                String sOrderBy = " order by CompanyName ";
                String sQuery = "select * from [SalesCustomer] ";

                if (SearchCustomer != null)
                {
                    if (SearchCustomer.Trim().Replace("'", "") != "")
                    {
                        sQuery += " where (CompanyName like '%" + SearchCustomer.Trim().Replace("'", "") + "%' ";
                        sQuery += " or CustNo = '" + SearchCustomer.Trim() + "') ";
                    }
                }

                sQuery += sOrderBy;

                return _database.Query<SalesCustomer>(sQuery);
            }
        }

        public void UpdateCustomerCartItems()
        {
            lock (locker)
            {
                int iRows = 0;

                String sQuery = "update SalesCustomer set ShoppingCartItems = (select sum(QtyOrder) from SuspendItem where SuspendItem.CustNo = SalesCustomer.CustNo and ServerURL = '" + App.g_ServerURL + "') ";
                sQuery += " where CustNo in (select distinct CustNo from SuspendItem where ServerURL = '" + App.g_ServerURL + "')";
                try
                {
                    iRows = _database.Execute(sQuery);
                }
                catch (Exception ex)
                {
                }

                sQuery = "update SalesCustomer set ShoppingCartItems = (select sum(QtyOrder) from Item where CustNo = '" + App.g_Customer.CustNo + "') ";
                sQuery += " where CustNo = '" + App.g_Customer.CustNo + "'";
                try
                {
                    iRows = _database.Execute(sQuery);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public List<SalesCustomer> GetSalesCustomersWithPendingOrders(string SearchCustomer)
        {
            lock (locker)
            {
                String sOrderBy = " order by CompanyName ";
                String sQuery = "select * from [SalesCustomer] ";

                if (SearchCustomer != null)
                {
                    if (SearchCustomer.Trim().Replace("'", "") != "")
                    {
                        sQuery += " where (CompanyName like '%" + SearchCustomer.Trim().Replace("'", "") + "%' ";
                        sQuery += " or CustNo = '" + SearchCustomer.Trim() + "') ";
                    }
                }

                if (sQuery.Contains("where"))
                {
                    sQuery += " and ";
                }
                else
                {
                    sQuery += " where ";
                }

                sQuery += " ShoppingCartItems > 0 ";

                sQuery += sOrderBy;

                return _database.Query<SalesCustomer>(sQuery);
            }
        }

        public SalesCustomer FindSalesCustomer(string CustNo)
        {
            lock (locker)
            {
                return _database.Find<SalesCustomer>(s => s.CustNo == CustNo);
            }
        }

        public List<Category> GetCategories()
        {
            lock (locker)
            {
                String sQuery = "select * from Category order by Rank";
                return _database.Query<Category>(sQuery);
            }
        }

        public int SaveSalesCustomer(SalesCustomer cust)
        {
            lock (locker)
            {
                _database.Delete(cust);
                return _database.Insert(cust);
            }
        }

        public List<Category> GetHomePageCategories()
        {
            lock (locker)
            {
                String sQuery = "select * from Category where HomePage > 0 order by HomePage limit 4";
                return _database.Query<Category>(sQuery);
            }
        }

        public Category GetCategory(string sCategoryCode)
        {
            lock (locker)
            {
                return _database.Find<Category>(s => s.Code == sCategoryCode);
            }
        }

        public int SaveCategory(List<Category> category)
        {
            lock (locker)
            {
                int i = _database.InsertAll(category);
                return i;
            }
        }

        public int DeleteCategories()
        {
            lock (locker)
            {
                return _database.Execute("delete from Category");
            }
        }

        public List<Subcategory> GetSubcategory()
        {
            lock (locker)
            {
                return _database.Table<Subcategory>().OrderBy(t => t.Description).ToList();
            }
        }

        public List<Subcategory> GetSubcategory(string sCategoryCode)
        {
            lock (locker)
            {
                String sQuery = "select * from Subcategory where Category = '" + sCategoryCode + "' order by Description";
                return _database.Query<Subcategory>(sQuery);
            }
        }

        public int SaveSubcategory(List<Subcategory> subcategory)
        {
            lock (locker)
            {
                int i = _database.InsertAll(subcategory);
                return i;
            }
        }

        public int GetSubcategoryCount(string sCategoryCode)
        {
            lock (locker)
            {
                String sQuery = "select count(*) from [Subcategory] where Category = '" + sCategoryCode + "'";
                return _database.ExecuteScalar<int>(sQuery);
            }
        }

        public int DeleteSubcategory(Subcategory subcategory)
        {
            return _database.Delete(subcategory);
        }

        public int DeleteSubcategories()
        {
            lock (locker)
            {
                return _database.Execute("delete from Subcategory");
            }
        }

        public List<Subsubcategory> GetSubsubcategory()
        {
            lock (locker)
            {
                return _database.Table<Subsubcategory>().OrderBy(t => t.Description).ToList();
            }
        }

        public List<Subsubcategory> GetSubsubcategory(string sCategoryCode, string sSubcategoryCode)
        {
            lock (locker)
            {
                var category = sCategoryCode?.Trim();
                var subcategory = sSubcategoryCode?.Trim();

                var result = _database.Query<Subsubcategory>(
                    "SELECT * FROM Subsubcategory",
                    category,
                    subcategory
                ).ToList();

                return result;
            }

        }

        public int GetSubsubcategoryCount(string sCategoryCode, string sSubcategoryCode)
        {
            lock (locker)
            {
                String sQuery = "select count(*) from [Subsubcategory] where Category = '" + sCategoryCode + "' and Subcategory = '" + sSubcategoryCode + "'";
                return _database.ExecuteScalar<int>(sQuery);
            }
        }

        public int SaveSubsubcategory(List<Subsubcategory> subsubcategory)
        {
            lock (locker)
            {
                int i = _database.InsertAll(subsubcategory);
                return i;
            }
        }

        public int DeleteSubsubcategory(Subsubcategory subsubcategory)
        {
            lock (locker)
            {
                return _database.Delete(subsubcategory);
            }
        }

        public int DeleteSubsubcategories()
        {
            lock (locker)
            {
                return _database.Execute("delete from Subsubcategory");
            }
        }

        public int DeleteBannersAsync()
        {
            lock (locker)
            {
                return _database.Execute("delete from Banner");
            }
        }

        public int SaveBannerAsync(Banner banner)
        {
            lock (locker)
            {
                return _database.Insert(banner);
            }
        }

        public List<Banner> GetBanners()
        {
            lock (locker)
            {
                return _database.Table<Banner>().OrderBy(t => t.BannerName).ToList();
            }
        }

        public int SaveCustomer(Customer cust)
        {
            lock (locker)
            {
                _database.Delete(cust);
                int i = _database.Insert(cust);
                return i;
            }
        }

        public Customer GetCustomer()
        {
            lock (locker)
            {
                //String sQuery = "select * from Customer limit 1";
                return _database.Find<Customer>(s => s.CustId == -1);
            }
        }

        public string GetSetting(string sKey)
        {
            lock (locker)
            {
                try
                {
                    var _setting = _database.Find<Setting>(s => s.Key == sKey);

                    if (_setting != null)
                    {
                        return _setting.Value;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        public int SaveSetting(string sKey, string sValue)
        {
            lock (locker)
            {
                Setting setting = new Setting();
                setting.Key = sKey;
                setting.Value = sValue;

                return _database.InsertOrReplace(setting);
            }
        }

        public List<Setting> GetSettings()
        {
            lock (locker)
            {
                return _database.Table<Setting>().ToList();
            }
        }

        public int SaveLocation(Location location)
        {
            lock (locker)
            {
                return _database.InsertOrReplace(location);
            }
        }

        public int DeleteLocations()
        {
            lock (locker)
            {
                return _database.Execute("delete from Location");
            }
        }

        public Location GetLocation(int iLocation)
        {
            lock (locker)
            {
                return _database.Find<Location>(s => s.LocationId == iLocation);
            }
        }

        public int SaveOrderHeader(OrderHeader oh)
        {
            lock (locker)
            {
                return _database.InsertOrReplace(oh);
            }
        }

        public List<OrderHeader> GetOrderHeaders()
        {
            //return _database.Table<OrderHeader>().OrderByDescending(t => t.OrderDate).ToList();
            lock (locker)
            {
                String sQuery = "select * from [OrderHeader] where [CustId] = " + App.g_Customer.CustNo + " order by OrderDate desc";

                return _database.Query<OrderHeader>(sQuery);
            }
        }

        public OrderHeader GetOrderHeader(string sOrderNo)
        {
            lock (locker)
            {
                return _database.Find<OrderHeader>(s => s.OrderNo == sOrderNo);
            }
        }

        public int DeleteOrderHistory()
        {
            lock (locker)
            {
                _database.Execute("delete from OrderHeader");
                _database.Execute("delete from OrderDetail");
                return 0;
            }
        }

        public int SaveOrderDetail(OrderDetail od)
        {
            lock (locker)
            {
                return _database.InsertOrReplace(od);
            }
        }

        public int DeleteOrderDetail(string sOrderNo)
        {
            lock (locker)
            {
                return _database.Execute("delete from OrderDetail where OrderNo = '" + sOrderNo + "'");
            }
        }

        public List<OrderDetail> GetOrderDetail(string sOrderNo)
        {
            lock (locker)
            {
                String sQuery = "select * from OrderDetail where OrderNo = '" + sOrderNo + "' order by Description";
                return _database.Query<OrderDetail>(sQuery);
            }
        }

        public int UpdateOrderDetailLastPurch()
        {
            lock (locker)
            {
                String sQuery = "update OrderDetail set LastPurchDate = (select LastPurchDate from Item i where OrderDetail.ItemNo = i.ItemNo) where ItemNo in (select ItemNo from [OrderDetail])";
                _database.Execute(sQuery);

                sQuery = "update OrderDetail set LastPurchDateDisplay = (select LastPurchDateDisplay from Item i where OrderDetail.ItemNo = i.ItemNo) where ItemNo in (select ItemNo from [OrderDetail])";
                _database.Execute(sQuery);

                sQuery = "update OrderDetail set QtyLastOrder = (select QtyLastOrder from Item i where OrderDetail.ItemNo = i.ItemNo) where ItemNo in (select ItemNo from [OrderDetail])";
                _database.Execute(sQuery);

                sQuery = "update OrderDetail set QtyOrderDisplay = (select QtyOrderDisplay from Item i where OrderDetail.ItemNo = i.ItemNo) where ItemNo in (select ItemNo from [OrderDetail])";
                _database.Execute(sQuery);

                sQuery = "update OrderDetail set QtyLast90 = (select QtyLast90 from Item i where OrderDetail.ItemNo = i.ItemNo) where ItemNo in (select ItemNo from [OrderDetail])";
                _database.Execute(sQuery);

                sQuery = "update OrderDetail set QtyLast90Display = (select QtyLast90Display from Item i where OrderDetail.ItemNo = i.ItemNo) where ItemNo in (select ItemNo from [OrderDetail])";
                _database.Execute(sQuery);

                return 1;
            }
        }

        public List<ReorderItem> GetReorderItemsOld()
        {
            lock (locker)
            {
                String sQuery = "select * from ReorderItem where Status = 'A' order by LastPurchDate desc, Description";
                return _database.Query<ReorderItem>(sQuery);
            }
        }

        public List<Item> GetReorderItems()
        {
            lock (locker)
            {
                String sQuery = "select * from Item where Status = 'A' and LastPurchDateDisplay > '' order by LastPurchDate desc, Description";
                return _database.Query<Item>(sQuery);
            }
        }

        public int SaveReorderItem(ReorderItem ri)
        {
            lock (locker)
            {
                return _database.InsertOrReplace(ri);
            }
        }

        public int GetReorderItemsCount()
        {
            lock (locker)
            {
                String sQuery = "select count(*) from [Item] where LastPurchDateDisplay > ''";
                return _database.ExecuteScalar<int>(sQuery);
            }
        }

        public int DeleteReorderItems()
        {
            lock (locker)
            {
                return _database.Execute("delete from ReorderItem");
            }
        }

        public int DeleteSavedCartItems()
        {
            return _database.Execute("delete from CartItem");
        }

        public int SaveCartItems()
        {
            String sQuery = "insert into CartItem select ItemNo, QtyOrder, QtyCredit, QtyLabel  from [Item] where QtyOrder > 0 or QtyOnOrderSellUnit1 > 0 or QtyOnOrderSellUnit2 > 0 or QtyOnOrderSellUnit3 > 0 or QtyOnOrderSellUnit4 > 0";
            return _database.Execute(sQuery);
        }

        public List<CartItem> GetSavedCartItems()
        {
            String sQuery = "select * from CartItem";
            return _database.Query<CartItem>(sQuery);
        }
        public int SuspendCartItems(string CustNo)
        {
            lock (locker)
            {
                String sQuery = "insert into SuspendItem select '" + CustNo + "', ItemNo, QtyOrder, QtyCredit, QtyLabel, '" + App.g_ServerURL + "', LineNo from [Item] where QtyOrder > 0 or QtyCredit > 0 or QtyLabel > 0";
                return _database.Execute(sQuery);
            }
        }

        public List<SuspendItem> GetSuspendedCartItems(string CustNo)
        {
            lock (locker)
            {
                String sQuery = "select * from SuspendItem where CustNo = '" + CustNo + "' and ServerURL = '" + App.g_ServerURL + "'";
                return _database.Query<SuspendItem>(sQuery);
            }
        }

        public int RestoreCartItems(string CustNo)
        {
            lock (locker)
            {
                List<SuspendItem> items = GetSuspendedCartItems(CustNo);

                foreach (SuspendItem item in items)
                {
                    if (item.QtyOrder > 0)
                    {
                        UpdateItemQtySet(item.ItemNo, item.QtyOrder, item.QtyCredit, item.QtyLabel, item.LineNo);
                    }
                }

                DeleteSuspendedCartItems(CustNo);

                return 0;
            }
        }

        public int DeleteSuspendedCartItems(string CustNo)
        {
            lock (locker)
            {
                return _database.Execute("delete from SuspendItem where CustNo = '" + CustNo + "' and ServerURL = '" + App.g_ServerURL + "'");
            }
        }

        public int ClearFlyerItems()
        {
            lock (locker)
            {
                return _database.Execute("update Item set FlyerPageNo = 0, FlyerBoxNo = 0, FlyerSection = '', FlyerStartDate = 0, FlyerEndDate = 0, FlyerTopLeftX = 0, FlyerTopLeftY = 0, FlyerBottomRightX = 0, FlyerBottomRightY = 0");
            }
        }
        public int UpdateItemFlyerInfo(FlyerItem item)
        {
            lock (locker)
            {
                String sUpdate = "update Item set FlyerPageNo = " + item.Page.ToString() + ", FlyerBoxNo = " + item.Box.ToString();
                sUpdate += ", FlyerSection = '" + item.Section + "', FlyerStartDate = " + item.StartDate.ToString() + ", FlyerEndDate = " + item.EndDate.ToString();
                sUpdate += ", FlyerTopLeftX = " + item.TopLeftX.ToString() + ", FlyerTopLeftY = " + item.TopLeftY.ToString() + ", FlyerBottomRightX = " + item.BottomRightX.ToString() + ", FlyerBottomRightY = " + item.BottomRightY.ToString();
                sUpdate += " where ItemNo = " + item.ItemNo.ToString();

                return _database.Execute(sUpdate);
            }
        }
        public int GetFlyerItemCount()
        {
            lock (locker)
            {
                String sQuery = "select count(*) from [Item] where FlyerStartDate <= " + DateTime.Now.ToString("1yyMMdd") + " and FlyerEndDate >= " + DateTime.Now.ToString("1yyMMdd");
                return _database.ExecuteScalar<int>(sQuery);
            }
        }

        public List<Item> SearchItemsMonthlyAdClick(int iPage, int iX, int iY)
        {
            lock (locker)
            {
                try
                {
                    String sQuery = "select * from [Item] ";
                    sQuery += " where FlyerPageNo =  " + iPage.ToString();
                    sQuery += " and " + iX.ToString() + " between FlyerTopLeftX and FlyerBottomRightX";
                    sQuery += " and " + iY.ToString() + " between FlyerTopLeftY and FlyerBottomRightY";
                    sQuery += " and Status <> 'D' ";
                    sQuery += " order by Description ";

                    return _database.Query<Item>(sQuery);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SearchItemsMonthlyAdClick " + ex.Message);
                    return new List<Item>();
                }
            }
        }

        public int DeleteAll()
        {
            lock (locker)
            {
                _database.Execute("delete from Item");
                _database.Execute("delete from Customer");
                _database.Execute("delete from Banner");
                _database.Execute("delete from Category");
                _database.Execute("delete from Subcategory");
                _database.Execute("delete from Subsubcategory");
                _database.Execute("delete from Setting where not key = 'ServerURL'");
                _database.Execute("delete from PaymentToken");
                _database.Execute("delete from Location");
                _database.Execute("delete from OrderHeader");
                _database.Execute("delete from OrderDetail");
                _database.Execute("delete from ReorderItem");
                _database.Execute("delete from CartItem");
                _database.Execute("delete from DiscontinuedItem");
                _database.Execute("delete from SalesCustomer");
                _database.Execute("delete from FlyerItem");

                return 0;
            }
        }
    }
}
