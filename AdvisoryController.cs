using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WealthClock.CodeFile;
using WealthClock.Models;
using Newtonsoft.Json;
using Wealthclock.CodeFile;
using WealthClock_Api.Models;
using Wealthclock.Models;
using System.Web.Configuration;

namespace WealthClock.Controllers
{
    public class AdvisoryController : Controller
    {
        public static string Type = string.Empty;
        public static decimal Amount = 0;
        public static int Percent = 0;
        public static string Year = string.Empty;
        public static string _renderHtml = "";
        public static string _Url = "";

        TransactModel TM = new TransactModel();

        public ActionResult GoalPlanner()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Marriage()
        {
            return PartialView("~/Views/Advisory/_partialMarriage.cshtml");
        }

        [HttpPost]
        public ActionResult BuyNewHome()
        {
            return PartialView("~/Views/Advisory/_partialBuyHome.cshtml");
        }

        public ActionResult BuyNewCar()
        {
            return PartialView("~/Views/Advisory/_partialBuyCar.cshtml");
        }

        public ActionResult Education()
        {
            return PartialView("~/Views/Advisory/_partialEducation.cshtml");
        }

        public ActionResult FamilyHoliday()
        {
            return PartialView("~/Views/Advisory/_partialHoliday.cshtml");
        }

        public ActionResult ChildMarriage()
        {
            return PartialView("~/Views/Advisory/_partialChildMarriage.cshtml");
        }

        public ActionResult Retairment()
        {
            return PartialView("~/Views/Advisory/_partialRetairment.cshtml");
        }

