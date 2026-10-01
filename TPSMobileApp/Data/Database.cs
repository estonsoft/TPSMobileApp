using Realms;
using System.Reflection;

namespace TPSMobileApp
{
    public class Database
    {
        private readonly RealmConfiguration _config;
        private Realm _mainThreadRealm;
        private static Database _instance;
        private static readonly object _lock = new object();

        private Database()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tps_mobile_data.realm");

            _config = new RealmConfiguration(dbPath)
            {
                SchemaVersion = 1,
                MigrationCallback = (migration, oldSchemaVersion) => { }
            };
        }

        public static Database Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Database();
                    }
                }
            }
            return _instance;
        }

        private Realm GetRealm()
        {
            if (MainThread.IsMainThread)
            {
                if (_mainThreadRealm == null || _mainThreadRealm.IsClosed)
                {
                    _mainThreadRealm = Realm.GetInstance(_config);
                }
                return _mainThreadRealm;
            }

            return Realm.GetInstance(_config);
        }

        private static T? CopyRealmObject<T>(T? source) where T : RealmObject, new()
        {
            if (source == null) return null;

            var copy = new T();
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetIndexParameters().Length > 0 || property.GetMethod?.IsPublic != true || property.GetSetMethod(true) == null)
                {
                    continue;
                }

                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (propertyType.IsValueType || propertyType == typeof(string))
                {
                    property.SetValue(copy, property.GetValue(source));
                }
            }

            if (source is Item sourceItem && copy is Item copyItem)
            {
                copyItem.custNo = sourceItem.custNo;
            }

            return copy;
        }

        private static List<T> CopyRealmObjects<T>(IEnumerable<T> source) where T : RealmObject, new() =>
            source.Select(item => CopyRealmObject(item)!).ToList();

        #region Smart Item Search Engine
        public async Task<List<Item>> SearchItems(string sSearch, Category category, string sBarcode, Subcategory subcategory, Subsubcategory subsubcategory, int offset = 0, int pageSize = 30)
        {
            offset = Math.Max(0, offset);
            pageSize = Math.Max(1, pageSize);

            decimal dItemNo = 0;
            string sBarcodeShort = sBarcode ?? "";
            if (sBarcodeShort.Length > 11)
            {
                sBarcodeShort = sBarcodeShort.Substring(0, 11);
            }

            string savedCategoryCode = category?.Code ?? "";
            string savedSubcategoryCode = subcategory?.Code ?? "";

            try
            {
                if (!string.IsNullOrEmpty(sBarcode) && sBarcode.Length <= 6)
                {
                    dItemNo = decimal.Parse(sBarcode);
                    sBarcodeShort = dItemNo.ToString();
                    savedCategoryCode = "";
                    savedSubcategoryCode = "";
                }
            }
            catch { }

            if (decimal.TryParse(sSearch, out dItemNo))
            {
                sBarcodeShort = dItemNo.ToString();
                savedCategoryCode = "";
                savedSubcategoryCode = "";
            }

            string[] searchWords = (sSearch ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var realm = GetRealm();
            var query = realm.All<Item>().Where(i => i.Status == "A");

            if (App.g_InStockOnly)
            {
                query = query.Where(i => i.QOH > 0);
            }

            if (!string.IsNullOrEmpty(savedCategoryCode))
            {
                query = query.Where(i => i.CategoryCode == savedCategoryCode);
            }

            if (!string.IsNullOrEmpty(savedSubcategoryCode))
            {
                query = query.Where(i => i.SubcategoryCode == savedSubcategoryCode);
            }

            if (string.IsNullOrEmpty(sBarcode) && searchWords.Length == 0)
            {
                IEnumerable<Item> orderedItems = App.g_IsTopSellers
                    ? query.OrderBy(i => i.CategoryRank).AsEnumerable().Take(25)
                    : query.OrderBy(i => i.Description).AsEnumerable();

                return CopyRealmObjects(orderedItems.Skip(offset).Take(pageSize));
            }

            var workingList = query.ToList();

            if (!string.IsNullOrEmpty(sBarcode))
            {
                workingList = workingList.Where(i =>
                    ((i.UPC_1.Contains(sBarcode) || i.UPC_1.Contains(sBarcodeShort)) && i.UPC_1.Length > 0) ||
                    ((i.UPC_2.Contains(sBarcode) || i.UPC_2.Contains(sBarcodeShort)) && i.UPC_2.Length > 0) ||
                    i.ItemNoDisplay == sBarcode || i.ItemNoDisplay == sBarcodeShort
                ).ToList();
            }
            else
            {
                workingList = workingList.Where(i =>
                    i.ItemNoDisplay == sSearch ||
                    i.UPC_1 == sSearch ||
                    searchWords.All(word =>
                        i.Description.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                        i.ItemNoDisplay.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                        i.UPC_1.Contains(word, StringComparison.OrdinalIgnoreCase)
                    )
                ).ToList();
            }

            if (App.g_IsTopSellers)
            {
                return CopyRealmObjects(workingList.OrderBy(i => i.CategoryRank).Take(25).Skip(offset).Take(pageSize));
            }

            return CopyRealmObjects(workingList.OrderBy(i => i.Description).Skip(offset).Take(pageSize));
        }

        public async Task<List<Item>> SearchItemsQuickEntry(string sSearch, int offset = 0, int pageSize = int.MaxValue)
        {
            offset = Math.Max(0, offset);
            pageSize = Math.Max(1, pageSize);
            decimal dItemNo = 0;
            if (!string.IsNullOrEmpty(sSearch) && sSearch.Length <= 6 && decimal.TryParse(sSearch, out var parsed))
            {
                dItemNo = parsed;
            }

            string sSearch2 = "";
            string cleanSearch = (sSearch ?? "").Replace("'", "");

            if (cleanSearch.Length >= 6 && cleanSearch.Length <= 8)
            {
                sSearch2 = cleanSearch;
                string expanded = UPCExpand(cleanSearch);
                if (!string.IsNullOrEmpty(expanded))
                {
                    cleanSearch = expanded;
                }
            }

            string sSearchShort = cleanSearch;
            string sSearchShort2 = cleanSearch;
            if (cleanSearch.Length == 13)
            {
                sSearchShort = cleanSearch.Substring(2, 11);
            }
            else if (cleanSearch.Length > 11)
            {
                sSearchShort2 = cleanSearch.Substring(0, 11);
            }

            var realm = GetRealm();
            var query = realm.All<Item>().Where(i => i.Status != "D");
            string dItemNoStr = dItemNo > 0 ? dItemNo.ToString() : "";

            var resultList = query.ToList().Where(i =>
                ((i.UPC_1.Contains(cleanSearch) || i.UPC_1.Contains(sSearchShort) || i.UPC_1.Contains(sSearchShort2)) && i.UPC_1.Length > 0) ||
                ((i.UPC_2.Contains(cleanSearch) || i.UPC_2.Contains(sSearchShort) || i.UPC_2.Contains(sSearchShort2)) && i.UPC_2.Length > 0) ||
                ((i.UPC_3.Contains(cleanSearch) || i.UPC_3.Contains(sSearchShort) || i.UPC_3.Contains(sSearchShort2)) && i.UPC_3.Length > 0) ||
                ((i.UPC_4.Contains(cleanSearch) || i.UPC_4.Contains(sSearchShort) || i.UPC_4.Contains(sSearchShort2)) && i.UPC_4.Length > 0) ||
                (sSearch2 != "" && (i.UPC_1 == sSearch2 || i.UPC_2 == sSearch2 || i.UPC_3 == sSearch2 || i.UPC_4 == sSearch2)) ||
                (dItemNoStr != "" && i.ItemNoDisplay.Contains(dItemNoStr))
            ).ToList();

            return CopyRealmObjects(resultList.Skip(offset).Take(pageSize));
        }

        private string UPCExpand(string sUPC)
        {
            if (string.IsNullOrEmpty(sUPC)) return "";
            if (sUPC.Length == 8) sUPC = sUPC.Substring(1, 6);
            if (sUPC.Length == 6) sUPC = "0" + sUPC;
            if (sUPC.Length < 7) return "";

            string D1 = sUPC.Substring(0, 1);
            string D2 = sUPC.Substring(1, 1);
            string D3 = sUPC.Substring(2, 1);
            string D4 = sUPC.Substring(3, 1);
            string D5 = sUPC.Substring(4, 1);
            string D6 = sUPC.Substring(5, 1);
            string D7 = sUPC.Substring(6, 1);

            return D7 switch
            {
                "0" => D1 + D2 + D3 + "00000" + D4 + D5 + D6,
                "1" => D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6,
                "2" => D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6,
                "3" => D1 + D2 + D3 + D4 + "00000" + D5 + D6,
                "4" => D1 + D2 + D3 + D4 + D5 + "00000" + D6,
                _ => D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7
            };
        }
        #endregion

        #region Discontinued Processing Matrix
        public async Task<int> InsertDiscontinuedItems()
        {
            var realm = GetRealm();
            var allItems = realm.All<Item>().ToList();

            realm.Write(() =>
            {
                realm.RemoveAll<DiscontinuedItem>();
                foreach (var item in allItems)
                {
                    realm.Add(new DiscontinuedItem { ItemNo = item.ItemNo });
                }
            });
            return allItems.Count;
        }

        public async Task<int> DeleteDiscontinuedItem(string itemNo)
        {
            if (!int.TryParse(itemNo, out int targetNo)) return 0;

            var realm = GetRealm();
            var item = realm.All<DiscontinuedItem>().FirstOrDefault(d => d.ItemNo == targetNo);
            if (item == null) return 0;

            realm.Write(() => realm.Remove(item));
            return 1;
        }

        public async Task<int> UpdateDiscontinuedItems()
        {
            var realm = GetRealm();
            var targetIds = realm.All<DiscontinuedItem>().ToList().Select(d => d.ItemNo).ToList();
            var matches = realm.All<Item>().AsEnumerable().Where(i => targetIds.Contains(i.ItemNo)).ToList();

            realm.Write(() =>
            {
                foreach (var item in matches)
                {
                    item.Status = "D";
                }
            });
            return matches.Count;
        }

        public async Task DeleteDiscontinuedItems(List<int> itemNos)
        {
            if (itemNos == null || itemNos.Count == 0) return;

            var realm = GetRealm();
            var itemsToDelete = realm.All<DiscontinuedItem>()
                                     .AsEnumerable()
                                     .Where(d => itemNos.Contains(d.ItemNo))
                                     .ToList();

            realm.Write(() =>
            {
                foreach (var item in itemsToDelete)
                {
                    realm.Remove(item);
                }
            });
        }
        #endregion

        #region Cart & Order Processing Matrix
        public async Task<List<Item>> GetCartItems()
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<Item>()
                .Where(i => i.QtyOrder > 0 || i.QtyCredit > 0 || i.QtyLabel > 0));
        }

        public async Task<List<Item>> GetOrderCartItems()
        {
            var realm = GetRealm();
            var query = realm.All<Item>().Where(i => i.QtyOrder > 0);

            if (App.g_ShoppingCartSort == "F") return CopyRealmObjects(query.OrderBy(i => i.LineNo));
            if (App.g_ShoppingCartSort == "L") return CopyRealmObjects(query.OrderByDescending(i => i.LineNo));

            return CopyRealmObjects(query.OrderBy(i => i.Description));
        }

        public async Task<List<Item>> GetReturnCartItems()
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<Item>().Where(i => i.QtyCredit > 0).OrderBy(i => i.Description));
        }

        public async Task<List<Item>> GetLabelCartItems()
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<Item>().Where(i => i.QtyLabel > 0).OrderBy(i => i.Description));
        }

        public async Task<int> GetCartPieces()
        {
            var realm = GetRealm();
            var items = realm.All<Item>();
            int totalPieces = 0;

            // 🌟 FASTEST: Direct index iteration executes directly in the native layer
            for (int i = 0; i < items.Count(); i++)
            {
                totalPieces += items.ElementAt(i).QtyOrder;
            }
            return totalPieces;
        }

        public async Task<int> ClearCartItems()
        {
            var realm = GetRealm();
            var items = realm.All<Item>().Where(i => i.QtyOrder != 0 || i.QtyCredit != 0 || i.QtyLabel != 0 || i.PriceOrder != 0 || i.LineNo != 0).ToList();

            realm.Write(() =>
            {
                foreach (var i in items)
                {
                    i.QtyOrder = 0; i.QtyCredit = 0; i.QtyLabel = 0; i.PriceOrder = 0; i.LineNo = 0;
                }
            });
            return items.Count;
        }

        public async Task<int> ClearOrderCartItems()
        {
            var realm = GetRealm();
            var items = realm.All<Item>().Where(i => i.QtyOrder != 0 || i.PriceOrder != 0 || i.LineNo != 0).ToList();

            realm.Write(() =>
            {
                foreach (var i in items)
                {
                    i.QtyOrder = 0; i.PriceOrder = 0; i.LineNo = 0;
                }
            });
            return items.Count;
        }

        public async Task<int> ClearReturnCartItems()
        {
            var realm = GetRealm();
            var items = realm.All<Item>().Where(i => i.QtyCredit != 0).ToList();
            realm.Write(() => { foreach (var i in items) i.QtyCredit = 0; });
            return items.Count;
        }

        public async Task<int> ClearLabelCartItems()
        {
            var realm = GetRealm();
            var items = realm.All<Item>().Where(i => i.QtyLabel != 0).ToList();
            realm.Write(() => { foreach (var i in items) i.QtyLabel = 0; });
            return items.Count;
        }
        #endregion

        #region Basic CRUD Engine
        public async Task<int> GetItemCount()
        {
            var realm = GetRealm();
            return realm.All<Item>().Count();
        }

        public async Task<Item> FindItem(int item_no, string item_ref_no)
        {
            var realm = GetRealm();
            var item = App.g_IsRefNoLookup
                ? realm.All<Item>().FirstOrDefault(s => s.ItemRefNo == item_ref_no)
                : realm.Find<Item>(item_no);
            return CopyRealmObject(item)!;
        }

        public async Task<Item> FindItemUPC_1(string upc) => CopyRealmObject(GetRealm().All<Item>().FirstOrDefault(s => s.UPC_1 == upc))!;
        public async Task<Item> FindItemUPC_2(string upc) => CopyRealmObject(GetRealm().All<Item>().FirstOrDefault(s => s.UPC_2 == upc))!;
        public async Task<Item> FindItemUPC_3(string upc) => CopyRealmObject(GetRealm().All<Item>().FirstOrDefault(s => s.UPC_3 == upc))!;
        public async Task<Item> FindItemUPC_4(string upc) => CopyRealmObject(GetRealm().All<Item>().FirstOrDefault(s => s.UPC_4 == upc))!;

        public async Task<int> SaveItem(Item item) => await SaveItemReplace(item);

        public async Task<int> SaveItemReplace(Item item)
        {
            if (item == null) return 0;
            var itemSnapshot = CopyRealmObject(item)!;
            var realm = GetRealm();
            realm.Write(() => realm.Add(itemSnapshot, update: true));
            return 1;
        }

        public async Task<int> UpdateItem(Item item) => await SaveItemReplace(item);

        public async Task<int> DeleteItems()
        {
            var realm = GetRealm();
            int count = realm.All<Item>().Count();
            realm.Write(() => realm.RemoveAll<Item>());
            return count;
        }

        public async Task<List<Item>> GetItems()
        {
            return CopyRealmObjects(GetRealm().All<Item>());
        }

        public async Task SaveItems(List<Item> items)
        {
            var itemSnapshots = items == null ? new List<Item>() : CopyRealmObjects(items);
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<Item>();
                foreach (var item in itemSnapshots)
                {
                    realm.Add(item, update: true);
                }
            });
        }
        #endregion

        #region Server Configuration Settings
        public async Task<List<Server>> GetServers() => CopyRealmObjects(GetRealm().All<Server>());

        public async Task<int> SaveServer(Server server)
        {
            if (server == null) return 0;
            var serverSnapshot = CopyRealmObject(server)!;
            var realm = GetRealm();
            realm.Write(() => realm.Add(serverSnapshot, update: true));
            return 1;
        }

        public async Task<int> DeleteServer(Server server)
        {
            if (server == null) return 0;
            var realm = GetRealm();
            var match = realm.Find<Server>(server.ServerURL);
            if (match == null) return 0;
            realm.Write(() => realm.Remove(match));
            return 1;
        }
        #endregion

        #region Inventory Accumulator Controls
        private void TriggerDeviceVibration()
        {
            try { Vibration.Vibrate(200); } catch { }
        }

        public async Task<int> UpdateItemQty(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() =>
            {
                item.QtyOrder += iQty;
                int maxLineNo = realm.All<Item>().Any() ? realm.All<Item>().Max(i => i.LineNo) : 0;
                if (item.LineNo == 0) item.LineNo = maxLineNo + 1;
            });

            TriggerDeviceVibration();
            return 1;
        }

        public async Task<int> UpdateItemCreditQty(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() => item.QtyCredit += iQty);
            TriggerDeviceVibration();
            return 1;
        }

        public async Task<int> UpdateItemLabelQty(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() => item.QtyLabel += iQty);
            TriggerDeviceVibration();
            return 1;
        }

        public async Task<int> UpdateItemQty(int iItem, int iQtyOrder, int iQtyCredit, int iQtyLabel)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() =>
            {
                item.QtyOrder += iQtyOrder;
                item.QtyCredit += iQtyCredit;
                item.QtyLabel += iQtyLabel;
                int maxLineNo = realm.All<Item>().Any() ? realm.All<Item>().Max(i => i.LineNo) : 0;
                if (item.LineNo == 0) item.LineNo = maxLineNo + 1;
            });
            return 1;
        }

        public async Task<int> UpdateItemQtySet(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() =>
            {
                item.QtyOrder = iQty;
                int maxLineNo = realm.All<Item>().Any() ? realm.All<Item>().Max(i => i.LineNo) : 0;
                if (item.LineNo == 0) item.LineNo = maxLineNo + 1;
            });

            TriggerDeviceVibration();
            return 1;
        }

        public async Task<int> UpdateItemCreditQtySet(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() => item.QtyCredit = iQty);
            TriggerDeviceVibration();
            return 1;
        }

        public async Task<int> UpdateItemLabelQtySet(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() => item.QtyLabel = iQty);
            TriggerDeviceVibration();
            return 1;
        }

        public async Task<int> UpdateItemQtySet(int iItem, int iQtyOrder, int iQtyCredit, int iQtyLabel, int iLineNo)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() =>
            {
                item.QtyOrder = iQtyOrder;
                item.QtyCredit = iQtyCredit;
                item.QtyLabel = iQtyLabel;
                item.LineNo = iLineNo;
            });
            return 1;
        }

        public async Task<int> UpdateItemQOH(int iItem, int iQOH)
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                var item = realm.Find<Item>(iItem);
                if (item != null) item.QOH = iQOH;

                var reorder = realm.Find<ReorderItem>(iItem);
                if (reorder != null) reorder.QOH = iQOH;

                var orderDetails = realm.All<OrderDetail>().Where(d => d.ItemNo == iItem);
                foreach (var od in orderDetails) od.QOH = iQOH;
            });
            return 1;
        }

        public async Task<int> UpdateAllItemQOH(List<(int ItemNo, int QOH)> updates)
        {
            if (updates == null || updates.Count == 0) return 0;
            var realm = GetRealm();

            realm.Write(() =>
            {
                foreach (var update in updates)
                {
                    int itemNo = update.ItemNo;
                    int quantity = update.QOH;
                    var item = realm.Find<Item>(itemNo);
                    if (item != null) item.QOH = quantity;

                    var reorder = realm.Find<ReorderItem>(itemNo);
                    if (reorder != null) reorder.QOH = quantity;

                    var orderDetails = realm.All<OrderDetail>().Where(d => d.ItemNo == itemNo);
                    foreach (var od in orderDetails) od.QOH = quantity;
                }
            });
            return updates.Count;
        }

        public async Task<int> GetItemQty(int iItem)
        {
            var item = GetRealm().Find<Item>(iItem);
            return item?.QtyOrder ?? 0;
        }
        #endregion

        #region B2B Client CRM Mappings
        public async Task<int> DeleteSalesCustomers()
        {
            var realm = GetRealm();
            int count = realm.All<SalesCustomer>().Count();
            realm.Write(() => realm.RemoveAll<SalesCustomer>());
            return count;
        }

        public async Task<List<SalesCustomer>> GetSalesCustomers() => CopyRealmObjects(GetRealm().All<SalesCustomer>());

        public async Task<List<SalesCustomer>> GetSalesCustomers(string searchCustomer)
        {
            var realm = GetRealm();
            var query = realm.All<SalesCustomer>();

            if (!string.IsNullOrEmpty(searchCustomer))
            {
                string customerNo = searchCustomer.Trim();
                string cleanStr = customerNo.Replace("'", "");
                query = query.Where(c => c.CompanyName.Contains(cleanStr) || c.CustNo == customerNo);
            }

            return CopyRealmObjects(query.OrderBy(c => c.CompanyName));
        }

        public async Task UpdateCustomerCartItems()
        {
            var realm = GetRealm();
            string serverUrl = App.g_ServerURL;
            realm.Write(() =>
            {
                var customers = realm.All<SalesCustomer>().ToList();
                foreach (var customer in customers)
                {
                    string customerNo = customer.CustNo;
                    var pendingQuery = realm.All<SuspendItem>()
                        .Where(s => s.CustNo == customerNo && s.ServerURL == serverUrl);

                    int totalPending = 0;

                    // 2. 🌟 ZERO-ALLOCATION FIX: Loop via index to sum the fields natively
                    for (int idx = 0; idx < pendingQuery.Count(); idx++)
                    {
                        totalPending += pendingQuery.ElementAt(idx).QtyOrder;
                    }

                    if (totalPending > 0)
                    {
                        customer.ShoppingCartItems = totalPending;
                    }
                }

                if (App.g_Customer != null)
                {
                    var activeCust = realm.Find<SalesCustomer>(App.g_Customer.CustNo);
                    if (activeCust != null)
                    {
                        var activeCartQuery = realm.All<Item>().Where(i => i.QtyOrder > 0);
                        int totalActivePieces = 0;

                        // 2. 🌟 ZERO-ALLOCATION LOOP: Calculate total via direct index evaluation
                        for (int idx = 0; idx < activeCartQuery.Count(); idx++)
                        {
                            totalActivePieces += activeCartQuery.ElementAt(idx).QtyOrder;
                        }

                        // 3. Assign the final sum safely within your write transaction
                        activeCust.ShoppingCartItems = totalActivePieces;
                    }
                }
            });
        }

        public async Task<List<SalesCustomer>> GetSalesCustomersWithPendingOrders(string searchCustomer)
        {
            var realm = GetRealm();
            var query = realm.All<SalesCustomer>().Where(c => c.ShoppingCartItems > 0);

            if (!string.IsNullOrEmpty(searchCustomer))
            {
                string customerNo = searchCustomer.Trim();
                string cleanStr = customerNo.Replace("'", "");
                query = query.Where(c => c.CompanyName.Contains(cleanStr) || c.CustNo == customerNo);
            }

            return CopyRealmObjects(query.OrderBy(c => c.CompanyName));
        }

        public async Task<SalesCustomer> FindSalesCustomer(string custNo) => CopyRealmObject(GetRealm().Find<SalesCustomer>(custNo))!;
        public async Task<int> DeleteAllSalesCustomer() => await DeleteSalesCustomers();

        public async Task SaveSalesCustomer(List<SalesCustomer> customersList)
        {
            var customerSnapshots = customersList == null ? new List<SalesCustomer>() : CopyRealmObjects(customersList);
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<SalesCustomer>();
                foreach (var customer in customerSnapshots)
                {
                    realm.Add(customer, update: true);
                }
            });
        }
        #endregion

        #region Categorization Methods
        public async Task<List<Category>> GetCategories() => CopyRealmObjects(
            GetRealm().All<Category>().OrderBy(c => c.Rank));

        public async Task<List<Category>> GetHomePageCategories()
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<Category>()
                .Where(c => c.HomePage > 0)
                .OrderBy(c => c.HomePage)
                .ToList()
                .Take(4));
        }

        public async Task<Category> GetCategory(string sCategoryCode)
        {
            var category = GetRealm().Find<Category>(sCategoryCode);
            return CopyRealmObject(category)!;
        }

        public async Task<int> DeleteAllCategory() => await DeleteCategories();

        public async Task SaveCategory(List<Category> categoryList)
        {
            var categorySnapshots = categoryList == null ? new List<Category>() : CopyRealmObjects(categoryList);
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<Category>();
                foreach (var category in categorySnapshots)
                {
                    realm.Add(category, update: true);
                }
            });
        }

        public async Task<int> DeleteCategories()
        {
            var realm = GetRealm();
            int count = realm.All<Category>().Count();
            realm.Write(() => realm.RemoveAll<Category>());
            return count;
        }

        public async Task<List<Subcategory>> GetSubcategory() => CopyRealmObjects(GetRealm().All<Subcategory>().OrderBy(s => s.Description));
        public async Task<int> DeleteAllSubcategory()
        {
            var realm = GetRealm();
            int count = realm.All<Subcategory>().Count();
            realm.Write(() => realm.RemoveAll<Subcategory>());
            return count;
        }

        public async Task<List<Subcategory>> GetSubcategory(string sCategoryCode)
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<Subcategory>().Where(s => s.Category == sCategoryCode).OrderBy(s => s.Description));
        }

        public async Task SaveSubcategory(List<Subcategory> subcatList)
        {
            var subcategorySnapshots = subcatList == null ? new List<Subcategory>() : CopyRealmObjects(subcatList);
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<Subcategory>();
                foreach (var subcategory in subcategorySnapshots)
                {
                    realm.Add(subcategory, update: true);
                }
            });
        }

        public async Task<int> GetSubcategoryCount(string sCategoryCode)
        {
            return GetRealm().All<Subcategory>().Where(s => s.Category == sCategoryCode).Count();
        }

        public async Task<int> DeleteSubcategory(Subcategory subcategory)
        {
            if (subcategory == null) return 0;
            var realm = GetRealm();
            var match = realm.Find<Subcategory>(subcategory.Code);
            if (match == null) return 0;
            realm.Write(() => realm.Remove(match));
            return 1;
        }

        public async Task<int> DeleteSubcategories() => await DeleteAllSubcategory();

        public async Task<List<Subsubcategory>> GetSubsubcategory() => CopyRealmObjects(GetRealm().All<Subsubcategory>().OrderBy(s => s.Description));

        public async Task<List<Subsubcategory>> GetSubsubcategory(string sCategoryCode, string sSubcategoryCode)
        {
            var cat = sCategoryCode?.Trim() ?? "";
            var sub = sSubcategoryCode?.Trim() ?? "";
            return CopyRealmObjects(GetRealm().All<Subsubcategory>().Where(s => s.Category == cat && s.Subcategory == sub));
        }

        public async Task<int> DeleteAllSubsubcategory()
        {
            var realm = GetRealm();
            int count = realm.All<Subsubcategory>().Count();
            realm.Write(() => realm.RemoveAll<Subsubcategory>());
            return count;
        }

        public async Task<int> GetSubsubcategoryCount(string sCategoryCode, string sSubcategoryCode)
        {
            var cat = sCategoryCode?.Trim() ?? "";
            var sub = sSubcategoryCode?.Trim() ?? "";
            return GetRealm().All<Subsubcategory>().Where(s => s.Category == cat && s.Subcategory == sub).Count();
        }

        public async Task SaveSubsubcategory(List<Subsubcategory> subSubList)
        {
            var subsubcategorySnapshots = subSubList == null ? new List<Subsubcategory>() : CopyRealmObjects(subSubList);
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<Subsubcategory>();
                foreach (var subsubcategory in subsubcategorySnapshots)
                {
                    realm.Add(subsubcategory, update: true);
                }
            });
        }

        public async Task<int> DeleteSubsubcategory(Subsubcategory subsubcategory)
        {
            if (subsubcategory == null) return 0;
            var realm = GetRealm();
            var match = realm.Find<Subsubcategory>(subsubcategory.Code);
            if (match == null) return 0;
            realm.Write(() => realm.Remove(match));
            return 1;
        }

        public async Task<int> DeleteSubsubcategories() => await DeleteAllSubsubcategory();
        #endregion

        #region UI Graphic Banner Matrix
        public async Task<int> DeleteBannersAsync()
        {
            var realm = GetRealm();
            int count = realm.All<Banner>().Count();
            realm.Write(() => realm.RemoveAll<Banner>());
            return count;
        }

        public async Task SaveBannerAsync(List<Banner> bannerList)
        {
            var bannerSnapshots = bannerList == null ? new List<Banner>() : CopyRealmObjects(bannerList);
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<Banner>();
                foreach (var banner in bannerSnapshots)
                {
                    realm.Add(banner, update: true);
                }
            });
        }

        public async Task<List<Banner>> GetBanners() => CopyRealmObjects(GetRealm().All<Banner>().OrderBy(b => b.BannerName));
        #endregion

        #region B2B Profile
        public async Task<int> SaveCustomer(Customer cust)
        {
            if (cust == null) return 0;
            var customerSnapshot = CopyRealmObject(cust)!;
            var realm = GetRealm();
            realm.Write(() =>
            {
                var match = realm.Find<Customer>(customerSnapshot.CustId);
                if (match != null) realm.Remove(match);
                realm.Add(customerSnapshot);
            });
            return 1;
        }

        public async Task<Customer> GetCustomer() => CopyRealmObject(GetRealm().Find<Customer>(-1))!;
        #endregion

        #region Dynamic Local Key-Value Configuration Engine
        public async Task<string> GetSetting(string sKey)
        {
            try
            {
                var setting = GetRealm().Find<Setting>(sKey);
                return setting?.Value ?? "";
            }
            catch { return ""; }
        }

        public async Task<int> SaveSetting(string sKey, string sValue)
        {
            var realm = GetRealm();
            realm.Write(() => realm.Add(new Setting { Key = sKey, Value = sValue }, update: true));
            return 1;
        }

        public async Task<List<Setting>> GetSettings() => CopyRealmObjects(GetRealm().All<Setting>());
        #endregion

        #region Location Logistics Matrix
        public async Task<int> SaveLocation(Location location)
        {
            if (location == null) return 0;
            var locationSnapshot = CopyRealmObject(location)!;
            var realm = GetRealm();
            realm.Write(() => realm.Add(locationSnapshot, update: true));
            return 1;
        }

        public async Task<int> DeleteLocations()
        {
            var realm = GetRealm();
            int count = realm.All<Location>().Count();
            realm.Write(() => realm.RemoveAll<Location>());
            return count;
        }

        public async Task<Location> GetLocation(int iLocation) => CopyRealmObject(GetRealm().Find<Location>(iLocation))!;
        #endregion

        #region B2B Invoice History Engine
        public async Task<int> SaveOrderHeader(OrderHeader oh)
        {
            if (oh == null) return 0;
            var headerSnapshot = CopyRealmObject(oh)!;
            var realm = GetRealm();
            realm.Write(() => realm.Add(headerSnapshot, update: true));
            return 1;
        }

        public Task SaveOrderHeaders(List<OrderHeader> headers)
        {
            if (headers == null) return Task.CompletedTask;
            var headerSnapshots = CopyRealmObjects(headers);
            var realm = GetRealm();
            realm.Write(() => { foreach (var header in headerSnapshots) realm.Add(header, update: true); });
            return Task.CompletedTask;
        }

        public Task SaveOrderDetails(List<OrderDetail> details)
        {
            if (details == null) return Task.CompletedTask;
            var detailSnapshots = CopyRealmObjects(details);
            var realm = GetRealm();
            realm.Write(() => { foreach (var detail in detailSnapshots) realm.Add(detail, update: true); });
            return Task.CompletedTask;
        }

        public async Task<List<OrderHeader>> GetOrderHeaders()
        {
            var realm = GetRealm();
            string targetCustNo = App.g_Customer?.CustNo ?? "";
            return CopyRealmObjects(realm.All<OrderHeader>()
                .ToList()
                .Where(oh => oh.CustId.ToString() == targetCustNo)
                .OrderByDescending(oh => oh.OrderDate));
        }

        public async Task<OrderHeader> GetOrderHeader(string sOrderNo) => CopyRealmObject(GetRealm().Find<OrderHeader>(sOrderNo))!;

        public async Task<int> DeleteOrderHistory()
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<OrderHeader>();
                realm.RemoveAll<OrderDetail>();
            });
            return 0;
        }

        public async Task<int> SaveOrderDetail(OrderDetail od)
        {
            if (od == null) return 0;
            var detailSnapshot = CopyRealmObject(od)!;
            var realm = GetRealm();
            realm.Write(() => realm.Add(detailSnapshot, update: true));
            return 1;
        }

        public async Task<int> DeleteOrderDetail(string sOrderNo)
        {
            var realm = GetRealm();
            var targets = realm.All<OrderDetail>().Where(d => d.OrderNo == sOrderNo).ToList();
            realm.Write(() => { foreach (var t in targets) realm.Remove(t); });
            return targets.Count;
        }

        public async Task<List<OrderDetail>> GetOrderDetail(string sOrderNo)
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<OrderDetail>().Where(d => d.OrderNo == sOrderNo).OrderBy(d => d.Description));
        }

        public async Task<int> UpdateOrderDetailLastPurch()
        {
            var realm = GetRealm();
            var details = realm.All<OrderDetail>().ToList();

            realm.Write(() =>
            {
                foreach (var od in details)
                {
                    var matchingItem = realm.Find<Item>(od.ItemNo);
                    if (matchingItem != null)
                    {
                        od.LastPurchDate = matchingItem.LastPurchDate;
                        od.LastPurchDateDisplay = matchingItem.LastPurchDateDisplay;
                        od.QtyLastOrder = matchingItem.QtyLastOrder;
                        od.QtyOrderDisplay = matchingItem.QtyOrderDisplay;
                        od.QtyLast90 = matchingItem.QtyLast90;
                        od.QtyLast90Display = matchingItem.QtyLast90Display;
                    }
                }
            });
            return 1;
        }
        #endregion

        #region Stock Reordering Triggers
        public async Task<List<ReorderItem>> GetReorderItemsOld()
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<ReorderItem>().Where(r => r.Status == "A").OrderByDescending(r => r.LastPurchDate).ThenBy(r => r.Description));
        }

        public async Task<List<Item>> GetReorderItems()
        {
            var realm = GetRealm();
            return CopyRealmObjects(realm.All<Item>()
                .Where(i => i.Status == "A" && i.LastPurchDateDisplay != null && i.LastPurchDateDisplay != "")
                .OrderByDescending(i => i.LastPurchDate)
                .ThenBy(i => i.Description));
        }

        public async Task<int> SaveReorderItem(ReorderItem ri)
        {
            if (ri == null) return 0;
            var reorderSnapshot = CopyRealmObject(ri)!;
            var realm = GetRealm();
            realm.Write(() => realm.Add(reorderSnapshot, update: true));
            return 1;
        }

        public async Task<int> GetReorderItemsCount()
        {
            return GetRealm().All<Item>()
                .Where(i => i.LastPurchDateDisplay != null && i.LastPurchDateDisplay != "")
                .Count();
        }

        public async Task<int> DeleteReorderItems()
        {
            var realm = GetRealm();
            int count = realm.All<ReorderItem>().Count();
            realm.Write(() => realm.RemoveAll<ReorderItem>());
            return count;
        }
        #endregion

        #region Session Cache Serialization
        public async Task<int> DeleteSavedCartItems()
        {
            var realm = GetRealm();
            int count = realm.All<CartItem>().Count();
            realm.Write(() => realm.RemoveAll<CartItem>());
            return count;
        }

        public async Task<int> SaveCartItems()
        {
            var realm = GetRealm();
            var sourceItems = realm.All<Item>()
                                   .Where(i => i.QtyOrder > 0 || i.QtyOnOrderSellUnit1 > 0 || i.QtyOnOrderSellUnit2 > 0 || i.QtyOnOrderSellUnit3 > 0 || i.QtyOnOrderSellUnit4 > 0)
                                   .ToList();

            realm.Write(() =>
            {
                foreach (var i in sourceItems)
                {
                    realm.Add(new CartItem
                    {
                        ItemNo = i.ItemNo,
                        QtyOrder = i.QtyOrder,
                        QtyCredit = i.QtyCredit,
                        QtyLabel = i.QtyLabel
                    });
                }
            });
            return sourceItems.Count;
        }

        public async Task<List<CartItem>> GetSavedCartItems() => CopyRealmObjects(GetRealm().All<CartItem>());

        public async Task<int> SuspendCartItems(string custNo)
        {
            var realm = GetRealm();
            var sourceItems = realm.All<Item>().Where(i => i.QtyOrder > 0 || i.QtyCredit > 0 || i.QtyLabel > 0).ToList();

            realm.Write(() =>
            {
                foreach (var i in sourceItems)
                {
                    realm.Add(new SuspendItem
                    {
                        CustNo = custNo,
                        ItemNo = i.ItemNo,
                        QtyOrder = i.QtyOrder,
                        QtyCredit = i.QtyCredit,
                        QtyLabel = i.QtyLabel,
                        ServerURL = App.g_ServerURL,
                        LineNo = i.LineNo
                    });
                }
            });
            return sourceItems.Count;
        }

        public async Task<List<SuspendItem>> GetSuspendedCartItems(string custNo)
        {
            var realm = GetRealm();
            string serverUrl = App.g_ServerURL;
            return CopyRealmObjects(realm.All<SuspendItem>()
                .Where(s => s.CustNo == custNo && s.ServerURL == serverUrl));
        }

        public async Task<int> RestoreCartItems(string custNo)
        {
            var items = await GetSuspendedCartItems(custNo);
            foreach (var item in items)
            {
                if (item.QtyOrder > 0)
                {
                    await UpdateItemQtySet(item.ItemNo, item.QtyOrder, item.QtyCredit, item.QtyLabel, item.LineNo);
                }
            }
            await DeleteSuspendedCartItems(custNo);
            return 0;
        }

        public async Task<int> DeleteSuspendedCartItems(string custNo)
        {
            var realm = GetRealm();
            string serverUrl = App.g_ServerURL;
            var targets = realm.All<SuspendItem>().Where(s => s.CustNo == custNo && s.ServerURL == serverUrl).ToList();
            realm.Write(() => { foreach (var t in targets) realm.Remove(t); });
            return targets.Count;
        }
        #endregion

        #region Digital Flyer Management
        public async Task<int> ClearFlyerItems()
        {
            var realm = GetRealm();
            var items = realm.All<Item>().ToList();
            realm.Write(() =>
            {
                foreach (var i in items)
                {
                    i.FlyerPageNo = 0; i.FlyerBoxNo = 0; i.FlyerSection = ""; i.FlyerStartDate = 0;
                    i.FlyerEndDate = 0; i.FlyerTopLeftX = 0; i.FlyerTopLeftY = 0; i.FlyerBottomRightX = 0; i.FlyerBottomRightY = 0;
                }
            });
            return items.Count;
        }

        public async Task<int> UpdateItemFlyerInfo(FlyerItem item)
        {
            if (item == null) return 0;
            var realm = GetRealm();
            var match = realm.Find<Item>(item.ItemNo);
            if (match == null) return 0;

            realm.Write(() =>
            {
                match.FlyerPageNo = item.Page;
                match.FlyerBoxNo = item.Box;
                match.FlyerSection = item.Section;
                match.FlyerStartDate = item.StartDate;
                match.FlyerEndDate = item.EndDate;
                match.FlyerTopLeftX = item.TopLeftX;
                match.FlyerTopLeftY = item.TopLeftY;
                match.FlyerBottomRightX = item.BottomRightX;
                match.FlyerBottomRightY = item.BottomRightY;
            });
            return 1;
        }

        public async Task<int> GetFlyerItemCount()
        {
            if (!int.TryParse(DateTimeOffset.Now.ToString("1yyMMdd"), out int dateStamp)) return 0;
            return GetRealm().All<Item>().Where(i => i.FlyerStartDate <= dateStamp && i.FlyerEndDate >= dateStamp).Count();
        }

        public async Task<List<Item>> SearchItemsMonthlyAdClick(int iPage, int iX, int iY, int offset = 0, int pageSize = int.MaxValue)
        {
            offset = Math.Max(0, offset);
            pageSize = Math.Max(1, pageSize);
            try
            {
                var realm = GetRealm();
                return CopyRealmObjects(realm.All<Item>()
                    .Where(i => i.FlyerPageNo == iPage && i.Status != "D")
                    .ToList()
                    .Where(i => iX >= i.FlyerTopLeftX && iX <= i.FlyerBottomRightX && iY >= i.FlyerTopLeftY && iY <= i.FlyerBottomRightY)
                    .OrderBy(i => i.Description)
                    .Skip(offset)
                    .Take(pageSize));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SearchItemsMonthlyAdClick " + ex.Message);
                return new List<Item>();
            }
        }
        #endregion

        #region Purge Control Matrix
        public async Task<int> DeleteAll()
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<Item>();
                realm.RemoveAll<Customer>();
                realm.RemoveAll<Banner>();
                realm.RemoveAll<Category>();
                realm.RemoveAll<Subcategory>();
                realm.RemoveAll<Subsubcategory>();

                var settingsToPurge = realm.All<Setting>().Where(s => s.Key != "ServerURL").ToList();
                foreach (var s in settingsToPurge) realm.Remove(s);

                realm.RemoveAll<PaymentToken>();
                realm.RemoveAll<Location>();
                realm.RemoveAll<OrderHeader>();
                realm.RemoveAll<OrderDetail>();
                realm.RemoveAll<ReorderItem>();
                realm.RemoveAll<CartItem>();
                realm.RemoveAll<DiscontinuedItem>();
                realm.RemoveAll<SalesCustomer>();
                realm.RemoveAll<FlyerItem>();
            });
            return 0;
        }
        #endregion
    }
}