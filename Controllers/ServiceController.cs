using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCNW.Data.ContractResponse;
using PCNW.Data.Repository;
using PCNW.Helpers;

namespace PCNW.Controllers
{
    /// <summary>
    /// No Use
    /// </summary>
    [Authorize]
    public class ServiceController : BaseController
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(IServiceRepository serviceRepository, ILogger<ServiceController> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> AddNotification()
        {
            HttpResponseDetail<dynamic> response = new();
            int MemberId = GetMemberId(User.Identity.Name);
            List<AddNotification> result = await _serviceRepository.GetNotificationContactsAsync(MemberId);
            response.data = result;
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification(List<AddNotification> contacts)
        {
            HttpResponseDetail<dynamic> resposne = new();
            int MemberId = GetMemberId(User.Identity.Name);
            try
            {
                foreach (var item in contacts)
                {
                    item.MemberID = MemberId;
                    await _serviceRepository.AddNotificationContactsAsync(item.MemberID, item.Contact, item.Email, item.Daily);
                }
                return View(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return RedirectToAction("AddNotification");
            }
        }
    }
}
