using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;
using System.Data;

namespace PCNW.Data.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ReportRepository> _logger;
        private readonly string _connectionString;
        private readonly IEntityRepository _entityRepository;

        public ReportRepository(ApplicationDbContext dbContext, ILogger<ReportRepository> logger, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            _entityRepository = entityRepository;
        }
        public async Task<IEnumerable<AllMembers>> GetAllMembers()
        {
            List<AllMembers> response = new List<AllMembers>();
            //return await _dbContext.TblMembers.OrderByDescending(t => t. Id).ToListAsync();
            try
            {
                response = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            //join t in _dbContext.TblMemberTypeCounty on m.MemberType equals t.MemberType
                            // where any acondition apply
                            select new
                            {
                                m.Company,
                                c.Contact,
                                m.Inactive,
                                m.CompanyPhone,
                                c.Phone,
                                c.Email,
                                c.ConId,
                                m.MemberType
                                //t.Package
                            }).Take(100).ToList()
                                   .Select(x => new AllMembers
                                   {
                                       Company = x.Company,
                                       Contact = x.Contact,
                                       CompanyPhone = x.CompanyPhone,
                                       ConId = x.ConId,
                                       Inactive = x.Inactive,
                                       MemberType = x.MemberType

                                   }).ToList();
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        public async Task<IEnumerable<ArchEngineerViewModel>> GetArchEngineers()
        {
            IEnumerable<ArchEngineerViewModel> ArchEngineers = new List<ArchEngineerViewModel>();
            List<TblArchOwner> tblArchEngineers = await _dbContext.TblArchOwners.OrderByDescending(t => t.Id).Take(200).ToListAsync();
            if (tblArchEngineers != null)
            {
                ArchEngineers = tblArchEngineers.ToList().Select(x => new ArchEngineerViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Type1 = x.Type1,
                    State = x.State,
                    Phone = x.Phone,
                }).ToList();

            }
            return ArchEngineers;
        }
        public async Task<IEnumerable<ContractorViewModel>> GetContractors()
        {
            IEnumerable<ContractorViewModel> Contractors = new List<ContractorViewModel>();
            List<TblContractor> tblContractors = await _dbContext.TblContractors.OrderByDescending(t => t.Id).Take(200).ToListAsync();
            if (tblContractors != null)
            {
                Contractors = tblContractors.ToList().Select(x => new ContractorViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Type2 = x.Type2,
                    State = x.State,
                    Phone = x.Phone,
                }).ToList();

            }
            return Contractors;
        }
        public async Task<IEnumerable<ActiveMemberViewModel>> GetActiveMembers()
        {
            IEnumerable<ActiveMemberViewModel> ActMembers = new List<ActiveMemberViewModel>();
            List<TblMember> tblActMembers = _entityRepository.GetEntities().OrderByDescending(t => t.Id).Take(200).ToList();
            if (tblActMembers != null)
            {
                ActMembers = tblActMembers.ToList().Select(x => new ActiveMemberViewModel
                {
                    Id = x.Id,
                    Company = x.Company,
                    MailCity = x.MailCity,
                    MailState = x.MailState,
                    RenewalDate = x.RenewalDate,
                    Discipline = x.Discipline,

                }).ToList();
            }
            return ActMembers;
        }
        public async Task<IEnumerable<InactiveMemberViewModel>> GetInactiveMembers()
        {
            IEnumerable<InactiveMemberViewModel> InactMembers = new List<InactiveMemberViewModel>();
            List<TblMember> tblActMembers = _entityRepository.GetEntities().Where(M => M.Inactive != true).OrderByDescending(t => t.Id).Take(200).ToList();
            if (tblActMembers != null)
            {
                InactMembers = tblActMembers.ToList().Select(x => new InactiveMemberViewModel
                {
                    Id = x.Id,
                    Company = x.Company,
                    MailCity = x.MailCity,
                    MailState = x.MailState,
                    RenewalDate = x.RenewalDate,
                    Discipline = x.Discipline,
                }).ToList();
            }
            return InactMembers;
        }
        public async Task<dynamic> GetMemberSummaryReport()
        {
            HttpResponseDetail<dynamic> response = new();
            MemberSummaryViewModel data = new();
            List<MemberSummaryViewModel> result = new List<MemberSummaryViewModel>();

            try
            {
                result = await _dbContext.MemberSummaryViewModel.FromSqlRaw("GetMemberSummary").ToListAsync();
                foreach (var item in result)
                {
                    data = item;

                }
                response.success = true;
                response.statusCode = "200";


            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;

                _logger.LogWarning(ex.Message);
            }

            response.data = data;
            return (response);
        }
        public async Task<dynamic> GetCompanyTypes()
        {
            HttpResponseDetail<dynamic> response = new();
            MemberTypeViewModel data = new();
            List<MemberTypeViewModel> result = new List<MemberTypeViewModel>();

            try
            {
                result = await _dbContext.MemberTypeViewModel.FromSqlRaw("sp_GetMemberType").ToListAsync();
                response.success = true;
                response.statusCode = "200";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;

                _logger.LogWarning(ex.Message);
            }

            response.data = result;
            return (response);
        }
        public async Task<dynamic> GetSelectedCompanyCount(string MemberType, string SubscriptionLevel, string RenewalType, string CancellationType)
        {
            if (SubscriptionLevel == null)
            {
                SubscriptionLevel = "";
            }
            if (RenewalType == null)
            {
                RenewalType = "";
            }
            if (CancellationType == null)
            {
                CancellationType = "";
            }

            HttpResponseDetail<dynamic> response = new();
            MemberSummaryViewModel data = new();
            List<MemberSummaryViewModel> result = new List<MemberSummaryViewModel>();
            var ParamMemberType = new SqlParameter("@MemberType", MemberType);
            var ParamSubscriptionType = new SqlParameter("@SubscriptionLevel", SubscriptionLevel);
            var ParamRenewalType = new SqlParameter("@RenewalType", RenewalType);
            var ParamCancellationType = new SqlParameter("@CancellationType", CancellationType);
            try
            {
                result = await _dbContext.MemberSummaryViewModel.FromSqlRaw($"GetMemberSummary @MemberType,@SubscriptionLevel,@RenewalType,@CancellationType", ParamMemberType, ParamSubscriptionType, ParamRenewalType, ParamCancellationType).ToListAsync();
                foreach (var item in result)
                {
                    data = item;

                }
                response.success = true;
                response.statusCode = "200";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;

                _logger.LogWarning(ex.Message);
            }

            response.data = data;
            return (response);
        }

        public async Task<dynamic> GetNewMemberListAsync(DateTime st, DateTime et)
        {
            HttpResponseDetail<dynamic> response = new();
            List<NewMemberViewModel> dataList = new();
            try
            {
                dataList = _entityRepository.GetEntities().Where(x => (x.InsertDate != null && x.InsertDate >= st && x.InsertDate <= et))
                    .Select(x => new NewMemberViewModel
                    {
                        Id = x.Id,
                        Company = x.Company,
                        MailCity = x.MailCity,
                        MailState = x.MailState,
                        InsertDate = x.InsertDate,
                        Discipline = x.Discipline,
                    })
                    .ToList();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "New Members data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetNewMemberDailyAsync()
        {
            DateTime dt = DateTime.Now.Date;
            HttpResponseDetail<dynamic> response = new();
            List<NewMemberViewModel> dataList = new();
            try
            {
                dataList = _entityRepository.GetEntities().Where(x => (x.InsertDate != null && x.InsertDate >= dt))
                    .Select(x => new NewMemberViewModel
                    {
                        Id = x.Id,
                        Company = x.Company,
                        MailCity = x.MailCity,
                        MailState = x.MailState,
                        InsertDate = x.InsertDate,
                        Discipline = x.Discipline,
                    })
                    .ToList();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Daily New Members data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetInCompleteMemberAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblIncompleteSignUp> dataList = new();
            try
            {
                dataList = await _dbContext.tblIncompleteSignUp.Select(x => new tblIncompleteSignUp
                {
                    Company = x.Company,
                    CompanyPhone = x.CompanyPhone,
                    FirstName = x.FirstName,
                    ContactEmail = x.ContactEmail,
                    ContactPhone = x.ContactPhone,
                    MailAddress = x.MailAddress,
                }).ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "InComplete SignUp data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetTrialMembersAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblMember> dataList = new();
            try
            {
                dataList = _entityRepository.GetEntities().Where(x => x.MemberType == 12)
                    .Select(x => new TblMember
                    {
                        Id = x.Id,
                        Company = x.Company,
                        MailCity = x.MailCity,
                        MailState = x.MailState,
                        RenewalDate = x.RenewalDate,
                        Discipline = x.Discipline,
                    }).ToList();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Trial Members data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }

        public async Task<dynamic> GetMemberUsageAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblLogActivity> dataList = new();
            try
            {
                dataList = await _dbContext.TblLogActivity.Where(x => x.LoginFlag == true)
                    .Select(x => new TblLogActivity
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        LoginTime = x.LoginTime,
                        Activity = x.Activity,
                    }).ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Member Usage data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetSearchReportAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblAutoSearch> dataList = new();
            try
            {
                dataList = await _dbContext.TblAutoSearch.Select(x => new TblAutoSearch
                {
                    Name = x.Name,
                    Keywords = x.Keywords,
                }).Take(200).ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Search Report data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetDailySubscriptionsAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblMemberSubscribe> dataList = new();
            try
            {
                dataList = await _dbContext.TblMemberSubscribe.Where(m => m.Subscribe == false).Select(x => new TblMemberSubscribe
                {
                    Id = x.Id,
                    UserEmail = x.UserEmail,
                    UnSubscribeDate = x.UnSubscribeDate,
                }).ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Daily Subscriptions data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }

        public async Task<dynamic> GetAllCommunicationAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<AllMembers> dataList = new();
            try
            {
                dataList = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            where c.MainContact == true && c.Email != null && c.Email.Length > 0
                            select new
                            {
                                m.Company,
                                c.Email,
                            }).ToList()
                                   .Select(x => new AllMembers
                                   {
                                       Company = x.Company,
                                       Email = x.Email,

                                   }).ToList();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "All Communication data bind successfully";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetCommunicationAsync(string value)
        {
            HttpResponseDetail<dynamic> response = new();
            List<AllMembers> dataList = new();

            try
            {
                dataList = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            where c.MainContact == true && m.MailState == value && c.Email != null && c.Email.Length > 0
                            select new
                            {
                                m.Company,
                                c.Email,
                            }).ToList()
                                   .Select(x => new AllMembers
                                   {
                                       Company = x.Company,
                                       Email = x.Email,

                                   }).ToList();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Communication data bind successfully";
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        public async Task<dynamic> GetExpiringListAsync(DateTime st, DateTime et)
        {
            HttpResponseDetail<dynamic> response = new();
            List<AllMembers> dataList = new();
            try
            {
                dataList = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            where c.MainContact == true && c.Email != null && c.Email.Length > 0 && m.RenewalDate >= st && m.RenewalDate <= et
                            select new
                            {
                                m.Company,
                                c.Email,
                            }).ToList()
                                   .Select(x => new AllMembers
                                   {
                                       Company = x.Company,
                                       Email = x.Email,

                                   }).ToList();

                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Expiring Member data bind successfully";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
    }


}
