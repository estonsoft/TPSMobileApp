using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TPSMobileApp
{
    internal class XMLResponseParser
    {
        public static async Task commService_GetBannersCompleted(String response)
        {
            try
            {
                Console.WriteLine("Get Banners returned");
                String sBanners = response;
                String[] aBanners = sBanners.Split('|');
                ConcurrentBag<Banner> lstBanners = new ConcurrentBag<Banner>();
                if (aBanners.Length >= 1)
                {
                    // foreach (String s in aBanners)
                    // {
                    Parallel.ForEach(aBanners, s =>
                    {
                        Banner banner = new Banner();
                        banner.BannerName = s;
                        banner.BannerURL = Constants.BannerUrl + banner.BannerName;
                        lstBanners.Add(banner);
                    });
                }
                try
                {

                    await App.g_db.DeleteBannersAsync();
                    await App.g_db.SaveBannerAsync(lstBanners.ToList());

                    Console.WriteLine("Get Banners returned Completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while saving banners: " + ex.Message);
                }
                await App.CommManager.GetCategoriesAndSubcategoriesCust(App.g_Customer.CustNo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Banners Error");
                Console.WriteLine(ex.Message);
            }
        }


        public static async Task commService_GetCategoriesAndSubcategoriesCompleted(String response)
        {
            Console.WriteLine("Get Categories and Subcategories returned");

            try
            {
                String sCategories = response;
                String[] aCategories = sCategories.Split('~');
                ConcurrentBag<Category> lstCategories = new ConcurrentBag<Category>();
                ConcurrentBag<Subcategory> lstSubcategories = new ConcurrentBag<Subcategory>();
                if (aCategories.Length > 1)
                {
                    Parallel.ForEach(aCategories, s =>
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            return; // Skip this iteration if there are not enough elements
                        }

                        if (aCategory[1].Length == 0)
                        {
                            Category cat = new Category();
                            cat.Code = aCategory[0];
                            cat.Description = aCategory[2].Trim();
                            cat.ImageURL = Constants.CategoryImageUrl + cat.Code + ".png";
                            cat.Rank = GetIntegerValue("Category rank", aCategory[3], 0);
                            cat.HomePage = GetIntegerValue("Category home page", aCategory[4], 0);
                            lstCategories.Add(cat);
                        }
                        else
                        {
                            Subcategory subcat = new Subcategory();
                            subcat.Category = aCategory[0];
                            subcat.Code = aCategory[1];
                            subcat.Description = aCategory[2].Trim();
                            subcat.Rank = GetIntegerValue("Subcategory rank", aCategory[3], 0);
                            lstSubcategories.Add(subcat);
                        }
                    });
                    try
                    {

                        await App.g_db.DeleteAllCategory();
                        await App.g_db.DeleteAllSubcategory();
                        await App.g_db.SaveCategory(lstCategories.ToList());
                        await App.g_db.SaveSubcategory(lstSubcategories.ToList());

                        Console.WriteLine("Get Categories and Subcategories returned Completed");
                        App.g_HomePageCategoryList = await App.g_db.GetHomePageCategories();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while parsing categories and subcategories: " + ex.Message);
                    }
                }

                try
                {
                    String CustNo = "0";
                    try
                    {
                        CustNo = App.g_Customer.CustNo;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while parsing customer number: " + ex.Message);
                        CustNo = "0";
                    }

                    //Database db = new Database();
                    string sDate = "0";
                    if (App.g_Customer.CustNo == "0")
                    {
                        await App.CommManager.GetItems("0", sDate);
                    }
                    else
                    {
                        await App.CommManager.GetItems(App.g_Customer.CustNo, sDate);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Fetch Items Categories and SubCategories" + e.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SAVE Categories and SubCategories" + ex.Message);
            }
        }

        public static async Task commService_GetCategoriesAndSubcategoriesCustCompleted(String response)
        {
            Console.WriteLine("Get Categories Subcategories and Subsubcategories Cust returned");

            try
            {
                String sCategories = response;
                String[] aCategories = sCategories.Split('~');
                ConcurrentBag<Category> lstCategories = new ConcurrentBag<Category>();
                ConcurrentBag<Subcategory> lstSubcategories = new ConcurrentBag<Subcategory>();
                ConcurrentBag<Subsubcategory> lstSubsubcategories = new ConcurrentBag<Subsubcategory>();

                if (aCategories.Length > 1)
                {
                    Parallel.ForEach(aCategories, s =>
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            return; // Skip this iteration if there are not enough elements
                        }

                        string sSubsubcategory;
                        try
                        {
                            sSubsubcategory = aCategory[5];
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error occurred while parsing subsubcategory: " + ex.Message);
                            sSubsubcategory = "";
                        }

                        if (aCategory[1].Length == 0)  // no subcategory, just add category
                        {
                            Category cat = new Category();
                            cat.Code = aCategory[0];
                            cat.Description = aCategory[2].Trim();
                            cat.ImageURL = Constants.CategoryImageUrl + cat.Code + ".png";
                            cat.Rank = GetIntegerValue("Category rank", aCategory[3], 0);
                            cat.HomePage = GetIntegerValue("Category home page", aCategory[4], 0);
                            lstCategories.Add(cat);
                        }
                        else if (sSubsubcategory.Length == 0)  // no subsubcat, just add subcategory
                        {
                            Subcategory subcat = new Subcategory();
                            subcat.Category = aCategory[0];
                            subcat.Code = aCategory[1];
                            subcat.Description = aCategory[2].Trim();
                            subcat.Rank = GetIntegerValue("Subcategory rank", aCategory[3], 0);
                            lstSubcategories.Add(subcat);
                        }
                        else // add subsubcategory
                        {
                            Subsubcategory subsubcat = new Subsubcategory();
                            subsubcat.Category = aCategory[0];
                            subsubcat.Subcategory = aCategory[1];
                            subsubcat.Code = sSubsubcategory;
                            subsubcat.Description = aCategory[2].Trim();
                            subsubcat.Rank = GetIntegerValue("Subsubcategory rank", aCategory[3], 0);
                            lstSubsubcategories.Add(subsubcat);
                        }
                    });
                }
                try
                {

                    await App.g_db.DeleteAllCategory();
                    await App.g_db.DeleteAllSubcategory();
                    await App.g_db.DeleteAllSubsubcategory();
                    await App.g_db.SaveCategory(lstCategories.ToList());
                    await App.g_db.SaveSubcategory(lstSubcategories.ToList());
                    await App.g_db.SaveSubsubcategory(lstSubsubcategories.ToList());

                    Console.WriteLine("Get Categories Subcategories and Subsubcategories returned Completed");
                    App.g_HomePageCategoryList = await App.g_db.GetHomePageCategories();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while parsing categories subcategories and subsubcategories: " + ex.Message);
                }

                try
                {
                    Console.WriteLine("Get Categories Subcategories and Subsubcategories Cust returned Completed");
                    String CustNo = "0";
                    try
                    {
                        CustNo = App.g_Customer.CustNo;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while parsing customer number: " + ex.Message);
                        CustNo = "0";
                    }

                    //Database db = new Database();
                    string sDate = "0";
                    if (App.g_Customer.CustNo == "0")
                    {
                        await App.CommManager.GetItems("0", sDate);
                    }
                    else
                    {
                        await App.CommManager.GetItems(App.g_Customer.CustNo, sDate);
                    }
                    App.g_HomePageCategoryList = await App.g_db.GetHomePageCategories();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Fetch Items Categories and SubCategories" + e.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SAVE Categories and SubCategories" + ex.Message);
            }
        }

        public static async Task commService_GetItemsCompletedAsync(String response)
        {
            try
            {
                Console.WriteLine(DateTimeOffset.Now.ToString() + " - Get Items returned");
                String sItems = response;
                String[] aItems = sItems.Split('~');
                if (aItems.Length > 1)
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    List<Item> lstCartItems = await App.g_db.GetCartItems();
                    var cartDict = lstCartItems.ToDictionary(c => c.ItemNo); // O(1) lookup instead of nested loop

                    var itemsToSave = new ConcurrentBag<Item>();
                    var processedItemNos = new ConcurrentBag<int>();

                    Parallel.ForEach(aItems, s =>
                    {
                        if (string.IsNullOrWhiteSpace(s)) return; // Skip empty rows
                        String[] aItem = s.Split("|");

                        Item item = new Item();
                        item.ItemNo = GetIntegerValue("ItemNo", aItem[0], 0);
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
                            item.ItemNoDisplayUPC = item.UPC_1;
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
                        item.RetailPrice = GetDecimalValue("RetailPrice", aItem[14], 0);
                        item.RetailPriceDisplay = aItem[14].Trim();
                        item.UOM = aItem[15].Trim();
                        item.SizeUOM = "/" + item.UOM;
                        item.Size = GetIntegerValue("Size", aItem[16], 1);
                        item.SizeDisplay = aItem[16].Trim();
                        item.Form = aItem[17].Trim();
                        item.Price = GetDecimalValue("Price", aItem[18], 0);
                        item.PriceDisplay = string.Format("{0:C}", item.Price);
                        item.Tax = GetDecimalValue("Tax", aItem[19], 0);
                        item.TaxDisplay = string.Format("{0:C}", item.Tax);
                        item.CategoryRank = GetIntegerValue("CategoryRank", aItem[20], 0);
                        item.SellUnitsInPurchaseUnit = GetIntegerValue("SellUnitsInPurchaseUnit", aItem[21], 1);
                        item.Status = aItem[22];
                        item.QOH = GetIntegerValue("QOH", aItem[23], 0);
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
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing added date: " + e.Message);
                        }
                        item.AllocationQty = GetIntegerValue("AllocationQty", aItem[26], 0);
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
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing price visibility: " + e.Message);
                            item.IsPriceVisible = 1;
                        }

                        try
                        {
                            item.Keyword1 = aItem[28];
                            item.Keyword2 = aItem[29];
                            item.Keyword3 = aItem[30];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing keywords: " + e.Message);
                            item.Keyword1 = "";
                            item.Keyword2 = "";
                            item.Keyword3 = "";
                        }

                        try
                        {
                            item.LastPurchDateDisplay = aItem[31];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing last purchase date display: " + e.Message);
                            item.LastPurchDateDisplay = "";
                        }
                        if (item.LastPurchDateDisplay.Trim() != "")
                        {
                            item.LastPurchDate = GetDateTime("LastPurchDate", item.LastPurchDateDisplay);
                        }
                        if (aItem[32] == "")
                        {
                            item.QtyLastOrder = 0;
                        }
                        else
                        {
                            item.QtyLastOrder = GetIntegerValue("QtyLastOrder", aItem[32], 0);
                        }

                        try
                        {
                            item.SubsubcategoryCode = aItem[33];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing subsubcategory code: " + e.Message);
                            item.SubsubcategoryCode = "";
                        }
                        try
                        {
                            item.SubsubcategoryDesc = aItem[34];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing subsubcategory description: " + e.Message);
                            item.SubsubcategoryDesc = "";
                        }
                        try
                        {
                            item.ItemRefNo = aItem[35];
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occurred while parsing item reference number: " + e.Message);
                            item.ItemRefNo = "";
                        }


                        item.AddToOrderDisplay = "Add To Order";
                        item.QtyOrder = 0;
                        item.QtyCredit = 0;
                        item.QtyLabel = 0;
                        item.LineNo = 0;

                        if (cartDict.TryGetValue(item.ItemNo, out var ci))
                        {
                            item.QtyOrder = ci.QtyOrder;
                            item.QtyCredit = ci.QtyCredit;
                            item.QtyLabel = ci.QtyLabel;
                            item.LineNo = ci.LineNo;
                        }

                        itemsToSave.Add(item);
                        processedItemNos.Add(item.ItemNo);
                    });

                    Console.WriteLine($"Parse loop: {sw.ElapsedMilliseconds}ms"); sw.Restart();

                    try
                    {
                        // 
                        await App.g_db.InsertDiscontinuedItems();
                        await App.g_db.DeleteItems();
                        await App.g_db.SaveItems(itemsToSave.ToList());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while bulk-saving items: " + ex.Message);
                    }

                    Console.WriteLine($"Save items ({itemsToSave.Count}): {sw.ElapsedMilliseconds}ms"); sw.Restart();

                    try
                    {
                        await App.g_db.DeleteDiscontinuedItems(processedItemNos.ToList());

                        Console.WriteLine($"Delete discontinued: {sw.ElapsedMilliseconds}ms"); sw.Restart();

                        await App.g_db.UpdateDiscontinuedItems();
                        Console.WriteLine("Update Discontinued Items completed");
                        await App.g_db.UpdateOrderDetailLastPurch();
                        Console.WriteLine("Update Order Detail Last Purch completed");
                        await App.g_db.SaveSetting("LastUpdateItems", DateTimeOffset.Now.ToString("1yyMMdd"));

                        App.g_ItemList = await App.g_db.GetItems();

                        // 

                        Console.WriteLine($"Finalize + commit: {sw.ElapsedMilliseconds}ms");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while removing discontinued items: " + ex.Message);
                    }

                    await App.CommManager.GetItemQOH(App.g_Customer.CustNo);
                    await App.CommManager.GetOrderHistory(App.g_Customer.CustNo);
                    await App.CommManager.GetFlyerItemsPDF();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while updating items: " + ex.Message + ex.StackTrace);
            }
        }


        public static async Task commService_GetItemQOHCompletedAsync(string response)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return;

                // if (response == "X")
                // {
                //     App.g_Shell.Logout();
                //     return;
                // }

                string[] aItems = response.Split(
                    '~',
                    StringSplitOptions.RemoveEmptyEntries);

                if (aItems.Length == 0)
                    return;

                var qohUpdates = new List<(int ItemNo, int QOH)>(aItems.Length);

                foreach (string item in aItems)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    string[] aItem = item.Split('|');

                    if (aItem.Length < 2)
                        continue;

                    int itemNo = GetIntegerValue(
                        "Item Number",
                        aItem[0],
                        0);

                    int qoh = GetIntegerValue(
                        "QOH",
                        aItem[1],
                        0);

                    if (itemNo <= 0)
                        continue;

                    qohUpdates.Add((itemNo, qoh));
                }

                if (qohUpdates.Count == 0)
                    return;

                // Bulk update
                await App.g_db.UpdateAllItemQOH(qohUpdates);

                Console.WriteLine(
                    $"Item QOH updated successfully: {qohUpdates.Count} items");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Get Item QOH Exception: {ex.Message}");

                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task commService_GetItemQOH2CompletedAsync(string response)
        {
            Console.WriteLine("Get Item QOH 2 returned");

            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return;

                // if (response == "X")
                // {
                //     App.g_Shell.Logout();
                //     return;
                // }

                var updates = new List<(int ItemNo, int QOH)>();

                foreach (string item in response.Split(
                    '~',
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] values = item.Split('|');

                    if (values.Length < 2)
                        continue;

                    int itemNo = GetIntegerValue(
                        "Item Number",
                        values[0],
                        0);

                    int qoh = GetIntegerValue(
                        "QOH",
                        values[1],
                        0);

                    if (itemNo <= 0)
                        continue;

                    updates.Add((itemNo, qoh));
                }

                if (updates.Count > 0)
                {
                    // ONE DB call + ONE transaction
                    await App.g_db.UpdateAllItemQOH(updates);

                    Console.WriteLine(
                        $"QOH 2 updated: {updates.Count} items");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Get Item QOH 2 Exception: {ex.Message}");

                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Get Item QOH 2 Completed");
        }

        public static async Task commService_ValidateLoginCompletedAsync(String response)
        {
            Debug.WriteLine("ValidateLogin Complete");
            try
            {
                String sUser = response;
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
                            await App.g_db.SaveSetting("Credits", aUser[2]);

                            if (aUser[3] == "1")
                            {
                                App.g_HoldForReview = true;
                            }
                            else
                            {
                                App.g_HoldForReview = false;
                            }
                            await App.g_db.SaveSetting("HoldForReview", aUser[3]);

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
                                await App.g_db.SaveSetting("ForceSubmit", aUser[4]);
                            }
                            catch
                            {
                                App.g_ForceSubmit = false;
                                await App.g_db.SaveSetting("ForceSubmit", "0");
                            }

                            try
                            {
                                App.g_QOHDisplay = aUser[5];
                            }
                            catch
                            {
                                App.g_QOHDisplay = "X";
                            }
                            await App.g_db.SaveSetting("QOHDisplay", App.g_QOHDisplay);

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
                                await App.g_db.SaveSetting("BlockItemsNoQOH", aUser[6]);
                            }
                            catch
                            {
                                App.g_BlockItemsNoQOH = false;
                                await App.g_db.SaveSetting("BlockItemsNoQOH", "0");
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
                                await App.g_db.SaveSetting("IsScandit", aUser[7]);
                            }
                            catch
                            {
                                App.g_IsScandit = false;
                                await App.g_db.SaveSetting("IsScandit", "0");
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
                                await App.g_db.SaveSetting("IsSalesUser", aUser[8]);
                            }
                            catch
                            {
                                App.g_IsSalesUser = false;
                                await App.g_db.SaveSetting("IsSalesUser", "0");
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
                                await App.g_db.SaveSetting("MonthlyFlyer", aUser[9]);
                            }
                            catch
                            {
                                App.g_IsMonthlyFlyer = false;
                                await App.g_db.SaveSetting("MonthlyFlyer", "0");
                            }
                            int iFlyerStartDate = 0;
                            try
                            {
                                string sFlyerStartDate = aUser[10];
                                int.TryParse(sFlyerStartDate, out iFlyerStartDate);
                            }
                            catch { }
                            await App.g_db.SaveSetting("FlyerStartDate", iFlyerStartDate.ToString());
                            App.g_FlyerStartDate = iFlyerStartDate;
                            int iFlyerEndDate = 0;
                            try
                            {
                                string sFlyerEndDate = aUser[11];
                                int.TryParse(sFlyerEndDate, out iFlyerEndDate);
                            }
                            catch { }
                            await App.g_db.SaveSetting("FlyerEndDate", iFlyerEndDate.ToString());
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
                                await App.g_db.SaveSetting("AutoAdd1", aUser[13]);
                            }
                            catch
                            {
                                App.g_IsAutoAdd1 = false;
                                await App.g_db.SaveSetting("AutoAdd1", "0");
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
                                await App.g_db.SaveSetting("RefNoLookup", aUser[14]);
                            }
                            catch
                            {
                                App.g_IsRefNoLookup = false;
                                await App.g_db.SaveSetting("RefNoLookup", "0");
                            }
                            try
                            {
                                App.g_ShoppingCartSort = aUser[15];
                                await App.g_db.SaveSetting("ShoppingCartSort", aUser[15]);
                            }
                            catch
                            {
                                App.g_ShoppingCartSort = "A";
                                await App.g_db.SaveSetting("ShoppingCartSort", "A");
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
                                await App.g_db.SaveSetting("IsChainManager", aUser[16]);
                            }
                            catch
                            {
                                App.g_IsChainManager = false;
                                await App.g_db.SaveSetting("IsChainManager", "0");
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
                                await App.g_db.SaveCustomer(App.g_Customer);
                                await App.g_db.SaveLocation(loc);

                                await App.g_db.RestoreCartItems(App.g_Customer.CustNo);
                            }
                        }
                        catch (Exception ex)
                        {
                        }



                        if (App.g_Customer.CustNo != OldCustNo)
                        {
                            if (App.g_UserName.ToLower() == "app_test")
                            {
                                await App.g_db.DeleteCategories();
                                await App.g_db.DeleteItems();
                            }
                        }

                        // await App.RefreshAll();
                        // if ((App.g_IsSalesUser) || (App.g_IsChainManager))
                        // {
                        //     await App.CommManager.GetSalespersonCustomers(App.g_UserName);
                        // }
                        // await App.CommManager.GetOrderHistory(App.g_Customer.CustNo);

                        await App.g_db.SaveSetting("LoggedIn", "1");
                        await App.g_db.SaveSetting("UserName", App.g_UserName);
                        App.g_IsLoggedIn = true;
                        MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await App.g_Shell.GoToHome();
                            });
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

        public static async Task commService_GetSettingsCompletedAsync(String response)
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
                await App.g_db.SaveSetting("HoldForReview", aSettings[0]);

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
                    await App.g_db.SaveSetting("ForceSubmit", aSettings[1]);
                }
                catch
                {
                    App.g_ForceSubmit = false;
                    await App.g_db.SaveSetting("ForceSubmit", "0");
                }

                try
                {
                    App.g_QOHDisplay = aSettings[2];
                }
                catch
                {
                    App.g_QOHDisplay = "X";
                }
                await App.g_db.SaveSetting("QOHDisplay", App.g_QOHDisplay);

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
                    await App.g_db.SaveSetting("BlockItemsNoQOH", aSettings[3]);
                }
                catch
                {
                    App.g_BlockItemsNoQOH = false;
                    await App.g_db.SaveSetting("BlockItemsNoQOH", "0");
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
                    await App.g_db.SaveSetting("MonthlyFlyer", aSettings[4]);
                }
                catch
                {
                    App.g_IsMonthlyFlyer = false;
                    await App.g_db.SaveSetting("MonthlyFlyer", "0");
                }

                int iFlyerStartDate = 0;
                try
                {
                    string sFlyerStartDate = aSettings[5];
                    int.TryParse(sFlyerStartDate, out iFlyerStartDate);
                }
                catch { }
                await App.g_db.SaveSetting("FlyerStartDate", iFlyerStartDate.ToString());
                App.g_FlyerStartDate = iFlyerStartDate;

                int iFlyerEndDate = 0;
                try
                {
                    string sFlyerEndDate = aSettings[6];
                    int.TryParse(sFlyerEndDate, out iFlyerEndDate);
                }
                catch { }
                await App.g_db.SaveSetting("FlyerEndDate", iFlyerEndDate.ToString());
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
                    await App.g_db.SaveSetting("AutoAdd1", aSettings[8]);
                }
                catch
                {
                    App.g_IsAutoAdd1 = false;
                    await App.g_db.SaveSetting("AutoAdd1", "0");
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
                    await App.g_db.SaveSetting("RefNoLookup", aSettings[9]);
                }
                catch
                {
                    App.g_IsRefNoLookup = false;
                    await App.g_db.SaveSetting("RefNoLookup", "0");
                }

                try
                {
                    App.g_ShoppingCartSort = aSettings[10];
                    await App.g_db.SaveSetting("ShoppingCartSort", aSettings[10]);
                }
                catch
                {
                    App.g_ShoppingCartSort = "A";
                    await App.g_db.SaveSetting("ShoppingCartSort", "A");
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task commService_SubmitOrderCompletedAsync(String response)
        {
            try
            {
                if (response == "S")
                {
                    await App.g_db.ClearOrderCartItems();
                    App.g_Notes = "";

                    await Shell.Current.DisplayAlertAsync("Profit Order", "Thank you! Your order has been placed.", "OK");

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.g_Shell.GoToHome();
                    });
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
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Order has already been submitted.", "Ok");

                    await App.g_db.ClearOrderCartItems();
                    App.g_Notes = "";
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.g_Shell.GoToHome();
                    });
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Error submitting order.  Please try again.", "Ok");
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task commService_SubmitReturnCompletedAsync(String response)
        {
            try
            {

                if (response == "S")
                {
                    await App.g_db.ClearReturnCartItems();

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

        public static async Task commService_GetOrderHistoryCompletedAsync(string response)
        {
            Debug.WriteLine("Get Order History Returned");

            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return;

                string[] orders = response.Split(
                    '~',
                    StringSplitOptions.RemoveEmptyEntries);

                if (orders.Length == 0)
                    return;

                var existingHeaders = await App.g_db.GetOrderHeaders();

                var existingOrderNos = new HashSet<string>();

                for (int i = 0; i < existingHeaders.Count; i++)
                {
                    var orderNo = existingHeaders[i].OrderNo;
                    if (orderNo != null)
                    {
                        existingOrderNos.Add(orderNo);
                    }
                }

                var addedHeaders = new HashSet<string>();

                var headersToSave = new List<OrderHeader>();
                var detailsToSave = new List<OrderDetail>();

                foreach (string order in orders)
                {
                    string[] aOrder = order.Split('|');

                    if (aOrder.Length < 24)
                        continue;

                    string orderNo = aOrder[0];

                    // Header
                    if (!existingOrderNos.Contains(orderNo) &&
                        addedHeaders.Add(orderNo))
                    {
                        headersToSave.Add(new OrderHeader
                        {
                            OrderNo = orderNo,
                            CustId = GetIntegerValue("CustId", aOrder[1], 0),
                            OrderDate = GetDateTime("OrderDate", aOrder[2]),
                            OrderDateDisplay = aOrder[2],
                            Total = GetDecimalValue("Total", aOrder[3], 0),
                            TotalDisplay = GetDecimalValue("Total", aOrder[3], 0).ToString("0.00"),
                            Items = GetIntegerValue("Items", aOrder[4], 0),
                            Pieces = GetIntegerValue("Pieces", aOrder[5], 0)
                        });
                    }

                    int itemNo = GetIntegerValue("ItemNo", aOrder[7], 0);
                    int qoh = GetIntegerValue("QOH", aOrder[23], 0);

                    int lineNo = GetIntegerValue("LineNo", aOrder[6], 0);

                    detailsToSave.Add(new OrderDetail
                    {
                        Id = $"{orderNo}|{lineNo}",
                        OrderNo = orderNo,
                        LineNo = lineNo,
                        ItemNo = itemNo,
                        ItemNoDisplay = aOrder[7],
                        QtyOrdered = GetIntegerValue("QtyOrdered", aOrder[8], 0),
                        QtyShipped = GetIntegerValue("QtyShipped", aOrder[8], 0),
                        Price = GetDecimalValue("Price", aOrder[9], 0),
                        PriceDisplay = GetDecimalValue("Price", aOrder[9], 0).ToString("0.00"),
                        UPC = aOrder[10],
                        ItemNoDisplayUPC = string.IsNullOrWhiteSpace(aOrder[10])
                            ? string.Empty
                            : aOrder[10],
                        Description = aOrder[11],
                        UOM = aOrder[12],
                        SellUnitsInPurch = aOrder[13],
                        SizeDisplay = $"{aOrder[12]}/{aOrder[13]}",
                        SizeUOM = $"/{aOrder[12]}",
                        Size = aOrder[14],
                        Form = aOrder[15],
                        CategoryCode = aOrder[16],
                        CategoryDesc = aOrder[17],
                        SubcategoryCode = aOrder[18],
                        SubcategoryDesc = aOrder[19],
                        VendorId = aOrder[20],
                        VendorName = aOrder[21],
                        Status = aOrder[22],
                        QOH = qoh,
                        IsAvailable = aOrder[22] == "A" && qoh > 0,
                        ImageURL = $"{Constants.ItemImageUrl}{itemNo}.jpg"
                    });
                }

                //
                // BULK SAVE
                //
                await App.g_db.DeleteOrderHistory();
                await App.g_db.SaveOrderHeaders(headersToSave);
                await App.g_db.SaveOrderDetails(detailsToSave);

                await App.g_db.UpdateOrderDetailLastPurch();

                App.g_ReorderItemList =
                    await App.g_db.GetReorderItems();

                Debug.WriteLine(
                    $"Headers:{headersToSave.Count} Details:{detailsToSave.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"GetOrderHistory Exception: {ex}");
            }
            Debug.WriteLine("Get Order History Complete");
        }

        public static async Task commService_GetInvoicePDFCompletedAsync(String response)
        {
            Debug.WriteLine("Get Invoice PDF Complete");

            try
            {
                String sBase64PDF = response;
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task commService_GetSalespersonCustomersCompletedAsync(String response)
        {
            try
            {
                Console.WriteLine("Get Salesperson Customers returned");

                String sCustomers = response;
                String[] aCustomers = sCustomers.Split('~');
                ConcurrentBag<SalesCustomer> lstCustomer = new ConcurrentBag<SalesCustomer>();
                if (aCustomers.Length > 1)
                {
                    Parallel.ForEach(aCustomers, s =>
                    {
                        String[] aCust = s.Split("|");
                        if (aCust.Count() < 2)
                        {
                            return;
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
                            c.ARBalance = GetDecimalValue("SalesCustomer.ARBalance", aCust[6], 0);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error occurred while parsing ARBalance: " + ex.Message);
                        }
                        c.ARBalanceDisplay = string.Format("{0:C2}", c.ARBalance);
                        c.CreditLimit = 0;
                        try
                        {
                            string creditLimitStr = aCust[7];
                            if (!string.IsNullOrEmpty(creditLimitStr))
                            {
                                c.CreditLimit = GetDecimalValue("SalesCustomer.CreditLimit", creditLimitStr, 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error occurred while parsing CreditLimit: " + ex.Message);
                        }
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
                                c.LastOrderDate = FormatLastOrderDate(aCust[13]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error occurred while parsing LastPaymentDate: " + ex.Message);
                        }
                        try
                        {

                            if (aCust[14] == "0")
                            {
                                c.LastOrderDate = "N/A";
                            }
                            else
                            {
                                c.LastOrderDate = FormatLastOrderDate(aCust[14]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error occurred while parsing LastOrderDate: " + ex.Message);
                        }
                        try
                        {
                            c.MinOrderAmount = GetDecimalValue("SalesCustomer.MinOrderAmount", aCust[15], 0);
                            c.ShippingFee = GetDecimalValue("SalesCustomer.ShippingFee", aCust[16], 0);
                            c.MinOrderQty = GetDecimalValue("SalesCustomer.MinOrderQty", aCust[17], 0);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error occurred while parsing min order values: " + ex.Message);
                        }
                        lstCustomer.Add(c);
                    });

                    await App.g_db.DeleteAllSalesCustomer();
                    await App.g_db.SaveSalesCustomer(lstCustomer.ToList());
                    Console.WriteLine("Saving SalesPerson Customers");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeception in parsing SalesPerson" + ex.Message);
            }
        }

        public static async Task commService_GetFlyerItemsPDFCompleted(String response)
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



                    await App.g_db.ClearFlyerItems();

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
                            await App.g_db.UpdateItemFlyerInfo(item);
                        }
                        catch (Exception ex)
                        {
                            String sMsg = ex.Message;
                        }
                    }


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

        public static async Task commService_ValidateUserActiveCompletedAsync(String response)
        {
            String sUser = response;
            if (sUser == "0")
            {
                try
                {
                    await App.g_db.SaveSetting("LoggedIn", "0");
                    await App.g_db.SaveSetting("UserName", App.g_UserName);

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
        public static DateTimeOffset GetDateTime(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTimeOffset.MinValue;

            value = value.Trim();

            // First try exact formats
            string[] formats =
            {
                "M/d/yyyy",
                "MM/dd/yyyy",
                "yyyy-MM-dd",
                "yyyyMMdd",
                "M/d/yy",
                "MM/dd/yy"
            };

            if (DateTimeOffset.TryParseExact(
                    value,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            // Fallback to normal parsing
            if (DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
            {
                return date;
            }

            Console.WriteLine($"{key} Invalid Date: '{value}'");

            return DateTimeOffset.MinValue;
        }
        public static int GetIntegerValue(String key, String value, int defaultValue)
        {
            try
            {
                string sizeValue = value.Trim();
                if (sizeValue.Length > 0)
                {
                    string digits = Regex.Match(sizeValue ?? "", @"^\d+").Value;

                    return int.TryParse(digits, out var size)
                        ? size
                        : defaultValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(key + "Converting string to int" + e.Message);
                return defaultValue;
            }
        }

        public static Decimal GetDecimalValue(String key, String value, Decimal defaultValue)
        {
            try
            {
                string sizeValue = value.Trim();
                if (sizeValue.Length != 0)
                    return Convert.ToDecimal(sizeValue);
                else
                    return defaultValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(key + "Converting string to Decimal " + e.Message);
                return defaultValue;
            }
        }
        private static string FormatLastOrderDate(string rawDate)
        {
            if (string.IsNullOrWhiteSpace(rawDate) || rawDate.Length < 7)
            {
                return "N/A";
            }
            string date = $"{rawDate[3]}{rawDate[4]}/" +
                   $"{rawDate[5]}{rawDate[6]}/" +
                   $"{rawDate[1]}{rawDate[2]}";
            return date;
        }
    }
}
