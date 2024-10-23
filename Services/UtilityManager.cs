using Microsoft.EntityFrameworkCore;
using PCNW.Data.Repository;
using PCNW.Models;
using PCNW.Models.ProcessContracts;

namespace PCNW.Services
{
    public class UtilityManager : IUtilityManager
    {
        private readonly IEmailServiceManager _emailServiceManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UtilityManager> _logger;
        private readonly IEntityRepository _entityRepository;

        public UtilityManager(IEmailServiceManager emailServiceManager, ApplicationDbContext dbContext, ILogger<UtilityManager> logger, IEntityRepository entityRepository)
        {
            _emailServiceManager = emailServiceManager;
            _dbContext = dbContext;
            _logger = logger;
            _entityRepository = entityRepository;
        }
        public async Task<bool> LogIP(string ipAddress, int? MemberId, string Notes, bool? Show)
        {
            bool rowsAffected = false;
            try
            {
                TblIprelation tblIprelation = new();
                tblIprelation.Ip = ipAddress;
                tblIprelation.MemId = MemberId;
                tblIprelation.Notes = Notes;
                tblIprelation.LastUsed = DateTime.Now;
                tblIprelation.Show = Show;
                await _dbContext.TblIprelations.AddAsync(tblIprelation);
                await _dbContext.SaveChangesAsync();
                rowsAffected = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rowsAffected;
        }

        public async Task<bool> EmailNewIP(string ipAddress, int? MemberId)
        {
            bool rowsAffected = false;
            try
            {
                string strBody = string.Empty;
                var memberList = (from tab in _entityRepository.GetEntities()
                                  join contact in _dbContext.TblContacts on tab.Id equals contact.Id
                                  where tab.Id == MemberId
                                  select new { tab.Company, contact.Uid, tab.RenewalDate, tab.Grace, tab.MemberType })
                .Select(x => new
                {
                    Company = x.Company,
                    Uid = x.Uid,
                    RenewalDate = x.RenewalDate,
                    Grace = x.Grace,
                    MemberType = x.MemberType
                }).ToList();
                foreach (var memberInfo in memberList)
                {
                    strBody = $"{memberInfo.Company} has accessed our website with a new IP Address at {DateTime.Now}<br />";
                    strBody = $"IP Address: {ipAddress}<br />";
                    strBody = $"User ID: {memberInfo.Uid}<br />";
                    strBody = $"Member Type: {memberInfo.MemberType}<br />";
                    strBody = $"Renewal Date: {memberInfo.RenewalDate}<br /><br />";
                    strBody = $"<a href='http://www.contractorplancenter.com/member/RptIP.asp?IP={ipAddress}'>Click here</a> for more information.";
                    EmailViewModel vm = new();
                    vm.Subject = $"New IP Address - {ipAddress}";
                    List<string> emails = new();
                    emails.Add("nathan@contractorplancenter.com");
                    emails.Add("scott@contractorplancenter.com");
                    vm.EmailTos = emails;
                    _emailServiceManager.SendEmail(vm);
                }
                rowsAffected = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rowsAffected;
        }
        public async Task<bool> InBlockedList(string ipAddress)
        {
            bool rows = false;
            try
            {
                var tblBlockedIp = await _dbContext.TblBlockedIps.FirstOrDefaultAsync(m => m.Disable == false && m.Ipaddress == ipAddress);
                if (tblBlockedIp != null)
                    rows = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rows;
        }
        public async Task<bool> Holiday(DateTime holidayDt)
        {
            bool rows = false;
            try
            {
                var tblDailyInfoHolidays = await _dbContext.TblDailyInfoHoliday.FirstOrDefaultAsync(m => m.HolidayDt == holidayDt);
                if (tblDailyInfoHolidays != null)
                    rows = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rows;
        }
    }
}