        public ActionResult GoalScheme(decimal amount, string type, string targetYear)
        {
            _Url = Request.Url.ToString();
            if (Request.IsAuthenticated)
            {
                _Url = "";
                AdvisoryCodeFile _PageObj = new AdvisoryCodeFile();
                List<GoalPlannerScheme> _Data = new List<GoalPlannerScheme>();
                int count = 0;
                if (amount > 0 && type != "")
                {
                    string ret = _PageObj.GetGoalPlannerTypeByText(type);
                    string ret_Scheme = _PageObj.GetSchemeInfobyType(Convert.ToInt32(ret));
                    if (ret_Scheme != "")
                    {
                        _Data = JsonConvert.DeserializeObject<List<GoalPlannerScheme>>(ret_Scheme);
                    }
                    if (_Data != null && _Data.Count > 1)
                    {
                        if (amount <= 2000)
                        {
                            for (int i = 0; i < _Data.Count; i++)
                            {
                                _Data[i].Price = amount;

                            }
                            _Data.RemoveAt(1);
                            _Data.RemoveAt(1);
                        }
                        else if (amount > 2000 && amount <= 3500)
                        {
                            for (int i = 0; i < _Data.Count; i++)
                            {
                                if (count == 0)
                                {

                                    _Data[i].Price = (amount * 60) / 100;
                                }
                                else
                                {
                                    _Data[i].Price = (amount * 40) / 100;
                                }
                                count++;

                            }
                        }
                        else
                        {
                            for (int i = 0; i < _Data.Count; i++)
                            {

                                _Data[i].Price = (amount * _Data[i]._goalSchemeProportion) / 100;


                            }
                        }

                    }
                    else if (_Data.Count > 0)
                    {
                        _Data[0].Price = amount;        //set total amount to one fund..
                    }
                }

                /*Set the Static Data*/
                Type = type;
                Year = targetYear;
               // Percent = targetPercent;
                Amount = amount;
                ViewBag.Type = type;
                /*Set the Static Data*/

                return View(_Data);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        


        [HttpPost]
        public ActionResult openModelPopupForMultipleBuy(string AlluniqueNumber, string AllPrices)
        {
            List<MutualFundDetails> Listmf = new List<MutualFundDetails>();
            int j = 0;
            if (AlluniqueNumber != "" && AllPrices != "")
            {
                MutualFundDetails mf = new MutualFundDetails();
                FolioInfo allFolioData = new FolioInfo();
                DashboardCode pageObj = new DashboardCode();
                string[] unqNumber = new string[3];
                string[] price = new string[3];
                if (AlluniqueNumber.Contains(','))
                {
                    unqNumber = AlluniqueNumber.Split(',');
                }
                else
                {
                    unqNumber[0] = AlluniqueNumber;
                }
                if (AllPrices.Contains(','))
                {
                    price = AllPrices.Split(',');
                }
                else
                {
                    price[0] = AllPrices;
                }

                //if (AlluniqueNumber.Contains(',') && AllPrices.Contains(','))
                //{
                    for (int i = 0; i < unqNumber.Length; i++)
                    {
                        string getSchemeInformation = TM.getFundinfobyUniqueNumber(Convert.ToInt64(unqNumber[i]), User.Identity.Name.Split('|')[2]);
                        if (getSchemeInformation != "")
                        {
                            mf = JsonConvert.DeserializeObject<List<MutualFundDetails>>(getSchemeInformation)[0];
                            string FolioNumber = pageObj.getfolionumberfromOrdertablebyAmc(mf.AMCCode, User.Identity.Name.Split('|')[2]);
                            string sipDates = TM.getSipDatebySchemecode(mf.SchemeCode);
                            mf.SipDates = sipDates;
                            if (FolioNumber != "")
                            {
                                allFolioData = JsonConvert.DeserializeObject<List<FolioInfo>>(FolioNumber)[0];
                            }
                            else
                            {
                                allFolioData.FolioNo = string.Empty;
                            }
                            mf.FolioNo = allFolioData.FolioNo;

                            if (mf != null)
                            {
                                Listmf.Add(mf);
                            }
                        }
                    }
                    for (int i = 0; i < price.Length; i++)
                    {
                        while (j < Listmf.Count)
                        {
                            Listmf[j].MaximumPurchaseAmount = price[i];
                            j++;
                            break;
                        }
                    }
                //}
                //else
                //{
                //    string getSchemeInformation = TM.getFundinfobyUniqueNumber(Convert.ToInt64(AlluniqueNumber), User.Identity.Name.Split('|')[2]);
                //    if (getSchemeInformation != "")
                //    {
                //        mf = JsonConvert.DeserializeObject<List<MutualFundDetails>>(getSchemeInformation)[0];
                //        if (mf != null)
                //        {
                //            Listmf.Add(mf);
                //        }
                //    }
                //    Listmf[0].MaximumPurchaseAmount = AllPrices;
                //}
            }
            return PartialView("~/Views/Advisory/_partialModalBuymultipleMutualfund.cshtml", Listmf);
        }

        [HttpPost]
        public JsonResult BuyMultipleMutualFundTest(string jsonData)
        {
            ResultInfo<string> ret = new ResultInfo<string>();
            string resApi = "";
            MultipleBuyMutualfund buyFund = new MultipleBuyMutualfund();
            orderEntryParam orderData = new orderEntryParam();
            string[] schemecode = new string[3];
            string[] prices = new string[3];
            Api_Req postReq = new Api_Req();
            if (jsonData != "")
            {
                buyFund = JsonConvert.DeserializeObject<MultipleBuyMutualfund>(jsonData);
            }
            if (buyFund.schemecode.Contains(','))
            {
                schemecode = buyFund.schemecode.Split(',');
            }
            if (buyFund.price.Contains(','))
            {
                prices = buyFund.price.Split(',');
            }

            for (int i = 0; i < schemecode.Length; i++)
            {
                orderData.ClientCode = User.Identity.Name.Split('|')[2];
                orderData.OrderVal = prices[i];
                orderData.SchemeCd = schemecode[i];
                if (i == 0)
                {
                    orderData.FolioNo = buyFund.Folio1;
                }
                else if (i == 1)
                {
                    orderData.FolioNo = buyFund.Folio2;
                }
                else if (i == 2)
                {
                    orderData.FolioNo = buyFund.Folio3;
                }

                string JData = JsonConvert.SerializeObject(orderData);
                string BaseApi = WebConfigurationManager.AppSettings["apiUrl"];
                //string BaseApi = "http://localhost:36188/";
                if (orderData.FolioNo != "")
                {
                    resApi = postReq.Postrequest(BaseApi + "Order/AdditionalOrder", JData);
                }
                else if (orderData.FolioNo == "")
                {
                    resApi = postReq.Postrequest(BaseApi + "Order/NormalOrder", JData);
                }
                if (resApi != "")
                {
                    ret = JsonConvert.DeserializeObject<ResultInfo<string>>(resApi);
                }
            }
            return Json(ret.Description, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult BuyMultipleSipfund(string jsonData)
        //{
        //    ResultInfo<string> ret = new ResultInfo<string>();
        //    string resApi = "";
        //    MultipleBuySip buyFund = new MultipleBuySip();

        //    DashboardCode pageobj = new DashboardCode();
        //    OnboardDatainfo allBankdata = new OnboardDatainfo();
        //    List<SiporderData> objListOrderData = new List<SiporderData>();
        //    string[] schemecode = new string[3];
        //    string[] prices = new string[3];
        //    string[] frequency = new string[3];
        //    string[] sipDates = new string[3];
        //    Api_Req postReq = new Api_Req();
        //    if (jsonData != "")
        //    {
        //        buyFund = JsonConvert.DeserializeObject<MultipleBuySip>(jsonData);
        //    }
        //    if (buyFund.schemecode.Contains(','))
        //    {
        //        schemecode = buyFund.schemecode.Split(',');
        //    }
        //    else
        //    {
        //        schemecode[0] = buyFund.schemecode;
        //    }
        //    if (buyFund.price.Contains(','))
        //    {
        //        prices = buyFund.price.Split(',');
        //    }
        //    else
        //    {
        //        prices[0] = buyFund.price;
        //    }
        //    if (buyFund.Frequency.Contains(','))
        //    {
        //        frequency = buyFund.Frequency.Split(',');
        //    }
        //    else
        //    {
        //        frequency[0] = buyFund.Frequency;
        //    }
        //    if (buyFund.SipDates.Contains(','))
        //    {
        //        sipDates = buyFund.SipDates.Split(',');
        //    }
        //    else
        //    {
        //        sipDates[0] = buyFund.SipDates;
        //    }
        //    for (int i = 0; i < schemecode.Length; i++)
        //    {
        //        SiporderData orderData = new SiporderData();
        //        string Amcocde = string.Empty;
        //        //  string date = TM.getSipDatebySchemecode(schemecode[i]);
        //        //string[] dates = date.Split(',');
        //        string TodayDate = DateTime.Now.ToString("dd/MM/yyyy");
        //        string[] todayDate = TodayDate.Split('/');
        //        todayDate[0] = sipDates[i];
        //        string FinalDate = String.Join("/", todayDate);
        //        if (i == 0)
        //        {
        //            orderData.NoOfInstallment = buyFund.noOfinstallment1;
        //            orderData.FolioNo = buyFund.Folio1;
        //        }
        //        else if (i == 1)
        //        {
        //            orderData.NoOfInstallment = buyFund.noOfinstallment2;
        //            orderData.FolioNo = buyFund.Folio2;
        //        }
        //        else if (i == 2)
        //        {
        //            orderData.NoOfInstallment = buyFund.noOfinstallment3;
        //            orderData.FolioNo = buyFund.Folio3;
        //        }
        //        orderData.InstallmentAmount = prices[i];
        //        orderData.SchemeCode = schemecode[i];
        //        orderData.FrequencyType = frequency[i];
        //        orderData.StartDate = sipDates[i];
        //        orderData.ClientCode = User.Identity.Name.Split('|')[2];
        //        string bankReply = pageobj.getonbaordBankdataUsingemail(User.Identity.Name.Split('|')[0]);
        //        if (bankReply != "")
        //        {
        //            allBankdata = JsonConvert.DeserializeObject<List<OnboardDatainfo>>(bankReply)[0];
        //        }
        //        orderData.IFSC = allBankdata.IfscCode;
        //        orderData.BankacNo = allBankdata.BankAcntNumber;
        //        orderData.BankName = allBankdata.BankName;
        //        orderData.PaymentMode = "yes";
        //        if (orderData.SchemeCode != "")
        //        {
        //            Amcocde = pageobj.getamcCodeBySchemecode(schemecode[i]);
        //        }
        //        //1st Check with the Amc if it's Isip or Not
        //        if (pageobj.IsAmcIsaISIP(Amcocde.Trim()))
        //        {
        //            //Then check with the Bank if it's Isip or Not
        //            if (pageobj.IsBankIsaISIP(orderData.IFSC.Trim()))
        //            {
        //                orderData.Param2 = User.Identity.Name.Split('|')[3].Split('-')[0];
        //                orderData.MandateID = string.Empty;
        //            }
        //            else
        //            {
        //                orderData.Param2 = string.Empty;
        //                orderData.MandateID = User.Identity.Name.Split('|')[3].Split('-')[1];
        //            }
        //        }
        //        else
        //        {
        //            orderData.Param2 = string.Empty;
        //            orderData.MandateID = User.Identity.Name.Split('|')[3].Split('-')[1];
        //        }
        //        objListOrderData.Add(orderData);
        //    }

        //    string JData = JsonConvert.SerializeObject(objListOrderData);
        //    string BaseApi = WebConfigurationManager.AppSettings["apiUrl"];
        //    resApi = postReq.Postrequest(BaseApi + "MultipleOrder/SipOrder", JData);
        //    if (resApi != "")
        //    {
        //        ret = JsonConvert.DeserializeObject<ResultInfo<string>>(resApi);
        //    }
        //    if (ret.IsSuccess)
        //    {
        //        _renderHtml = ret.Info;

        //    }
        //    return Json(ret, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult BuyMultipleMutualFund(string jsonData)
        {
            ResultInfo<string> ret = new ResultInfo<string>();
            string resApi = "";
            //MultipleBuySip buyFund = new MultipleBuySip();
            MultipleBuyMutualfund buyFund = new MultipleBuyMutualfund();

            DashboardCode pageobj = new DashboardCode();
            OnboardDatainfo allBankdata = new OnboardDatainfo();
            List<SiporderData> objListOrderData = new List<SiporderData>();
            string[] schemecode = new string[3];
            string[] prices = new string[3];
            string[] frequency = new string[3];
            string[] sipDates = new string[3];
            Api_Req postReq = new Api_Req();
            if (jsonData != "")
            {
                buyFund = JsonConvert.DeserializeObject<MultipleBuyMutualfund>(jsonData);
            }
            if (buyFund.schemecode.Contains(','))
            {
                schemecode = buyFund.schemecode.Split(',');
            }
            else
            {
                schemecode[0] = buyFund.schemecode;
            }
            if (buyFund.price.Contains(','))
            {
                prices = buyFund.price.Split(',');
            }
            else
            {
                prices[0] = buyFund.price;
            }
            if (buyFund.Frequency.Contains(','))
            {
                frequency = buyFund.Frequency.Split(',');
            }
            else
            {
                frequency[0] = buyFund.Frequency;
            }
            if (buyFund.SipDates.Contains(','))
            {
                sipDates = buyFund.SipDates.Split(',');
            }
            else
            {
                sipDates[0] = buyFund.SipDates;
            }
            for (int i = 0; i < schemecode.Length; i++)
            {
                SiporderData orderData = new SiporderData();
                string Amcocde = string.Empty;
                //  string date = TM.getSipDatebySchemecode(schemecode[i]);
                //string[] dates = date.Split(',');
                string TodayDate = DateTime.Now.ToString("dd/MM/yyyy");
                string[] todayDate = TodayDate.Split('/');
                todayDate[0] = sipDates[i];
                string FinalDate = String.Join("/", todayDate);
                if (i == 0)
                {
                    orderData.NoOfInstallment = buyFund.noOfinstallment1;
                    orderData.FolioNo = buyFund.Folio1;
                }
                else if (i == 1)
                {
                    orderData.NoOfInstallment = buyFund.noOfinstallment2;
                    orderData.FolioNo = buyFund.Folio2;
                }
                else if (i == 2)
                {
                    orderData.NoOfInstallment = buyFund.noOfinstallment3;
                    orderData.FolioNo = buyFund.Folio3;
                }
                orderData.InstallmentAmount = prices[i];
                orderData.SchemeCode = schemecode[i];
                orderData.FrequencyType = frequency[i];
                orderData.StartDate = sipDates[i];
                orderData.ClientCode = User.Identity.Name.Split('|')[2];
                string bankReply = pageobj.getonbaordBankdataUsingemail(User.Identity.Name.Split('|')[0]);
                if (bankReply != "")
                {
                    allBankdata = JsonConvert.DeserializeObject<List<OnboardDatainfo>>(bankReply)[0];
                }
                orderData.IFSC = allBankdata.IfscCode;
                orderData.BankacNo = allBankdata.BankAcntNumber;
                orderData.BankName = allBankdata.BankName;
                orderData.PaymentMode = "yes";
                if (orderData.SchemeCode != "")
                {
                    Amcocde = pageobj.getamcCodeBySchemecode(schemecode[i]);
                }
                //1st Check with the Amc if it's Isip or Not
                if (pageobj.IsAmcIsaISIP(Amcocde.Trim()))
                {
                    //Then check with the Bank if it's Isip or Not
                    if (pageobj.IsBankIsaISIP(orderData.IFSC.Trim()))
                    {
                        orderData.Param2 = User.Identity.Name.Split('|')[3].Split('-')[0];
                        orderData.MandateID = string.Empty;
                    }
                    else
                    {
                        orderData.Param2 = string.Empty;
                        orderData.MandateID = User.Identity.Name.Split('|')[3].Split('-')[1];
                    }
                }
                else
                {
                    orderData.Param2 = string.Empty;
                    orderData.MandateID = User.Identity.Name.Split('|')[3].Split('-')[1];
                }
                objListOrderData.Add(orderData);
            }

            string JData = JsonConvert.SerializeObject(objListOrderData);
            string BaseApi = WebConfigurationManager.AppSettings["apiUrl"];
            resApi = postReq.Postrequest(BaseApi + "MultipleOrder/SipOrder", JData);
            if (resApi != "")
            {
                ret = JsonConvert.DeserializeObject<ResultInfo<string>>(resApi);
            }
            if (ret.IsSuccess)
            {
                _renderHtml = ret.Info;

            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}
