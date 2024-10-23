using ChargeBee.Api;
using ChargeBee.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Intuit.Ipp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Services;
using PCNW.ViewModel;
using System.Data.Entity;
using System.Text;

namespace PCNW.Controllers
{
    public class ChargeBeeController : BaseController
    {
        private readonly IMembershipRepository _membershipRepository;
        public string? chargebeesite = null;
        public string? chargebeesitekey = null;
        public string? chargebeeredirecturl = null;
        public string? chargebeecancelurl = null;
        public string? chargebeeRegisterredirecturl = null;
        private readonly ChargeBeeInfo _chargeBeeInfo;
        private readonly ILogger<ChargeBeeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStaffRepository _staffRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ChargeBeeAPIService _chargebeeAPI;

        public ChargeBeeController(IMembershipRepository membershipRepository,ChargeBeeAPIService chargebeeAPI, IOptions<ChargeBeeInfo> chargeBeeInfo, IStaffRepository staffRepository, ILogger<ChargeBeeController> logger, IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ICommonRepository commonRepository, ApplicationDbContext dbContext)
        {
            
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _commonRepository = commonRepository;
            _staffRepository = staffRepository;
            _chargebeeAPI = chargebeeAPI;
            _membershipRepository = membershipRepository;

            _chargeBeeInfo = chargeBeeInfo.Value;
           
            chargebeesite = _chargeBeeInfo.chargebeesite;
            chargebeesitekey = _chargeBeeInfo.chargebeesitekey;
            chargebeeredirecturl = _chargeBeeInfo.chargebeeredirecturl;
            chargebeeRegisterredirecturl = _chargeBeeInfo.chargebeeRegisterredirecturl;
            chargebeecancelurl = _chargeBeeInfo.chargebeecancelurl;
            

            ApiConfig.Configure(chargebeesite, chargebeesitekey);
        }

