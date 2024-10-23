using ChargeBee.Api;
using ChargeBee.Models;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PCNW.Controllers;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;

namespace PCNW.Services
{
    public class ChargeBeeAPIService
    {
        public string? chargebeesite = null;
        public string? chargebeesitekey = null;
        public string? chargebeeredirecturl = null;
        public string? chargebeecancelurl = null;
        public string? chargebeeRegisterredirecturl = null;
        private readonly ChargeBeeInfo _chargeBeeInfo;
        private readonly ILogger<ChargeBeeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly OAuth2Client auth2Client1;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStaffRepository _staffRepository;
        private readonly OAuth2Client _auth2Client;
        private readonly ICommonRepository _commonRepository;
        private readonly ApplicationDbContext _dbContext;
        public ChargeBeeAPIService(IOptions<ChargeBeeInfo> chargeBeeInfo, IStaffRepository staffRepository, ILogger<ChargeBeeController> logger, IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ICommonRepository commonRepository, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _commonRepository = commonRepository;
            _staffRepository = staffRepository;
            _chargeBeeInfo = chargeBeeInfo.Value;

            chargebeesite = _chargeBeeInfo.chargebeesite;
            chargebeesitekey = _chargeBeeInfo.chargebeesitekey;
            chargebeeredirecturl = _chargeBeeInfo.chargebeeredirecturl;
            chargebeeRegisterredirecturl = _chargeBeeInfo.chargebeeRegisterredirecturl;
            chargebeecancelurl = _chargeBeeInfo.chargebeecancelurl;

            ApiConfig.Configure(chargebeesite, chargebeesitekey);
        }
        public async Task<string> AddUserSubcription(MemberShipRegistration model)
        {
            try
            {
                var membertyp = Convert.ToInt32(model.MemberType);
                var data = _dbContext.tblPaymentCardDetail.FirstOrDefault(m => m.MemberType == membertyp);
                var pkg = data.PackageName.Replace(" ", "-");
                pkg = pkg + "-" + model.RadioExValue;
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
                return hostedPageUrl;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> AddUserSubcriptionByStaff(MemberShipRegistration model)
        {
            try
            {
                EntityResult result = Customer.Create()
                    .Id(model.ASPUserId.ToString())
                    .FirstName(model.FirstName)
                    .LastName(model.FirstName)
                    .Email(model.Email)
                    .BillingAddressFirstName(model.FirstName)
                    .BillingAddressLastName(model.LastName)
                    .BillingAddressLine1(model.BillAddress ?? model.MailAddress)
                    .BillingAddressCity(model.BillCity ?? model.MailCity)
                    .BillingAddressState(model.BillState ?? model.MailState)
                    .BillingAddressZip(model.BillZip ?? model.MailZip)
                    .BillingAddressCountry("US")
                    .Request();
                var membertyp = Convert.ToInt32(model.MemberType);
                var data = _dbContext.tblPaymentCardDetail.FirstOrDefault(m => m.MemberType == membertyp);
                var pkg = data.PackageName.Replace(" ", "-");
                pkg = pkg + "-" + model.hdnTerm;

                EntityResult result1 = Subscription.CreateWithItems(model.ASPUserId.ToString())
                    .SubscriptionItemItemPriceId(0, pkg)
                    .SubscriptionItemQuantity(0, 1)
                    .Request();

                return "success";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
