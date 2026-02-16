using System.Diagnostics;

namespace TPSMobileApp
{
    internal class XMLResponseParser
    {
        public static async void commService_GetBannersCompleted(String response)
        {
            try
            {
                Debug.WriteLine("Get Banners returned");

                String sBanners = response;
                String[] aBanners = sBanners.Split('|');
                if (aBanners.Length >= 1)
                {
                    //Database db = new Database();

                    App.g_db.BeginTransaction();
                    App.g_db.DeleteBannersAsync();

                    foreach (String s in aBanners)
                    {
                        Banner banner = new Banner();
                        banner.BannerName = s;
                        banner.BannerURL = Constants.BannerUrl + banner.BannerName;

                        try
                        {
                            App.g_db.SaveBannerAsync(banner);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.CommitTransaction();
                }

                App.CommManager.GetCategoriesAndSubcategoriesCust(App.g_Customer.CustNo);
            }
            catch (Exception ex)
            {
            }
        }


        public static async void commService_GetCategoriesAndSubcategoriesCompleted(String response)
        {
            Debug.WriteLine("Get Categories and Subcategories returned");

            try
            {
                String sCategories = response;
                String[] aCategories = sCategories.Split('~');
                if (aCategories.Length > 1)
                {
                    //Database db = new Database();

                    App.g_db.BeginTransaction();

                    App.g_db.DeleteCategories();
                    App.g_db.DeleteSubcategories();

                    foreach (String s in aCategories)
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            continue;
                        }

                        if (aCategory[1].Length == 0)
                        {
                            Category cat = new Category();
                            cat.Code = aCategory[0];
                            cat.Description = aCategory[2].Trim();
                            cat.ImageURL = Constants.CategoryImageUrl + cat.Code + ".png";
                            cat.Rank = Convert.ToInt32(aCategory[3].Trim());
                            cat.HomePage = Convert.ToInt32(aCategory[4].Trim());

                            try
                            {
                                //App.g_db.SaveCategory(cat);
                            }
                            catch (Exception ex)
                            {
                                String sMsg = ex.Message;
                            }
                        }
                        else
                        {
                            Subcategory subcat = new Subcategory();
                            subcat.Category = aCategory[0];
                            subcat.Code = aCategory[1];
                            subcat.Description = aCategory[2].Trim();
                            subcat.Rank = Convert.ToInt32(aCategory[3].Trim());

                            try
                            {
                                //App.g_db.SaveSubcategory(subcat);
                            }
                            catch (Exception ex)
                            {
                                String sMsg = ex.Message;
                            }
                        }
                    }

                    App.g_db.CommitTransaction();

                    //App.g_CategoryList = App.g_db.GetCategories();
                    App.g_HomePageCategoryList = App.g_db.GetHomePageCategories();
                }

                try
                {
                    App.g_HomePage.LoadCategories();
                }
                catch (Exception ex)
                {
                }

                try
                {
                    String CustNo = "0";
                    try
                    {
                        CustNo = App.g_Customer.CustNo;
                    }
                    catch
                    {
                        CustNo = "0";
                    }

                    //Database db = new Database();
                    string sDate = App.g_db.GetSetting("LastUpdateItems");

                    if (sDate == "")
                    {
                        sDate = "0";
                    }

                    // for now always refresh all items
                    sDate = "0";
                    if (App.g_Customer.CustNo == "0")
                    {
                        App.CommManager.GetItems("0", sDate);
                    }
                    else
                    {
                        App.CommManager.GetItems(App.g_Customer.CustNo, sDate);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async void commService_GetCategoriesAndSubcategoriesCustCompleted(String response)
        {
            Debug.WriteLine("Get Categories and Subcategories Cust returned");

            try
            {
                List<Category> categories = new List<Category>();
                List<Subcategory> subcategories = new List<Subcategory>();
                List<Subsubcategory> subsubcategories = new List<Subsubcategory>();

                String sCategories = response;
                String[] aCategories = sCategories.Split('~');

                if (aCategories.Length > 1)
                {
                    foreach (String s in aCategories)
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            continue;
                        }

                        string sSubsubcategory;
                        try
                        {
                            sSubsubcategory = aCategory[5];
                        }
                        catch
                        {
                            sSubsubcategory = "";
                        }

                        if (aCategory[1].Length == 0)  // no subcategory, just add category
                        {
                            Category cat = new Category();
                            cat.Code = aCategory[0];
                            cat.Description = aCategory[2].Trim();
                            cat.ImageURL = Constants.CategoryImageUrl + cat.Code + ".png";
                            cat.Rank = Convert.ToInt32(aCategory[3].Trim());
                            cat.HomePage = Convert.ToInt32(aCategory[4].Trim());
                            categories.Add(cat);
                        }
                        else if (sSubsubcategory.Length == 0)  // no subsubcat, just add subcategory
                        {
                            Subcategory subcat = new Subcategory();
                            subcat.Category = aCategory[0];
                            subcat.Code = aCategory[1];
                            subcat.Description = aCategory[2].Trim();
                            subcat.Rank = Convert.ToInt32(aCategory[3].Trim());
                            subcategories.Add(subcat);
                        }
                        else // add subsubcategory
                        {
                            Subsubcategory subsubcat = new Subsubcategory();
                            subsubcat.Category = aCategory[0];
                            subsubcat.Subcategory = aCategory[1];
                            subsubcat.Code = sSubsubcategory;
                            subsubcat.Description = aCategory[2].Trim();
                            subsubcat.Rank = Convert.ToInt32(aCategory[3].Trim());
                            subsubcategories.Add(subsubcat);
                        }
                    }
                }
                try
                {

                }
                catch (Exception ex)
                {
                }

                try
                {
                    App.g_db.BeginTransaction();
                    App.g_db.DeleteCategories();
                    App.g_db.DeleteSubcategories();
                    App.g_db.DeleteSubsubcategories();
                    App.g_db.SaveCategory(categories);
                    App.g_db.SaveSubcategory(subcategories);
                    App.g_db.SaveSubsubcategory(subsubcategories);

                    String CustNo = "0";
                    try
                    {
                        CustNo = App.g_Customer.CustNo;
                    }
                    catch
                    {
                        CustNo = "0";
                    }

                    //Database db = new Database();
                    string sDate = App.g_db.GetSetting("LastUpdateItems");

                    if (sDate == "")
                    {
                        sDate = "0";
                    }

                    // for now always refresh all items
                    sDate = "0";
                    if (App.g_Customer.CustNo == "0")
                    {
                        App.CommManager.GetItems("0", sDate);
                    }
                    else
                    {
                        App.CommManager.GetItems(App.g_Customer.CustNo, sDate);
                    }
                    App.g_HomePageCategoryList = App.g_db.GetHomePageCategories();
                    App.g_db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Fetch Items Categories and SubCategories" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SAVE Categories and SubCategories" + ex.Message);
            }
        }

        public static async void commService_GetItemsCompletedAsync(String response)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " - Get Items returned");

                String sItems = response;
                String[] aItems = sItems.Split('~');
                if (aItems.Length > 1)
                {
                    App.g_db.BeginTransaction();

                    App.g_db.InsertDiscontinuedItems();

                    List<Item> lstCartItems = App.g_db.GetCartItems();

                    foreach (String s in aItems)
                    {
                        String[] aItem = s.Split("|");

                        if (aItem.Count() < 20)
                        {
                            continue;
                        }

                        Item item = new Item();
                        try
                        {
                            item.ItemNo = Convert.ToInt32(aItem[0]);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                            continue;
                        }
                        item.ItemNoDisplay = aItem[0];
                        item.Description = aItem[1].Trim();
                        item.ImageURL = Constants.ItemImageUrl + item.ItemNo.ToString() + ".jpg";
                        item.CategoryCode = aItem[2].Trim();
                        item.CategoryDesc = aItem[3].Trim();
                        item.SubcategoryCode = aItem[4].Trim();
                        item.SubcategoryDesc = aItem[5].Trim();
                        item.VendorCode = aItem[6].Trim();
                        item.VendorName = aItem[7].Trim();
                        item.UPC_1 = aItem[8].Trim();
                        if (item.UPC_1.Length > 0)
                        {
                            item.ItemNoDisplayUPC = "(" + item.UPC_1 + ")";
                        }
                        else
                        {
                            item.ItemNoDisplayUPC = "";
                        }
                        item.UPC_2 = aItem[9].Trim();
                        item.UPC_3 = aItem[10].Trim();
                        item.UPC_4 = aItem[11].Trim();
                        item.RetailUOM = aItem[12].Trim();
                        item.RetailSize = aItem[13].Trim();
                        try
                        {
                            item.RetailPrice = Convert.ToDecimal(aItem[14].Trim());
                        }
                        catch
                        {
                            item.RetailPrice = 0;
                        }
                        item.RetailPriceDisplay = aItem[14].Trim();
                        item.UOM = aItem[15].Trim();
                        item.SizeUOM = "/" + item.UOM;
                        try
                        {
                            item.Size = Convert.ToInt32(aItem[16].Trim());
                        }
                        catch
                        {
                            item.Size = 1;
                        }
                        item.SizeDisplay = aItem[16].Trim();
                        item.Form = aItem[17].Trim();
                        try
                        {
                            item.Price = Convert.ToDecimal(aItem[18].Trim());
                        }
                        catch
                        {
                            item.Price = 0;
                        }
                        item.PriceDisplay = string.Format("{0:C}", item.Price);
                        try
                        {
                            item.Tax = Convert.ToDecimal(aItem[19].Trim());
                        }
                        catch
                        {
                            item.Tax = 0;
                        }
                        item.TaxDisplay = string.Format("{0:C}", item.Tax);
                        try
                        {
                            item.CategoryRank = Convert.ToInt32(aItem[20].Trim());
                        }
                        catch
                        {
                            item.CategoryRank = 0;
                        }
                        try
                        {
                            item.SellUnitsInPurchaseUnit = Convert.ToInt32(aItem[21].Trim());
                        }
                        catch
                        {
                            item.SellUnitsInPurchaseUnit = 1;
                        }
                        item.Status = aItem[22];
                        try
                        {
                            item.QOH = Convert.ToInt32(aItem[23].Trim());
                        }
                        catch
                        {
                            item.QOH = 0;
                        }
                        try
                        {
                            item.IsNew = false;
                            if (aItem[24] == "Y")
                            {
                                item.IsNew = true;
                            }
                        }
                        catch
                        {
                            item.IsNew = false;
                        }
                        try
                        {
                            if ((aItem[25] == "0") || (aItem[25] == ""))
                            {
                                item.AddedDateDisplay = "N/A";
                            }
                            else
                            {
                                item.AddedDateDisplay = aItem[25].Substring(3, 2) + "/";
                                item.AddedDateDisplay += aItem[25].Substring(5, 2) + "/";
                                item.AddedDateDisplay += aItem[25].Substring(1, 2);
                            }
                        }
                        catch { }
                        try
                        {
                            item.AllocationQty = Convert.ToInt32(aItem[26].Trim());
                        }
                        catch
                        {
                            item.AllocationQty = 0;
                        }
                        try
                        {
                            if (aItem[27] == "1")
                            {
                                item.IsPriceVisible = 0;
                            }
                            else
                            {
                                item.IsPriceVisible = 1;
                            }
                        }
                        catch
                        {
                            item.IsPriceVisible = 1;
                        }

                        try
                        {
                            item.Keyword1 = aItem[28];
                            item.Keyword2 = aItem[29];
                            item.Keyword3 = aItem[30];
                        }
                        catch
                        {
                            item.Keyword1 = "";
                            item.Keyword2 = "";
                            item.Keyword3 = "";
                        }

                        try
                        {
                            item.LastPurchDateDisplay = aItem[31];
                        }
                        catch
                        {
                            item.LastPurchDateDisplay = "";
                        }
                        if (item.LastPurchDateDisplay == "")
                        {
                            try
                            {
                                item.LastPurchDate = Convert.ToDateTime(item.LastPurchDateDisplay);
                            }
                            catch
                            {
                            }
                        }
                        try
                        {
                            if (aItem[32] == "")
                            {
                                item.QtyLastOrder = 0;
                            }
                            else
                            {
                                item.QtyLastOrder = Convert.ToInt32(aItem[32]);
                            }
                        }
                        catch
                        {
                            item.QtyLastOrder = 0;
                        }
                        try
                        {
                            item.SubsubcategoryCode = aItem[33];
                        }
                        catch
                        {
                            item.SubsubcategoryCode = "";
                        }
                        try
                        {
                            item.SubsubcategoryDesc = aItem[34];
                        }
                        catch
                        {
                            item.SubsubcategoryDesc = "";
                        }

                        item.AddToOrderDisplay = "Add To Order";
                        item.QtyOrder = 0;
                        item.QtyCredit = 0;
                        item.QtyLabel = 0;
                        item.LineNo = 0;

                        foreach (Item ci in lstCartItems)
                        {
                            if (item.ItemNo == ci.ItemNo)
                            {
                                item.QtyOrder = ci.QtyOrder;
                                item.QtyCredit = ci.QtyCredit;
                                item.QtyLabel = ci.QtyLabel;
                                item.LineNo = ci.LineNo;

                                break;
                            }
                        }

                        try
                        {
                            App.g_db.SaveItem(item);
                            App.g_db.DeleteDiscontinuedItem(item.ItemNo.ToString());
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.UpdateDiscontinuedItems();

                    App.g_db.UpdateOrderDetailLastPurch();

                    App.g_db.SaveSetting("LastUpdateItems", DateTime.Now.ToString("1yyMMdd"));

                    App.g_ItemList = App.g_db.GetItems();

                    App.g_db.CommitTransaction();

                    App.CommManager.GetItemQOH(App.g_Customer.CustNo);
                    App.CommManager.GetOrderHistory(App.g_Customer.CustNo);
                    App.CommManager.GetFlyerItemsPDF();
                }
            }
            catch (Exception ex)
            {
                String sMsg = ex.Message + ex.StackTrace;
            }
        }


        public static async void commService_GetItemQOHCompletedAsync(String response)
        {
            try
            {
                //if (response == "X")
                //{
                //    App.g_Shell.Logout();
                //    return;
                //}

                String sItems = response;
                String[] aItems = sItems.Split('~');
                int iItemNo;
                int iQOH;

                if (aItems.Length > 1)
                {
                    App.g_db.BeginTransaction();

                    foreach (String s in aItems)
                    {
                        String[] aItem = s.Split("|");

                        if (aItem.Count() < 2)
                        {
                            continue;
                        }

                        try
                        {
                            iItemNo = Convert.ToInt32(aItem[0]);
                            iQOH = Convert.ToInt32(aItem[1]);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        try
                        {
                            App.g_db.UpdateItemQOH(iItemNo, iQOH);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                String sMsg = ex.Message + ex.StackTrace;
            }
        }

        public static async void commService_GetItemQOH2CompletedAsync(String response)
        {
            Debug.WriteLine("Get Item QOH 2 returned");

            try
            {
                if (response == "X")
                {
                    App.g_Shell.Logout();
                    return;
                }

                String sItems = response;
                String[] aItems = sItems.Split('~');
                int iItemNo;
                int iQOH;

                if (aItems.Length > 1)
                {
                    App.g_db.BeginTransaction();

                    foreach (String s in aItems)
                    {
                        String[] aItem = s.Split("|");

                        if (aItem.Count() < 2)
                        {
                            continue;
                        }

                        try
                        {
                            iItemNo = Convert.ToInt32(aItem[0]);
                            iQOH = Convert.ToInt32(aItem[1]);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        try
                        {
                            App.g_db.UpdateItemQOH(iItemNo, iQOH);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                String sMsg = ex.Message + ex.StackTrace;
            }
        }

        public static async void commService_ValidateLoginCompletedAsync(String response)
        {
            Debug.WriteLine("ValidateLogin Complete");
            try
            {
                String sUser = response;

                //Database db = new Database();
                //await db.SaveCustomerAsync(App.g_Customer);

                String[] aInfo = sUser.Split("~");
                String[] aUser = aInfo[0].Split("|");
                String[] aCust = aInfo[1].Split("|");
                String OldCustNo = "0";
                String userValue = aUser[0];
                try
                {
                    if (userValue.Equals("V"))
                    {
                        try
                        {
                            if (aUser[2] == "1")
                            {
                                App.g_IsCredits = true;
                            }
                            else
                            {
                                App.g_IsCredits = false;
                            }
                            App.g_db.SaveSetting("Credits", aUser[2]);

                            if (aUser[3] == "1")
                            {
                                App.g_HoldForReview = true;
                            }
                            else
                            {
                                App.g_HoldForReview = false;
                            }
                            App.g_db.SaveSetting("HoldForReview", aUser[3]);

                            try
                            {
                                if (aUser[4] == "1")
                                {
                                    App.g_ForceSubmit = true;
                                }
                                else
                                {
                                    App.g_ForceSubmit = false;
                                }
                                App.g_db.SaveSetting("ForceSubmit", aUser[4]);
                            }
                            catch
                            {
                                App.g_ForceSubmit = false;
                                App.g_db.SaveSetting("ForceSubmit", "0");
                            }

                            try
                            {
                                App.g_QOHDisplay = aUser[5];
                            }
                            catch
                            {
                                App.g_QOHDisplay = "X";
                            }
                            App.g_db.SaveSetting("QOHDisplay", App.g_QOHDisplay);

                            try
                            {
                                if (aUser[6] == "1")
                                {
                                    App.g_BlockItemsNoQOH = true;
                                }
                                else
                                {
                                    App.g_BlockItemsNoQOH = false;
                                }
                                App.g_db.SaveSetting("BlockItemsNoQOH", aUser[6]);
                            }
                            catch
                            {
                                App.g_BlockItemsNoQOH = false;
                                App.g_db.SaveSetting("BlockItemsNoQOH", "0");
                            }

                            try
                            {
                                if (aUser[7] == "1")
                                {
                                    App.g_IsScandit = true;
                                }
                                else
                                {
                                    App.g_IsScandit = false;
                                }
                                App.g_db.SaveSetting("IsScandit", aUser[7]);
                            }
                            catch
                            {
                                App.g_IsScandit = false;
                                App.g_db.SaveSetting("IsScandit", "0");
                            }
                            try
                            {
                                if (aUser[8] == "1")
                                {
                                    App.g_IsSalesUser = true;
                                }
                                else
                                {
                                    App.g_IsSalesUser = false;
                                }
                                App.g_db.SaveSetting("IsSalesUser", aUser[8]);
                            }
                            catch
                            {
                                App.g_IsSalesUser = false;
                                App.g_db.SaveSetting("IsSalesUser", "0");
                            }
                            try
                            {
                                if (aUser[9] == "1")
                                {
                                    App.g_IsMonthlyFlyer = true;
                                }
                                else
                                {
                                    App.g_IsMonthlyFlyer = false;
                                }
                                App.g_db.SaveSetting("MonthlyFlyer", aUser[9]);
                            }
                            catch
                            {
                                App.g_IsMonthlyFlyer = false;
                                App.g_db.SaveSetting("MonthlyFlyer", "0");
                            }
                            int iFlyerStartDate = 0;
                            try
                            {
                                string sFlyerStartDate = aUser[10];
                                int.TryParse(sFlyerStartDate, out iFlyerStartDate);
                            }
                            catch { }
                            App.g_db.SaveSetting("FlyerStartDate", iFlyerStartDate.ToString());
                            App.g_FlyerStartDate = iFlyerStartDate;
                            int iFlyerEndDate = 0;
                            try
                            {
                                string sFlyerEndDate = aUser[11];
                                int.TryParse(sFlyerEndDate, out iFlyerEndDate);
                            }
                            catch { }
                            App.g_db.SaveSetting("FlyerEndDate", iFlyerEndDate.ToString());
                            App.g_FlyerEndDate = iFlyerEndDate;
                            try
                            {
                                if (aUser[13] == "1")
                                {
                                    App.g_IsAutoAdd1 = true;
                                }
                                else
                                {
                                    App.g_IsAutoAdd1 = false;
                                }
                                App.g_db.SaveSetting("AutoAdd1", aUser[13]);
                            }
                            catch
                            {
                                App.g_IsAutoAdd1 = false;
                                App.g_db.SaveSetting("AutoAdd1", "0");
                            }
                            try
                            {
                                if (aUser[14] == "1")
                                {
                                    App.g_IsRefNoLookup = true;
                                }
                                else
                                {
                                    App.g_IsRefNoLookup = false;
                                }
                                App.g_db.SaveSetting("RefNoLookup", aUser[14]);
                            }
                            catch
                            {
                                App.g_IsRefNoLookup = false;
                                App.g_db.SaveSetting("RefNoLookup", "0");
                            }
                            try
                            {
                                App.g_ShoppingCartSort = aUser[15];
                                App.g_db.SaveSetting("ShoppingCartSort", aUser[15]);
                            }
                            catch
                            {
                                App.g_ShoppingCartSort = "A";
                                App.g_db.SaveSetting("ShoppingCartSort", "A");
                            }
                            try
                            {
                                if (aUser[16] == "1")
                                {
                                    App.g_IsChainManager = true;
                                }
                                else
                                {
                                    App.g_IsChainManager = false;
                                }
                                App.g_db.SaveSetting("IsChainManager", aUser[16]);
                            }
                            catch
                            {
                                App.g_IsChainManager = false;
                                App.g_db.SaveSetting("IsChainManager", "0");
                            }

                            if (!App.g_IsSalesUser)
                            {
                                App.g_Customer.Status = "9";
                                App.g_Customer.CompanyName = aCust[1];
                                App.g_Customer.Warehouse = Convert.ToInt32(aCust[3]);
                                App.g_Customer.Address1 = aCust[4];
                                App.g_Customer.City = aCust[5];
                                App.g_Customer.State = aCust[6];
                                App.g_Customer.Zip = aCust[7];
                                App.g_Customer.CityStateZip = aCust[5] + ", " + aCust[6] + "  " + aCust[7];
                                App.g_Customer.Phone = aCust[8];
                                App.g_Customer.Contact = aCust[9];
                                App.g_Customer.Delivery = Convert.ToInt32(aCust[10]);
                                App.g_Customer.Pickup = Convert.ToInt32(aCust[11]);
                                App.g_Customer.CreditLimit = Convert.ToDecimal(aCust[12]);
                                App.g_Customer.ARBalance = Convert.ToDecimal(aCust[13]);

                                App.g_Customer.MinOrderAmount = Convert.ToDecimal(aCust[20]);
                                App.g_Customer.ShippingFee = Convert.ToDecimal(aCust[21]);
                                App.g_Customer.MinOrderQty = Convert.ToDecimal(aCust[22]);


                                Location loc = new Location();
                                loc.LocationId = 1;
                                loc.Name = aCust[14];
                                loc.Address = aCust[15];
                                loc.City = aCust[16];
                                loc.State = aCust[17];
                                loc.Zip = aCust[18];
                                loc.CityStateZip = loc.City + ", " + loc.State + " " + loc.Zip;
                                loc.Phone = aCust[19];

                                OldCustNo = App.g_Customer.CustNo;
                                App.g_Customer.CustNo = aUser[1];
                                //Database db = new Database();
                                App.g_db.SaveCustomer(App.g_Customer);
                                App.g_db.SaveLocation(loc);

                                App.g_db.RestoreCartItems(App.g_Customer.CustNo);
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        if ((App.g_IsSalesUser) || (App.g_IsChainManager))
                        {
                            App.CommManager.GetSalespersonCustomers(App.g_UserName);
                        }

                        if (App.g_Customer.CustNo != OldCustNo)
                        {
                            //App.g_db.ClearCartItems();
                            //App.g_db.DeleteOrderHistory();
                            //App.g_db.DeleteReorderItems();
                            //await App.CommManager.GetOrderHistory(App.g_Customer.CustNo);

                            if (App.g_UserName.ToLower() == "app_test")
                            {
                                App.g_db.DeleteCategories();
                                App.g_db.DeleteItems();
                            }
                        }


                        App.CommManager?.GetOrderHistory(App.g_Customer.CustNo);

                        App.RefreshAll();

                        App.g_db.SaveSetting("LoggedIn", "1");
                        App.g_db.SaveSetting("UserName", App.g_UserName);
                        App.g_IsLoggedIn = true;
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                _ = App.g_Shell.GoToHome();
                            });
                        }
                        catch
                        {
                        }
                    }
                    else if (userValue == "P")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Profit Order", "Invalid password.  Please try again.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                            return;
                        }
                        catch
                        {
                        }
                    }
                    else if (userValue == "I")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Profit Order", "Inactive account.  Please contact Customer Service.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                            return;
                        }
                        catch
                        {
                        }
                    }
                    else if (userValue == "U")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Profit Order", "Account does not exist.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                            return;
                        }
                        catch
                        {
                        }
                    }
                    else if (userValue == "X")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Profit Order", "Error attempting to login.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                            return;
                        }
                        catch
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Profit Order", "Error attempting to login.", "Ok");
                            App.g_LoginPage.HideAnimation();
                        });
                        return;
                    }
                    catch
                    {
                    }
                }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await AppShell.Current.Navigation.PopAsync(true);
                });
            }
            catch (Exception ex)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlertAsync("Profit Order", "Error attempting to login.", "Ok");
                        App.g_LoginPage.HideAnimation();
                    });
                }
                catch
                {
                }
            }
        }

        public static async void commService_GetSettingsCompletedAsync(String response)
        {
            Debug.WriteLine("GetSettings Complete");

            try
            {
                String sSettings = response;

                String[] aSettings = sSettings.Split("|");
                if (aSettings[0] == "1")
                {
                    App.g_HoldForReview = true;
                }
                else
                {
                    App.g_HoldForReview = false;
                }
                App.g_db.SaveSetting("HoldForReview", aSettings[0]);

                try
                {
                    if (aSettings[1] == "1")
                    {
                        App.g_ForceSubmit = true;
                    }
                    else
                    {
                        App.g_ForceSubmit = false;
                    }
                    App.g_db.SaveSetting("ForceSubmit", aSettings[1]);
                }
                catch
                {
                    App.g_ForceSubmit = false;
                    App.g_db.SaveSetting("ForceSubmit", "0");
                }

                try
                {
                    App.g_QOHDisplay = aSettings[2];
                }
                catch
                {
                    App.g_QOHDisplay = "X";
                }
                App.g_db.SaveSetting("QOHDisplay", App.g_QOHDisplay);

                try
                {
                    if (aSettings[3] == "1")
                    {
                        App.g_BlockItemsNoQOH = true;
                    }
                    else
                    {
                        App.g_BlockItemsNoQOH = false;
                    }
                    App.g_db.SaveSetting("BlockItemsNoQOH", aSettings[3]);
                }
                catch
                {
                    App.g_BlockItemsNoQOH = false;
                    App.g_db.SaveSetting("BlockItemsNoQOH", "0");
                }

                try
                {
                    if (aSettings[4] == "1")
                    {
                        App.g_IsMonthlyFlyer = true;
                    }
                    else
                    {
                        App.g_IsMonthlyFlyer = false;
                    }
                    App.g_db.SaveSetting("MonthlyFlyer", aSettings[4]);
                }
                catch
                {
                    App.g_IsMonthlyFlyer = false;
                    App.g_db.SaveSetting("MonthlyFlyer", "0");
                }

                int iFlyerStartDate = 0;
                try
                {
                    string sFlyerStartDate = aSettings[5];
                    int.TryParse(sFlyerStartDate, out iFlyerStartDate);
                }
                catch { }
                App.g_db.SaveSetting("FlyerStartDate", iFlyerStartDate.ToString());
                App.g_FlyerStartDate = iFlyerStartDate;

                int iFlyerEndDate = 0;
                try
                {
                    string sFlyerEndDate = aSettings[6];
                    int.TryParse(sFlyerEndDate, out iFlyerEndDate);
                }
                catch { }
                App.g_db.SaveSetting("FlyerEndDate", iFlyerEndDate.ToString());
                App.g_FlyerEndDate = iFlyerEndDate;

                try
                {
                    if (aSettings[8] == "1")
                    {
                        App.g_IsAutoAdd1 = true;
                    }
                    else
                    {
                        App.g_IsAutoAdd1 = false;
                    }
                    App.g_db.SaveSetting("AutoAdd1", aSettings[8]);
                }
                catch
                {
                    App.g_IsAutoAdd1 = false;
                    App.g_db.SaveSetting("AutoAdd1", "0");
                }

                try
                {
                    if (aSettings[9] == "1")
                    {
                        App.g_IsRefNoLookup = true;
                    }
                    else
                    {
                        App.g_IsRefNoLookup = false;
                    }
                    App.g_db.SaveSetting("RefNoLookup", aSettings[9]);
                }
                catch
                {
                    App.g_IsRefNoLookup = false;
                    App.g_db.SaveSetting("RefNoLookup", "0");
                }

                try
                {
                    App.g_ShoppingCartSort = aSettings[10];
                    App.g_db.SaveSetting("ShoppingCartSort", aSettings[10]);
                }
                catch
                {
                    App.g_ShoppingCartSort = "A";
                    App.g_db.SaveSetting("ShoppingCartSort", "A");
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async void commService_SubmitOrderCompletedAsync(String response)
        {
            try
            {
                if (response == "S")
                {
                    App.g_db.ClearOrderCartItems();
                    App.g_Notes = "";

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlertAsync("Profit Order", "Thank you! Your order has been placed.", "OK");
                    });


                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch
                    {
                    }
                }
                else if (response == "X")
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Profit Order", "Account disabled.  Please contact customer support.", "Ok");
                            await App.g_Shell.GoToHome();
                            App.g_Shell.Logout();
                        });
                    }
                    catch
                    {
                    }
                }
                else if (response == "Z")
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlertAsync("Profit Order", "Order has already been submitted.", "Ok");
                    });

                    App.g_db.ClearOrderCartItems();
                    App.g_Notes = "";

                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Profit Order", "Error submitting order.  Please try again.", "Ok");
                        });
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async void commService_SubmitReturnCompletedAsync(String response)
        {
            try
            {

                if (response == "S")
                {
                    App.g_db.ClearReturnCartItems();

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlertAsync("Profit Order", "Thank you! Your return request has been submitted.", "OK");
                    });


                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch
                    {
                    }
                }
                else if (response == "X")
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Profit Order", "Account disabled.  Please contact customer support.", "Ok");
                            await App.g_Shell.GoToHome();
                            App.g_Shell.Logout();
                        });
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Profit Order", "Error submitting return request.  Please try again.", "Ok");
                        });
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async void commService_GetOrderHistoryCompletedAsyncOld(String response)
        {
            try
            {
                Debug.WriteLine("Get Order History returned");

                String sOrders = response;
                String[] aOrders = sOrders.Split('~');
                List<String> lstHeader = new List<String>();

                if (aOrders.Length > 1)
                {
                    //Database db = new Database();

                    App.g_db.BeginTransaction();

                    foreach (String s in aOrders)
                    {
                        String[] aOrder = s.Split("|");
                        if (aOrder.Count() < 2)
                        {
                            continue;
                        }

                        bool bDeleteDetail = true;
                        foreach (String sOrder in lstHeader)
                        {
                            if (sOrder == aOrder[0])
                            {
                                bDeleteDetail = false;
                                break;
                            }
                        }
                        if (bDeleteDetail)
                        {
                            App.g_db.DeleteOrderDetail(aOrder[0]);
                            lstHeader.Add(aOrder[0]);
                        }

                        OrderHeader oh = new OrderHeader();
                        oh.OrderNo = aOrder[0];
                        oh.CustId = Convert.ToInt32(aOrder[1]);
                        oh.OrderDate = Convert.ToDateTime(aOrder[2]);
                        oh.OrderDateDisplay = aOrder[2];
                        oh.Total = Convert.ToDecimal(aOrder[3]);
                        oh.TotalDisplay = string.Format("{0:C}", oh.Total);
                        oh.Items = Convert.ToInt32(aOrder[4]);
                        oh.Pieces = Convert.ToInt32(aOrder[5]);

                        OrderDetail od = new OrderDetail();
                        od.OrderNo = aOrder[0];
                        od.LineNo = Convert.ToInt32(aOrder[6]);
                        od.ItemNo = Convert.ToInt32(aOrder[7]);
                        od.ItemNoDisplay = aOrder[7];
                        od.QtyOrdered = Convert.ToInt32(aOrder[8]);
                        od.QtyShipped = Convert.ToInt32(aOrder[8]);
                        od.Price = Convert.ToDecimal(aOrder[9]);
                        od.PriceDisplay = string.Format("{0:C}", od.Price);
                        od.UPC = aOrder[10];
                        if (od.UPC.Length > 0)
                        {
                            od.ItemNoDisplayUPC = "(" + od.UPC + ")";
                        }
                        else
                        {
                            od.ItemNoDisplayUPC = "";
                        }
                        od.Description = aOrder[11];
                        od.UOM = aOrder[12];
                        od.SellUnitsInPurch = aOrder[13];
                        od.SizeDisplay = od.UOM + "/" + od.SellUnitsInPurch;
                        od.SizeUOM = "/" + od.UOM;
                        od.Size = aOrder[14];
                        od.Form = aOrder[15];
                        od.CategoryCode = aOrder[16];
                        od.CategoryDesc = aOrder[17];
                        od.SubcategoryCode = aOrder[18];
                        od.SubcategoryDesc = aOrder[19];
                        od.VendorId = aOrder[20];
                        od.VendorName = aOrder[21];
                        od.Status = aOrder[22];
                        if (od.Status == "A")
                        {
                            od.IsAvailable = true;
                        }
                        else
                        {
                            od.IsAvailable = false;
                        }
                        try
                        {
                            od.QOH = Convert.ToInt32(aOrder[23].Trim());
                        }
                        catch
                        {
                            od.QOH = 0;
                        }
                        if (od.QOH == 0)
                        {
                            od.IsAvailable = false;
                        }
                        od.ImageURL = Constants.ItemImageUrl + od.ItemNo.ToString() + ".jpg";
                        od.LastPurchDate = Convert.ToDateTime(aOrder[2]);
                        od.LastPurchDateDisplay = aOrder[2];
                        od.QtyLastOrder = Convert.ToInt32(aOrder[8]);
                        od.QtyOrderDisplay = aOrder[8];
                        try
                        {
                            od.QtyLast90 = Convert.ToInt32(aOrder[24].Trim());
                            od.QtyLast90Display = aOrder[24];
                        }
                        catch
                        {
                            od.QtyLast90 = 0;
                            od.QtyLast90Display = "N/A";
                        }

                        ReorderItem ri = new ReorderItem();
                        ri.ItemNo = Convert.ToInt32(aOrder[7]);
                        ri.ItemNoDisplay = aOrder[7];
                        ri.LastPurchDate = Convert.ToDateTime(aOrder[2]);
                        ri.LastPurchDateDisplay = aOrder[2];
                        ri.QtyLastOrder = Convert.ToInt32(aOrder[8]);
                        ri.QtyOrderDisplay = aOrder[8];
                        ri.Description = aOrder[11];
                        ri.Price = Convert.ToDecimal(aOrder[9]);
                        ri.PriceDisplay = string.Format("{0:C}", ri.Price);
                        ri.ImageURL = Constants.ItemImageUrl + ri.ItemNo.ToString() + ".jpg";
                        ri.UPC = aOrder[10];
                        if (ri.UPC.Length > 0)
                        {
                            ri.ItemNoDisplayUPC = "(" + ri.UPC + ")";
                        }
                        else
                        {
                            ri.ItemNoDisplayUPC = "";
                        }
                        ri.UOM = aOrder[12];
                        ri.SellUnitsInPurch = aOrder[13];
                        ri.SizeDisplay = ri.UOM + "/" + ri.SellUnitsInPurch;
                        ri.SizeUOM = "/" + ri.UOM;
                        ri.Size = aOrder[14];
                        ri.Form = aOrder[15];
                        ri.CategoryCode = aOrder[16];
                        ri.CategoryDesc = aOrder[17];
                        ri.SubcategoryCode = aOrder[18];
                        ri.SubcategoryDesc = aOrder[19];
                        ri.VendorId = aOrder[20];
                        ri.VendorName = aOrder[21];
                        ri.Status = aOrder[22];
                        try
                        {
                            ri.QOH = Convert.ToInt32(aOrder[23].Trim());
                        }
                        catch
                        {
                            ri.QOH = 0;
                        }
                        try
                        {
                            ri.QtyLast90 = Convert.ToInt32(aOrder[24].Trim());
                            ri.QtyLast90Display = aOrder[24];
                        }
                        catch
                        {
                            ri.QtyLast90 = 0;
                            ri.QtyLast90Display = "N/A";
                        }
                        ri.ImageURL = Constants.ItemImageUrl + ri.ItemNo.ToString() + ".jpg";

                        try
                        {
                            App.g_db.SaveOrderHeader(oh);
                            App.g_db.SaveOrderDetail(od);
                            App.g_db.SaveReorderItem(ri);
                            Item item = App.g_db.FindItem(ri.ItemNo, ri.ItemNo.ToString());
                            if (item != null)
                            {
                                item.LastPurchDate = ri.LastPurchDate;
                                item.LastPurchDateDisplay = ri.LastPurchDateDisplay;
                                item.QtyLastOrder = ri.QtyLastOrder;
                                item.QtyOrderDisplay = ri.QtyOrderDisplay;
                                item.QtyLast90 = ri.QtyLast90;
                                item.QtyLast90Display = ri.QtyLast90Display;
                                App.g_db.UpdateItem(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_ReorderItemList = App.g_db.GetReorderItems();

                    App.g_db.CommitTransaction();

                }

                Debug.WriteLine("Get Order History Updated");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get Order History Error");
                Debug.WriteLine(ex.Message);
            }
        }

        public static async void commService_GetOrderHistoryCompletedAsync(String response)
        {
            Debug.WriteLine("Get Order History Complete");

            try
            {
                String sOrders = response;
                String[] aOrders = sOrders.Split('~');

                if (aOrders.Length > 1)
                {
                    //Database db = new Database();

                    List<OrderHeader> lstOrders = App.g_db.GetOrderHeaders();
                    List<String> lstOrderHeadersAdded = new List<String>();

                    App.g_db.BeginTransaction();

                    foreach (String s in aOrders)
                    {
                        String[] aOrder = s.Split("|");
                        if (aOrder.Count() < 2)
                        {
                            continue;
                        }

                        bool bFound = false;
                        foreach (OrderHeader h in lstOrders)
                        {
                            if (h.OrderNo == aOrder[0])
                            {
                                bFound = true;
                                break;
                            }
                        }
                        if (bFound)
                        {
                            continue;
                        }

                        bFound = false;
                        foreach (String sHeader in lstOrderHeadersAdded)
                        {
                            if (sHeader == aOrder[0])
                            {
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            lstOrderHeadersAdded.Add(aOrder[0]);

                            OrderHeader oh = new OrderHeader();
                            oh.OrderNo = aOrder[0];
                            oh.CustId = Convert.ToInt32(aOrder[1]);
                            oh.OrderDate = Convert.ToDateTime(aOrder[2]);
                            oh.OrderDateDisplay = aOrder[2];
                            oh.Total = Convert.ToDecimal(aOrder[3]);
                            oh.TotalDisplay = string.Format("{0:C}", oh.Total);
                            oh.Items = Convert.ToInt32(aOrder[4]);
                            oh.Pieces = Convert.ToInt32(aOrder[5]);

                            App.g_db.SaveOrderHeader(oh);
                        }

                        OrderDetail od = new OrderDetail();
                        od.OrderNo = aOrder[0];
                        od.LineNo = Convert.ToInt32(aOrder[6]);
                        od.ItemNo = Convert.ToInt32(aOrder[7]);
                        od.ItemNoDisplay = aOrder[7];
                        od.QtyOrdered = Convert.ToInt32(aOrder[8]);
                        od.QtyShipped = Convert.ToInt32(aOrder[8]);
                        od.Price = Convert.ToDecimal(aOrder[9]);
                        od.PriceDisplay = string.Format("{0:C}", od.Price);
                        od.UPC = aOrder[10];
                        if (od.UPC.Length > 0)
                        {
                            od.ItemNoDisplayUPC = "(" + od.UPC + ")";
                        }
                        else
                        {
                            od.ItemNoDisplayUPC = "";
                        }
                        od.Description = aOrder[11];
                        od.UOM = aOrder[12];
                        od.SellUnitsInPurch = aOrder[13];
                        od.SizeDisplay = od.UOM + "/" + od.SellUnitsInPurch;
                        od.SizeUOM = "/" + od.UOM;
                        od.Size = aOrder[14];
                        od.Form = aOrder[15];
                        od.CategoryCode = aOrder[16];
                        od.CategoryDesc = aOrder[17];
                        od.SubcategoryCode = aOrder[18];
                        od.SubcategoryDesc = aOrder[19];
                        od.VendorId = aOrder[20];
                        od.VendorName = aOrder[21];
                        od.Status = aOrder[22];
                        if (od.Status == "A")
                        {
                            od.IsAvailable = true;
                        }
                        else
                        {
                            od.IsAvailable = false;
                        }
                        try
                        {
                            od.QOH = Convert.ToInt32(aOrder[23].Trim());
                        }
                        catch
                        {
                            od.QOH = 0;
                        }
                        if (od.QOH == 0)
                        {
                            od.IsAvailable = false;
                        }
                        od.ImageURL = Constants.ItemImageUrl + od.ItemNo.ToString() + ".jpg";

                        //ReorderItem ri = new ReorderItem();
                        //ri.ItemNo = Convert.ToInt32(aOrder[7]);
                        //ri.ItemNoDisplay = aOrder[7];
                        //ri.LastPurchDate = Convert.ToDateTime(aOrder[2]);
                        //ri.LastPurchDateDisplay = aOrder[2];
                        //ri.QtyLastOrder = Convert.ToInt32(aOrder[8]);
                        //ri.QtyOrderDisplay = aOrder[8];
                        //ri.Description = aOrder[11];
                        //ri.Price = Convert.ToDecimal(aOrder[9]);
                        //ri.PriceDisplay = string.Format("{0:C}", ri.Price);
                        //ri.ImageURL = Constants.ItemImageUrl + ri.ItemNo.ToString() + ".jpg";
                        //ri.UPC = aOrder[10];
                        //if (ri.UPC.Length > 0)
                        //{
                        //    ri.ItemNoDisplayUPC = "(" + ri.UPC + ")";
                        //}
                        //else
                        //{
                        //    ri.ItemNoDisplayUPC = "";
                        //}
                        //ri.UOM = aOrder[12];
                        //ri.SellUnitsInPurch = aOrder[13];
                        //ri.SizeDisplay = ri.UOM + "/" + ri.SellUnitsInPurch;
                        //ri.SizeUOM = "/" + ri.UOM;
                        //ri.Size = aOrder[14];
                        //ri.Form = aOrder[15];
                        //ri.CategoryCode = aOrder[16];
                        //ri.CategoryDesc = aOrder[17];
                        //ri.SubcategoryCode = aOrder[18];
                        //ri.SubcategoryDesc = aOrder[19];
                        //ri.VendorId = aOrder[20];
                        //ri.VendorName = aOrder[21];
                        //ri.Status = aOrder[22];
                        //try
                        //{
                        //    ri.QOH = Convert.ToInt32(aOrder[23].Trim());
                        //}
                        //catch
                        //{
                        //    ri.QOH = 0;
                        //}
                        //ri.ImageURL = Constants.ItemImageUrl + ri.ItemNo.ToString() + ".jpg";

                        try
                        {
                            App.g_db.SaveOrderDetail(od);
                            //App.g_db.SaveReorderItem(ri);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.UpdateOrderDetailLastPurch();

                    App.g_ReorderItemList = App.g_db.GetReorderItems();

                    App.g_db.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async void commService_GetSalespersonCustomersCompletedAsync(String response)
        {
            try
            {
                Debug.WriteLine("Get Salesperson Customers returned");
                Debug.WriteLine(response);

                String sCustomers = response;
                String[] aCustomers = sCustomers.Split('~');

                if (aCustomers.Length > 1)
                {
                    App.g_db.BeginTransaction();

                    App.g_db.DeleteSalesCustomers();

                    foreach (String s in aCustomers)
                    {
                        String[] aCust = s.Split("|");
                        if (aCust.Count() < 2)
                        {
                            continue;
                        }

                        SalesCustomer c = new SalesCustomer();
                        c.CustNo = aCust[0];
                        c.CompanyName = aCust[1];
                        c.Address1 = aCust[2];
                        c.City = aCust[3];
                        c.State = aCust[4];
                        c.Zip = aCust[5];
                        c.CityStateZip = c.City.Trim() + ", " + c.State.Trim() + " " + c.Zip.Trim();
                        c.ARBalance = 0;
                        try
                        {
                            c.ARBalance = Convert.ToDecimal(aCust[6]);
                        }
                        catch { }
                        c.ARBalanceDisplay = string.Format("{0:C2}", c.ARBalance);
                        c.CreditLimit = 0;
                        try
                        {
                            c.CreditLimit = Convert.ToDecimal(aCust[7]);
                        }
                        catch { }
                        if (c.CreditLimit > 0)
                        {
                            c.CreditLimitDisplay = string.Format("{0:C2}", c.CreditLimit);
                        }
                        else
                        {
                            c.CreditLimitDisplay = "N/A";
                        }
                        c.Contact = aCust[8];
                        c.Phone = aCust[9];
                        c.Email = aCust[10];
                        // invoice multiplier aCust[11]
                        c.TermsDesc = aCust[12];
                        try
                        {
                            if (aCust[13] == "0")
                            {
                                c.LastPaymentDate = "N/A";
                            }
                            else
                            {
                                c.LastPaymentDate = aCust[13].Substring(3, 2) + "/";
                                c.LastPaymentDate += aCust[13].Substring(5, 2) + "/";
                                c.LastPaymentDate += aCust[13].Substring(1, 2);
                            }
                        }
                        catch { }
                        try
                        {
                            if (aCust[14] == "0")
                            {
                                c.LastOrderDate = "N/A";
                            }
                            else
                            {
                                c.LastOrderDate = aCust[17].Substring(3, 2) + "/";
                                c.LastOrderDate += aCust[17].Substring(5, 2) + "/";
                                c.LastOrderDate += aCust[17].Substring(1, 2);
                            }
                        }
                        catch { }
                        try
                        {
                            c.MinOrderAmount = Decimal.Parse(aCust[15]);
                            c.ShippingFee = Decimal.Parse(aCust[16]);
                            c.MinOrderQty = Decimal.Parse(aCust[17]);
                        }
                        catch { }

                        try
                        {
                            App.g_db.SaveSalesCustomer(c);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async void commService_GetFlyerItemsPDFCompleted(String response)
        {
            Debug.WriteLine("GetFlyerItemsPDFCompleted");

            try
            {
                String sItems = response;
                String[] sFlyerInfo = sItems.Split('^');
                String[] aItems = sFlyerInfo[0].Split('~');

                if (aItems.Length > 1)
                {
                    //Database db = new Database();

                    App.g_db.BeginTransaction();

                    App.g_db.ClearFlyerItems();

                    foreach (String s in aItems)
                    {
                        String[] aItem = s.Split("|");
                        if (aItem.Count() < 3)
                        {
                            continue;
                        }

                        FlyerItem item = new FlyerItem();

                        item.ItemNo = Convert.ToInt32(aItem[0].Trim());
                        item.Page = Convert.ToInt32(aItem[1].Trim());
                        item.Box = Convert.ToInt32(aItem[2].Trim());
                        item.Section = aItem[3].Trim();
                        item.StartDate = Convert.ToInt32(aItem[4].Trim());
                        item.EndDate = Convert.ToInt32(aItem[5].Trim());
                        item.TopLeftX = (int)Convert.ToDecimal(aItem[6].Trim());
                        item.TopLeftY = (int)Convert.ToDecimal(aItem[7].Trim());
                        item.BottomRightX = (int)Convert.ToDecimal(aItem[8].Trim());
                        item.BottomRightY = (int)Convert.ToDecimal(aItem[9].Trim());

                        if (item.Section == "COVER")
                        {
                            item.Section = " COVER";
                        }

                        try
                        {
                            App.g_db.UpdateItemFlyerInfo(item);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }

                    App.g_db.CommitTransaction();
                }

                if (sFlyerInfo[1].Length > 0)
                {
                    try
                    {
                        byte[] data = Convert.FromBase64String(sFlyerInfo[1]);
                        File.Delete(App.g_FlyerFilename);
                        File.WriteAllBytes(App.g_FlyerFilename, data);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
        }

        public static async void commService_ValidateUserActiveCompletedAsync(String response)
        {
            String sUser = response;
            if (sUser == "0")
            {
                try
                {
                    App.g_db.SaveSetting("LoggedIn", "0");
                    App.g_db.SaveSetting("UserName", App.g_UserName);

                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToLogin();
                        });
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
