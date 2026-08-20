using Scandit.DataCapture.Barcode.Data;
using SQLite;

namespace TPSMobileApp
{
    public class Database
    {
        readonly SQLiteConnection _database;
        static object locker = new object();
        private static Database database;
        private Database()
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

        public static Database Instance()
        {
            if (database == null)
            {
                database = new Database();
            }
            return database;
        }

        public async Task<List<Item>> SearchItems(String sSearch, Category category, String sBarcode, Subcategory subcategory, Subsubcategory subsubcategory)
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
            string[] searchWords = sSearch.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string sQuery = "SELECT * FROM [Item] WHERE ";
            string searchFilter = "";

            if (!string.IsNullOrEmpty(sBarcode))
            {
                // Barcode/Specific ID logic
                searchFilter = $@" (
                (([UPC_1] LIKE '%{sBarcode}%' OR [UPC_1] LIKE '%{sBarcodeShort}%') AND [UPC_1] > '') OR 
                (([UPC_2] LIKE '%{sBarcode}%' OR [UPC_2] LIKE '%{sBarcodeShort}%') AND [UPC_2] > '') OR 
                ([ItemNoDisplay] = '{sBarcode}' OR [ItemNoDisplay] = '{sBarcodeShort}')
                ) ";
            }
            else if (searchWords.Length > 0)
            {
                // 2. ANY ORDER LOGIC
                // We create a requirement that EVERY word must exist SOMEWHERE in the item fields
                List<string> wordClauses = new List<string>();
                foreach (string word in searchWords)
                {
                    wordClauses.Add($"( [Description] LIKE '%{word}%' OR [ItemNoDisplay] LIKE '%{word}%' OR [UPC_1] LIKE '%{word}%' )");
                }

                // Joining with AND ensures all words must be present, but order doesn't matter
                string flexibleMatch = string.Join(" AND ", wordClauses);

                // We still prioritize an exact match for the whole string if it exists
                searchFilter = $@" (
                [ItemNoDisplay] = '{sSearch}' OR 
                [UPC_1] = '{sSearch}' OR 
                ({flexibleMatch})
                 ) ";
            }
            else
            {
                searchFilter = " 1=1 "; // Default if no search text
            }

            sQuery += searchFilter;

            // Apply existing filters
            if (!string.IsNullOrEmpty(category?.Code)) sQuery += $" AND CategoryCode = '{category.Code}' ";
            if (!string.IsNullOrEmpty(subcategory?.Code)) sQuery += $" AND SubcategoryCode = '{subcategory.Code}' ";
            if (App.g_InStockOnly) sQuery += " AND QOH > 0 ";

            sQuery += " AND Status = 'A' ";
            sQuery += App.g_IsTopSellers ? " ORDER BY CategoryRank LIMIT 25 " : " ORDER BY Description ";

            return _database.Query<Item>(sQuery);
        }

        public async Task<int> InsertDiscontinuedItems()
        {
            String sQuery = "delete from [DiscontinuedItem]";
            _database.Execute(sQuery);

            sQuery = "insert into [DiscontinuedItem] select ItemNo from [Item]";
            return _database.Execute(sQuery);
        }

        public async Task<int> DeleteDiscontinuedItem(string ItemNo)
        {
            String sQuery = "delete from [DiscontinuedItem] where ItemNo = " + ItemNo;
            return _database.Execute(sQuery);
        }

        public async Task<int> UpdateDiscontinuedItems()
        {
            String sQuery = "update [Item] set Status = 'D' where ItemNo in (select ItemNo from [DiscontinuedItem])";
            return _database.Execute(sQuery);
        }

        public async Task<List<Item>> GetCartItems()
        {
            String sQuery = "select * from [Item] where QtyOrder > 0 or QtyCredit > 0 or QtyLabel > 0";
            return _database.Query<Item>(sQuery);
        }

        public async Task<List<Item>> GetOrderCartItems()
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

        public async Task<List<Item>> GetReturnCartItems()
        {
            String sQuery = "select * from [Item] where QtyCredit > 0 order by Description";
            return _database.Query<Item>(sQuery);
        }

        public async Task<List<Item>> GetLabelCartItems()
        {


            String sQuery = "select * from [Item] where QtyLabel > 0 order by Description";
            return _database.Query<Item>(sQuery);

        }

        public async Task<int> GetCartPieces()
        {


            String sQuery = "select sum(QtyOrder) from [Item]";
            return _database.ExecuteScalar<int>(sQuery);

        }

        public async Task<int> ClearCartItems()
        {


            String sQuery = "update [Item] set QtyOrder = 0, QtyCredit = 0, QtyLabel = 0, PriceOrder = 0, LineNo = 0";
            return _database.Execute(sQuery);

        }

        public async Task<int> ClearOrderCartItems()
        {


            String sQuery = "update [Item] set QtyOrder = 0, PriceOrder = 0, LineNo = 0";
            return _database.Execute(sQuery);
        }


        public async Task<int> ClearReturnCartItems()
        {


            String sQuery = "update [Item] set QtyCredit = 0";
            return _database.Execute(sQuery);
        }


        public async Task<int> ClearLabelCartItems()
        {


            String sQuery = "update [Item] set QtyLabel = 0";
            return _database.Execute(sQuery);
        }


        public async Task<int> GetItemCount()


        {
            String sQuery = "select count(*) from [Item]";
            return _database.ExecuteScalar<int>(sQuery);
        }


        public async Task<Item> FindItem(int item_no, string item_ref_no)


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


        public async Task<Item> FindItemUPC_1(string UPC)


        {
            return _database.Find<Item>(s => s.UPC_1 == UPC);
        }


        public async Task<Item> FindItemUPC_2(string UPC)


        {
            return _database.Find<Item>(s => s.UPC_1 == UPC);
        }


        public async Task<Item> FindItemUPC_3(string UPC)


        {
            return _database.Find<Item>(s => s.UPC_3 == UPC);
        }


        public async Task<Item> FindItemUPC_4(string UPC)


        {
            return _database.Find<Item>(s => s.UPC_4 == UPC);
        }


        public async Task<List<Item>> SearchItemsQuickEntry(String sSearch)


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

        public async Task<List<Server>> GetServers()
        {
            String sQuery = "select * from Server";
            return _database.Query<Server>(sQuery);
        }

        public async Task<int> SaveServer(Server server)
        {
            return _database.InsertOrReplace(server);
        }

        public async Task<int> DeleteServer(Server server)
        {
            return _database.Delete(server);
        }

        public async Task SaveItems(List<Item> items)
        {
            _database.RunInTransaction(() =>
            {
                _database.Execute("delete from Item");

                if (items != null && items.Count > 0)
                {
                    _database.InsertAll(items);
                }
            });
        }

        public async Task DeleteDiscontinuedItems(List<int> itemNos)
        {
            if (itemNos == null || itemNos.Count == 0) return;

            const int chunkSize = 500; // stay under SQLite's default variable/expression limits

            for (int i = 0; i < itemNos.Count; i += chunkSize)
            {
                var chunk = itemNos.Skip(i).Take(chunkSize);
                string idList = string.Join(",", chunk); // ints only — no injection risk
                _database.Execute($"delete from DiscontinuedItem where ItemNo in ({idList})");
            }
        }

        public async Task<int> SaveItem(Item item)
        {
            int i = _database.InsertOrReplace(item);
            return i;

        }

        public async Task<int> SaveItemReplace(Item item)
        {
            return _database.InsertOrReplace(item);
        }

        public async Task<int> UpdateItem(Item item)
        {
            return _database.Update(item);

        }

        public async Task<int> DeleteItems()
        {
            return _database.Execute("delete from Item");

        }

        public async Task<List<Item>> GetItems()
        {
            String sQuery = "select * from [Item] ";
            return _database.Query<Item>(sQuery);

        }

        public async Task<int> UpdateItemQty(int iItem, int iQty)
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

        public async Task<int> UpdateItemCreditQty(int iItem, int iQty)
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

        public async Task<int> UpdateItemLabelQty(int iItem, int iQty)
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

        public async Task<int> UpdateItemQty(int iItem, int iQtyOrder, int iQtyCredit, int iQtyLabel)
        {
            _database.Execute("update Item set QtyOrder = QtyOrder + " + iQtyOrder.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update Item set QtyCredit = QtyCredit + " + iQtyCredit.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update Item set QtyLabel = QtyLabel + " + iQtyLabel.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update Item set LineNo = ifnull((select max(LineNo) from Item), 0) + 1 where LineNo = 0 and ItemNo = " + iItem.ToString());

            return 1;

        }

        public async Task<int> UpdateItemQtySet(int iItem, int iQty)
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

        public async Task<int> UpdateItemCreditQtySet(int iItem, int iQty)
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

        public async Task<int> UpdateItemLabelQtySet(int iItem, int iQty)
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

        public async Task<int> UpdateItemQtySet(int iItem, int iQtyOrder, int iQtyCredit, int iQtyLabel, int iLineNo)
        {
            _database.Execute("update Item set QtyOrder = " + iQtyOrder.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update Item set QtyCredit = " + iQtyCredit.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update Item set QtyLabel = " + iQtyLabel.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update Item set LineNo = " + iLineNo.ToString() + " where ItemNo = " + iItem.ToString());

            return 1;

        }

        public async Task<int> UpdateItemQOH(int iItem, int iQOH)
        {
            _database.Execute("update Item set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update ReorderItem set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update OrderDetail set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
            return 1;
        }

        public async Task<int> GetItemQty(int iItem)
        {

            return _database.ExecuteScalar<int>("select QtyOrder from Item where ItemNo = " + iItem.ToString());

        }

        public async Task<int> DeleteSalesCustomers()
        {


            return _database.Execute("delete from [SalesCustomer]");

        }

        public async Task<List<SalesCustomer>> GetSalesCustomers()
        {
            String sQuery = "select * from [SalesCustomer] ";
            return _database.Query<SalesCustomer>(sQuery);
        }

        public async Task<List<SalesCustomer>> GetSalesCustomers(string SearchCustomer)
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

        public async Task UpdateCustomerCartItems()
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

        public async Task<List<SalesCustomer>> GetSalesCustomersWithPendingOrders(string SearchCustomer)
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

        public async Task<SalesCustomer> FindSalesCustomer(string CustNo)
        {
            return _database.Find<SalesCustomer>(s => s.CustNo == CustNo);

        }

        public async Task<List<Category>> GetCategories()
        {
            String sQuery = "select * from Category order by Rank";
            return _database.Query<Category>(sQuery);

        }
        public async Task<int> DeleteAllSalesCustomer()
        {
            return _database.DeleteAll<SalesCustomer>();
        }

        public async Task SaveSalesCustomer(List<SalesCustomer> cust)
        {
            _database.RunInTransaction(() =>
            {
                _database.DeleteAll<SalesCustomer>();

                if (cust != null && cust.Count > 0)
                {
                    _database.InsertAll(cust);
                }
            });
        }

        public async Task<List<Category>> GetHomePageCategories()
        {

            String sQuery = "select * from Category where HomePage > 0 order by HomePage limit 4";
            return _database.Query<Category>(sQuery);

        }

        public async Task<Category> GetCategory(string sCategoryCode)
        {
            return _database.Find<Category>(s => s.Code == sCategoryCode);

        }

        public async Task<int> DeleteAllCategory()
        {
            return _database.DeleteAll<Category>();
        }

        public async Task SaveCategory(List<Category> category)
        {
            _database.RunInTransaction(() =>
            {
                _database.DeleteAll<Category>();

                if (category != null && category.Count > 0)
                {
                    _database.InsertAll(category);
                }
            });
        }

        public async Task<int> DeleteCategories()
        {

            return _database.Execute("delete from Category");

        }

        public async Task<List<Subcategory>> GetSubcategory()
        {
            return _database.Table<Subcategory>().OrderBy(t => t.Description).ToList();

        }
        public async Task<int> DeleteAllSubcategory()
        {
            return _database.DeleteAll<Subcategory>();
        }
        public async Task<List<Subcategory>> GetSubcategory(string sCategoryCode)
        {
            String sQuery = "select * from Subcategory where Category = '" + sCategoryCode + "' order by Description";
            return _database.Query<Subcategory>(sQuery);

        }

        public async Task SaveSubcategory(List<Subcategory> subcategory)
        {
            _database.RunInTransaction(() =>
            {
                _database.DeleteAll<Subcategory>();

                if (subcategory != null && subcategory.Count > 0)
                {
                    _database.InsertAll(subcategory);
                }
            });
        }

        public async Task<int> GetSubcategoryCount(string sCategoryCode)
        {
            String sQuery = "select count(*) from [Subcategory] where Category = '" + sCategoryCode + "'";
            return _database.ExecuteScalar<int>(sQuery);

        }

        public async Task<int> DeleteSubcategory(Subcategory subcategory)
        {
            return _database.Delete(subcategory);
        }

        public async Task<int> DeleteSubcategories()
        {
            return _database.Execute("delete from Subcategory");

        }

        public async Task<List<Subsubcategory>> GetSubsubcategory()
        {
            return _database.Table<Subsubcategory>().OrderBy(t => t.Description).ToList();

        }

        public async Task<List<Subsubcategory>> GetSubsubcategory(string sCategoryCode, string sSubcategoryCode)
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

        public async Task<int> DeleteAllSubsubcategory()
        {
            return _database.DeleteAll<Subsubcategory>();
        }

        public async Task<int> GetSubsubcategoryCount(string sCategoryCode, string sSubcategoryCode)
        {
            String sQuery = "select count(*) from [Subsubcategory] where Category = '" + sCategoryCode + "' and Subcategory = '" + sSubcategoryCode + "'";
            return _database.ExecuteScalar<int>(sQuery);

        }

        public async Task SaveSubsubcategory(List<Subsubcategory> Subsubcategory)
        {
            _database.RunInTransaction(() =>
            {
                _database.DeleteAll<Subsubcategory>();

                if (Subsubcategory != null && Subsubcategory.Count > 0)
                {
                    _database.InsertAll(Subsubcategory);
                }
            });
        }

        public async Task<int> DeleteSubsubcategory(Subsubcategory subsubcategory)
        {


            return _database.Delete(subsubcategory);
        }


        public async Task<int> DeleteSubsubcategories()
        {


            return _database.Execute("delete from Subsubcategory");
        }


        public async Task<int> DeleteBannersAsync()
        {


            return _database.Execute("delete from Banner");
        }


        public async Task SaveBannerAsync(List<Banner> banner)
        {
            _database.RunInTransaction(() =>
            {
                _database.Execute("delete from Banner");

                if (banner != null && banner.Count > 0)
                {
                    _database.InsertAll(banner);
                }
            });
        }


        public async Task<List<Banner>> GetBanners()
        {


            return _database.Table<Banner>().OrderBy(t => t.BannerName).ToList();
        }


        public async Task<int> SaveCustomer(Customer cust)
        {


            _database.Delete(cust);
            int i = _database.Insert(cust);
            return i;
        }


        public async Task<Customer> GetCustomer()
        {


            //String sQuery = "select * from Customer limit 1";
            return _database.Find<Customer>(s => s.CustId == -1);
        }


        public async Task<string> GetSetting(string sKey)
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
                Console.WriteLine(ex.Message);
                return "";
            }
        }


        public async Task<int> SaveSetting(string sKey, string sValue)
        {


            Setting setting = new Setting();
            setting.Key = sKey;
            setting.Value = sValue;

            return _database.InsertOrReplace(setting);

        }

        public async Task<List<Setting>> GetSettings()
        {


            return _database.Table<Setting>().ToList();
        }


        public async Task<int> SaveLocation(Location location)
        {


            return _database.InsertOrReplace(location);
        }


        public async Task<int> DeleteLocations()
        {


            return _database.Execute("delete from Location");
        }


        public async Task<Location> GetLocation(int iLocation)
        {


            return _database.Find<Location>(s => s.LocationId == iLocation);
        }


        public async Task<int> SaveOrderHeader(OrderHeader oh)
        {


            return _database.InsertOrReplace(oh);
        }


        public async Task<List<OrderHeader>> GetOrderHeaders()
        {
            //return _database.Table<OrderHeader>().OrderByDescending(t => t.OrderDate).ToList();


            String sQuery = "select * from [OrderHeader] where [CustId] = " + App.g_Customer.CustNo + " order by OrderDate desc";

            return _database.Query<OrderHeader>(sQuery);
        }


        public async Task<OrderHeader> GetOrderHeader(string sOrderNo)
        {


            return _database.Find<OrderHeader>(s => s.OrderNo == sOrderNo);
        }


        public async Task<int> DeleteOrderHistory()
        {


            _database.Execute("delete from OrderHeader");
            _database.Execute("delete from OrderDetail");
            return 0;
        }


        public async Task<int> SaveOrderDetail(OrderDetail od)
        {


            return _database.InsertOrReplace(od);
        }


        public async Task<int> DeleteOrderDetail(string sOrderNo)
        {


            return _database.Execute("delete from OrderDetail where OrderNo = '" + sOrderNo + "'");
        }


        public async Task<List<OrderDetail>> GetOrderDetail(string sOrderNo)
        {


            String sQuery = "select * from OrderDetail where OrderNo = '" + sOrderNo + "' order by Description";
            return _database.Query<OrderDetail>(sQuery);
        }


        public async Task<int> UpdateOrderDetailLastPurch()
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

        public async Task<List<ReorderItem>> GetReorderItemsOld()
        {


            String sQuery = "select * from ReorderItem where Status = 'A' order by LastPurchDate desc, Description";
            return _database.Query<ReorderItem>(sQuery);

        }

        public async Task<List<Item>> GetReorderItems()
        {


            String sQuery = "select * from Item where Status = 'A' and LastPurchDateDisplay > '' order by LastPurchDate desc, Description";
            return _database.Query<Item>(sQuery);

        }

        public async Task<int> SaveReorderItem(ReorderItem ri)
        {


            return _database.InsertOrReplace(ri);

        }

        public async Task<int> GetReorderItemsCount()
        {


            String sQuery = "select count(*) from [Item] where LastPurchDateDisplay > ''";
            return _database.ExecuteScalar<int>(sQuery);

        }

        public async Task<int> DeleteReorderItems()
        {


            return _database.Execute("delete from ReorderItem");

        }

        public async Task<int> DeleteSavedCartItems()
        {
            return _database.Execute("delete from CartItem");
        }

        public async Task<int> SaveCartItems()
        {
            String sQuery = "insert into CartItem select ItemNo, QtyOrder, QtyCredit, QtyLabel  from [Item] where QtyOrder > 0 or QtyOnOrderSellUnit1 > 0 or QtyOnOrderSellUnit2 > 0 or QtyOnOrderSellUnit3 > 0 or QtyOnOrderSellUnit4 > 0";
            return _database.Execute(sQuery);
        }

        public async Task<List<CartItem>> GetSavedCartItems()
        {
            String sQuery = "select * from CartItem";
            return _database.Query<CartItem>(sQuery);
        }
        public async Task<int> SuspendCartItems(string CustNo)
        {


            String sQuery = "insert into SuspendItem select '" + CustNo + "', ItemNo, QtyOrder, QtyCredit, QtyLabel, '" + App.g_ServerURL + "', LineNo from [Item] where QtyOrder > 0 or QtyCredit > 0 or QtyLabel > 0";
            return _database.Execute(sQuery);

        }

        public async Task<List<SuspendItem>> GetSuspendedCartItems(string CustNo)
        {


            String sQuery = "select * from SuspendItem where CustNo = '" + CustNo + "' and ServerURL = '" + App.g_ServerURL + "'";
            return _database.Query<SuspendItem>(sQuery);

        }

        public async Task<int> RestoreCartItems(string CustNo)
        {


            List<SuspendItem> items = await GetSuspendedCartItems(CustNo);

            foreach (SuspendItem item in items)
            {
                if (item.QtyOrder > 0)
                {
                    await UpdateItemQtySet(item.ItemNo, item.QtyOrder, item.QtyCredit, item.QtyLabel, item.LineNo);
                }
            }

            await DeleteSuspendedCartItems(CustNo);

            return 0;

        }

        public async Task<int> DeleteSuspendedCartItems(string CustNo)
        {


            return _database.Execute("delete from SuspendItem where CustNo = '" + CustNo + "' and ServerURL = '" + App.g_ServerURL + "'");

        }

        public async Task<int> ClearFlyerItems()
        {


            return _database.Execute("update Item set FlyerPageNo = 0, FlyerBoxNo = 0, FlyerSection = '', FlyerStartDate = 0, FlyerEndDate = 0, FlyerTopLeftX = 0, FlyerTopLeftY = 0, FlyerBottomRightX = 0, FlyerBottomRightY = 0");

        }
        public async Task<int> UpdateItemFlyerInfo(FlyerItem item)
        {


            String sUpdate = "update Item set FlyerPageNo = " + item.Page.ToString() + ", FlyerBoxNo = " + item.Box.ToString();
            sUpdate += ", FlyerSection = '" + item.Section + "', FlyerStartDate = " + item.StartDate.ToString() + ", FlyerEndDate = " + item.EndDate.ToString();
            sUpdate += ", FlyerTopLeftX = " + item.TopLeftX.ToString() + ", FlyerTopLeftY = " + item.TopLeftY.ToString() + ", FlyerBottomRightX = " + item.BottomRightX.ToString() + ", FlyerBottomRightY = " + item.BottomRightY.ToString();
            sUpdate += " where ItemNo = " + item.ItemNo.ToString();

            return _database.Execute(sUpdate);

        }
        public async Task<int> GetFlyerItemCount()
        {


            String sQuery = "select count(*) from [Item] where FlyerStartDate <= " + DateTime.Now.ToString("1yyMMdd") + " and FlyerEndDate >= " + DateTime.Now.ToString("1yyMMdd");
            return _database.ExecuteScalar<int>(sQuery);

        }

        public async Task<List<Item>> SearchItemsMonthlyAdClick(int iPage, int iX, int iY)
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

        public async Task<int> DeleteAll()
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
