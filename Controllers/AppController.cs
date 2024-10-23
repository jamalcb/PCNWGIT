using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcCodeFlowClientManual.Models.Quickbook;
using Newtonsoft.Json;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace PCNW.Controllers
{
    public class AppController : BaseController
    {
        public string? clientid = null;
        public string? clientsecret = null;
        public string? redirectUrl = null;
        public string? environment = null;
        public string? CompanyId = null;


        //public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);
        private readonly ILogger<AppController> _logger;
        private readonly IConfiguration _configuration;
        private readonly OAuth2Client auth2Client1;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly OAuth2Client _auth2Client;
        private readonly ICommonRepository _commonRepository;
        private readonly ApplicationDbContext _dbContext;
        public AppController(ILogger<AppController> logger, IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ICommonRepository commonRepository, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _commonRepository = commonRepository;
            QuickbookInfo quickbookInfo = _configuration.GetValue<QuickbookInfo>("QuickbookInfo");
            //clientid = quickbookInfo.clientid;
            //clientsecret = quickbookInfo.clientsecret;
            //redirectUrl = quickbookInfo.redirectUrl;
            //environment = quickbookInfo.environment;
            var clientId = _configuration.GetSection("AppSettings")["ClientId"];
            var clientSecret = _configuration.GetSection("AppSettings")["ClientSecret"];
            var redirectUrl = _configuration.GetSection("AppSettings")["RedirectUrl"];
            CompanyId = _configuration.GetSection("AppSettings")["PaymentCompanyId"];


            _auth2Client = new OAuth2Client(clientId, clientSecret, redirectUrl, "sandbox");

        }

        /// <summary>
        /// Use the Index page of App controller to get all endpoints from discovery url
        /// </summary>
        public ActionResult Index()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //HttpContext.GetOwinContext().Authentication.SignOut("Cookies");

            // _signInManager.SignOutAsync();
            int toDel = Convert.ToInt32(TempData["Model"]);
            string PayModeRef = TempData["PayModeRef"].ToString();
            TempData["Model"] = toDel;
            TempData["PayModeRef"] = PayModeRef;
            return View();
        }

        /// <summary>
        /// Start Auth flow
        /// </summary>
        public async Task<ActionResult> InitiateAuth()
        {

            int toDel = Convert.ToInt32(TempData["Model"]);
            TempData["Model"] = toDel;
            List<OidcScopes> scopes = new()
                    {
                        OidcScopes.Accounting,
                        OidcScopes.Payment
                    };
            // _signInManager.SignInAsync(new IdentityUser("codingbrains33@gmail.com"), true);
            UserInfoResponse info = await _auth2Client.GetUserInfoAsync("eyJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiYWxnIjoiZGlyIn0..SMODk49fZIXY91s_qH3aDg.-Vr5GnyvUAqJA-CjFMPgY3dhjPHYUKledddf-q2m3Rs-3-U-_zXD5_mbFDIVHvCNhJFnrHN3vH6q49UCE9fu-xgYo6ASN9ub6RhvfAlxFoo8i6A6XQKrmj_gK7MyIlr73sHOVU5gCW8RPtrpEkfx0CqIQpEtCWF1_4QePn_cQPJCjUHTYYbh9F-f0Oy9l5P8BK9R9__8grXFq6cG29s2pUiVwzETU6-pAyZjnkGcfD6hzLYvTrFURacMm4JS74ueyp-rdVgtNV31AF1CkhCa33OLFwKzKW9Jh3eQGX2BJEN4FkZot1Qq8fVXBXMvvJ7o4ZFOmFqkz5EB6XhBnVBf-WSqBhOB7mISG12czrtB4er5VWWzeVZBMLFjZDHn7IzZvXWKub9us6Vkm3vnpodX5NkH7j8jlG4ONrqjWUsi4O4kF2dOsOZi_VFWgjJgfw_isss2CnIrsJXIWllLI4BGzTK_99SEJ_Qjvp_IsntJ2b_hwgUTKwWPesxnh1tQ7D_zRIuno20CEYMzsrGQm9NgJGLS4NSQpYZ5wjZkT1qsrZf7JUi8XvepuIoGfV7NMG4rOST1ftc4zMEyHl-3lzKOGcI4cfWEvcvYKjTw0IUmYNSf79f6vMt2MisqR6kD2Ik1flZ5wMtisroejJ2sf9hjpLKmk_M_nHXQWt2n52DL7LdH2wpyomdn7LOdtOCRmzXCp21xZDSGV_M7iHblT01haQMSjbLnn4ky6qmxZSy6tniRL0s-OQOB3Rd_HIyc3mcX.9boxf2NIBdsQ4rsrQXHjkw");
            string authorizeUrl = _auth2Client.GetAuthorizationURL(scopes);
            return Redirect(authorizeUrl);
        }
        /// <summary>
        /// Copy center Payment
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pathChk"></param>
        /// <param name="ContPath"></param>
        /// <param name="ActPath"></param>
        /// <param name="SuccessPath"></param>
        /// <returns></returns>

        public JsonResult InitiateAuthCopyCenter(OrderTables model, string pathChk, string ContPath, string ActPath, string SuccessPath)
        {
            if (!string.IsNullOrEmpty(pathChk))
            {
                Response.Cookies.Append("pathChk", pathChk, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            if (model != null)
            {

                string modelString = JsonConvert.SerializeObject(model);
                Response.Cookies.Append("copyCenterModel", modelString, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });

                if (Convert.ToBoolean(model.NonMember))
                {
                    Response.Cookies.Append("PayType", "CopyCenterPaymentNonMember", new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }
                else
                {
                    if (model.PaymentMode == "CC")
                    {
                        Response.Cookies.Append("PayType", "CopyCenterPaymentMember", new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            HttpOnly = false,
                            Secure = Convert.ToBoolean(false)
                        });
                    }
                    else if (model.PaymentMode == "Invoice")
                    {
                        Response.Cookies.Append("PayType", "CopyCenterInvoice", new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            HttpOnly = false,
                            Secure = Convert.ToBoolean(false)
                        });
                    }
                }
            }
            if (!string.IsNullOrEmpty(ContPath))
            {
                Response.Cookies.Append("ContPath", ContPath, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            if (!string.IsNullOrEmpty(ActPath))
            {
                Response.Cookies.Append("ActPath", ActPath, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            if (!string.IsNullOrEmpty(SuccessPath))
            {
                Response.Cookies.Append("SuccessPath", SuccessPath, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            //TempData["CopyModel"] = model; 
            //TempData["PathChk"]= pathChk;
            List<OidcScopes> scopes = new()
                    {
                        OidcScopes.Accounting,
                        OidcScopes.Payment
                    };
            string authorizeUrl = _auth2Client.GetAuthorizationURL(scopes);
            return Json(new { Status = "success", Data = authorizeUrl });
        }


        /// <summary>
        /// Renewal Payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult InitiateAuthRenewal(MemberShipRegistration model)
        {
            if (model != null)
            {

                string modelString = JsonConvert.SerializeObject(model);
                Response.Cookies.Append("renewalpayment", modelString, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
                Response.Cookies.Append("PayFor", "ProfileRenewalPayment", new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            List<OidcScopes> scopes = new()
                    {
                        OidcScopes.Accounting,
                        OidcScopes.Payment
                    };
            string authorizeUrl = _auth2Client.GetAuthorizationURL(scopes);
            return Json(new { Status = "success", Data = authorizeUrl });
        }
        /// <summary>
        /// QBO API Request
        /// </summary>
        public async Task<ActionResult> ApiCallServiceAsync(string Token)
        {
            string PayType = Request.Cookies["PayType"] != null ? Request.Cookies["PayType"].ToString() : "";
            string pathChk = Request.Cookies["pathChk"] != null ? Request.Cookies["pathChk"].ToString() : "";
            string controllername = Request.Cookies["ContPath"] != null ? Request.Cookies["ContPath"].ToString() : "";
            string SuccessPath = Request.Cookies["SuccessPath"] != null ? Request.Cookies["SuccessPath"].ToString() : "";
            string ActPath = Request.Cookies["ActPath"] != null ? Request.Cookies["ActPath"].ToString() : "";
            string PayFor = Request.Cookies["PayFor"] != null ? Request.Cookies["PayFor"].ToString() : "";
            string RenewalPay = Request.Cookies["renewalpayment"] != null ? Request.Cookies["renewalpayment"].ToString() : "";
            if (PayType == "CopyCenterPaymentNonMember" && Request.Cookies["copyCenterModel"] != null && !string.IsNullOrEmpty(pathChk))
            {
                if (Request.Cookies["copyCenterModel"] != null)
                {
                    string copyModel = Request.Cookies["copyCenterModel"].ToString();
                    OrderTables CopyModel = JsonConvert.DeserializeObject<OrderTables>(copyModel);
                    try
                    {
                        if (HttpContext.Session.GetString("realmId") != null)
                        {
                            //string? realmId = "4620816365247983250";
                            string? realmId = "9130355305808056";
                            Token = Token.Replace("access_token: ", "");
                            try
                            {
                                var obj = new JsonObject();
                                obj.Add("amount", CopyModel.ShipAmt);
                                var cardObj = new JsonObject();
                                cardObj.Add("expYear", "2025");
                                cardObj.Add("expMonth", "02");
                                cardObj.Add("name", "Anubhav");
                                cardObj.Add("cvc", "123");
                                cardObj.Add("number", "4111111111155555");
                                obj.Add("card", cardObj);
                                obj.Add("currency", "USD");
                                var contextObj = new JsonObject();
                                contextObj.Add("mobile", "false");
                                contextObj.Add("isEcommerce", "true");
                                obj.Add("context", contextObj);
                                HttpContent content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
                                try
                                {
                                    var client = new HttpClient();

                                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                                    client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                    client.DefaultRequestHeaders.Add("Request-Id", Guid.NewGuid().ToString());
                                    System.Net.Http.HttpResponseMessage response = await client.PostAsync("https://sandbox.api.intuit.com/quickbooks/v4/payments/charges", content);
                                    string responseConetent = await response.Content.ReadAsStringAsync();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        responseConetent = await response.Content.ReadAsStringAsync();
                                        var DesResContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                        string ChargeId = DesResContent["id"].ToString();
                                        HttpContent retCharge = new StringContent("");
                                        System.Net.Http.HttpResponseMessage Itemresponse = await client.GetAsync("https://sandbox.api.intuit.com/quickbooks/v4/payments/charges/" + ChargeId);
                                        responseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                        if (Itemresponse.IsSuccessStatusCode)
                                        {
                                            Response.Cookies.Append("GetCopyModel", copyModel, new Microsoft.AspNetCore.Http.CookieOptions
                                            {
                                                HttpOnly = false,
                                                Secure = Convert.ToBoolean(false)
                                            });
                                            return RedirectToAction(SuccessPath, controllername);
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();
                                            if (ActPath == "Preview")
                                            {
                                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                            else
                                            {
                                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                        }
                                    }

                                    else
                                    {
                                        //MemberShipRegistration member = new();
                                        if (ActPath == "Preview")
                                        {
                                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                        else
                                        {
                                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return View("ApiCallService", (object)"QBO API call Failed!");
                                }
                            }
                            catch (Exception ex)
                            {
                                return View("ApiCallService", "QBO API call Failed!" + " Error message: " + ex.Message);
                            }
                        }
                        // }
                        else
                        { return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action"); }
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogWarning(Ex.Message);
                        return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action");
                    }
                }
                else
                {
                    return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action");
                }
            }
            else if (PayType == "CopyCenterPaymentMember" && Request.Cookies["copyCenterModel"] != null && !string.IsNullOrEmpty(pathChk))
            {
                string copyModel = Request.Cookies["copyCenterModel"].ToString();
                OrderTables CopyModel = JsonConvert.DeserializeObject<OrderTables>(copyModel);
                MemberShipRegistration member = new();
                member = await _commonRepository.GetMemberAsync(Convert.ToInt32(CopyModel.OrderChrgId));
                try
                {
                    if (HttpContext.Session.GetString("realmId") != null)
                    {
                        //string? realmId = "4620816365247983250";
                        string? realmId = "9130355305808056";
                        Token = Token.Replace("access_token: ", "");
                        if (member != null)
                        {
                            if (member.MemID == 0)
                            {

                                try
                                {
                                    string result = "";
                                    var obj = new JsonObject();
                                    obj.Add("FullyQualifiedName", member.Company);
                                    var obj1 = new JsonObject();
                                    obj1.Add("Address", member.ContactEmail);
                                    obj.Add("PrimaryEmailAddr", obj1);
                                    obj.Add("DisplayName", member.Company + "(" + member.FirstName + ")");
                                    obj.Add("Notes", "Order place on." + DateTime.Now.ToString("MM/dd/yyyy"));
                                    obj.Add("FamilyName", member.LastName);
                                    var obj2 = new JsonObject();
                                    obj2.Add("FreeFormNumber", member.CompanyPhone);
                                    obj.Add("PrimaryPhone", obj2);
                                    obj.Add("CompanyName", member.Company);
                                    var obj3 = new JsonObject();
                                    obj3.Add("CountrySubDivisionCode", member.BillState);
                                    obj3.Add("City", member.BillCity);
                                    obj3.Add("PostalCode", member.BillZip);
                                    obj3.Add("Line1", member.BillAddress);
                                    obj3.Add("Country", "USA");
                                    obj.Add("BillAddr", obj3);
                                    obj.Add("GivenName", member.FirstName);
                                    var ItemObj = new JsonObject();
                                    ItemObj.Add("Name", "Customer Registered and copycenter - " + Guid.NewGuid().ToString());
                                    var Itemobj2 = new JsonObject();
                                    Itemobj2.Add("value", "1");
                                    Itemobj2.Add("name", "Registration");
                                    ItemObj.Add("IncomeAccountRef", Itemobj2);
                                    ItemObj.Add("Type", "Service");
                                    HttpContent content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
                                    HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                    try
                                    {
                                        var client = new HttpClient();

                                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                                        client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                        System.Net.Http.HttpResponseMessage response = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/customer?minorversion=65", content);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            string responseConetent = await response.Content.ReadAsStringAsync();
                                            var DesResContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                            string CompId = DesResContent["Customer"]["Id"].ToString();
                                            await _commonRepository.UpdateMemberMemId(member.ID, Convert.ToInt32(CompId));
                                            member.MemID = Convert.ToInt32(CompId);
                                            System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                            if (Itemresponse.IsSuccessStatusCode)
                                            {
                                                string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                                var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                                string ItemName = DesItemContent["Item"]["Name"].ToString();
                                                string ItemId = DesItemContent["Item"]["Id"].ToString();
                                                //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                                // }
                                                QuickBookInvoice payload = new QuickBookInvoice();
                                                List<SaleInfo> saleList = new List<SaleInfo>();
                                                SaleInfo saleInfo = new SaleInfo();
                                                SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                                ItemRefClass itemRefClass = new ItemRefClass();
                                                itemRefClass.name = ItemName;
                                                itemRefClass.value = ItemId;

                                                salesItemLineDetailClass.ItemRef = itemRefClass;
                                                saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                                saleInfo.Amount = Convert.ToDecimal(CopyModel.ShipAmt);
                                                saleInfo.DetailType = "SalesItemLineDetail";
                                                saleList.Add(saleInfo);

                                                payload.Line = saleList;

                                                CustomerInfo customerInfo = new CustomerInfo();
                                                customerInfo.value = CompId;

                                                payload.CustomerRef = customerInfo;
                                                string Invoice = JsonConvert.SerializeObject(payload);
                                                HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                                System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                                if (Invresponse.IsSuccessStatusCode)
                                                {
                                                    string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                    var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                    string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                    member.InvoiceId = Convert.ToInt32(InvId);
                                                    decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                    QuickBookPayment payment = new QuickBookPayment();
                                                    CustomerRefInfo info = new CustomerRefInfo();
                                                    info.value = CompId;
                                                    payment.CustomerRef = info;
                                                    payment.TotalAmt = Amount;
                                                    List<LineInfo> Line = new List<LineInfo>();
                                                    LineInfo li = new LineInfo();
                                                    li.Amount = Amount;
                                                    List<TxnInfo> Txns = new List<TxnInfo>();
                                                    TxnInfo Ti = new TxnInfo();
                                                    Ti.TxnId = InvId;
                                                    Ti.TxnType = "Invoice";
                                                    Txns.Add(Ti);
                                                    li.LinkedTxn = Txns;
                                                    Line.Add(li);
                                                    payment.Line = Line;
                                                    string payInfo = JsonConvert.SerializeObject(payment);
                                                    //var PayObj = new JsonObject();
                                                    //var PayObj1 = new JsonObject();
                                                    //PayObj1.Add("value", CompId);
                                                    //PayObj.Add("CustomerRef", PayObj1);
                                                    //PayObj.Add("TotalAmt", Amount);
                                                    //var PayObj2 = new JsonObject();
                                                    //PayObj2.Add("Amount", Amount);
                                                    //var PayObj3 = new JsonObject();
                                                    //PayObj3.Add("TxnId", InvId);
                                                    //PayObj3.Add("TxnType", "Invoice");
                                                    //PayObj2.Add("LinkedTxn", PayObj3);
                                                    //PayObj.Add("Line", PayObj2);
                                                    HttpContent Paycontent = new StringContent(payInfo, Encoding.UTF8, "application/json");
                                                    System.Net.Http.HttpResponseMessage Payresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/payment?minorversion=65", Paycontent);
                                                    if (Payresponse.IsSuccessStatusCode)
                                                    {
                                                        string CopyCenterModels = JsonConvert.SerializeObject(copyModel);
                                                        Response.Cookies.Append("GetCopyModel", CopyCenterModels, new Microsoft.AspNetCore.Http.CookieOptions
                                                        {
                                                            HttpOnly = false,
                                                            Secure = Convert.ToBoolean(false)
                                                        });
                                                        //string ChkPath = JsonConvert.SerializeObject(pathChk);
                                                        //Response.Cookies.Append("GetChkPath", ChkPath, new Microsoft.AspNetCore.Http.CookieOptions
                                                        //{
                                                        //    HttpOnly = false,
                                                        //    Secure = Convert.ToBoolean(false)
                                                        //});

                                                        //MemberShipRegistration member = new();
                                                        return RedirectToAction(SuccessPath, controllername);
                                                    }
                                                    else
                                                    {
                                                        //MemberShipRegistration member = new();
                                                        if (ActPath == "Preview")
                                                        {
                                                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                        }
                                                        else
                                                        {
                                                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //MemberShipRegistration member = new();
                                                    if (ActPath == "Preview")
                                                    {
                                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                    }
                                                    else
                                                    {
                                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //MemberShipRegistration member = new();
                                                if (ActPath == "Preview")
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                                else
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();

                                            if (ActPath == "Preview")
                                            {
                                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                            else
                                            {
                                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ActPath == "Preview")
                                        {
                                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                        else
                                        {
                                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ActPath == "Preview")
                                    {
                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                    else
                                    {
                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                }

                            }
                            else
                            {
                                try
                                {
                                    var ItemObj = new JsonObject();
                                    ItemObj.Add("Name", "Copycenter - " + Guid.NewGuid().ToString());
                                    var Itemobj2 = new JsonObject();
                                    Itemobj2.Add("value", "1");
                                    Itemobj2.Add("name", "Registration");
                                    ItemObj.Add("IncomeAccountRef", Itemobj2);
                                    ItemObj.Add("Type", "Service");
                                    HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                    try
                                    {
                                        var client = new HttpClient();

                                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                                        client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                                        string CompId = member.MemID.ToString();

                                        member.MemID = Convert.ToInt32(CompId);
                                        System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                        if (Itemresponse.IsSuccessStatusCode)
                                        {
                                            string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                            var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                            string ItemName = DesItemContent["Item"]["Name"].ToString();
                                            string ItemId = DesItemContent["Item"]["Id"].ToString();
                                            //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                            // }
                                            QuickBookInvoice payload = new QuickBookInvoice();
                                            List<SaleInfo> saleList = new List<SaleInfo>();
                                            SaleInfo saleInfo = new SaleInfo();
                                            SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                            ItemRefClass itemRefClass = new ItemRefClass();
                                            itemRefClass.name = ItemName;
                                            itemRefClass.value = ItemId;

                                            salesItemLineDetailClass.ItemRef = itemRefClass;
                                            saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                            saleInfo.Amount = Convert.ToDecimal(CopyModel.ShipAmt);
                                            saleInfo.DetailType = "SalesItemLineDetail";
                                            saleList.Add(saleInfo);

                                            payload.Line = saleList;

                                            CustomerInfo customerInfo = new CustomerInfo();
                                            customerInfo.value = CompId;

                                            payload.CustomerRef = customerInfo;
                                            string Invoice = JsonConvert.SerializeObject(payload);
                                            HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                            System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                            if (Invresponse.IsSuccessStatusCode)
                                            {
                                                string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                member.InvoiceId = Convert.ToInt32(InvId);
                                                decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                QuickBookPayment payment = new QuickBookPayment();
                                                CustomerRefInfo info = new CustomerRefInfo();
                                                info.value = CompId;
                                                payment.CustomerRef = info;
                                                payment.TotalAmt = Amount;
                                                List<LineInfo> Line = new List<LineInfo>();
                                                LineInfo li = new LineInfo();
                                                li.Amount = Amount;
                                                List<TxnInfo> Txns = new List<TxnInfo>();
                                                TxnInfo Ti = new TxnInfo();
                                                Ti.TxnId = InvId;
                                                Ti.TxnType = "Invoice";
                                                Txns.Add(Ti);
                                                li.LinkedTxn = Txns;
                                                Line.Add(li);
                                                payment.Line = Line;
                                                string payInfo = JsonConvert.SerializeObject(payment);
                                                HttpContent Paycontent = new StringContent(payInfo, Encoding.UTF8, "application/json");
                                                System.Net.Http.HttpResponseMessage Payresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/payment?minorversion=65", Paycontent);
                                                if (Payresponse.IsSuccessStatusCode)
                                                {
                                                    //string CopyCenterModels = JsonConvert.SerializeObject(copyModel);
                                                    Response.Cookies.Append("GetCopyModel", copyModel, new Microsoft.AspNetCore.Http.CookieOptions
                                                    {
                                                        HttpOnly = false,
                                                        Secure = Convert.ToBoolean(false)
                                                    });
                                                    // string ChkPath = JsonConvert.SerializeObject(pathChk);
                                                    //Response.Cookies.Append("GetChkPath", ChkPath, new Microsoft.AspNetCore.Http.CookieOptions
                                                    //{
                                                    //    HttpOnly = false,
                                                    //    Secure = Convert.ToBoolean(false)
                                                    //});

                                                    //MemberShipRegistration member = new();
                                                    return RedirectToAction(SuccessPath, controllername);

                                                }
                                                else
                                                {
                                                    //MemberShipRegistration member = new();
                                                    if (ActPath == "Preview")
                                                    {
                                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                    }
                                                    else
                                                    {
                                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //MemberShipRegistration member = new();
                                                if (ActPath == "Preview")
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                                else
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();
                                            if (ActPath == "Preview")
                                            {
                                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                            else
                                            {
                                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                        }
                                    }


                                    catch (Exception ex)
                                    {
                                        if (ActPath == "Preview")
                                        {
                                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                        else
                                        {
                                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ActPath == "Preview")
                                    {
                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                    else
                                    {
                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ActPath == "Preview")
                            {
                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                            }
                            else
                            {
                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                            }
                        }
                    }
                    else
                    {
                        if (ActPath == "Preview")
                        {
                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                        }
                        else
                        {
                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                        }
                    }
                }
                catch (Exception Ex)
                {

                    if (ActPath == "Preview")
                    {
                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                    }
                    else
                    {
                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                    }
                }

            }
            else if (PayType == "CopyCenterInvoice" && Request.Cookies["copyCenterModel"] != null && !string.IsNullOrEmpty(pathChk))
            {

                string copyModel = Request.Cookies["copyCenterModel"].ToString();
                OrderTables CopyModel = JsonConvert.DeserializeObject<OrderTables>(copyModel);
                MemberShipRegistration member = new();
                member = await _commonRepository.GetMemberAsync(Convert.ToInt32(CopyModel.OrderChrgId));
                try
                {
                    if (HttpContext.Session.GetString("realmId") != null)
                    {
                        //string? realmId = "4620816365247983250";
                        string? realmId = "9130355305808056";
                        Token = Token.Replace("access_token: ", "");
                        if (member != null)
                        {
                            if (member.MemID == 0)
                            {

                                try
                                {
                                    string result = "";
                                    var obj = new JsonObject();
                                    obj.Add("FullyQualifiedName", member.Company);
                                    var obj1 = new JsonObject();
                                    obj1.Add("Address", member.ContactEmail);
                                    obj.Add("PrimaryEmailAddr", obj1);
                                    obj.Add("DisplayName", member.Company + "(" + member.FirstName + ")");
                                    obj.Add("Notes", "Order place on." + DateTime.Now.ToString("MM/dd/yyyy"));
                                    obj.Add("FamilyName", "");
                                    var obj2 = new JsonObject();
                                    obj2.Add("FreeFormNumber", member.CompanyPhone);
                                    obj.Add("PrimaryPhone", obj2);
                                    obj.Add("CompanyName", member.Company);
                                    var obj3 = new JsonObject();
                                    obj3.Add("CountrySubDivisionCode", member.BillState);
                                    obj3.Add("City", member.BillCity);
                                    obj3.Add("PostalCode", member.BillZip);
                                    obj3.Add("Line1", member.BillAddress);
                                    obj3.Add("Country", "USA");
                                    obj.Add("BillAddr", obj3);
                                    obj.Add("GivenName", member.FirstName);
                                    var ItemObj = new JsonObject();
                                    ItemObj.Add("Name", "Customer Registered and copycenter - " + Guid.NewGuid().ToString());
                                    var Itemobj2 = new JsonObject();
                                    Itemobj2.Add("value", "1");
                                    Itemobj2.Add("name", "Registration");
                                    ItemObj.Add("IncomeAccountRef", Itemobj2);
                                    ItemObj.Add("Type", "Service");
                                    HttpContent content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
                                    HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                    try
                                    {
                                        var client = new HttpClient();

                                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                                        client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                        System.Net.Http.HttpResponseMessage response = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/customer?minorversion=65", content);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            string responseConetent = await response.Content.ReadAsStringAsync();
                                            var DesResContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                            string CompId = DesResContent["Customer"]["Id"].ToString();
                                            await _commonRepository.UpdateMemberMemId(member.ID, Convert.ToInt32(CompId));
                                            member.MemID = Convert.ToInt32(CompId);
                                            System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                            if (Itemresponse.IsSuccessStatusCode)
                                            {
                                                string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                                var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                                string ItemName = DesItemContent["Item"]["Name"].ToString();
                                                string ItemId = DesItemContent["Item"]["Id"].ToString();
                                                //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                                // }
                                                QuickBookInvoice payload = new QuickBookInvoice();
                                                List<SaleInfo> saleList = new List<SaleInfo>();
                                                SaleInfo saleInfo = new SaleInfo();
                                                SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                                ItemRefClass itemRefClass = new ItemRefClass();
                                                itemRefClass.name = ItemName;
                                                itemRefClass.value = ItemId;

                                                salesItemLineDetailClass.ItemRef = itemRefClass;
                                                saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                                saleInfo.Amount = Convert.ToDecimal(CopyModel.ShipAmt);
                                                saleInfo.DetailType = "SalesItemLineDetail";
                                                saleList.Add(saleInfo);

                                                payload.Line = saleList;

                                                CustomerInfo customerInfo = new CustomerInfo();
                                                customerInfo.value = CompId;

                                                payload.CustomerRef = customerInfo;
                                                string Invoice = JsonConvert.SerializeObject(payload);
                                                HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                                System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                                if (Invresponse.IsSuccessStatusCode)
                                                {
                                                    string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                    var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                    string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                    member.InvoiceId = Convert.ToInt32(InvId);
                                                    decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                    CopyModel.PaymentMode = "Invoice: Id=" + InvId;
                                                    copyModel = JsonConvert.SerializeObject(CopyModel);

                                                    Response.Cookies.Append("GetCopyModel", copyModel, new Microsoft.AspNetCore.Http.CookieOptions
                                                    {
                                                        HttpOnly = false,
                                                        Secure = Convert.ToBoolean(false)
                                                    });
                                                    return RedirectToAction(SuccessPath, controllername);
                                                }
                                                else
                                                {
                                                    //MemberShipRegistration member = new();
                                                    if (ActPath == "Preview")
                                                    {
                                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                    }
                                                    else
                                                    {
                                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //MemberShipRegistration member = new();
                                                if (ActPath == "Preview")
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                                else
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();

                                            if (ActPath == "Preview")
                                            {
                                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                            else
                                            {
                                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ActPath == "Preview")
                                        {
                                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                        else
                                        {
                                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ActPath == "Preview")
                                    {
                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                    else
                                    {
                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                }

                            }
                            else
                            {
                                try
                                {
                                    var ItemObj = new JsonObject();
                                    ItemObj.Add("Name", "Copycenter - " + Guid.NewGuid().ToString());
                                    var Itemobj2 = new JsonObject();
                                    Itemobj2.Add("value", "1");
                                    Itemobj2.Add("name", "Registration");
                                    ItemObj.Add("IncomeAccountRef", Itemobj2);
                                    ItemObj.Add("Type", "Service");
                                    HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                    try
                                    {
                                        var client = new HttpClient();

                                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                                        client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                                        string CompId = member.MemID.ToString();

                                        member.MemID = Convert.ToInt32(CompId);
                                        System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                        if (Itemresponse.IsSuccessStatusCode)
                                        {
                                            string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                            var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                            string ItemName = DesItemContent["Item"]["Name"].ToString();
                                            string ItemId = DesItemContent["Item"]["Id"].ToString();
                                            //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                            // }
                                            QuickBookInvoice payload = new QuickBookInvoice();
                                            List<SaleInfo> saleList = new List<SaleInfo>();
                                            SaleInfo saleInfo = new SaleInfo();
                                            SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                            ItemRefClass itemRefClass = new ItemRefClass();
                                            itemRefClass.name = ItemName;
                                            itemRefClass.value = ItemId;

                                            salesItemLineDetailClass.ItemRef = itemRefClass;
                                            saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                            saleInfo.Amount = Convert.ToDecimal(CopyModel.ShipAmt);
                                            saleInfo.DetailType = "SalesItemLineDetail";
                                            saleList.Add(saleInfo);

                                            payload.Line = saleList;

                                            CustomerInfo customerInfo = new CustomerInfo();
                                            customerInfo.value = CompId;

                                            payload.CustomerRef = customerInfo;
                                            string Invoice = JsonConvert.SerializeObject(payload);
                                            HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                            System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                            if (Invresponse.IsSuccessStatusCode)
                                            {
                                                string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                member.InvoiceId = Convert.ToInt32(InvId);
                                                decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                CopyModel.PaymentMode = "Invoice: Id=" + InvId;
                                                copyModel = JsonConvert.SerializeObject(CopyModel);
                                                Response.Cookies.Append("GetCopyModel", copyModel, new Microsoft.AspNetCore.Http.CookieOptions
                                                {
                                                    HttpOnly = false,
                                                    Secure = Convert.ToBoolean(false)
                                                });
                                                return RedirectToAction(SuccessPath, controllername);
                                            }
                                            else
                                            {
                                                //MemberShipRegistration member = new();
                                                if (ActPath == "Preview")
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                                else
                                                {
                                                    return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();
                                            if (ActPath == "Preview")
                                            {
                                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                            else
                                            {
                                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                            }
                                        }
                                    }


                                    catch (Exception ex)
                                    {
                                        if (ActPath == "Preview")
                                        {
                                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                        else
                                        {
                                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ActPath == "Preview")
                                    {
                                        return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                    else
                                    {
                                        return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ActPath == "Preview")
                            {
                                return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                            }
                            else
                            {
                                return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                            }
                        }
                    }
                    else
                    {
                        if (ActPath == "Preview")
                        {
                            return RedirectToAction(ActPath, controllername, new { id = CopyModel.ProjOrderId, LoadValChk = "Issuse in retriving your charge details please try again" });
                        }
                        else
                        {
                            return RedirectToAction(ActPath, controllername, new { LoadValChk = "Issuse in retriving your charge details please try again" });
                        }
                    }

                }
                catch (Exception Ex)
                {

                    return View("ApiCallService", "QBO API call Failed!" + " Error message: " + Ex.Message);
                }


            }

            else
            {
                MemberShipRegistration member = new();
                if (PayFor == "ProfileRenewalPayment" && Request.Cookies["renewalpayment"] != null)
                {
                    if (Request.Cookies["renewalpayment"] != null)
                    {
                        string paymodal = Request.Cookies["renewalpayment"].ToString();
                        MemberShipRegistration PaymentModal = JsonConvert.DeserializeObject<MemberShipRegistration>(paymodal);
                        try
                        {
                            if (HttpContext.Session.GetString("realmId") != null)
                            {
                                string? realmId = "9130355305808056";
                                Token = Token.Replace("access_token: ", "");
                                MemberShipRegistration tblMember = await _commonRepository.GetMemberAsync(PaymentModal.ID);
                                if (tblMember.MemID == 0)
                                {
                                    try
                                    {
                                        string result = "";
                                        var obj = new JsonObject();
                                        obj.Add("FullyQualifiedName", tblMember.Company);
                                        var obj1 = new JsonObject();
                                        obj1.Add("Address", tblMember.Email);
                                        obj.Add("PrimaryEmailAddr", obj1);
                                        obj.Add("DisplayName", tblMember.Company + "(" + tblMember.FirstName + ")");
                                        obj.Add("Notes", "Registerd on." + DateTime.Now.ToString("MM/dd/yyyy"));
                                        obj.Add("FamilyName", tblMember.LastName);
                                        var obj2 = new JsonObject();
                                        obj2.Add("FreeFormNumber", tblMember.CompanyPhone);
                                        obj.Add("PrimaryPhone", obj2);
                                        obj.Add("CompanyName", tblMember.Company);
                                        var obj3 = new JsonObject();
                                        obj3.Add("CountrySubDivisionCode", tblMember.BillState);
                                        obj3.Add("City", tblMember.BillCity);
                                        obj3.Add("PostalCode", tblMember.BillZip);
                                        obj3.Add("Line1", tblMember.BillAddress);
                                        obj3.Add("Country", "USA");
                                        obj.Add("BillAddr", obj3);
                                        obj.Add("GivenName", tblMember.FirstName);
                                        var ItemObj = new JsonObject();
                                        ItemObj.Add("Name", "Customer Reg - " + Guid.NewGuid().ToString());
                                        var Itemobj2 = new JsonObject();
                                        Itemobj2.Add("value", "1");
                                        Itemobj2.Add("name", "Registration");
                                        ItemObj.Add("IncomeAccountRef", Itemobj2);
                                        ItemObj.Add("Type", "Service");
                                        HttpContent content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
                                        HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                        try
                                        {
                                            var client = new HttpClient();

                                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                                            client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                            System.Net.Http.HttpResponseMessage response = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/customer?minorversion=65", content);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                string responseConetent = await response.Content.ReadAsStringAsync();
                                                var DesResContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                                string CompId = DesResContent["Customer"]["Id"].ToString();

                                                member.MemID = Convert.ToInt32(CompId);
                                                System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                                if (Itemresponse.IsSuccessStatusCode)
                                                {
                                                    string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                                    var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                                    string ItemName = DesItemContent["Item"]["Name"].ToString();
                                                    string ItemId = DesItemContent["Item"]["Id"].ToString();
                                                    //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                                    // }
                                                    QuickBookInvoice payload = new QuickBookInvoice();
                                                    List<SaleInfo> saleList = new List<SaleInfo>();
                                                    SaleInfo saleInfo = new SaleInfo();
                                                    SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                                    ItemRefClass itemRefClass = new ItemRefClass();
                                                    itemRefClass.name = ItemName;
                                                    itemRefClass.value = ItemId;

                                                    salesItemLineDetailClass.ItemRef = itemRefClass;
                                                    saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                                    saleInfo.Amount = Convert.ToDecimal(PaymentModal.MemberCost);
                                                    saleInfo.DetailType = "SalesItemLineDetail";
                                                    saleList.Add(saleInfo);

                                                    payload.Line = saleList;

                                                    CustomerInfo customerInfo = new CustomerInfo();
                                                    customerInfo.value = CompId;

                                                    payload.CustomerRef = customerInfo;
                                                    string Invoice = JsonConvert.SerializeObject(payload);

                                                    HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                                    System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                                    if (Invresponse.IsSuccessStatusCode)
                                                    {
                                                        string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                        var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                        string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                        member.InvoiceId = Convert.ToInt32(InvId);
                                                        decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                        QuickBookPayment payment = new QuickBookPayment();
                                                        CustomerRefInfo info = new CustomerRefInfo();
                                                        info.value = CompId;
                                                        payment.CustomerRef = info;
                                                        payment.TotalAmt = Amount;
                                                        List<LineInfo> Line = new List<LineInfo>();
                                                        LineInfo li = new LineInfo();
                                                        li.Amount = Amount;
                                                        List<TxnInfo> Txns = new List<TxnInfo>();
                                                        TxnInfo Ti = new TxnInfo();
                                                        Ti.TxnId = InvId;
                                                        Ti.TxnType = "Invoice";
                                                        Txns.Add(Ti);
                                                        li.LinkedTxn = Txns;
                                                        Line.Add(li);
                                                        payment.Line = Line;
                                                        string payInfo = JsonConvert.SerializeObject(payment);
                                                        HttpContent Paycontent = new StringContent(payInfo, Encoding.UTF8, "application/json");
                                                        System.Net.Http.HttpResponseMessage Payresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/payment?minorversion=65", Paycontent);
                                                        if (Payresponse.IsSuccessStatusCode)
                                                        {
                                                            PaymentModal.MemID = member.MemID;
                                                            PaymentModal.InvoiceId = member.InvoiceId;
                                                            PaymentModal.PayStatus = "Done";
                                                            string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                            Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                            {
                                                                HttpOnly = false,
                                                                Secure = Convert.ToBoolean(false)
                                                            });
                                                            return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                        }
                                                        else
                                                        {
                                                            PaymentModal.MemID = member.MemID;
                                                            PaymentModal.InvoiceId = member.InvoiceId;
                                                            PaymentModal.PayStatus = "Payment Issue";
                                                            string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                            Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                            {
                                                                HttpOnly = false,
                                                                Secure = Convert.ToBoolean(false)
                                                            });
                                                            return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PaymentModal.MemID = member.MemID;
                                                        PaymentModal.PayStatus = "Invoice Issue";
                                                        string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                        Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                        {
                                                            HttpOnly = false,
                                                            Secure = Convert.ToBoolean(false)
                                                        });
                                                        return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                    }
                                                }
                                                else
                                                {
                                                    PaymentModal.MemID = member.MemID;
                                                    PaymentModal.PayStatus = "Item Issue";
                                                    string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                    Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                    {
                                                        HttpOnly = false,
                                                        Secure = Convert.ToBoolean(false)
                                                    });
                                                    return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                }
                                            }
                                            else
                                            {
                                                PaymentModal.PayStatus = "Customer Issue";
                                                string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                {
                                                    HttpOnly = false,
                                                    Secure = Convert.ToBoolean(false)
                                                });
                                                return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            return View("ApiCallService", (object)"QBO API call Failed!");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        return View("ApiCallService", "QBO API call Failed!" + " Error message: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    try
                                    {


                                        string result = "";

                                        var ItemObj = new JsonObject();
                                        ItemObj.Add("Name", "Customer Reg - " + Guid.NewGuid().ToString());
                                        var Itemobj2 = new JsonObject();
                                        Itemobj2.Add("value", "1");
                                        Itemobj2.Add("name", "Registration");
                                        ItemObj.Add("IncomeAccountRef", Itemobj2);
                                        ItemObj.Add("Type", "Service");
                                        HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                        try
                                        {
                                            var client = new HttpClient();

                                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                                            client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                                            System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                            if (Itemresponse.IsSuccessStatusCode)
                                            {
                                                string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                                var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                                string ItemName = DesItemContent["Item"]["Name"].ToString();
                                                string ItemId = DesItemContent["Item"]["Id"].ToString();
                                                //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                                // }
                                                QuickBookInvoice payload = new QuickBookInvoice();
                                                List<SaleInfo> saleList = new List<SaleInfo>();
                                                SaleInfo saleInfo = new SaleInfo();
                                                SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                                ItemRefClass itemRefClass = new ItemRefClass();
                                                itemRefClass.name = ItemName;
                                                itemRefClass.value = ItemId;

                                                salesItemLineDetailClass.ItemRef = itemRefClass;
                                                saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                                saleInfo.Amount = Convert.ToDecimal(PaymentModal.MemberCost);
                                                saleInfo.DetailType = "SalesItemLineDetail";
                                                saleList.Add(saleInfo);

                                                payload.Line = saleList;

                                                CustomerInfo customerInfo = new CustomerInfo();
                                                customerInfo.value = tblMember.MemID.ToString();

                                                payload.CustomerRef = customerInfo;
                                                string Invoice = JsonConvert.SerializeObject(payload);

                                                HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                                System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                                if (Invresponse.IsSuccessStatusCode)
                                                {
                                                    string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                    var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                    string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                    member.InvoiceId = Convert.ToInt32(InvId);
                                                    decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                    QuickBookPayment payment = new QuickBookPayment();
                                                    CustomerRefInfo info = new CustomerRefInfo();
                                                    info.value = tblMember.MemID.ToString();
                                                    payment.CustomerRef = info;
                                                    payment.TotalAmt = Amount;
                                                    List<LineInfo> Line = new List<LineInfo>();
                                                    LineInfo li = new LineInfo();
                                                    li.Amount = Amount;
                                                    List<TxnInfo> Txns = new List<TxnInfo>();
                                                    TxnInfo Ti = new TxnInfo();
                                                    Ti.TxnId = InvId;
                                                    Ti.TxnType = "Invoice";
                                                    Txns.Add(Ti);
                                                    li.LinkedTxn = Txns;
                                                    Line.Add(li);
                                                    payment.Line = Line;
                                                    string payInfo = JsonConvert.SerializeObject(payment);
                                                    HttpContent Paycontent = new StringContent(payInfo, Encoding.UTF8, "application/json");
                                                    System.Net.Http.HttpResponseMessage Payresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/payment?minorversion=65", Paycontent);
                                                    if (Payresponse.IsSuccessStatusCode)
                                                    {
                                                        PaymentModal.MemID = tblMember.MemID;
                                                        PaymentModal.InvoiceId = member.InvoiceId;
                                                        PaymentModal.PayStatus = "Done";
                                                        string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                        Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                        {
                                                            HttpOnly = false,
                                                            Secure = Convert.ToBoolean(false)
                                                        });
                                                        return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                    }
                                                    else
                                                    {
                                                        PaymentModal.MemID = tblMember.MemID;
                                                        PaymentModal.InvoiceId = member.InvoiceId;
                                                        PaymentModal.PayStatus = "Payment Issue";
                                                        string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                        Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                        {
                                                            HttpOnly = false,
                                                            Secure = Convert.ToBoolean(false)
                                                        });
                                                        return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                    }
                                                }
                                                else
                                                {
                                                    PaymentModal.MemID = tblMember.MemID;
                                                    PaymentModal.PayStatus = "Invoice Issue";
                                                    string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                    Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                    {
                                                        HttpOnly = false,
                                                        Secure = Convert.ToBoolean(false)
                                                    });
                                                    return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                                }
                                            }
                                            else
                                            {
                                                PaymentModal.MemID = tblMember.MemID;
                                                PaymentModal.PayStatus = "Item Issue";
                                                string paymodelString = JsonConvert.SerializeObject(PaymentModal);
                                                Response.Cookies.Append("renewalpaymentModel", paymodelString, new Microsoft.AspNetCore.Http.CookieOptions
                                                {
                                                    HttpOnly = false,
                                                    Secure = Convert.ToBoolean(false)
                                                });
                                                return RedirectToAction("SaveRenewalPayment1", PaymentModal.Controller);
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            return View("ApiCallService", (object)"QBO API call Failed!");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        return View("ApiCallService", "QBO API call Failed!" + " Error message: " + ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action");
                            }
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogWarning(Ex.Message);
                            return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action");
                        }
                    }
                }

                int toDel = Convert.ToInt32(TempData["Model"]);
                TempData["Model"] = toDel;
                string PayModeRef = TempData["PayModeRef"].ToString();
                TempData["PayModeRef"] = PayModeRef;
                tblIncompleteSignUp tbl = await _commonRepository.GetSignUpData(toDel);
                //if (HttpContext.Session.GetString("realmId") != null)
                //{
                //if (HttpContext.Session.GetString("realmId") != null)
                //	{
                //string? realmId = Convert.ToString(HttpContext.Session.GetString("realmId"));
                try
                {
                    if (HttpContext.Session.GetString("realmId") != null)
                    {
                        //string? realmId = "4620816365247983250";
                        string? realmId = "9130355305808056";
                        Token = Token.Replace("access_token: ", "");
                        if (member.MemID == 0)
                        {
                            try
                            {
                                string result = "";
                                var obj = new JsonObject();
                                obj.Add("FullyQualifiedName", tbl.Company);
                                var obj1 = new JsonObject();
                                obj1.Add("Address", tbl.ContactEmail);
                                obj.Add("PrimaryEmailAddr", obj1);
                                obj.Add("DisplayName", tbl.Company + "(" + tbl.FirstName + ")");
                                obj.Add("Notes", "Registerd on." + DateTime.Now.ToString("MM/dd/yyyy"));
                                obj.Add("FamilyName", tbl.LastName);
                                var obj2 = new JsonObject();
                                obj2.Add("FreeFormNumber", tbl.CompanyPhone);
                                obj.Add("PrimaryPhone", obj2);
                                obj.Add("CompanyName", tbl.Company);
                                var obj3 = new JsonObject();
                                obj3.Add("CountrySubDivisionCode", tbl.BillState);
                                obj3.Add("City", tbl.BillCity);
                                obj3.Add("PostalCode", tbl.BillZip);
                                obj3.Add("Line1", tbl.BillAddress);
                                obj3.Add("Country", "USA");
                                obj.Add("BillAddr", obj3);
                                obj.Add("GivenName", tbl.FirstName);
                                var ItemObj = new JsonObject();
                                ItemObj.Add("Name", "Customer Reg - " + Guid.NewGuid().ToString());
                                var Itemobj2 = new JsonObject();
                                Itemobj2.Add("value", "1");
                                Itemobj2.Add("name", "Registration");
                                ItemObj.Add("IncomeAccountRef", Itemobj2);
                                ItemObj.Add("Type", "Service");
                                HttpContent content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
                                HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                                try
                                {
                                    var client = new HttpClient();

                                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                                    client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                    System.Net.Http.HttpResponseMessage response = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/customer?minorversion=65", content);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        string responseConetent = await response.Content.ReadAsStringAsync();
                                        var DesResContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                        string CompId = DesResContent["Customer"]["Id"].ToString();

                                        member.MemID = Convert.ToInt32(CompId);
                                        System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                        if (Itemresponse.IsSuccessStatusCode)
                                        {
                                            string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                            var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                            string ItemName = DesItemContent["Item"]["Name"].ToString();
                                            string ItemId = DesItemContent["Item"]["Id"].ToString();
                                            //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                            // }
                                            QuickBookInvoice payload = new QuickBookInvoice();
                                            List<SaleInfo> saleList = new List<SaleInfo>();
                                            SaleInfo saleInfo = new SaleInfo();
                                            SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                            ItemRefClass itemRefClass = new ItemRefClass();
                                            itemRefClass.name = ItemName;
                                            itemRefClass.value = ItemId;

                                            salesItemLineDetailClass.ItemRef = itemRefClass;
                                            saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                            saleInfo.Amount = Convert.ToDecimal(tbl.MemberCost);
                                            saleInfo.DetailType = "SalesItemLineDetail";
                                            saleList.Add(saleInfo);

                                            payload.Line = saleList;

                                            CustomerInfo customerInfo = new CustomerInfo();
                                            customerInfo.value = CompId;

                                            payload.CustomerRef = customerInfo;
                                            string Invoice = JsonConvert.SerializeObject(payload);
                                            HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                            System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                            if (Invresponse.IsSuccessStatusCode)
                                            {
                                                string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                                var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                                string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                                member.InvoiceId = Convert.ToInt32(InvId);
                                                decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                                QuickBookPayment payment = new QuickBookPayment();
                                                CustomerRefInfo info = new CustomerRefInfo();
                                                info.value = CompId;
                                                payment.CustomerRef = info;
                                                payment.TotalAmt = Amount;
                                                List<LineInfo> Line = new List<LineInfo>();
                                                LineInfo li = new LineInfo();
                                                li.Amount = Amount;
                                                List<TxnInfo> Txns = new List<TxnInfo>();
                                                TxnInfo Ti = new TxnInfo();
                                                Ti.TxnId = InvId;
                                                Ti.TxnType = "Invoice";
                                                Txns.Add(Ti);
                                                li.LinkedTxn = Txns;
                                                Line.Add(li);
                                                payment.Line = Line;
                                                string payInfo = JsonConvert.SerializeObject(payment);
                                                HttpContent Paycontent = new StringContent(payInfo, Encoding.UTF8, "application/json");
                                                System.Net.Http.HttpResponseMessage Payresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/payment?minorversion=65", Paycontent);
                                                if (Payresponse.IsSuccessStatusCode)
                                                {
                                                    if (PayModeRef == "CC")
                                                    {
                                                        try
                                                        {
                                                            var CredObj = new JsonObject();
                                                            CredObj.Add("amount", Convert.ToDecimal(tbl.MemberCost));
                                                            var cardObj = new JsonObject();
                                                            cardObj.Add("expYear", "2025");
                                                            cardObj.Add("expMonth", "02");
                                                            cardObj.Add("name", "Anubhav");
                                                            cardObj.Add("cvc", "123");
                                                            cardObj.Add("number", "4111111111155555");
                                                            CredObj.Add("card", cardObj);
                                                            CredObj.Add("currency", "USD");
                                                            var contextObj = new JsonObject();
                                                            contextObj.Add("mobile", "false");
                                                            contextObj.Add("isEcommerce", "true");
                                                            CredObj.Add("context", contextObj);
                                                            HttpContent CredContent = new StringContent(CredObj.ToString(), Encoding.UTF8, "application/json");
                                                            var CredClient = new HttpClient();

                                                            CredClient.DefaultRequestHeaders.Add("Accept", "application/json");
                                                            CredClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                                                            CredClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                                            CredClient.DefaultRequestHeaders.Add("Request-Id", Guid.NewGuid().ToString());
                                                            System.Net.Http.HttpResponseMessage CredResponse = await CredClient.PostAsync("https://sandbox.api.intuit.com/quickbooks/v4/payments/charges", CredContent);
                                                            string credResponseConetent = await CredResponse.Content.ReadAsStringAsync();
                                                            if (CredResponse.IsSuccessStatusCode)
                                                            {
                                                                credResponseConetent = await CredResponse.Content.ReadAsStringAsync();
                                                                var CredResContent = JsonConvert.DeserializeObject<dynamic>(credResponseConetent);
                                                                string ChargeId = CredResContent["id"].ToString();
                                                                System.Net.Http.HttpResponseMessage CredItemresponse = await CredClient.GetAsync("https://sandbox.api.intuit.com/quickbooks/v4/payments/charges/" + ChargeId);
                                                                responseConetent = await CredItemresponse.Content.ReadAsStringAsync();
                                                                if (CredItemresponse.IsSuccessStatusCode)
                                                                {
                                                                    var CredItemresponseContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                                                    member.PayModeRef = "Credit Card:" + CredItemresponseContent["id"].ToString();
                                                                }
                                                                else
                                                                {
                                                                    member.PayModeRef = "Direct Pay";
                                                                    //MemberShipRegistration member = new();

                                                                }
                                                            }
                                                        }
                                                        catch (Exception)
                                                        {

                                                            member.PayModeRef = "Direct Pay";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            var CredObj = new JsonObject();
                                                            CredObj.Add("amount", Convert.ToDecimal(tbl.MemberCost));
                                                            var accObj = new JsonObject();
                                                            accObj.Add("name", tbl.FirstName + " " + tbl.LastName);
                                                            accObj.Add("routingNumber", "322079353");
                                                            accObj.Add("accountNumber", "11000000333456781");
                                                            accObj.Add("accountType", "PERSONAL_CHECKING");
                                                            accObj.Add("phone", "1234567890");
                                                            CredObj.Add("bankAccount", accObj);
                                                            CredObj.Add("paymentMode", "WEB");
                                                            CredObj.Add("checkNumber", "12345678");
                                                            CredObj.Add("description", "Invoice Id: " + InvId);
                                                            var contextObj = new JsonObject();
                                                            var devInfo = new JsonObject();
                                                            devInfo.Add("id", "1");
                                                            devInfo.Add("type", "type");
                                                            devInfo.Add("longitude", "longitude");
                                                            devInfo.Add("latitude", "");
                                                            devInfo.Add("phoneNumber", "phonenu");
                                                            devInfo.Add("macAddress", "macaddress");
                                                            devInfo.Add("ipAddress", "34");
                                                            contextObj.Add("deviceInfo", devInfo);
                                                            CredObj.Add("context", contextObj);
                                                            string credStr = CredObj.ToString();
                                                            HttpContent CredContent = new StringContent(credStr, Encoding.UTF8, "application/json");
                                                            var CredClient = new HttpClient();
                                                            CredClient.DefaultRequestHeaders.Add("Accept", "application/json");
                                                            CredClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                                                            CredClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                                                            CredClient.DefaultRequestHeaders.Add("Request-Id", Guid.NewGuid().ToString());
                                                            System.Net.Http.HttpResponseMessage CredResponse = await CredClient.PostAsync("https://sandbox.api.intuit.com/quickbooks/v4/payments/echecks", CredContent);
                                                            string credResponseConetent = await CredResponse.Content.ReadAsStringAsync();
                                                            if (CredResponse.IsSuccessStatusCode)
                                                            {
                                                                credResponseConetent = await CredResponse.Content.ReadAsStringAsync();
                                                                var CredResContent = JsonConvert.DeserializeObject<dynamic>(credResponseConetent);
                                                                string ChargeId = CredResContent["id"].ToString();
                                                                System.Net.Http.HttpResponseMessage CredItemresponse = await CredClient.GetAsync("https://sandbox.api.intuit.com/quickbooks/v4/payments/echecks/" + ChargeId);
                                                                responseConetent = await CredItemresponse.Content.ReadAsStringAsync();
                                                                if (CredItemresponse.IsSuccessStatusCode)
                                                                {
                                                                    var CredItemresponseContent = JsonConvert.DeserializeObject<dynamic>(responseConetent);
                                                                    member.PayModeRef = "Check Id:" + CredItemresponseContent["id"].ToString();
                                                                }
                                                                else
                                                                {
                                                                    member.PayModeRef = "Direct Pay";
                                                                }
                                                            }
                                                        }
                                                        catch (Exception)
                                                        {
                                                            member.PayModeRef = "Direct Pay";
                                                        }

                                                    }
                                                    //MemberShipRegistration member = new();
                                                    member.MSPChk = "Y";
                                                    member.PayStatus = "Y";
                                                    member.Inactive = false;
                                                    member.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                                                    member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                                    member.Company = tbl.Company;
                                                    member.BillAddress = tbl.BillAddress;
                                                    member.BillCity = tbl.BillCity;
                                                    member.BillState = tbl.BillState;
                                                    member.BillZip = tbl.BillZip;
                                                    member.hdnTerm = tbl.Term;
                                                    member.MemberType = tbl.MemberType;
                                                    member.MemberCost = tbl.MemberCost;
                                                    member.ToDelete = tbl.ID;
                                                    member.MailAddress = tbl.MailAddress;
                                                    member.MailCity = tbl.MailCity;
                                                    member.MailState = tbl.MailState;
                                                    member.MailZip = tbl.MailZip;
                                                    member.Dba = tbl.DBA;
                                                    member.FirstName = tbl.FirstName;
                                                    member.LastName = tbl.LastName;
                                                    member.ContactPhone = tbl.ContactPhone;
                                                    member.Extension = tbl.Extension;
                                                    member.ContactEmail = tbl.ContactEmail;
                                                    member.hdnPass = tbl.ContactPassword;
                                                    member.CompanyPhone = tbl.CompanyPhone;
                                                    member.DiscountId = tbl.DiscountId;
                                                    return RedirectToAction("GoToStep4", "MemberShip", member);
                                                }
                                                else
                                                {
                                                    //MemberShipRegistration member = new();
                                                    member.MSPChk = "Y";
                                                    member.PayStatus = "Payment Issue";
                                                    member.Inactive = true;
                                                    member.LastPayDate = "";
                                                    member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                                    member.Company = tbl.Company;
                                                    member.BillAddress = tbl.BillAddress;
                                                    member.BillCity = tbl.BillCity;
                                                    member.BillState = tbl.BillState;
                                                    member.BillZip = tbl.BillZip;
                                                    member.hdnTerm = tbl.Term;
                                                    member.MemberType = tbl.MemberType;
                                                    member.MemberCost = tbl.MemberCost;
                                                    member.ToDelete = tbl.ID;
                                                    member.MailAddress = tbl.MailAddress;
                                                    member.MailCity = tbl.MailCity;
                                                    member.MailState = tbl.MailState;
                                                    member.MailZip = tbl.MailZip;
                                                    member.Dba = tbl.DBA;
                                                    member.FirstName = tbl.FirstName;
                                                    member.LastName = tbl.LastName;
                                                    member.ContactPhone = tbl.ContactPhone;
                                                    member.Extension = tbl.Extension;
                                                    member.ContactEmail = tbl.ContactEmail;
                                                    member.hdnPass = tbl.ContactPassword;
                                                    member.CompanyPhone = tbl.CompanyPhone;
                                                    member.DiscountId = tbl.DiscountId;
                                                    return RedirectToAction("GoToStep4", "MemberShip", member);
                                                }
                                            }
                                            else
                                            {
                                                //MemberShipRegistration member = new();
                                                member.MSPChk = "Y";
                                                member.PayStatus = "Invoice Issue";
                                                member.Inactive = true;
                                                member.LastPayDate = "";
                                                member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                                member.Company = tbl.Company;
                                                member.BillAddress = tbl.BillAddress;
                                                member.BillCity = tbl.BillCity;
                                                member.BillState = tbl.BillState;
                                                member.BillZip = tbl.BillZip;
                                                member.hdnTerm = tbl.Term;
                                                member.MemberType = tbl.MemberType;
                                                member.MemberCost = tbl.MemberCost;
                                                member.ToDelete = tbl.ID;
                                                member.MailAddress = tbl.MailAddress;
                                                member.MailCity = tbl.MailCity;
                                                member.MailState = tbl.MailState;
                                                member.MailZip = tbl.MailZip;
                                                member.Dba = tbl.DBA;
                                                member.FirstName = tbl.FirstName;
                                                member.LastName = tbl.LastName;
                                                member.ContactPhone = tbl.ContactPhone;
                                                member.Extension = tbl.Extension;
                                                member.ContactEmail = tbl.ContactEmail;
                                                member.hdnPass = tbl.ContactPassword;
                                                member.CompanyPhone = tbl.CompanyPhone;
                                                member.DiscountId = tbl.DiscountId;
                                                return RedirectToAction("GoToStep4", "MemberShip", member);
                                            }
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();
                                            member.MSPChk = "Y";
                                            member.PayStatus = "Item Issue";
                                            member.Inactive = true;
                                            member.LastPayDate = "";
                                            member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                            member.Company = tbl.Company;
                                            member.BillAddress = tbl.BillAddress;
                                            member.BillCity = tbl.BillCity;
                                            member.BillState = tbl.BillState;
                                            member.BillZip = tbl.BillZip;
                                            member.hdnTerm = tbl.Term;
                                            member.MemberType = tbl.MemberType;
                                            member.MemberCost = tbl.MemberCost;
                                            member.ToDelete = tbl.ID;
                                            member.MailAddress = tbl.MailAddress;
                                            member.MailCity = tbl.MailCity;
                                            member.MailState = tbl.MailState;
                                            member.MailZip = tbl.MailZip;
                                            member.Dba = tbl.DBA;
                                            member.FirstName = tbl.FirstName;
                                            member.LastName = tbl.LastName;
                                            member.ContactPhone = tbl.ContactPhone;
                                            member.Extension = tbl.Extension;
                                            member.ContactEmail = tbl.ContactEmail;
                                            member.hdnPass = tbl.ContactPassword;
                                            member.CompanyPhone = tbl.CompanyPhone;
                                            member.DiscountId = tbl.DiscountId;
                                            return RedirectToAction("GoToStep4", "MemberShip", member);
                                        }
                                    }
                                    else
                                    {
                                        //MemberShipRegistration member = new();
                                        member.MSPChk = "Y";
                                        member.PayStatus = "Customer Issue";
                                        member.Inactive = true;
                                        member.LastPayDate = "";
                                        member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                        member.Company = tbl.Company;
                                        member.BillAddress = tbl.BillAddress;
                                        member.BillCity = tbl.BillCity;
                                        member.BillState = tbl.BillState;
                                        member.BillZip = tbl.BillZip;
                                        member.hdnTerm = tbl.Term;
                                        member.MemberType = tbl.MemberType;
                                        member.MemberCost = tbl.MemberCost;
                                        member.ToDelete = tbl.ID;
                                        member.MailAddress = tbl.MailAddress;
                                        member.MailCity = tbl.MailCity;
                                        member.MailState = tbl.MailState;
                                        member.MailZip = tbl.MailZip;
                                        member.Dba = tbl.DBA;
                                        member.FirstName = tbl.FirstName;
                                        member.LastName = tbl.LastName;
                                        member.ContactPhone = tbl.ContactPhone;
                                        member.Extension = tbl.Extension;
                                        member.ContactEmail = tbl.ContactEmail;
                                        member.hdnPass = tbl.ContactPassword;
                                        member.CompanyPhone = tbl.CompanyPhone;
                                        member.DiscountId = tbl.DiscountId;
                                        return RedirectToAction("GoToStep4", "MemberShip", member);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return View("ApiCallService", (object)"QBO API call Failed!");
                                }
                            }
                            catch (Exception ex)
                            {
                                return View("ApiCallService", "QBO API call Failed!" + " Error message: " + ex.Message);
                            }
                        }
                        else
                        {
                            var ItemObj = new JsonObject();
                            ItemObj.Add("Name", "Customer Reg - " + Guid.NewGuid().ToString());
                            var Itemobj2 = new JsonObject();
                            Itemobj2.Add("value", "1");
                            Itemobj2.Add("name", "Registration");
                            ItemObj.Add("IncomeAccountRef", Itemobj2);
                            ItemObj.Add("Type", "Service");
                            HttpContent Itemcontent = new StringContent(ItemObj.ToString(), Encoding.UTF8, "application/json");
                            try
                            {
                                var client = new HttpClient();

                                client.DefaultRequestHeaders.Add("Accept", "application/json");
                                client.DefaultRequestHeaders.Add("ContentType", "application/json");
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);


                                System.Net.Http.HttpResponseMessage Itemresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/item?minorversion=65", Itemcontent);
                                if (Itemresponse.IsSuccessStatusCode)
                                {
                                    string ItemresponseConetent = await Itemresponse.Content.ReadAsStringAsync();
                                    var DesItemContent = JsonConvert.DeserializeObject<dynamic>(ItemresponseConetent);
                                    string ItemName = DesItemContent["Item"]["Name"].ToString();
                                    string ItemId = DesItemContent["Item"]["Id"].ToString();
                                    //string jsonStr = { "Line": [ { "DetailType": "SalesItemLineDetail", "Amount": 100.0, "SalesItemLineDetail": { "ItemRef": { "name": "Services", "value": "1" } } } ], "CustomerRef": { "value": "1" }
                                    // }
                                    QuickBookInvoice payload = new QuickBookInvoice();
                                    List<SaleInfo> saleList = new List<SaleInfo>();
                                    SaleInfo saleInfo = new SaleInfo();
                                    SalesItemLineDetailClass salesItemLineDetailClass = new SalesItemLineDetailClass();

                                    ItemRefClass itemRefClass = new ItemRefClass();
                                    itemRefClass.name = ItemName;
                                    itemRefClass.value = ItemId;

                                    salesItemLineDetailClass.ItemRef = itemRefClass;
                                    saleInfo.SalesItemLineDetail = salesItemLineDetailClass;
                                    saleInfo.Amount = Convert.ToDecimal(tbl.MemberCost);
                                    saleInfo.DetailType = "SalesItemLineDetail";
                                    saleList.Add(saleInfo);

                                    payload.Line = saleList;

                                    CustomerInfo customerInfo = new CustomerInfo();
                                    customerInfo.value = member.MemID.ToString();

                                    payload.CustomerRef = customerInfo;
                                    string Invoice = JsonConvert.SerializeObject(payload);

                                    HttpContent Invcontent = new StringContent(Invoice, Encoding.UTF8, "application/json");
                                    System.Net.Http.HttpResponseMessage Invresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/invoice?minorversion=65", Invcontent);
                                    if (Invresponse.IsSuccessStatusCode)
                                    {
                                        string InvresponseConetent = await Invresponse.Content.ReadAsStringAsync();
                                        var DesInvContent = JsonConvert.DeserializeObject<dynamic>(InvresponseConetent);
                                        string InvId = DesInvContent["Invoice"]["Id"].ToString();
                                        member.InvoiceId = Convert.ToInt32(InvId);
                                        decimal Amount = Convert.ToDecimal(DesInvContent["Invoice"]["TotalAmt"]);
                                        QuickBookPayment payment = new QuickBookPayment();
                                        CustomerRefInfo info = new CustomerRefInfo();
                                        info.value = member.MemID.ToString();
                                        payment.CustomerRef = info;
                                        payment.TotalAmt = Amount;
                                        List<LineInfo> Line = new List<LineInfo>();
                                        LineInfo li = new LineInfo();
                                        li.Amount = Amount;
                                        List<TxnInfo> Txns = new List<TxnInfo>();
                                        TxnInfo Ti = new TxnInfo();
                                        Ti.TxnId = InvId;
                                        Ti.TxnType = "Invoice";
                                        Txns.Add(Ti);
                                        li.LinkedTxn = Txns;
                                        Line.Add(li);
                                        payment.Line = Line;
                                        string payInfo = JsonConvert.SerializeObject(payment);

                                        HttpContent Paycontent = new StringContent(payInfo, Encoding.UTF8, "application/json");
                                        System.Net.Http.HttpResponseMessage Payresponse = await client.PostAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + CompanyId + "/payment?minorversion=65", Paycontent);
                                        if (Payresponse.IsSuccessStatusCode)
                                        {
                                            //MemberShipRegistration member = new();
                                            member.MSPChk = "Y";
                                            member.PayStatus = "Y";
                                            member.Inactive = false;
                                            member.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                                            member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                            member.Company = tbl.Company;
                                            member.BillAddress = tbl.BillAddress;
                                            member.BillCity = tbl.BillCity;
                                            member.BillState = tbl.BillState;
                                            member.BillZip = tbl.BillZip;
                                            member.hdnTerm = tbl.Term;
                                            member.MemberType = tbl.MemberType;
                                            member.MemberCost = tbl.MemberCost;
                                            member.ToDelete = tbl.ID;
                                            member.MailAddress = tbl.MailAddress;
                                            member.MailCity = tbl.MailCity;
                                            member.MailState = tbl.MailState;
                                            member.MailZip = tbl.MailZip;
                                            member.Dba = tbl.DBA;
                                            member.FirstName = tbl.FirstName;
                                            member.LastName = tbl.LastName;
                                            member.ContactPhone = tbl.ContactPhone;
                                            member.Extension = tbl.Extension;
                                            member.ContactEmail = tbl.ContactEmail;
                                            member.hdnPass = tbl.ContactPassword;
                                            member.CompanyPhone = tbl.CompanyPhone;
                                            member.DiscountId = tbl.DiscountId;
                                            return RedirectToAction("GoToStep4", "MemberShip", member);
                                        }
                                        else
                                        {
                                            //MemberShipRegistration member = new();
                                            member.MSPChk = "Y";
                                            member.PayStatus = "Payment Issue";
                                            member.Inactive = true;
                                            member.LastPayDate = "";
                                            member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                            member.Company = tbl.Company;
                                            member.BillAddress = tbl.BillAddress;
                                            member.BillCity = tbl.BillCity;
                                            member.BillState = tbl.BillState;
                                            member.BillZip = tbl.BillZip;
                                            member.hdnTerm = tbl.Term;
                                            member.MemberType = tbl.MemberType;
                                            member.MemberCost = tbl.MemberCost;
                                            member.ToDelete = tbl.ID;
                                            member.MailAddress = tbl.MailAddress;
                                            member.MailCity = tbl.MailCity;
                                            member.MailState = tbl.MailState;
                                            member.MailZip = tbl.MailZip;
                                            member.Dba = tbl.DBA;
                                            member.FirstName = tbl.FirstName;
                                            member.LastName = tbl.LastName;
                                            member.ContactPhone = tbl.ContactPhone;
                                            member.Extension = tbl.Extension;
                                            member.ContactEmail = tbl.ContactEmail;
                                            member.hdnPass = tbl.ContactPassword;
                                            member.CompanyPhone = tbl.CompanyPhone;
                                            member.DiscountId = tbl.DiscountId;
                                            return RedirectToAction("GoToStep4", "MemberShip", member);
                                        }
                                    }
                                    else
                                    {
                                        //MemberShipRegistration member = new();
                                        member.MSPChk = "Y";
                                        member.PayStatus = "Invoice Issue";
                                        member.Inactive = true;
                                        member.LastPayDate = "";
                                        member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                        member.Company = tbl.Company;
                                        member.BillAddress = tbl.BillAddress;
                                        member.BillCity = tbl.BillCity;
                                        member.BillState = tbl.BillState;
                                        member.BillZip = tbl.BillZip;
                                        member.hdnTerm = tbl.Term;
                                        member.MemberType = tbl.MemberType;
                                        member.MemberCost = tbl.MemberCost;
                                        member.ToDelete = tbl.ID;
                                        member.MailAddress = tbl.MailAddress;
                                        member.MailCity = tbl.MailCity;
                                        member.MailState = tbl.MailState;
                                        member.MailZip = tbl.MailZip;
                                        member.Dba = tbl.DBA;
                                        member.FirstName = tbl.FirstName;
                                        member.LastName = tbl.LastName;
                                        member.ContactPhone = tbl.ContactPhone;
                                        member.Extension = tbl.Extension;
                                        member.ContactEmail = tbl.ContactEmail;
                                        member.hdnPass = tbl.ContactPassword;
                                        member.CompanyPhone = tbl.CompanyPhone;
                                        member.DiscountId = tbl.DiscountId;
                                        return RedirectToAction("GoToStep4", "MemberShip", member);
                                    }
                                }
                                else
                                {
                                    //MemberShipRegistration member = new();
                                    member.MSPChk = "Y";
                                    member.PayStatus = "Item Issue";
                                    member.Inactive = true;
                                    member.LastPayDate = "";
                                    member.ContactName = tbl.FirstName + " " + tbl.LastName;
                                    member.Company = tbl.Company;
                                    member.BillAddress = tbl.BillAddress;
                                    member.BillCity = tbl.BillCity;
                                    member.BillState = tbl.BillState;
                                    member.BillZip = tbl.BillZip;
                                    member.hdnTerm = tbl.Term;
                                    member.MemberType = tbl.MemberType;
                                    member.MemberCost = tbl.MemberCost;
                                    member.ToDelete = tbl.ID;
                                    member.MailAddress = tbl.MailAddress;
                                    member.MailCity = tbl.MailCity;
                                    member.MailState = tbl.MailState;
                                    member.MailZip = tbl.MailZip;
                                    member.Dba = tbl.DBA;
                                    member.FirstName = tbl.FirstName;
                                    member.LastName = tbl.LastName;
                                    member.ContactPhone = tbl.ContactPhone;
                                    member.Extension = tbl.Extension;
                                    member.ContactEmail = tbl.ContactEmail;
                                    member.hdnPass = tbl.ContactPassword;
                                    member.CompanyPhone = tbl.CompanyPhone;
                                    member.DiscountId = tbl.DiscountId;
                                    return RedirectToAction("GoToStep4", "MemberShip", member);
                                }


                            }
                            catch (Exception ex)
                            {
                                return View("ApiCallService", (object)"QBO API call Failed!");
                            }


                        }
                    }
                    // }
                    else
                        return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action");
                }
                catch (Exception Ex)
                {

                    _logger.LogWarning(Ex.Message);
                    return View("ApiCallService", "QBO API call Failed!" + " Error message: Invalid action");
                }
                //}
                //else
                //  return View("ApiCallService", "QBO API call Failed!"); 
            }
        }

        /// <summary>
        /// Use the Index page of App controller to get all endpoints from discovery url
        /// </summary>
        public ActionResult Error()
        {
            return View("Error");
        }

        /// <summary>
        /// Action that takes redirection from Callback URL
        /// </summary>
        public ActionResult Tokens(string Token)
        {
            if (Token != null)
            {
                TempData["Token"] = Token;
            }
            return View();
        }



    }
}