        public IActionResult AddUserSubcription(MemberShipRegistration model)
        {
            try
            {
                var membertyp = Convert.ToInt32(model.MemberType);
                var data = _dbContext.tblPaymentCardDetail.FirstOrDefault(m => m.MemberType == membertyp);
                var pkg = data!.PackageName!.Replace(" ", "-");
                pkg = pkg + "-" + model.RadioExValue;
                //EntityResult result = HostedPage.CheckoutNewForItems()
                //    .SubscriptionItemItemPriceId(0, pkg)
                //    .SubscriptionItemQuantity(0, 1)
                //    .CustomerId(model.ASPUserId.ToString())
                //    .CustomerFirstName(model.FirstName)
                //    .CustomerLastName(model.LastName)
                //    .RedirectUrl(chargebeeRegisterredirecturl)
                //    .CancelUrl(chargebeecancelurl)
                //    .Request();

                var result = HostedPage.CheckoutNewForItems()
                    .SubscriptionItemItemPriceId(0, pkg)
                    .CustomerCompany(model.Company)
                    .CustomerEmail(model.Email)
                    .CustomerFirstName(model.FirstName)
                    .CustomerLastName(model.LastName)
                    .CustomerPhone(model.CompanyPhone)
                    .ShippingAddressLine1(model.MailAddress)
                    .BillingAddressFirstName(model.FirstName)
                    .BillingAddressLastName(model.LastName)
                    .BillingAddressCompany(model.Company)
                    .BillingAddressEmail(model.BillEmail)
                    .BillingAddressLine1(model.BillAddress)
                    .ShippingAddressCity(model.MailCity)
                    .BillingAddressCity(model.BillCity)
                    .ShippingAddressCompany(model.Company)
                    .ShippingAddressEmail(model.Email)
                    .ShippingAddressLastName(model.LastName)
                    .ShippingAddressState(model.MailState.ToUpper())
                    .ShippingAddressZip(model.MailZip)
                    .BillingAddressZip(model.BillZip)
                    .ShippingAddressCountry("US")
                    .RedirectUrl(chargebeeRegisterredirecturl)
                    .CancelUrl(chargebeecancelurl)
                    .CustomerId(model.ASPUserId.ToString())
                    .Request(); 

                HostedPage hostedPage = result.HostedPage;
                string hostedPageUrl = result.HostedPage.Url;
                return Redirect(hostedPageUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> NewUserSuccessPay(string id, string state)
        {
            int toDel = Convert.ToInt32(TempData["Model"]);

            tblIncompleteSignUp tbl = await _commonRepository.GetSignUpData(toDel);
            MemberShipRegistration member = new();
            member.ASPUserId = (Guid)tbl.UserId!;
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

        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(string userId, string newPlan, string pkg)
        {
            try
            {
                Response.Cookies.Append("redirection", "/Member/MemberProfile/", new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
                DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity!.Name!);

                // Retrieve the customer by ID
                var user = await _userManager.FindByEmailAsync(info.Email);
                ChargeBee.Models.Customer customer;

                try
                {
                    customer = ChargeBee.Models.Customer.Retrieve(user.Id).Request().Customer;
                }
                catch (Exception)
                {
                    return await CreateHostedPageAndReturnResponse(newPlan, user.Id, info.Name, pkg, info.Id);
                }

                // Retrieve the customer's subscriptions
                List<ListResult.Entry> subscriptions;
                try
                {
                    subscriptions = Subscription.List().CustomerId().Is(user.Id).Request().List;
                }
                catch (Exception)
                {
                    return await CreateHostedPageAndReturnResponse(newPlan, user.Id, info.Name, pkg, info.Id);
                }

                // If no subscriptions found, create a new subscription
                if (subscriptions == null || subscriptions.Count == 0)
                {
                    return await CreateHostedPageAndReturnResponse(newPlan, user.Id, info.Name, pkg, info.Id);
                }

                // Get the first subscription
                var subscription = subscriptions[0].Subscription;

                // Update the subscription with new items
                Subscription.UpdateForItems(subscription.Id)
                    .SubscriptionItemItemPriceId(0, newPlan)
                    .SubscriptionItemQuantity(0, 1)
                    .Request();
                var data = _dbContext.Members.FirstOrDefault(m => m.BusinessEntityId == info.Id);
                var memberType = _dbContext.tblPaymentCardDetail.FirstOrDefault(m => m.PackageName!.Replace(" ", "").ToLower() == pkg.Replace(" ", "").ToLower());

                if (data != null && memberType != null)
                {
                    data.MemberType = memberType.MemberType;
                    _dbContext.Members.Update(data);
                    _dbContext.SaveChanges();
                }

                //EntityResult result = HostedPage.CheckoutExistingForItems()
                //    .SubscriptionId(subscription.Id)
                //    .SubscriptionItemItemPriceId(0, newPlan)
                //    .SubscriptionItemQuantity(0, 1)
                //        .RedirectUrl(chargebeeredirecturl)
                //        .CancelUrl(chargebeecancelurl)
                //    .Request();
                // Return success response
                return Json(new { Status = "success", Message = "Subscription updated successfully." });
            }
            catch (ChargeBee.Exceptions.InvalidRequestException ex)
            {
                // Log and return the error message
                string errorMessage = ex.Message;
                Console.WriteLine(errorMessage);
                return Json(new { Status = "error", Message = errorMessage });
            }
            catch (Exception)
            {
                // Return a generic error message for unexpected exceptions
                return Json(new { Status = "error", Message = "There was an issue with updating the subscription. Please try again later." });
            }
        }

        private async Task<IActionResult> CreateHostedPageAndReturnResponse(string newPlan, string userId, string userName, string pkg, int id)
        {
            try
            {
                int ConId = 0;
                string UserName = "";
                DisplayLoginInfo logInfo = new();
                var data = await _dbContext.Members.FirstOrDefaultAsync(m => m.BusinessEntityId == id);
                var memberType = _dbContext.tblPaymentCardDetail.FirstOrDefault(m => m.PackageName!.Replace(" ", "").ToLower() == pkg.Replace(" ", "").ToLower());

                if (data != null && memberType != null)
                {
                    data.MemberType = memberType.MemberType;
                    _dbContext.Members.Update(data);
                    _dbContext.SaveChanges();
                }
                if (id <= 0)
                {
                    logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
                    if (logInfo != null)
                    {
                        id = logInfo.Id;
                        ConId = logInfo.ConId;
                        UserName = logInfo.Name;

                        var model = await _membershipRepository.GetMemberProfileAsync(id, ConId, UserName);
                        var result = HostedPage.CheckoutNewForItems()
                            .SubscriptionItemItemPriceId(0, newPlan)
                            .CustomerCompany(model.Company)
                            .CustomerEmail(model.Email)
                            .CustomerFirstName(model.FirstName)
                            .CustomerLastName(model.LastName)
                            .CustomerPhone(model.CompanyPhone)
                            .ShippingAddressLine1(model.MailAddress)
                            .BillingAddressFirstName(model.FirstName)
                            .BillingAddressLastName(model.LastName)
                            .BillingAddressCompany(model.Company)
                            .BillingAddressEmail(model.BillEmail)
                            .BillingAddressLine1(model.BillAddress)
                            .ShippingAddressCity(model.MailCity)
                            .BillingAddressCity(model.BillCity)
                            .ShippingAddressCompany(model.Company)
                            .ShippingAddressFirstName(model.FirstName)
                            .ShippingAddressLastName(model.LastName)
                            .ShippingAddressStateCode(model.MailState.ToUpper())
                            .ShippingAddressZip(model.MailZip)
                            .BillingAddressZip(model.BillZip)
                            .ShippingAddressCountry("US")
                            .RedirectUrl(chargebeeRegisterredirecturl)
                            .CancelUrl(chargebeecancelurl)
                            .CustomerId(model.ASPUserId.ToString())
                            .Request();

                        HostedPage hostedPage = result.HostedPage;
                        string hostedPageUrl = result.HostedPage.Url;

                        return Json(new { Status = "new", Data = hostedPageUrl });
                    }
                }

                return Json(new { Status = "error", Message = "There was an issue with updating the subscription. Please try again later." });
            }
            catch (ChargeBee.Exceptions.InvalidRequestException ex)
            {
                // Log and return the error message
                string errorMessage = ex.Message;
                Console.WriteLine(errorMessage);
                return Json(new { Status = "error", Message = errorMessage });
            }
            catch (Exception)
            {
                return Json(new { Status = "error", Message = "There was an issue with updating the subscription. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUpdatePaymentMethodPage()
        {
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity!.Name!);
            var user = await _userManager.FindByEmailAsync(info.Email);
            Response.Cookies.Append("redirection", "/Member/MemberProfile", new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = false,
                Secure = Convert.ToBoolean(false)
            });
            try
            {
                // Create a hosted page for updating payment method
                var result = HostedPage.ManagePaymentSources()
                    .CustomerId(user.Id)
                        .RedirectUrl(chargebeeredirecturl)
                    .Request();

                var hostedPageUrl = result.HostedPage.Url;

                // Return the hosted page URL
                return Json(new { Status = "success", Data = hostedPageUrl });
            }
            catch (ChargeBee.Exceptions.InvalidRequestException ex)
            {
                string errorMessage = ex.Message;
                Console.WriteLine(errorMessage);
                return Json(new { Status = "error", Message = errorMessage });
            }
            catch (Exception)
            {
                return Json(new { Status = "error", Message = "There was an issue with creating the payment method update page. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGracePeriod(MemberShipRegistration model)
        {
            try
            {
               
                // Configure the ChargeBee API
                var user = await _userManager.FindByEmailAsync(model.Email);
                // Configure the ChargeBee API
                ChargeBee.Models.Customer customer;

                try
                {
                    customer = ChargeBee.Models.Customer.Retrieve(user.Id).Request().Customer;
                }
                catch (Exception)
                {
                    return Json(new { Status = "Error", Message = "Contact Admin Regarding this." });
                }

                // Retrieve the customer's subscriptions
                List<ListResult.Entry> subscriptions;
                try
                {
                    subscriptions = Subscription.List().CustomerId().Is(user.Id).Request().List;
                }
                catch (Exception)
                {
                    return Json(new { Status = "Error", Message = "Contact Admin Regarding this." });
                }

                // If no subscriptions found, create a new subscription
                if (subscriptions == null || subscriptions.Count == 0)
                {
                    return Json(new { Status = "Error", Message = "Contact Admin Regarding this." });
                }

                // Get the first subscription
                    var subscription = subscriptions[0].Subscription;

                //DateTime newEndDate = subscription.CurrentTermEnd!.Value.AddDays((double)model.Grace!);
                DateTime newEndDate = (DateTime)model.RenewalDate;
                long gracePeriodEndTimestamp = ((DateTimeOffset)newEndDate).ToUnixTimeSeconds();
                // Check if the new end date is exactly the same as the current end date
                if (subscription.CurrentTermEnd.Value == newEndDate)
                {
                    // If they are the same, add one more second to ensure they are different
                    newEndDate = newEndDate.AddSeconds(1);
                    gracePeriodEndTimestamp = ((DateTimeOffset)newEndDate).ToUnixTimeSeconds();
                }
                // Update the subscription's cancellation date
                EntityResult result = Subscription.ChangeTermEnd(subscription.Id)
                    .TermEndsAt(gracePeriodEndTimestamp)
                    .Request();
                model.RenewalDate = newEndDate;
                await _staffRepository.UpdateGracePeriodAsync(model);
                return Json(new { Status = "success", Message = "Grace period upgraded successfully." });
            }
            catch (ChargeBee.Exceptions.InvalidRequestException ex)
            {
                string errorMessage = ex.Message;
                Console.WriteLine(errorMessage);
                return Json(new { Status = "error", Message = errorMessage });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { Status = "error", Message = "There was an issue upgrading the grace period. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPaymentHistory(string email, string login)
        {
            try
            {
                if (login == "staff")
                {
                    var user = await _userManager.FindByEmailAsync(email);

                    // Retrieve the payment history for the customer
                    var transactions = ChargeBee.Models.Transaction.List()
                        .CustomerId().Is(user.Id)
                        .Request().List;

                    // Prepare HTML for payment history table
                    StringBuilder sb = new StringBuilder();
                    foreach (var transaction in transactions)
                    {
                        var subscriptionId = transaction.Transaction.SubscriptionId;
                        var subscription = Subscription.Retrieve(subscriptionId).Request();

                        // Retrieve plan name from subscription detcheckails
                        var planName = subscription.Subscription;

                        sb.Append("<tr>");
                        sb.Append($"<td>{transaction.Transaction.SubscriptionId}</td>");
                        sb.Append($"<td>{transaction.Transaction.Date}</td>");
                        sb.Append($"<td class='amount-cents'>{transaction.Transaction.Amount}</td>");
                        sb.Append($"<td>{transaction.Transaction.Status}</td>");
                        sb.Append("</tr>");
                    }
                    if (string.IsNullOrEmpty(sb.ToString()))
                    {
                        sb.Append("<tr>");
                        sb.Append($"<td>No Records available at this moment.</td>");
                        sb.Append("</tr>");
                    }
                    return Content(sb.ToString(), "text/html");
                }
                else
                {
                    DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity!.Name!);
                    var user = await _userManager.FindByEmailAsync(info.Email);

                    // Retrieve the payment history for the customer
                    var transactions = ChargeBee.Models.Transaction.List()
                        .CustomerId().Is(user.Id)
                        .Request().List;

                    // Prepare HTML for payment history table
                    StringBuilder sb = new StringBuilder();
                    foreach (var transaction in transactions)
                    {
                        var subscriptionId = transaction.Transaction.SubscriptionId;
                        var subscription = Subscription.Retrieve(subscriptionId).Request();

                        // Retrieve plan name from subscription details
                        var planName = subscription.Subscription;

                        sb.Append("<tr>");
                        sb.Append($"<td>{transaction.Transaction.SubscriptionId}</td>");
                        sb.Append($"<td>{transaction.Transaction.Date}</td>");
                        sb.Append($"<td>${transaction.Transaction.Amount / 100.0}</td>");
                        sb.Append($"<td>{transaction.Transaction.Status}</td>");
                        sb.Append("</tr>");
                    }

                    return Content(sb.ToString(), "text/html");
                }
            }
            catch (ChargeBee.Exceptions.InvalidRequestException ex)
            {
                string errorMessage = ex.Message;
                Console.WriteLine(errorMessage);
                return Content("Error retrieving payment history.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Content("Error retrieving payment history.");
            }
        }

        public IActionResult SuccessonPay(string id, string state)
        {
            try
            {
                // Fetch the hosted page details to verify the payment status
                EntityResult result = HostedPage.Retrieve(id).Request();
                HostedPage hostedPage = result.HostedPage;
                if (Request.Cookies.TryGetValue("redirection", out string? redirectionUrl))
                {
                    Response.Cookies.Delete("redirection");
                    if (Url.IsLocalUrl(redirectionUrl))
                    {
                        return Redirect(redirectionUrl);
                    }
                }
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index", "Home");
        }

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