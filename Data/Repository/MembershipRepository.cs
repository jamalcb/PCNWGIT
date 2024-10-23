using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PCNW.ExtentionMethods;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using PCNW.ViewModel;
using SolrNet;
using System.Data;
using System.Data.Entity.Validation;
using System.Text;

namespace PCNW.Data.Repository
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<MembershipRepository> _logger;
        private readonly string _connectionString;
        private readonly IEntityRepository _entityRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ISolrOperations<SolrDocument> _solr;

        public MembershipRepository(ISolrOperations<SolrDocument> solr, IStaffRepository staffRepository, ApplicationDbContext dbContext, ILogger<MembershipRepository> logger, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            _entityRepository = entityRepository;
            _staffRepository = staffRepository;
            _solr = solr;
        }
        /// <summary>
        /// Get member division value in register page and member profile for both staff and member site 
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetMemberDivisionAsync(string SelectedValue = "")
        {
            var response = await (from tab in _dbContext.TblWebDivs.Where(m => m.IsActive == true && m.IsDeleted == false) select tab).ToListAsync();
            var result = response.ToList().OrderBy(m => m.Priority).Select(x => new SelectListItem
            {
                Text = x.DivName,
                Value = x.DivNo.ToString(),
                Selected = (!string.IsNullOrEmpty(SelectedValue) && (x.DivNo.ToString() == SelectedValue))
            }).ToList();

            return result;
        }
        /// <summary>
        /// Get state for populating state on sate dropdown on various page
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetStates(string SelectedValue = "")
        {
            var response = await (from tab in _dbContext.TblState select tab).ToListAsync();
            var result = response.ToList().OrderBy(m => m.StateId).Select(x => new SelectListItem
            {
                Text = x.State,
                Value = x.StateId.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.StateId.ToString() == SelectedValue ? true : false))
            }).ToList();
            return result;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> AddDivisionAsync(MemberShipRegistration model)
        {
            string strTmp = "";
            TblMemberDiv? meberDivisionInfo = await _dbContext.TblMemberDivs.SingleOrDefaultAsync(m => m.MemberId == model.lngMemID);
            if (meberDivisionInfo == null)
            {
                strTmp = strTmp + "<br />&nbsp&nbsp&nbsp&nbsp&nbsp ";
                meberDivisionInfo.WebDivId = model.WebDivId;
                meberDivisionInfo.MemberId = model.lngMemID;
                await _dbContext.SaveChangesAsync();
            }
            model.strMessage = model.strMessage + "Added Divisions: " + strTmp + "<br />";
            return strTmp;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDivisionAsync(MemberShipRegistration model)
        {
            string strTmp = "";
            TblMemberDiv meberDivisionInfo = await _dbContext.TblMemberDivs.SingleOrDefaultAsync(m => m.MemberId == model.lngMemID);
            if (meberDivisionInfo != null)
            {
                strTmp = strTmp + "<br />&nbsp&nbsp&nbsp&nbsp&nbsp ";
                meberDivisionInfo.WebDivId = model.WebDivId;
                meberDivisionInfo.MemberId = model.lngMemID;
                _dbContext.Entry(meberDivisionInfo).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            model.strMessage = model.strMessage + "Updated Divisions: " + strTmp + "<br />";
            return strTmp;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateMemberDivAsync(MemberShipRegistration model)
        {
            string strTmp = "";
            TblMember meberDivisionInfo = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.lngMemID);
            if (meberDivisionInfo != null)
            {
                strTmp = strTmp + "<br />&nbsp&nbsp&nbsp&nbsp&nbsp ";
                meberDivisionInfo.Div = model.Div;
                _dbContext.Entry(meberDivisionInfo).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            model.strMessage = model.strMessage + "Updated Divisions: " + strTmp + "<br />";
            return strTmp;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> GetstrMessage(MemberShipRegistration model)
        {
            var memberInfo = _entityRepository.GetEntities().FirstOrDefault(m => m.Id == model.lngMemID);
            if (memberInfo != null)
            {
                if (model.BillAddress != memberInfo.BillAddress)
                    model.strMessage = model.strMessage + "Billing Address: " + model.BillAddress + "<br />";
                if (model.BillCity != memberInfo.BillCity)
                    model.strMessage = model.strMessage + "Billing City: " + model.BillCity + "<br />";
                if (model.BillState != memberInfo.BillState)
                    model.strMessage = model.strMessage + "Billing State: " + model.BillState + "<br />";
                if (model.BillZip != memberInfo.BillZip)
                    model.strMessage = model.strMessage + "Billing Zip: " + model.BillZip + "<br />";
                if (model.MailAddress != memberInfo.MailAddress)
                    model.strMessage = model.strMessage + "Mailing Address: " + model.MailAddress + "<br />";
                if (model.MailCity != memberInfo.MailCity)
                    model.strMessage = model.strMessage + "Mailing City: " + model.MailCity + "<br />";
                if (model.MailState != memberInfo.MailState)
                    model.strMessage = model.strMessage + "Mailing State: " + model.MailState + "<br />";
                if (model.MailZip != memberInfo.MailZip)
                    model.strMessage = model.strMessage + "Mailing Zip: " + model.MailZip + "<br />";
                if (model.Fax != memberInfo.Fax)
                    model.strMessage = model.strMessage + "Fax: " + model.Fax + "<br />";
                if (model.PaperlessBilling != memberInfo.PaperlessBilling)
                    model.strMessage = model.strMessage + "Billing Email: " + model.PaperlessBilling + " < br /> ";
                await UpdateDivisionAsync(model);
                await AddDivisionAsync(model);
                if (model.Discipline != memberInfo.Discipline)
                    model.strMessage = model.strMessage + "Discipline: " + model.Discipline + "<br />";
                if (model.MinorityStatus != memberInfo.MinorityStatus)
                    model.strMessage = model.strMessage + "Minority Status: " + model.MinorityStatus + "<br />";
                if (model.Note != "")
                    model.strMessage = model.strMessage + "<br/>Comments: " + model.Note + "<br />";
            }
            return model.strMessage;
        }
        /// <summary>
        /// Get selected text of divisions on behalf of id (No use)
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public string GetSelectedText(string SelectedValue)
        {
            var response = (from tab in _dbContext.TblWebDivs.Where(m => m.DivNo.ToString() == SelectedValue) select new { tab.DivName }).Single();
            string result = response.DivName;
            return result;
        }
        /// <summary>
        /// Get text of state on behalf of state id 
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public string GetSelectedStateText(string SelectedValue)
        {
            var response = (from tab in _dbContext.TblState.Where(m => m.StateId.ToString() == SelectedValue) select new { tab.State }).SingleOrDefault();
            string result = response != null ? response.State : "NA";
            //if (response != null)
            //    result = response.State;
            //else
            //    result = "NA";
            return result;
        }
        /// <summary>
        /// Get div name on behalf of div No (No use)
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public int GetWebDiv(string SelectedValue)
        {
            var response = (from tab in _dbContext.TblWebDivs.Where(m => m.DivNo.ToString() == SelectedValue) select new { tab.WebDivId }).Single();
            int result = response.WebDivId;
            return result;
        }
        /// <summary>
        /// For registering member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> RegisterMembershipAsync(MemberShipRegistration model)
        {
            tblDiscount tblDiscount = new();
            var discount = "";
            if (model.DiscountId != null && model.DiscountId > 0)
            {
                tblDiscount = await _dbContext.tblDiscount.SingleOrDefaultAsync(m => m.DiscountId == model.DiscountId);
                discount = "Discounted @" + tblDiscount.DiscountRate + "% under offer " + tblDiscount.Description + " applicable between " + Convert.ToDateTime(tblDiscount.StartDate).ToString("MM/dd/yyyy") + " to " + Convert.ToDateTime(tblDiscount.EndDate).ToString("MM/dd/yyyy");
            }
            if (model.BillState != null)
            {
                model.BillState = GetSelectedStateText(model.BillState);
            }
            model.MailState = GetSelectedStateText(model.MailState);
            model.ContactName = model.FirstName + " " + model.LastName;
            var rowsAffected = 0;
            model.InsertDate = DateTime.Now;

            if (model.Inactive == false)
            {
                if (model.hdnTerm == "Yearly")
                    model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddYears(1).AddDays(-1) : null;
                if (model.hdnTerm == "Quarterly")
                    model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(4).AddDays(-1) : null;
                if (model.hdnTerm == "Monthly")
                    model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(1).AddDays(-1) : null;
                if (model.hdnTerm == "Free Trial")
                    model.RenewalDate = DateTime.Now.AddDays(364);
            }
            try
            {
                var response = await _staffRepository.SaveNewRegMemberAsync(model);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        /// <summary>
        /// Mo use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> RegisterMembershipAsync1(MemberShipRegistration model)
        {
            var rowsAffected = 0;
            try
            {
                //@Insert_Date datetime = null,
                //@Company nvarchar(50) = null,  
                // @Inactive bit = 1,
                // @BillAddress nvarchar(50) = null,  
                // @BillCity nvarchar(50) = null,  
                // @BillState nvarchar(2) = null,  
                // @BillZip nvarchar(50) = null,  
                // @RenewalDate datetime = null,
                // @Term nvarchar(50) = null,  
                // @Discipline nvarchar(255) = null,  
                // @HowdUHearAboutUs nvarchar(255) = null,  
                // @MinorityStatus nvarchar(50)  = null,  
                // @MemberType int = null,
                // @Paperless_Billing nvarchar(50) = null,  
                // @Member_Cost money = null,
                // @Fax nvarchar(100) = null,  
                // @MailAddress nvarchar(50) = null,  
                // @MailCity nvarchar(50) = null,  
                // @MailState nvarchar(2) = null,  
                // @MailZip nvarchar(50) = null,  
                // @MemberID int = 0,
                // @ID int = null output

                //@Insert_Date datetime, @Company nvarchar(12), @Inactive int,
                //@Bill' expects the parameter '@RenewalDate', which was not supplied.'

                //'(@Insert_Date datetime,@Company nvarchar(12), @Inactive int
                //,@Bill' expects the parameter '@Term', which was not supplied.'

                model.Inactive = true;
                //model.Term = "1";
                if (model.Company.Length > 12)
                    model.Company = model.Company.ToString().Substring(0, 10);
                model.InsertDate = DateTime.Now;
                if (model.Inactive == false)
                {
                    if (model.hdnTerm == "Yearly")
                        model.RenewalDate = Convert.ToDateTime(model.LastPayDate).AddYears(1).AddDays(-1);
                    if (model.hdnTerm == "Quaterly")
                        model.RenewalDate = Convert.ToDateTime(model.LastPayDate).AddMonths(4).AddDays(-1);
                    if (model.hdnTerm == "Monthly")
                        model.RenewalDate = Convert.ToDateTime(model.LastPayDate).AddMonths(1).AddDays(-1);
                    if (model.hdnTerm == "Free Trial")
                        model.RenewalDate = Convert.ToDateTime(model.LastPayDate).AddDays(6);
                }
                if (!string.IsNullOrEmpty(model.Div) && model.Div != "0")
                    model.WebDivId = GetWebDiv(model.Div);
                else
                {
                    model.WebDivId = 37;
                }
                if (!string.IsNullOrEmpty(model.Div) && model.Div != "0")
                    model.Div = GetSelectedText(model.Div);
                else
                {
                    model.Div = "No division is selected";
                }


                var paramInsert_Date = new SqlParameter("@Insert_Date", model.InsertDate);
                var paramCompany = new SqlParameter("@Company", model.Company);
                var paramInactive = new SqlParameter("@Inactive", model.Inactive);
                var paramBillAddress = new SqlParameter("@BillAddress", model.BillAddress);
                var paramBillCity = new SqlParameter("@BillCity", model.BillCity);
                var paramBillState = new SqlParameter("@BillState", model.BillState);
                var paramBillZip = new SqlParameter("@BillZip", model.BillZip);
                var paramRenewalDate = new SqlParameter("@RenewalDate", model.RenewalDate); ;
                var paramTerm = new SqlParameter("@Term", model.Term);
                var paramDiscipline = new SqlParameter("@Discipline", model.Discipline);
                var paramHowdUHearAboutUs = new SqlParameter("@HowdUHearAboutUs", model.HowdUhearAboutUs);
                var paramMinorityStatus = new SqlParameter("@MinorityStatus", model.MinorityStatus);
                var paramMemberType = new SqlParameter("@MemberType", Convert.ToInt32(model.MemberType));
                var paramPaperlessBilling = new SqlParameter("@Paperless_Billing", model.PaperlessBilling);
                var paramMemberCost = new SqlParameter("@Member_Cost", Convert.ToDecimal(model.MemberCost));
                var paramFax = new SqlParameter("@Fax", model.Fax);
                var paramMailAddress = new SqlParameter("@MailAddress", model.MailAddress);
                var paramMailCity = new SqlParameter("@MailCity", model.MailCity);
                var paramMailState = new SqlParameter("@MailState", model.MailState);
                var paramMailZip = new SqlParameter("@MailZip", model.MailZip);
                var paramMemberID = new SqlParameter("@MemberID", 0);
                var paramID = new SqlParameter("@ID", 0);
                paramID.Direction = ParameterDirection.InputOutput;

                Dictionary<string, object> Param = new();
                Param.Add("@Insert_Date", model.InsertDate);
                Param.Add("@Company", model.Company);
                Param.Add("@Inactive", model.Inactive);
                Param.Add("@BillAddress", model.BillAddress);
                Param.Add("@BillCity", model.BillCity);
                Param.Add("@BillState", model.BillState);
                Param.Add("@BillZip", model.BillZip);
                Param.Add("@RenewalDate", model.RenewalDate); ;
                Param.Add("@Term", model.Term);
                Param.Add("@Discipline", model.Discipline);
                Param.Add("@HowdUHearAboutUs", model.HowdUhearAboutUs);
                Param.Add("@MinorityStatus", model.MinorityStatus);
                Param.Add("@MemberType", model.MemberType);
                Param.Add("@Paperless_Billing", model.PaperlessBilling);
                Param.Add("@Member_Cost", Convert.ToDecimal(model.MemberCost));
                Param.Add("@Fax", model.Fax);
                Param.Add("@MailAddress", model.MailAddress);
                Param.Add("@MailCity", model.MailCity);
                Param.Add("@MailState", model.MailState);
                Param.Add("@MailZip", model.MailZip);
                Param.Add("@MemberID", 0);
                Param.Add("@ID", 0);
                TblMember tblMember = new();


                tblMember.InsertDate = model.InsertDate;
                tblMember.Company = model.Company;
                tblMember.Inactive = model.Inactive;
                tblMember.BillAddress = model.BillAddress;
                tblMember.BillCity = model.BillCity;
                tblMember.BillState = model.BillState;
                tblMember.BillZip = model.BillZip;
                tblMember.RenewalDate = model.RenewalDate; ;
                tblMember.Term = model.Term;
                tblMember.Discipline = model.Discipline;
                tblMember.HowdUhearAboutUs = model.HowdUhearAboutUs;
                tblMember.MinorityStatus = model.MinorityStatus;
                if (!string.IsNullOrEmpty(model.MemberType))
                    tblMember.MemberType = Convert.ToInt32(model.MemberType);
                tblMember.PaperlessBilling = model.PaperlessBilling;
                tblMember.MemberCost = Convert.ToDecimal(model.MemberCost);
                tblMember.Fax = model.Fax;
                tblMember.MailAddress = model.MailAddress;
                tblMember.MailCity = model.MailCity;
                tblMember.MailState = model.MailState;
                tblMember.MailZip = model.MailZip;
                #region Default values setting
                tblMember.AddPkgCost = 0;
                tblMember.ArchPkgCost = 0;
                tblMember.ConId = 0;
                tblMember.DailyEmail = null;
                tblMember.InsertDate = DateTime.Now;
                tblMember.MagCost = 0;
                tblMember.MemberCost = 0;
                tblMember.PaperlessBilling = "";
                tblMember.ResourceAdd = "0";
                tblMember.ResourceColor = "0";
                tblMember.ResourceCost = 0;
                tblMember.ResourceDate = null;
                tblMember.ResourceLogo = "0";
                tblMember.ResourceStandard = "0";
                tblMember.WebAdCost = 0;
                tblMember.WebAdDate = null;
                #endregion
                var businessEntity = _entityRepository.BusinessEntity_instance(tblMember);
                await _dbContext.BusinessEntities.AddAsync(businessEntity);
                await _dbContext.SaveChangesAsync();

                var address = _entityRepository.Address_instance(tblMember);
                var member = _entityRepository.Member_instance(tblMember);
                await _dbContext.Addresses.AddAsync(address);
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();

                //await _entityRepository.GetEntities().AddAsync(tblMember);
                //await _dbContext.SaveChangesAsync();

                model.lngMemID = model.ID;

                var response = await GeneralMethods.DynamicListFromSqlAsync(_dbContext, "cpc_InsertUpdateMember", Param);

                //rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync("cpc_InsertUpdateMember @Company, @Fax, @ID", paramCompany, paramFax, paramID);
                //rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync("cpc_InsertUpdateMember @Insert_Date, @Company, @Inactive, @BillAddress, @BillCity, @BillState, @BillZip, @RenewalDate, @Term, @Discipline, @HowdUHearAboutUs, @MinorityStatus, @MemberType, @Paperless_Billing, @Member_Cost, @Fax, @MailAddress, @MailCity, @MailState, @MailZip, @MemberID, @ID", paramInsert_Date, paramCompany, paramInactive, paramBillAddress, paramBillCity, paramBillState, paramBillZip, paramRenewalDate, paramTerm, paramDiscipline, paramHowdUHearAboutUs, paramMinorityStatus, paramMemberType, paramPaperlessBilling, paramMemberCost, paramFax, paramMailAddress, paramMailCity, paramMailState, paramMailZip, paramMemberID, paramID);
                //try
                //{
                //	model.lngMemID = (int)paramID.Value;
                //}
                //catch (Exception ex)
                //{

                //}
                //if (model.lngMemID == null || model.lngMemID <= 0)
                //{
                //	try
                //	{
                //		var res = _entityRepository.GetEntities().OrderByDescending(m => m.Id).Take(1).ToListAsync();
                //		foreach (var item in res)
                //		{
                //			model.lngMemID = item.Id;
                //		}
                //	}
                //	catch (Exception ex)
                //	{
                //		Param = new();
                //		var res = await GeneralMethods.DynamicListFromSqlAsync(_dbContext, "usp_GetMemebrId", Param);
                //		foreach (var item in res)
                //		{
                //			model.lngMemID = item.ID;
                //		}
                //	}
                //}
                //await AddDivisionAsync(model);
                //await UpdateDivisionAsync(model);
                //await AddContactAsync(model);
                //            await UpdateMemberDivAsync(model);

            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new();
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        sb.Append("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                string msg = sb.ToString();

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rowsAffected;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> AddContactAsync(MemberShipRegistration model)
        {
            var rowsAffected = 0;
            try
            {
                int counter = 0;
                int xUID = 0;
                string strUID = "";
                xUID = await _dbContext.TblContacts.Select(m => m.ConId).MaxAsync();
                char character = (char)(96 + counter);
                strUID = strUID + character.ToString();

                var paramID = new SqlParameter("@ID", model.lngMemID);
                var paramContact = new SqlParameter("@Contact", model.ContactName);
                var paramPhone = new SqlParameter("@Phone", model.ContactPhone);
                var paramEmail = new SqlParameter("@Email", model.ContactEmail);
                var paramDaily = new SqlParameter("@Daily", 1);
                var paramUID = new SqlParameter("@UID", strUID);
                var paramPwd = new SqlParameter("@Pwd", model.hdnPass);
                var paramConID = new SqlParameter("@ConID", model.ConID);
                paramConID.Direction = ParameterDirection.InputOutput;
                //rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync("cpc_InsertContact @ID,@Contact,@Phone,@Email,@Daily,@UID,@Pwd,@ConID", paramID, paramContact, paramPhone, paramEmail, paramDaily, paramUID, paramPwd, paramConID);

                Dictionary<string, object> Param = new();
                Param.Add("@ID", model.ContactId);
                Param.Add("@Contact", model.ContactName);
                Param.Add("@Phone", model.ContactPhone);
                Param.Add("@Email", model.ContactEmail);
                Param.Add("@Daily", 1);
                Param.Add("@UID", strUID);
                Param.Add("@Pwd", model.hdnPass);
                Param.Add("@ConID", model.ConID);
                var response = await GeneralMethods.DynamicListFromSqlAsync(_dbContext, "cpc_InsertContact", Param);
                try
                {
                    model.ConID = (int)paramConID.Value;
                }
                catch (Exception ex)
                {

                }
                if (model.lngMemID <= 0)
                {
                    model.lngMemID = _entityRepository.GetEntities().Select(m => m.Id).Max();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rowsAffected;
        }
        /// <summary>
        /// get Find Project here view value
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<FindProjectModel> GetDashboardProjectsAsync(DisplayLoginInfo info)
        {
            FindProjectModel response = new();
            response.ActiveProjs = new();
            response.FutureProjs = new();
            response.PrevProjs = new();
            try
            {
                response.ActiveProjs = await _dbContext.FindProjectView
        .FromSqlRaw("EXEC SP_FindProjectHere @MemberId, @ConId", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId))
        .ToListAsync();
                response.PrevProjs = await _dbContext.FindProjectView
        .FromSqlRaw("EXEC SP_FindProjectHerePrev @MemberId, @ConId", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId))
        .ToListAsync();
                response.FutureProjs = await _dbContext.FindProjectView
.FromSqlRaw("EXEC SP_FindProjectHereFuture @MemberId, @ConId", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId))
.ToListAsync();
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
            return response;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TblProjectPreview>> GetProjectdetail(int id)
        {

            Dictionary<string, object> Params = new();
            Params.Add("@ProjId", id);
            string ProcName = "GetProjectDetail";
            List<TblProjectPreview> response = new();
            try
            {
                /*dynamic response =*/
                //var results = await GeneralMethods.DynamicListFromSqlAsync(_dbContext, ProcName, Params);
                //response = results.Tol.Select(x => new TblProjectPreview
                //{
                //	ProjId = x.ProjId,
                //	ProjNote = x.ProjNote,
                //}).ToList();
                var results = await _dbContext.tblProject.SingleOrDefaultAsync(m => m.ProjId == id);

                TblProjectPreview item = new();
                if (results != null)
                {
                    item.ProjId = results.ProjId;
                    item.ProjNote = results.ProjNote;
                    item.ProjTypeId = results.ProjTypeId;
                    item.LocAddr1 = results.LocAddr1;
                    item.BidDt = results.BidDt;
                    item.PreBidDt = results.PreBidDt;

                }
                response.Add(item);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Get package name on behalf of member type
        /// </summary>
        /// <param name="id">membertype</param>
        /// <returns></returns>
        public string GetPackage(int id)
        {
            if (!_dbContext.TblMemberTypeCounty.Any(m => m.MemberType == id))
            {
                id = 4;
            }
            string package = (from x in _dbContext.TblMemberTypeCounty where x.MemberType == id select x.Package).FirstOrDefault();
            if (package != null)
                return package;
            else
                return package;
        }
        /// <summary>
        /// Get Member profile of staff and member
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ConId"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public async Task<MemberShipRegistration> GetMemberProfileAsync(int id, int ConId, string UserName)
        {
            MemberShipRegistration response = new MemberShipRegistration();
            try
            {
                TblMember tbl = _entityRepository.GetEntities().SingleOrDefault(t => t.Id == id);

                // MemberShipRegistration item = new();
                if (tbl != null)
                {
                    DateTime lastpaydate = Convert.ToDateTime(tbl.RenewalDate);
                    DateTime currentdate = DateTime.Now.Date;
                    if (currentdate > lastpaydate)
                    {
                        tbl.Inactive = true;
                       await _entityRepository.UpdateEntityAsync(tbl);
                    }
                    TblContact contact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.Id == tbl.Id && x.MainContact == true);
                    response.Grace = tbl.Grace;
                    response.Company = tbl.Company;
                    response.InsertDate = tbl.InsertDate;
                    response.UserName = UserName;
                    response.Dba = tbl.Dba;
                    response.Email = tbl.Email;
                    response.Logo = tbl.Logo;
                    response.MemberType = (tbl.MemberType).ToString();
                    response.CheckDirectory = tbl.CheckDirectory;
                    response.Term = tbl.Term;
                    response.Discount = tbl.Discount;
                    response.MemberCost = ((decimal)tbl.MemberCost).ToString("F2");
                    response.Package = string.IsNullOrEmpty(tbl.MemberType.ToString()) ? "Not Known" : GetPackage(Convert.ToInt32(tbl.MemberType));
                    if (!string.IsNullOrEmpty(tbl.Email))
                    {
                        response.Email = tbl.Email;
                    }
                    else
                    {
                        if (contact != null)
                        {
                            response.Email = contact.Email;
                        }
                        else
                        {
                            response.Email = "";
                        }
                    }
                    response.ContactName = contact != null ? contact.Contact : "";
                    if (!string.IsNullOrEmpty(tbl.CompanyPhone))
                    {
                        response.CompanyPhone = tbl.CompanyPhone;
                    }
                    else
                    {
                        if (contact != null)
                        {
                            response.CompanyPhone = contact.Phone;
                        }
                        else
                        {
                            response.ContactPhone = "";
                        }
                    }
                    response.RenewalDate = tbl.RenewalDate;
                    response.LastPayDate = tbl.LastPayDate;
                    response.IsAutoRenew = tbl.IsAutoRenew;
                    response.Inactive = tbl.Inactive;
                    response.ID = tbl.Id;
                    response.BillAddress = tbl.BillAddress;
                    response.MailAddress = tbl.MailAddress;
                    if (!string.IsNullOrEmpty(tbl.BillAddress))
                    {
                        response.ConsBillAddress = tbl.BillAddress;
                        response.ConsBillAddress = string.IsNullOrEmpty(tbl.BillCity) ? response.ConsBillAddress : response.ConsBillAddress + ", " + tbl.BillCity;
                        response.ConsBillAddress = string.IsNullOrEmpty(tbl.BillState) ? response.ConsBillAddress : response.ConsBillAddress + ", " + tbl.BillState;
                        response.ConsBillAddress = string.IsNullOrEmpty(tbl.BillZip) ? response.ConsBillAddress : response.ConsBillAddress + " " + tbl.BillZip;
                    }
                    response.BillCity = tbl.BillCity;
                    response.BillState = (from x in _dbContext.TblState where x.State == tbl.BillState select x.StateId.ToString()).FirstOrDefault();
                    response.BillZip = tbl.BillZip;
                    if (!string.IsNullOrEmpty(tbl.MailAddress))
                    {
                        response.ConsMailAddress = tbl.MailAddress;
                        response.ConsMailAddress = string.IsNullOrEmpty(tbl.MailCity) ? response.ConsMailAddress : response.ConsMailAddress + ", " + tbl.MailCity;
                        response.ConsMailAddress = string.IsNullOrEmpty(tbl.MailState) ? response.ConsMailAddress : response.ConsMailAddress + ", " + tbl.MailState;
                        response.ConsMailAddress = string.IsNullOrEmpty(tbl.MailZip) ? response.ConsMailAddress : response.ConsMailAddress + " " + tbl.MailZip;
                    }
                    response.MailCity = tbl.MailCity;
                    response.MailState = (from x in _dbContext.TblState where x.State == tbl.MailState select x.StateId.ToString()).FirstOrDefault();
                    response.MailZip = tbl.MailZip;
                    response.Discipline = tbl.Discipline;
                    response.ASPUserId = contact.UserId;
                    response.LicenseInfos = new();
                    int customDiv = 0;
                    Int32.TryParse(tbl.Div, out customDiv);
                    List<int> TempDiv = new();
                    if (customDiv > 0)
                        response.Div = tbl.Div;
                    else
                    {

                        response.Div = (from x in _dbContext.TblMemberDivs where x.MemberId == id && x.IsDeleted == false select x.WebDivId.ToString()).FirstOrDefault();

                    }
                    if (string.IsNullOrEmpty(response.Div))
                    {
                        response.DivisionList = null;
                    }
                    else
                    {
                        List<string> DivList = (from x in _dbContext.TblMemberDivs where x.MemberId == id && x.IsDeleted == false select x.WebDivId.ToString()).ToList();
                        TempDiv = (from x in _dbContext.TblMemberDivs where x.MemberId == id && x.IsDeleted == false select Convert.ToInt32(x.WebDivId)).ToList();
                        response.DivisionList = DivList;
                    }
                    StringBuilder sb = new();
                    if (TempDiv != null)
                    {
                        if (TempDiv.Count > 0)
                        {
                            foreach (int i in TempDiv)
                            {

                                if (i > 0)
                                {
                                    TblWebDiv WebDiv = await _dbContext.TblWebDivs.SingleOrDefaultAsync(x => x.DivNo == i);
                                    if (WebDiv != null)
                                    {
                                        string WebDivName = WebDiv.DivName;
                                        string WebDivDisc = WebDiv.DivDesc;
                                        if (WebDivName.Contains(WebDivDisc))
                                        {
                                            WebDivName = WebDivName.Replace(WebDivDisc, "");
                                            WebDivName = WebDivName.Replace(" - ", "");
                                        }
                                        sb.Append(WebDivName + ", ");
                                    }

                                }
                            }

                            response.Div = sb.ToString();
                            int CommaIndex = 0;
                            CommaIndex = response.Div.LastIndexOf(",");
                            response.Div = response.Div.Substring(0, CommaIndex);
                        }
                    }
                    response.MinorityStatus = tbl.MinorityStatus;
                    List<tblLicense> licenses = await _dbContext.tblLicense.Where(m => m.MemId == id).ToListAsync();
                    IEnumerable<string> uniqueLicNum = licenses.Select(x => x.LicNum).Distinct();
                    if (licenses != null)
                    {
                        if (licenses.Count > 0)
                        {
                            foreach (var item in licenses)
                            {
                                TblState tblState = await _dbContext.TblState.SingleOrDefaultAsync(x => x.StateId == item.LicState);
                                string StateText = tblState.State;
                                response.License += "(" + StateText + ")" + item.LicNum + " | ";
                            }
                            int DelIndex = response.License.LastIndexOf("|") - 1;
                            response.License = response.License.Substring(0, DelIndex);
                        }
                    }
                    else
                    {
                        response.License = "";
                    }
                    if (uniqueLicNum != null)
                    {

                        foreach (var item in uniqueLicNum)
                        {
                            List<int?> stId = new();
                            LicenseInfo info = new();
                            stId = licenses.Where(x => x.LicNum == item).Select(x => x.LicState).ToList();
                            info.LicDesc = licenses.Where(x => x.LicNum == item).Select(x => x.LicDesc).ToList().First();
                            info.LicNum = item;
                            info.State = stId;
                            response.LicenseInfos.Add(info);
                        }
                    }
                }
                if (ConId != 0)
                {
                    TblContact _tblcontact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Id == id && m.ConId == ConId);
                    if (_tblcontact != null)
                    {
                        response.MainContact = _tblcontact.MainContact;
                    }
                }
                List<MemberContactInfo> contactList = (from c in _dbContext.TblContacts where c.Id == id && c.Active == true && c.CompType == 1 select c).ToList()
                                   .Select(c => new MemberContactInfo
                                   {

                                       Contact = c.Contact,
                                       Phone = c.Phone,
                                       Email = c.Email,
                                       ConID = c.ConId,
                                       MainContact = c.MainContact,
                                       Daily = c.Daily,
                                       LocId = c.LocId
                                   }).ToList();
                response.ContactList = contactList;

                List<NoteInfo> NoteList = (from c in _dbContext.TblMemNotes where c.MemId == id && c.Flag == false select c).ToList()
                                   .Select(c => new NoteInfo
                                   {
                                       Id = c.Id,
                                       Note = c.Note,
                                       LogDate = c.LogDate

                                   }).ToList();
                response.NoteList = NoteList;
                TblDirectoryCheck check = await _dbContext.TblDirectoryCheck.SingleOrDefaultAsync(x => x.MemId == id);
                if (check != null)
                {
                    response.DirectoryCheck = check;
                }
                else
                {
                    response.DirectoryCheck = new();
                }
                List<TblLocList> locList = await _dbContext.TblLocList.Where(x => x.MemId == id).ToListAsync();
                if (locList != null)
                {
                    response.LocationsList = locList.Select(x => new LocListViewModel
                    {
                        LocStateCode = _dbContext.TblState.SingleOrDefault(m => m.State == x.LocState).StateId,
                        LocState = x.LocState,
                        LocAddr = x.LocAddr,
                        LocCity = x.LocCity,
                        LocCounty = x.LocCounty,
                        LocId = x.LocId,
                        LocPhone = x.LocPhone,
                        LocZip = x.LocZip
                    }).ToList();
                    string loc = "";
                    foreach (var location in locList)
                    {
                        loc = location.LocAddr;
                        loc = string.IsNullOrEmpty(location.LocCity) ? loc : loc + ", " + location.LocCity;
                        loc = string.IsNullOrEmpty(location.LocCounty) ? loc : loc + ", " + location.LocCounty;
                        loc = string.IsNullOrEmpty(location.LocState) ? loc : loc + ", " + location.LocState;
                        loc = string.IsNullOrEmpty(location.LocZip) ? loc : loc + " " + location.LocZip;
                        if (string.IsNullOrEmpty(location.LocPhone))
                            response.Locations.Add(loc, "");
                        else
                            response.Locations.Add(loc, location.LocPhone);
                    }
                }
                else
                {
                    response.LocationsList = new();
                }

            }
            catch (Exception ex)
            {

                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Set Auto renew on at member profile page of staff and member site
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<dynamic> autoRenewOn(string sId, bool val)
        {
            int Id = Convert.ToInt32(sId);
            TblMember member = _entityRepository.GetEntities().Single(s => s.Id == Id);
            member.IsAutoRenew = val;

            _entityRepository.UpdateEntity(member);

            return member;
        }
        /// <summary>
        /// Set Auto renew off at member profile page of staff and member site
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<dynamic> autoRenewOff(string sId, bool val)
        {
            int Id = Convert.ToInt32(sId);
            TblMember member = _entityRepository.GetEntities().Single(s => s.Id == Id);
            member.IsAutoRenew = val;
            _entityRepository.UpdateEntity(member);
            return member;
        }
        /// <summary>
        /// Set member active at member profile page of staff and member site
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<dynamic> activemember(string sId, bool val)
        {
            int Id = Convert.ToInt32(sId);
            TblMember member = _entityRepository.GetEntities().Single(s => s.Id == Id);
            member.Inactive = val;
            _entityRepository.UpdateEntity(member);
            return member;
        }
        /// <summary>
        /// Set member inactive at member profile page of staff and member site
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<dynamic> inactivemember(string sId, bool val)
        {
            int Id = Convert.ToInt32(sId);
            TblMember member = _entityRepository.GetEntities().Single(s => s.Id == Id);
            member.Inactive = val;
            _entityRepository.UpdateEntity(member);
            return member;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>

        public async Task<IEnumerable<ProjectInformation>> GetMemberDashboardProjectsAsync(string username)
        {
            //MyConnection con = new MyConnection(_connectionString);
            //con.cmd.CommandText = "sp_GetYourProjectDashboadInfo";
            //con.cmd.CommandType = CommandType.StoredProcedure;
            //con.cmd.Parameters.AddWithValue("@username", username);



            string useremail = username;
            IEnumerable<ProjectInformation> projects = new List<ProjectInformation>();
            //int xUID = 0;
            //xUID = await _dbContext.TblContacts.Select(m => m.Id).MaxAsync();
            int MemberIDs = (from c in _dbContext.TblContacts where c.Email == username select c.Id).FirstOrDefault();
            int ConIds = (from c in _dbContext.TblContacts where c.Email == username select c.ConId).FirstOrDefault();
            string UID = (from c in _dbContext.TblContacts where c.Email == username select c.Uid).FirstOrDefault();
            string userid = Convert.ToString(ConIds);
            //List<TblProject> tblProjects = await _dbContext.tblProject.Where(M => M.memberId == MemberIDs).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            List<TblProject> tblProjects = await _dbContext.tblProject.Where(m => m.ProjId != null).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            //List<tblTracking> tbltracks = await _dbContext.TblTracking.Where(M => M.ConId == ConIds).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            try
            {
                if (tblProjects != null)
                {
                    projects = tblProjects.ToList().Where(M => M.IsActive != false && !string.IsNullOrEmpty(M.ProjNumber)).Select(x => new ProjectInformation
                    {
                        Title = x.Title,
                        ArrivalDt = x.ArrivalDt,
                        BidDt = x.BidDt,
                        strBidDt5 = x.StrBidDt5,
                        BidDt2 = x.BidDt2,
                        BidDt3 = x.BidDt3,
                        BidDt4 = x.BidDt4,
                        BidDt5 = x.BidDt5,
                        strBidDt = x.StrBidDt,
                        ProjTypeId = x.ProjTypeId,
                        ProjTypeIdString = (x.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(y => y.ProjTypeId == x.ProjTypeId) != null ? _dbContext.TblProjType.SingleOrDefault(y => y.ProjTypeId == x.ProjTypeId).ProjType : "") : ""),
                        ProjId = x.ProjId,
                        memberId = x.memberId,
                        CurrMemberId = MemberIDs,
                        MemberTrack = x.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == x.ProjId && y.ConId == ConIds) != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == x.ProjId && y.ConId == ConIds).IsTracking) : false) : true,
                        TrackCount = (from c in _dbContext.TblTracking where c.ProjId == x.ProjId && c.IsTracking == true select c.ProjId).Count(),
                        BidCount = (from c in _dbContext.TblBidStatus where c.Projid == x.ProjId && c.Bidding == true select c.Projid).Count(),
                        BiddingStatus = x.ProjId != null ? (_dbContext.TblBidStatus.SingleOrDefault(y => y.Projid == x.ProjId && y.Uid == UID) != null ? (_dbContext.TblBidStatus.SingleOrDefault(y => y.Projid == x.ProjId && y.Uid == UID).Bidding) : false) : true,
                        //BiddingStatus = x.ProjId != null ? (_dbContext.TblBidStatus.SingleOrDefault(y => y.Projid == x.ProjId && y.Uid == userid) != null ? (_dbContext.TblBidStatus.SingleOrDefault(y => y.Projid == x.ProjId && y.Uid == userid).Bidding) : false) : true,
                        chkCalendar = x.ProjId == null ? false : (_dbContext.TblCalendarNotice.SingleOrDefault(m => m.MemberId == MemberIDs && m.ProjId == x.ProjId) == null ? false : !(_dbContext.TblCalendarNotice.SingleOrDefault(m => m.MemberId == MemberIDs && m.ProjId == x.ProjId).Disable))
                    }).ToList();

                }
            }
            catch (Exception Ex)
            {

                throw;
            }
            return projects;
        }
        /// <summary>
        /// Member dashboard function
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<MemberDashboard> GetMemberDashboardProjects(DisplayLoginInfo info)
        {
            MemberDashboard memberDashboard = new();
            memberDashboard.MemberProject = new();
            memberDashboard.TrackedProject = new();
            //string useremail = username;
            List<ProjectInformation> projects = new List<ProjectInformation>();
            //int xUID = 0;
            //xUID = await _dbContext.TblContacts.Select(m => m.Id).MaxAsync();

            //int MemberIDs = await (from c in _dbContext.TblContacts where c.Email == username select c.Id).FirstOrDefaultAsync();
            //int ConIds = await (from c in _dbContext.TblContacts where c.Email == username select c.ConId).FirstOrDefaultAsync();
            //string UID = (from c in _dbContext.TblContacts where c.Email == username select c.Uid).FirstOrDefault();
            //string userid = Convert.ToString(ConIds);
            ////List<TblProject> tblProjects = await _dbContext.tblProject.Where(M => M.memberId == MemberIDs && M.IsActive != false && !string.IsNullOrEmpty(M.ProjNumber) && M.ProjNumber != "0").OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            //List<TblProject> tblProjects = await _dbContext.tblProject.Where(m => m.ProjId != null).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            List<tblTracking> tbltracks = await _dbContext.TblTracking.Where(M => M.ConId == info.ConId).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();

            try
            {
                memberDashboard.MemberProject = await _dbContext.MemberProjectsView
        .FromSqlRaw("EXEC SP_GetMemberDashBoardData @MemberId", new SqlParameter("@MemberId", info.Id))
        .ToListAsync();
                memberDashboard.TrackedProject = await _dbContext.TrackedProjectsView.FromSqlRaw("EXEC SP_GetTrackedProjDetail @MemberId, @ConId, @ConIdStr, @Uid", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId), new SqlParameter("@ConIdStr", info.ConId.ToString()), new SqlParameter("@Uid", info.Uid))
        .ToListAsync();

            }
            catch (Exception)
            {

                throw;
            }

            return memberDashboard;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <param name="usern"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectInformation>> GetMemberDashboardSearchProjectsAsync(string searchTxt, string usern)
        {
            int MemberIDs = (from c in _dbContext.TblContacts where c.Email == usern select c.Id).FirstOrDefault();
            int ConIds = (from c in _dbContext.TblContacts where c.Email == usern select c.ConId).FirstOrDefault();
            string userid = Convert.ToString(ConIds);
            List<TblProject> tbl2 = new();
            IEnumerable<ProjectInformation> response = new List<ProjectInformation>();
            try
            {
                tbl2 = await _dbContext.tblProject.Where(M => (M.memberId == MemberIDs) && (M.Title.Contains(searchTxt))).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
                //tbl2 = await _dbContext.tblProject.Where(M => M.Title.Contains(searchTxt)).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
                response = tbl2.Select(t => new ProjectInformation
                {
                    ProjId = t.ProjId,
                    ProjNumber = t.ProjNumber,
                    Publish = Convert.ToBoolean(t.Publish),
                    SpcChk = Convert.ToBoolean(t.SpcChk),
                    SpecsOnPlans = Convert.ToBoolean(t.SpecsOnPlans),
                    Title = t.Title,
                    LocCity = t.LocCity,
                    LocState = t.LocState,
                    ProjTypeIdString = t.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId) != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId).ProjType) : "") : "",
                    BidDt = t.BidDt,
                    ArrivalDt = t.ArrivalDt,

                    CurrMemberId = MemberIDs,
                    MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIds) != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIds).IsTracking) : false) : true,
                    TrackCount = (from c in _dbContext.TblTracking where c.ProjId == t.ProjId && c.IsTracking == true select c.ProjId).Count(),
                    BidCount = (from c in _dbContext.TblBidStatus where c.Projid == t.ProjId && c.Bidding == true select c.Projid).Count(),
                    BiddingStatus = t.ProjId != null ? (_dbContext.TblBidStatus.SingleOrDefault(y => y.Projid == t.ProjId && y.Uid == userid) != null ? (_dbContext.TblBidStatus.SingleOrDefault(y => y.Projid == t.ProjId && y.Uid == userid).Bidding) : false) : true,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;

        }
        /// <summary>
        /// For updating profile from member profile on stAFF AND MEMBER
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> EditMemberProfile(MemberShipRegistration model)
        {
            model.BillState = GetSelectedStateText(model.BillState);
            model.MailState = GetSelectedStateText(model.MailState);
            StringBuilder sb = new();
            if (model.DivisionList != null)
            {
                foreach (var item in model.DivisionList)
                {
                    if (item != "0")
                        sb.Append(item + ",");
                }
                string fBuilder = sb.ToString();
                if (fBuilder.Contains(','))
                {
                    int place = fBuilder.LastIndexOf(',');
                    fBuilder = fBuilder.Remove(place);
                }
                model.Div = fBuilder;
            }
            else
            {
                model.Div = "";
            }
            //model.Div = GetSelectedStateText(model.Div);
            TblMember member = _entityRepository.GetEntities().SingleOrDefault(s => s.Id == model.ID);
            member.Company = model.Company;
            string fileName = Path.GetFileName(model.Logo);
            if (fileName != null)
            {
                member.Logo = "/Profile/" + fileName;
            }

            member.Dba = model.Dba;
            member.Email = model.Email;
            member.Discipline = model.Discipline;
            member.Fax = model.Fax;
            member.BillAddress = model.BillAddress;
            member.BillCity = model.BillCity;
            member.BillState = model.BillState;
            member.BillZip = model.BillZip;
            member.MailAddress = model.MailAddress;
            member.MailCity = model.MailCity;
            member.MailState = model.MailState;
            member.MailZip = model.MailZip;
            member.Div = model.Div;
            member.MinorityStatus = model.MinorityStatus;

            _entityRepository.UpdateEntity(member);

            TblContact contacts = await _dbContext.TblContacts.SingleOrDefaultAsync(x => x.Id == model.ID && x.Contact == model.ContactName);
            if (contacts != null)
            {
                contacts.Phone = model.CompanyPhone;
                contacts.Email = model.Email;
                _dbContext.Entry(contacts).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            if (model.ContactList != null)
            {
                foreach (var item in model.ContactList)
                {
                    TblContact contact = await _dbContext.TblContacts.SingleOrDefaultAsync(x => x.ConId == item.ConID);
                    if (contact != null)
                    {
                        contact.Contact = item.Contact;
                        contact.Phone = item.Phone;
                        contact.Email = item.Email;
                        _dbContext.Entry(contact).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                }

            }

            if (model.DivisionList != null)
            {
                foreach (var item in model.DivisionList)
                {

                    if (item != "0")
                    {
                        TblMemberDiv MemDiv = await _dbContext.TblMemberDivs.SingleOrDefaultAsync(x => (x.MemberId == model.ID) && (x.WebDivId == Convert.ToInt32(item)));
                        if (MemDiv == null)
                        {
                            MemDiv = new();
                            MemDiv.WebDivId = Convert.ToInt32(item);
                            MemDiv.MemberId = model.ID;
                            MemDiv.IsDeleted = false;
                            _dbContext.TblMemberDivs.Add(MemDiv);

                            try
                            {
                                await _dbContext.SaveChangesAsync();
                            }
                            catch (Exception Ex)
                            {

                                throw;
                            }
                        }
                        else
                        {
                            if (MemDiv.IsDeleted == true)
                            {
                                MemDiv.IsDeleted = false;
                                _dbContext.Entry(MemDiv).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();
                            }
                            if (MemDiv.IsDeleted == null)
                            {
                                MemDiv.IsDeleted = false;
                                _dbContext.Entry(MemDiv).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();
                            }

                        }
                    }
                }
            }
            List<int> tbl = await (from tab in _dbContext.TblMemberDivs where tab.MemberId == model.ID select Convert.ToInt32(tab.WebDivId)).ToListAsync();
            List<int> toDelete = new();
            foreach (var item in tbl)
            {
                int counter = 0;
                if (model.DivisionList != null)
                {
                    foreach (var div in model.DivisionList)
                    {
                        if (item == Convert.ToInt32(div))
                        {
                            counter = -1;
                        }
                        else
                        {
                            if (counter != -1)
                                counter++;
                        }

                    }
                }
                if (counter > 0)
                    toDelete.Add(item);
            }
            foreach (int i in toDelete)
            {
                if (i != 0)
                {
                    TblMemberDiv memberDiv = await _dbContext.TblMemberDivs.SingleOrDefaultAsync(x => (x.MemberId == model.ID) && (x.WebDivId == i));
                    memberDiv.IsDeleted = true;
                    _dbContext.Entry(memberDiv).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
            }
            if (model.LicenseInfos != null)
            {
                List<int> LicIds = new();
                foreach (var information in model.LicenseInfos)
                {
                    foreach (int i in information.State)
                    {
                        tblLicense tblLicense = new();
                        tblLicense.LicDesc = information.LicDesc;
                        tblLicense.LicState = i;
                        tblLicense.MemId = model.ID;
                        tblLicense.LicNum = information.LicNum;
                        await _dbContext.tblLicense.AddAsync(tblLicense);
                        await _dbContext.SaveChangesAsync();
                        LicIds.Add(tblLicense.LicId);
                    }
                }
                List<tblLicense> licenses = await _dbContext.tblLicense.Where(x => x.MemId == model.ID).ToListAsync();
                foreach (int id in licenses.Select(x => x.LicId))
                {
                    int flag = 0;
                    foreach (int ids in LicIds)
                    {
                        if (id == ids)
                            flag = 1;
                    }
                    if (flag == 0)
                    {
                        tblLicense toDel = await _dbContext.tblLicense.FirstOrDefaultAsync(x => x.LicId == id);
                        if (toDel != null)
                        {
                            _dbContext.Entry(toDel).State = EntityState.Deleted;
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            if (model.LocationsList != null)
            {
                foreach (var loc in model.LocationsList)
                {
                    TblLocList locList = await _dbContext.TblLocList.SingleOrDefaultAsync(x => x.LocId == loc.LocId);
                    if (locList != null)
                    {
                        locList.MemId = model.ID;
                        locList.LocAddr = loc.LocAddr;
                        locList.LocCity = loc.LocCity;
                        locList.LocCounty = loc.LocCounty;
                        locList.LocState = loc.LocState;
                        locList.LocZip = loc.LocZip;
                        locList.LocPhone = loc.LocPhone;
                        _dbContext.Entry(locList).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            return member;
        }
        /// <summary>
        /// To add new user from member profile on stAFF AND MEMBER
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> AddUserNew(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            TblContact user = new();
            try
            {
                var _tblContact = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.ID);
                if (_tblContact.MemberType == 7)
                {
                    var tblContact = await _dbContext.TblContacts.Where(m => m.Id == model.ID).ToListAsync();
                    if (tblContact.Count >= 10)
                    {
                        response.success = false;
                        response.statusCode = "400";
                        response.statusMessage = "Maximum user created";
                        return response;
                    }

                }
                if (_tblContact.MemberType == 4 || _tblContact.MemberType == 5 || _tblContact.MemberType == 8 || _tblContact.MemberType == 9 || _tblContact.MemberType == 10 || _tblContact.MemberType == 11)
                {
                    var tblContact = await _dbContext.TblContacts.Where(m => m.Id == model.ID).ToListAsync();
                    if (tblContact.Count >= 3)
                    {
                        response.success = false;
                        response.statusCode = "400";
                        response.statusMessage = "Maximum user created";
                        return response;
                    }

                }
                if (_tblContact.MemberType == 1 || _tblContact.MemberType == 12)
                {
                    response.success = false;
                    response.statusCode = "400";
                    response.statusMessage = "you are not eligible for create user";
                    return response;


                }
                else
                {
                    user.Id = model.ID;
                    user.Contact = model.FirstName + " " + model.LastName;
                    user.Email = model.ContactEmail;
                    user.Phone = model.ContactPhone;
                    user.Password = model.ContactPassword;
                    user.Uid = await CreateUid();
                    user.LocId = model.LocId;
                    user.CompType = 1;
                    user.Active = true;
                    user.MainContact = false;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    await _dbContext.TblContacts.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "User added successfully";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectInformation>> GetSearchFindProjectsAsync(string searchTxt)
        {
            IEnumerable<ProjectInformation> response = new List<ProjectInformation>();
            try
            {
                var tbl2 = await _dbContext.tblProject.Where(M => (M.Title.Contains(searchTxt) || M.LocState.Contains(searchTxt))).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
                response = tbl2.Select(t => new ProjectInformation
                {
                    ProjId = t.ProjId,
                    ProjNumber = t.ProjNumber,
                    Publish = Convert.ToBoolean(t.Publish),
                    SpcChk = Convert.ToBoolean(t.SpcChk),
                    SpecsOnPlans = Convert.ToBoolean(t.SpecsOnPlans),
                    Title = t.Title,
                    LocCity = t.LocCity,
                    LocState = t.LocState,
                    strAddenda = t.StrAddenda,
                    strBidDt = t.StrBidDt,
                    PlanNo = t.PlanNo,
                    ProjTypeIdString = t.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId) != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId).ProjType) : "") : "",
                    BidDt = t.BidDt,
                    ArrivalDt = t.ArrivalDt
                }).ToList();


                // modell = await _dbContext.tblProject.Where(M => (M.Title.Contains(searchTxt))).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;

        }
        /// <summary>
        /// Search from find project here page
        /// </summary>
        /// <param name="project"></param>
        /// <param name="model"></param>
        /// <param name="from"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<FindProjectModel> GetSortByStateFindProjectsAsync(List<ProjectInformation> project, SearchViewModel model, int from, DisplayLoginInfo info)
        {
            FindProjectModel result = new();
            result.FutureProjs = new();
            result.PrevProjs = new();
            result.ActiveProjs = new();
            DateTime BidDateFrom = string.IsNullOrEmpty(model.strBidDateFrom) ? Convert.ToDateTime(_dbContext.tblProject.Min(x => x.BidDt)) : Convert.ToDateTime(model.strBidDateFrom);
            DateTime BidDateTo = string.IsNullOrEmpty(model.strBidDateTo) ? Convert.ToDateTime(_dbContext.tblProject.Max(x => x.BidDt)) : Convert.ToDateTime(model.strBidDateTo); ;
            List<ProjectInformation> response = new List<ProjectInformation>();
            List<ProjectInformation> responseList = new List<ProjectInformation>();
            List<ProjectInformation> responseStateFilter = new List<ProjectInformation>();
            List<ProjectInformation> responseFilterObj = new List<ProjectInformation>();
            List<TblProject> tbl2 = new();
            try
            {//.Where(M => (M.LocState.Contains(searchTxt)))
                if (from == 1)
                {
                    if (project != null && project.Count > 0)
                    {
                        if (string.IsNullOrEmpty(model.strBidDateFrom) && string.IsNullOrEmpty(model.strBidDateTo))
                            responseList = project.Where(m => (m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo) || (m.BidDt == null)).OrderByDescending(t => t.ProjId).Take(1000).ToList();
                        else
                            responseList = project.Where(m => m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo).OrderByDescending(t => t.ProjId).Take(1000).ToList();
                    }
                    else
                    {
                        return result;
                    }
                }
                else if (from == 2)
                {
                    if (string.IsNullOrEmpty(model.strBidDateFrom) && string.IsNullOrEmpty(model.strBidDateTo))
                        tbl2 = await _dbContext.tblProject.Where(m => (m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo || (m.BidDt == null)) && m.Publish == true && m.IsActive != false && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0").OrderByDescending(t => t.ProjId).Take(1000).ToListAsync();
                    else
                        tbl2 = await _dbContext.tblProject.Where(m => (m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo) && m.Publish == true && m.IsActive != false && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0").OrderByDescending(t => t.ProjId).Take(1000).ToListAsync();

                                       

                    responseList = tbl2.Select(t => new ProjectInformation
                    {
                        ProjId = t.ProjId,
                        ProjNumber = t.ProjNumber,
                        Publish = Convert.ToBoolean(t.Publish),
                        SpcChk = Convert.ToBoolean(t.SpcChk),
                        SpecsOnPlans = Convert.ToBoolean(t.SpecsOnPlans),
                        Title = t.Title,
                        LocCity = t.LocCity,
                        LocState = t.LocState,
                        strAddenda = t.StrAddenda,
                        strBidDt = string.IsNullOrEmpty(t.StrBidDt) ? "" : t.StrBidDt,
                        strBidDt5 = t.StrBidDt5,
                        PlanNo = t.PlanNo,
                        ProjTypeIdString = t.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId) != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId).ProjType) : "") : "",
                        BidDt = t.BidDt,
                        ArrivalDt = t.ArrivalDt,
                        ProjTypeId = t.ProjTypeId,
                        ProjSubTypeId = t.ProjSubTypeId,
                        EstCost = t.EstCost,
                        EstCost2 = t.EstCost2,
                        EstCost3 = t.EstCost3,
                        EstCost4 = t.EstCost4,
                        EstCost5 = t.EstCost5,
                        ProjScope = t.ProjScope,
                        FutureWork = t.FutureWork is DBNull ? false : Convert.ToBoolean(t.FutureWork),
                        PrevailingWage = t.PrevailingWage is DBNull ? false : Convert.ToBoolean(t.PrevailingWage),
                        MBDCheck = _dbContext.tblBidDateTime.Where(x => x.ProjId == t.ProjId && x.IsDeleted == false && x.PhlId != null).Count() > 0 ? "Y" : "N"
                    }).ToList();
                }

                

                if (!string.IsNullOrEmpty(model.SearchText))
                {
                    var projs = responseList;
                    responseList = new();
                    responseList.AddRange(projs.Where(m => ((!string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0")
                    && m.ProjNumber.Contains(model.SearchText.ToLower())) || (!string.IsNullOrEmpty(m.Title) &&
                    m.Title.ToLower().Contains(model.SearchText.ToLower()))));

                    List<int> projIdlist = new();
                    if (!string.IsNullOrEmpty(model.SearchText))
                    {
                        var solrQuery = CreateSolrQuery(model.SearchText);

                        if (solrQuery != null)
                        {

                            var results = _solr.Query(solrQuery);
                            //    var solrQuery = new SolrQuery($"content:{model.SearchText}");
                            //var results = _solr.Query(solrQuery);
                            projIdlist = results.Where(doc => doc.Content != null && int.TryParse(doc.projectId.FirstOrDefault(), out _))
                                .Select(doc => int.Parse(doc.projectId.FirstOrDefault()))
                                .Distinct().ToList();
                        }
                        if (projIdlist != null && projIdlist.Count > 0)
                        {
                            //var projects = await _dbContext.tblProject.Where(m => (m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo) && m.Publish == true && m.IsActive != false && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0" && projIdlist.Contains(m.ProjId)).OrderByDescending(t => t.ProjId).Take(1000).ToListAsync();
                            var projects = projs.Where(m => projIdlist.Contains(m.ProjId)).ToList();
                            result.IsSolrSearched = true;
                            result.SolrKeyword = model.SearchText;

                            responseList.AddRange(projects);
                            responseList = responseList.Distinct().ToList();
                        }
                    }

                }



                //List<string> StateList = new List<string>();
                //responseList = new();
                //            bool PrevailingWageFlag = false;
                //            DateTime BitDatFrom = new DateTime();
                //            DateTime BidDateTo = new DateTime();
                //            int ProjectTypeId = 0;
                //            int ProjectSubTypeId = 0;
                //            string SearchText = string.Empty;
                responseStateFilter.AddRange(responseList);
                if (model.ProjectStates != null)
                {
                    if (model.ProjectStates.Count > 0)
                    {
                        responseStateFilter = new();
                        foreach (string stName in model.ProjectStates)
                        {
                            if (!string.IsNullOrEmpty(stName))
                            {
                                string tempSt = stName;
                                int chkState = 0;
                                int.TryParse(tempSt, out chkState);
                                if (chkState != 0)
                                    tempSt = GetSelectedStateText(stName);
                                List<ProjectInformation>? proj = responseList.Where(m => m.LocState != null && m.LocState.Contains(tempSt)).ToList();
                                if (proj != null)
                                    responseStateFilter.AddRange(proj);
                            }
                        }
                    }
                }
                responseList = new();

                // Filter by ProjectTypeId
                if (model.ProjectTypeIds != null && model.ProjectTypeIds.Count > 0)
                {
                    List<ProjectInformation> responseProjectTypeFilter = new List<ProjectInformation>();

                    foreach (var projectTypeId in model.ProjectTypeIds)
                    {
                        List<ProjectInformation> proj = responseStateFilter.Where(m => m.ProjTypeId == Convert.ToInt32(projectTypeId)).ToList();

                        if (proj != null)
                            responseProjectTypeFilter.AddRange(proj);
                    }

                    responseList = responseProjectTypeFilter;


                    // Filter by ProjectSubTypeIds
                    if (model.ProjectSubTypeIds != null && model.ProjectSubTypeIds.Count > 0)
                    {
                        List<ProjectInformation> responseProjectSubTypeFilter = new List<ProjectInformation>();

                        foreach (var projectSubTypeId in model.ProjectSubTypeIds)
                        {
                            List<ProjectInformation> proj = responseList.Where(m => m.ProjSubTypeId == Convert.ToInt32(projectSubTypeId)).ToList();

                            if (proj != null)
                                responseProjectSubTypeFilter.AddRange(proj);
                        }

                        responseStateFilter = responseProjectSubTypeFilter;
                    }
                    else
                    {
                        responseStateFilter = responseList;
                    }
                }
                responseList = responseStateFilter;

                if (model.ProjectScopes != null && model.ProjectScopes.Any() && responseStateFilter != null)
                {
                    responseStateFilter = responseStateFilter
                        .Where(item => item.ProjScope != null && model.ProjectScopes.Any(scope => item.ProjScope.Split(',').Contains(scope.Trim())))
                        .ToList();
                }


                if (model.PrevailingWageFlag)
                    responseFilterObj = responseStateFilter.Where(m => m.PrevailingWage == model.PrevailingWageFlag).ToList();
                else
                    responseFilterObj.AddRange(responseStateFilter);


                response = responseFilterObj;
                //Keyword Search including Solr Search
                
                responseFilterObj = new();
                foreach (var option in model.ProjectestCosts)
                {
                    if (option != null && option != "0")
                    {
                        int minRange = 0;
                        int maxRange = 0;
                        if (option.Contains('-'))
                        {
                            string[] costArr = option.Split('-');
                            int.TryParse(costArr[0], out minRange);
                            int.TryParse(costArr[1], out maxRange);
                        }
                        else
                        {
                            minRange = int.Parse(option);
                            maxRange = int.MaxValue;
                        }
                        if (response != null && response.Count > 0)
                        {
                            foreach (var item in response)
                            {
                                int mnEst = 0;
                                int mxEst = 0;
                                bool Added = false;
                                if (!string.IsNullOrEmpty(item.EstCost) && item.EstCost.Contains('-'))
                                {
                                    string[] costArr = item.EstCost.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[1], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost))
                                {
                                    int.TryParse(item.EstCost, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost2) && item.EstCost2.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost2.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost2) && Added == false)
                                {
                                    int.TryParse(item.EstCost2, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost3) && item.EstCost3.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost3.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost3) && Added == false)
                                {
                                    int.TryParse(item.EstCost3, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost4) && item.EstCost4.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost4.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost4) && Added == false)
                                {
                                    int.TryParse(item.EstCost4, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost5) && item.EstCost5.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost5.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost5) && Added == false)
                                {
                                    int.TryParse(item.EstCost5, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    response = new();
                    response.AddRange(responseFilterObj);
                }
                if (response != null && response.Count > 0)
                {
                    foreach (var item in response)
                    {
                        if ((item.BidDt == null || item.BidDt >= DateTime.Now.Date) && item.FutureWork == true)
                        {
                            FindProjectView findProjectView = new();
                            findProjectView.ProjId = item.ProjId;
                            findProjectView.Title = item.Title;
                            findProjectView.LocAddr1 = item.LocAddr1;
                            findProjectView.LocAddr2 = item.LocAddr2;
                            findProjectView.LocCity = item.LocCity;
                            findProjectView.LocState = item.LocState;
                            findProjectView.LocZip = item.LocZip;
                            findProjectView.strBidDt5 = item.strBidDt5;
                            findProjectView.ImportDt = item.ImportDt;
                            findProjectView.ProjNumber = item.ProjNumber;
                            findProjectView.BidDt = item.BidDt;
                            findProjectView.ArrivalDt = item.ArrivalDt;
                            findProjectView.strBidDt = item.strBidDt;
                            findProjectView.Publish = item.Publish;
                            findProjectView.SpcChk = item.SpcChk;
                            findProjectView.SpecsOnPlans = item.SpecsOnPlans;
                            findProjectView.MemberTrack = _dbContext.TblTracking.Where(x => x.ProjId == item.ProjId && x.IsTracking == true && x.ConId == info.ConId).Count() > 0 ? true : false;
                            findProjectView.ProjTypeIdString = item.ProjTypeIdString;
                            findProjectView.TrackCount = _dbContext.TblTracking.Where(x => x.ProjId == item.ProjId && x.IsTracking == true).Count();
                            findProjectView.MBDCheck = item.MBDCheck;
                            findProjectView.AddendaCount = _dbContext.TblSubAddenda.Where(x => x.ProjId == item.ProjId && x.Deleted == false).Count();
                            result.FutureProjs.Add(findProjectView);
                        }
                        if (item.BidDt == null || Convert.ToDateTime(item.BidDt).Date >= DateTime.Now.Date)
                        {
                            FindProjectView findProjectView = new();
                            findProjectView.ProjId = item.ProjId;
                            findProjectView.Title = item.Title;
                            findProjectView.LocAddr1 = item.LocAddr1;
                            findProjectView.LocAddr2 = item.LocAddr2;
                            findProjectView.LocCity = item.LocCity;
                            findProjectView.LocState = item.LocState;
                            findProjectView.LocZip = item.LocZip;
                            findProjectView.strBidDt5 = item.strBidDt5;
                            findProjectView.ImportDt = item.ImportDt;
                            findProjectView.ProjNumber = item.ProjNumber;
                            findProjectView.BidDt = item.BidDt;
                            findProjectView.ArrivalDt = item.ArrivalDt;
                            findProjectView.strBidDt = item.strBidDt;
                            findProjectView.Publish = item.Publish;
                            findProjectView.SpcChk = item.SpcChk;
                            findProjectView.SpecsOnPlans = item.SpecsOnPlans;
                            findProjectView.MemberTrack = _dbContext.TblTracking.Where(x => x.ProjId == item.ProjId && x.IsTracking == true && x.ConId == info.ConId).Count() > 0 ? true : false;
                            findProjectView.ProjTypeIdString = item.ProjTypeIdString;
                            findProjectView.TrackCount = _dbContext.TblTracking.Where(x => x.ProjId == item.ProjId && x.IsTracking == true).Count(); ;
                            findProjectView.MBDCheck = item.MBDCheck;
                            findProjectView.AddendaCount = _dbContext.TblSubAddenda.Where(x => x.ProjId == item.ProjId && x.Deleted == false).Count();
                            result.ActiveProjs.Add(findProjectView);
                        }
                        if (item.BidDt != null && Convert.ToDateTime(item.BidDt).Date < DateTime.Now.Date)
                        {
                            FindProjectView findProjectView = new();
                            findProjectView.ProjId = item.ProjId;
                            findProjectView.Title = item.Title;
                            findProjectView.LocAddr1 = item.LocAddr1;
                            findProjectView.LocAddr2 = item.LocAddr2;
                            findProjectView.LocCity = item.LocCity;
                            findProjectView.LocState = item.LocState;
                            findProjectView.LocZip = item.LocZip;
                            findProjectView.strBidDt5 = item.strBidDt5;
                            findProjectView.ImportDt = item.ImportDt;
                            findProjectView.ProjNumber = item.ProjNumber;
                            findProjectView.BidDt = item.BidDt;
                            findProjectView.ArrivalDt = item.ArrivalDt;
                            findProjectView.strBidDt = item.strBidDt;
                            findProjectView.Publish = item.Publish;
                            findProjectView.SpcChk = item.SpcChk;
                            findProjectView.SpecsOnPlans = item.SpecsOnPlans;
                            findProjectView.MemberTrack = _dbContext.TblTracking.Where(x => x.ProjId == item.ProjId && x.IsTracking == true && x.ConId == info.ConId).Count() > 0 ? true : false;
                            findProjectView.ProjTypeIdString = item.ProjTypeIdString;
                            findProjectView.TrackCount = _dbContext.TblTracking.Where(x => x.ProjId == item.ProjId && x.IsTracking == true).Count();
                            findProjectView.MBDCheck = item.MBDCheck;
                            findProjectView.AddendaCount = _dbContext.TblSubAddenda.Where(x => x.ProjId == item.ProjId && x.Deleted == false).Count();
                            result.PrevProjs.Add(findProjectView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return result;

        }



        public async Task<List<TblProject>> GetFilteredProjectsAsync(DisplayLoginInfo info)
        {
            List<TblProject> tbl2 = new();

            try
            {
                DateTime BidDateFrom = Convert.ToDateTime(_dbContext.tblProject.Min(x => x.BidDt));
                DateTime BidDateTo = Convert.ToDateTime(_dbContext.tblProject.Max(x => x.BidDt));

                tbl2 = await _dbContext.tblProject
                    .Where(m => (m.IsActive == null || m.IsActive == true)
                                && m.Publish == true
                                && (
                                    (m.BidDt != null && m.BidDt >= DateTime.Now.Date)
                                    || (m.BidDt >= DateTime.Now.AddMonths(-18) && m.BidDt < DateTime.Now.Date)
                                    || (m.BidDt == null)
                                )
                                && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0")
                    .OrderByDescending(t => t.ProjId)
                    .Take(2000)
                    .ToListAsync();


                var futureProj = tbl2.Where(m => (m.BidDt == null || m.BidDt >= DateTime.Now.Date) && m.FutureWork == true).ToList();
                tbl2.AddRange(futureProj);


            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return tbl2;
        }

        /// <summary>
        /// No use
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MemberShipRegistration>> GetMembersSearchAsync(string searchTxt)
        {
            //int MemberIDs = (from c in _dbContext.TblContacts where c.Email == usern select c.Id).FirstOrDefault();
            List<TblMember> tbl2 = new();
            IEnumerable<MemberShipRegistration> response = new List<MemberShipRegistration>();
            try
            {
                //tbl2 = _entityRepository.GetEntities().Where(M =>(M.Company.Contains(searchTxt)) || (M.Email.Contains(searchTxt)) || (M.BillAddress.Contains(searchTxt)) || (M.BillCity.Contains(searchTxt))).OrderByDescending(t => t.Id).Take(200).ToListAsync();
                response = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            //join t in _dbContext.TblMemberTypeCounty on m.MemberType equals t.MemberType
                            where m.Company.Contains(searchTxt) || m.Email.Contains(searchTxt) || m.BillAddress.Contains(searchTxt) || m.BillCity.Contains(searchTxt)
                            select new
                            {
                                m.Company,
                                c.Contact,
                                m.BillCity,
                                m.BillState,
                                c.Phone,
                                c.Email,
                                m.MemberCost,
                                //  Package=    (from x in _dbContext.TblMemberTypeCounty where x.MemberType == m.MemberType select new {x.Package}).Distinct().OrderBy(m=>m.Package),
                                m.MemberType,
                                m.RenewalDate,
                                m.Discipline,
                                m.Id,
                                //t.Package
                            }).Take(100).ToList().Select(x => new MemberShipRegistration
                            {
                                Company = x.Company,
                                ContactName = x.Contact,
                                BillCity = x.BillCity,
                                BillState = x.BillState,
                                ContactPhone = x.Phone,
                                Email = x.Email,
                                MemberType = x.MemberCost.ToString(),
                                MemberCost = x.MemberType.ToString(),
                                RenewalDate = x.RenewalDate,
                                Discipline = x.Discipline,
                                CompId = x.Id,
                                //Package = "Not Known"
                                Package = string.IsNullOrEmpty(x.MemberType.ToString()) ? "Not Known" : GetPackage(Convert.ToInt32(x.MemberType))
                            }).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;

        }
        /// <summary>
        /// Edit note from member profile page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateNote(NoteInfo model)
        {
            int rowAffected = 0;
            TblMemNote tblMemNote = new();
            tblMemNote = await _dbContext.TblMemNotes.FirstOrDefaultAsync(x => x.Id == model.Id && x.CompType == model.CompType);

            if (tblMemNote != null)
            {
                try
                {
                    //tblMemNote.Id = model.Id;
                    tblMemNote.Note = model.Note;
                    // _dbContext.TblMemNotes.Add(tblMemNote);
                    tblMemNote.Name = model.Name;
                    _dbContext.Entry(tblMemNote).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    rowAffected = 1;
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }

            }
            return rowAffected;
        }
        /// <summary>
        /// Add note from member profile page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> AddNote(NoteInfo model)
        {
            int rowAffected = 0;
            TblMemNote tblMemNote = new();
            tblMemNote = await _dbContext.TblMemNotes.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (tblMemNote == null)
            {
                try
                {
                    tblMemNote = new();
                    //tblMemNote.Id = model.Id;
                    tblMemNote.Note = model.Note;
                    tblMemNote.MemId = model.MemId;
                    tblMemNote.LogDate = DateTime.Now;
                    tblMemNote.Flag = false;
                    tblMemNote.Name = model.Name;
                    tblMemNote.CompType = model.CompType;
                    await _dbContext.TblMemNotes.AddAsync(tblMemNote);
                    await _dbContext.SaveChangesAsync();
                    rowAffected = 1;
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }

            }
            return rowAffected;
        }
        /// <summary>
        /// Delete note from member profile page
        /// </summary>
        /// <param name="NoteId"></param>
        /// <returns></returns>
        public async Task<int> DeleteNote(int NoteId)
        {
            int rowAffected = 0;
            TblMemNote tblMemNote = new();
            tblMemNote = await _dbContext.TblMemNotes.FirstOrDefaultAsync(x => x.Id == NoteId);

            if (tblMemNote != null)
            {
                try
                {
                    tblMemNote.Flag = true;
                    _dbContext.Entry(tblMemNote).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    rowAffected = 1;
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }

            }
            return rowAffected;
        }
        /// <summary>
        /// Add to dashboard from find project here
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="Id"></param>
        /// <param name="usern"></param>
        /// <returns></returns>
        public async Task<dynamic> AddToDashboard(bool Change, string Id, string usern)
        {
            int ConIDs = (from c in _dbContext.TblContacts where c.Email == usern select c.ConId).FirstOrDefault();
            int MemberIDs = (from c in _dbContext.TblContacts where c.Email == usern select c.Id).FirstOrDefault();
            int ProjId = Convert.ToInt32(Id)
;
            var tbl2 = await _dbContext.TblTracking.Where(M => M.ConId == ConIDs && M.MemberId == MemberIDs && M.ProjId == ProjId).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
            if (tbl2.Count == 0)
            {
                tblTracking track = new tblTracking();
                track.IsTracking = Change;
                track.ProjId = ProjId;
                track.ConId = ConIDs;
                track.MemberId = MemberIDs;
                _dbContext.TblTracking.Add(track);

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                tblTracking tracks = await _dbContext.TblTracking.Where(M => M.ConId == ConIDs && M.MemberId == MemberIDs && M.ProjId == ProjId).SingleAsync(s => s.ProjId == ProjId);
                tracks.IsTracking = Change;
                _dbContext.TblTracking.Update(tracks);
                await _dbContext.SaveChangesAsync();
            }
            return ProjId;
        }
        /// <summary>
        /// Find price of different package on card in member profile renewal and membership register
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentInfo>> FindPriceDetail()
        {
            IEnumerable<PaymentInfo> response = new List<PaymentInfo>();
            var result = await (from tab in _dbContext.tblMemberShipSubPlans
                                join mplan in _dbContext.tblMemberShipPlans
                                on tab.MemberShipPlanId equals mplan.MemberShipPlanId
                                select new
                                {
                                    PlanName = mplan.MemberShipPlanName,
                                    PlanId = tab.MemberShipPlanId,
                                    SubPlanId = tab.SubMemberShipPlanId,
                                    MPrice = tab.MonthlyPrice,
                                    YPrice = tab.YearlyPrice,
                                    QPrice = tab.QuarterlyPrice,
                                    SubPlanName = tab.SubMemberShipPlanName,
                                    AddMPrice = tab.AddMonthlyPrice,
                                    AddQPrice = tab.AddQuarterlyPrice,
                                    AddYPrice = tab.AddYearlyPrice
                                }).ToListAsync();
            //var result = _entityRepository.GetEntities()hipPlans.Where(x=> x.Active==true)
            //    .Join(_entityRepository.GetEntities()hipSubPlans.Where(x => x.Active == true), m => m.MemberShipPlanId,
            //    s => s.MemberShipPlanId, (Plan, SubPlan) => new
            //    {
            //        PlanName = Plan.MemberShipPlanName,
            //        PlanId = Plan.MemberShipPlanId,
            //        SubPlanId = SubPlan.SubMemberShipPlanId,
            //        MPrice = SubPlan.MonthlyPrice,
            //        YPrice = SubPlan.YearlyPrice,
            //        QPrice = SubPlan.QuarterlyPrice,
            //        SubPlanName = SubPlan.SubMemberShipPlanName
            //    }
            //    ).ToListAsync();
            response = result.Select(x => new PaymentInfo
            {
                MemberShipPlanName = x.PlanName,
                SubMemberShipPlanName = x.SubPlanName,
                MemberShipPlanId = x.PlanId,
                MonthlyPrice = x.MPrice,
                YearlyPrice = x.YPrice,
                QuarterlyPrice = x.QPrice,
                SubMemberShipPlanId = x.SubPlanId,
                AddYearlyPrice = x.AddYPrice,
                AddQuarterlyPrice = x.AddQPrice,
                AddMonthlyPrice = x.AddMPrice,
            });
            List<tblDiscount> discounts = new List<tblDiscount>();
            discounts = await _dbContext.tblDiscount.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now && x.isActive == true).ToListAsync();
            int discount = 0;
            int DiscountId = 0;
            if (discounts != null && discounts.Count > 0)
            {

                for (var i = 0; i < discounts.Count; i++)
                {
                    if (i == 0)
                    {
                        discount = Convert.ToInt32(discounts[i].DiscountRate);
                        DiscountId = discounts[i].DiscountId;
                    }
                    else
                    {
                        if (discounts[i].EndDate > discounts[i - 1].EndDate)
                            discount = Convert.ToInt32(discounts[i].DiscountRate);
                        DiscountId = discounts[i].DiscountId;
                    }
                }
                if (discount > 0)
                {
                    var lstResponse = response.ToList();
                    for (int i = 0; i < lstResponse.Count; i++)
                    {
                        lstResponse[i].MonthlyPrice = lstResponse[i].MonthlyPrice - (lstResponse[i].MonthlyPrice * discount) / 100;
                        lstResponse[i].YearlyPrice = lstResponse[i].YearlyPrice - (lstResponse[i].YearlyPrice * discount) / 100;
                        lstResponse[i].QuarterlyPrice = lstResponse[i].QuarterlyPrice - (lstResponse[i].QuarterlyPrice * discount) / 100;
                        lstResponse[i].DiscountId = DiscountId;
                    }

                    response = lstResponse;
                }

            }
            return response;
        }
        /// <summary>
        /// Staff Dashboard content
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<List<StaffDashboardViewModel>> GetProjectDashboardInfoAsync(string username)
        {
            int MemberIDs = (from c in _dbContext.TblContacts where c.Email == username select c.Id).FirstOrDefault();
            int ConIDs = (from c in _dbContext.TblContacts where c.Email == username select c.ConId).FirstOrDefault();
            string userId = Convert.ToString(ConIDs);
            List<StaffDashboardViewModel> response = new List<StaffDashboardViewModel>();
            var tbl2 = await _dbContext.tblProject.Where(M => M.IsActive != false && (!string.IsNullOrEmpty(M.ProjNumber) || M.ProjNumber == "0")).OrderByDescending(t => t.ProjId).Take(1000).OrderBy(t => t.ProjId).ToListAsync();
            response = tbl2.Select(t => new StaffDashboardViewModel
            {
                ProjId = t.ProjId,
                ProjNumber = t.ProjNumber,
                Publish = Convert.ToBoolean(t.Publish),
                SpcChk = Convert.ToBoolean(t.SpcChk),
                SpecsOnPlans = Convert.ToBoolean(t.SpecsOnPlans),
                Title = t.Title,
                memberId = t.memberId,
                LocCity = t.LocCity,
                LocState = t.LocState,
                ProjTypeIdString = t.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId) != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId).ProjType) : "") : "",
                BidDt = t.BidDt,
                strBidDt = t.StrBidDt,
                ArrivalDt = t.ArrivalDt,
                MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs) != null ? (_dbContext.TblTracking.SingleOrDefault(y => (y.ProjId == t.ProjId) && (y.ConId == ConIDs)).IsTracking) : false) : true,
                TrackCount = (from c in _dbContext.TblTracking where c.ProjId == t.ProjId && c.IsTracking == true select c.ProjId).Count(),
                BR = string.IsNullOrEmpty(t.Brnote) ? false : true,
                //MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs && y.MemberId == MemberIDs) != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs && y.MemberId == MemberIDs).IsTracking) : false) : true,
                LBidDt = t.StrBidDt5,
                Entities = _dbContext.TblEntity.Where(x => x.Projid == t.ProjId && x.IsActive == true).Select(x => x.EnityName).ToList()
            }).OrderByDescending(t => t.ProjNumber).ToList();

            return response;
        }
        /// <summary>
        /// To view phl pdf content for download on member dashboard
        /// </summary>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<List<string>> ListPdfContent(int ProjId)
        {
            List<string> response = new List<string>();
            var Uids = await _dbContext.TblBidStatus.Where(x => x.Projid == ProjId && x.CompType == 2).ToListAsync();
            foreach (var u in Uids)
            {
                if (u.CompType == 2)
                {

                    var company = _dbContext.TblContractors.SingleOrDefault(x => x.Id == Convert.ToInt32(u.Company)).Name;
                    if (!string.IsNullOrEmpty(company))
                    {
                        bool check = true;
                        foreach (string s in response)
                        {
                            if (s == company)
                                check = false;
                        }
                        if (check)
                            response.Add(company);
                    }
                }
            }
            return response;
        }
        /// <summary>
        /// To save project from member/postprojecthere
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveProjectInfoAsync(MemberProjectInfo model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            TblProject tblProject = new();
            try
            {
                tblProject.ProjNumber = model.ProjNumber;
                tblProject.BackProjNumber = model.ProjNumber;
                tblProject.Title = model.Title;
                tblProject.BidDt = model.BidDt;
                tblProject.ArrivalDt = DateTime.Now;
                tblProject.LocAddr1 = model.LocAddr1;
                tblProject.LocCity = model.LocCity;
                tblProject.LocState = model.LocState;
                tblProject.LocZip = model.LocZip;
                tblProject.LocAddr2 = model.LocAddr2;
                tblProject.Longitude = model.Longitude;
                tblProject.Latitude = model.Latitude;
                tblProject.ProjNote = "This project was uploaded by " + model.ContactName + " of " + model.ContactMember + " on " + DateTime.Now.ToString("MM/dd/yyyy");
                tblProject.Story = model.Story;
                tblProject.SpecsOnPlans = model.SpecsOnPlans;
                tblProject.SpcChk = model.SpcChk;
                tblProject.PrevailingWage = model.PrevailingWage;
                tblProject.ProjTypeId = Convert.ToInt32(model.ProjTypeId);
                tblProject.ProjSubTypeId = Convert.ToInt32(model.ProjSubTypeId);
                tblProject.SpcChk = model.SpcChk;
                tblProject.PrevailingWage = model.PrevailingWage;
                tblProject.FutureWork = model.FutureWork;
                if (!string.IsNullOrEmpty(model.ProjScope))
                {
                    model.ProjScope = model.ProjScope.Substring(0, model.ProjScope.Length - 1);
                }
                tblProject.ProjScope = model.ProjScope;
                tblProject.IsActive = true;
                tblProject.BidBond = model.BidBond;
                tblProject.StrBidDt = model.strBidDt;
                tblProject.StrBidDt2 = model.strBidDt2;
                tblProject.StrBidDt3 = model.strBidDt3;
                tblProject.StrBidDt4 = model.strBidDt4;
                tblProject.StrBidDt5 = model.strBidDt5;
                tblProject.createdDate = DateTime.Now;
                tblProject.createdBy = model.ContactName;
                tblProject.memberId = model.memberId;
                tblProject.LastBidDt = model.LastBidDt;
                tblProject.PrebidNote = model.PrebidNote;
                string OBidTime = "00:00";
                string hmValue = "00";
                if (model.hComp < 10)
                {
                    hmValue = "0" + model.hComp.ToString();
                }
                else
                {
                    hmValue = model.hComp.ToString();
                }
                if (model.mValue == "PM")
                {
                    int OtComp = 12 + Convert.ToInt32(model.tComp);
                    OBidTime = OtComp.ToString() + ":" + hmValue;
                }
                else if (model.mValue == "AM")
                {
                    if (model.tComp < 10)
                    {
                        OBidTime = "0" + model.tComp.ToString() + ":" + hmValue;
                    }
                }
                model.strBidDt5 = OBidTime;
                tblProject.StrBidDt5 = OBidTime;
                tblProject.StrBidDt = model.BidDt == null ? "" : Convert.ToDateTime(model.BidDt).ToString("MMM dd yyyy") + " " + model.strBidDt5 + " " + model.strBidDt3;
                tblProject.StrBidDt = model.Undecided == "TBD" ? "TBD" : tblProject.StrBidDt;
                tblProject.Undecided = string.IsNullOrEmpty(model.Undecided) ? "" : model.Undecided;
                int costCount = 0;
                bool costflag = true;
                #region SaveEstCostAtTblProject
                if (model.EstCostDetails != null && model.EstCostDetails.Count > 0)
                {
                    for (int i = 0; i < model.EstCostDetails.Count; i++)
                    {
                        costflag = true;
                        if (costCount == 0 && costflag == true)
                        {
                            if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstFrom))
                            {
                                if (!Convert.ToBoolean(model.EstCostDetails[i].Removed))
                                {
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains(',') ? model.EstCostDetails[i].EstFrom.Replace(",", "") : model.EstCostDetails[i].EstFrom;
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains('$') ? model.EstCostDetails[i].EstFrom.Replace("$", "") : model.EstCostDetails[i].EstFrom;
                                    if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstTo))
                                    {
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCost = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;

                                    }
                                    else
                                    {
                                        model.EstCost = model.EstCostDetails[i].EstFrom;
                                    }
                                    costCount++;
                                    costflag = false;
                                }
                                else
                                {
                                    model.EstCost = "-";
                                }
                            }
                            else
                            {
                                model.EstCost = "-";
                            }

                        }
                        else if (costCount == 1 && costflag == true)
                        {
                            if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstFrom))
                            {
                                if (!Convert.ToBoolean(model.EstCostDetails[i].Removed))
                                {
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains(',') ? model.EstCostDetails[i].EstFrom.Replace(",", "") : model.EstCostDetails[i].EstFrom;
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains('$') ? model.EstCostDetails[i].EstFrom.Replace("$", "") : model.EstCostDetails[i].EstFrom;
                                    if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstTo))
                                    {
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCost2 = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;
                                    }
                                    else
                                    {
                                        model.EstCost2 = model.EstCostDetails[i].EstFrom;
                                    }
                                    costCount++;
                                    costflag = false;
                                }
                                else
                                {
                                    model.EstCost2 = "-";
                                }
                            }
                            else
                            {
                                model.EstCost2 = "-";
                            }

                        }
                        else if (costCount == 2 && costflag == true)
                        {
                            if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstFrom))
                            {
                                if (!Convert.ToBoolean(model.EstCostDetails[i].Removed))
                                {
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains(',') ? model.EstCostDetails[i].EstFrom.Replace(",", "") : model.EstCostDetails[i].EstFrom;
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains('$') ? model.EstCostDetails[i].EstFrom.Replace("$", "") : model.EstCostDetails[i].EstFrom;
                                    if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstTo))
                                    {
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCost3 = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;
                                    }
                                    else
                                    {
                                        model.EstCost3 = model.EstCostDetails[i].EstFrom;
                                    }
                                    costCount++;
                                    costflag = false;
                                }
                                else
                                {
                                    model.EstCost3 = "-";
                                }
                            }
                            else
                            {
                                model.EstCost3 = "-";
                            }

                        }
                        else if (costCount == 3 && costflag == true)
                        {
                            if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstFrom))
                            {
                                if (!Convert.ToBoolean(model.EstCostDetails[i].Removed))
                                {
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains(',') ? model.EstCostDetails[i].EstFrom.Replace(",", "") : model.EstCostDetails[i].EstFrom;
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains('$') ? model.EstCostDetails[i].EstFrom.Replace("$", "") : model.EstCostDetails[i].EstFrom;
                                    if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstTo))
                                    {
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCost4 = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;
                                    }
                                    else
                                    {
                                        model.EstCost4 = model.EstCostDetails[i].EstFrom;
                                    }
                                    costCount++;
                                    costflag = false;
                                }
                                else
                                {
                                    model.EstCost4 = "-";
                                }
                            }
                            else
                            {
                                model.EstCost4 = "-";
                            }

                        }
                        else if (costCount == 4 && costflag == true)
                        {
                            if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstFrom))
                            {
                                if (!Convert.ToBoolean(model.EstCostDetails[i].Removed))
                                {
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains(',') ? model.EstCostDetails[i].EstFrom.Replace(",", "") : model.EstCostDetails[i].EstFrom;
                                    model.EstCostDetails[i].EstFrom = model.EstCostDetails[i].EstFrom.Contains('$') ? model.EstCostDetails[i].EstFrom.Replace("$", "") : model.EstCostDetails[i].EstFrom;
                                    if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstTo))
                                    {
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCost5 = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;
                                    }
                                    else
                                    {
                                        model.EstCost5 = model.EstCostDetails[i].EstFrom;
                                    }
                                    costflag = false;
                                }
                                else
                                {
                                    model.EstCost5 = "-";
                                }
                            }
                            else
                            {
                                model.EstCost5 = "-";
                            }

                        }

                    }
                }
                tblProject.EstCost = string.IsNullOrEmpty(model.EstCost) ? "-" : model.EstCost;
                tblProject.EstCost2 = string.IsNullOrEmpty(model.EstCost2) ? "-" : model.EstCost2;
                tblProject.EstCost3 = string.IsNullOrEmpty(model.EstCost3) ? "-" : model.EstCost3;
                tblProject.EstCost4 = string.IsNullOrEmpty(model.EstCost4) ? "-" : model.EstCost4;
                tblProject.EstCost5 = string.IsNullOrEmpty(model.EstCost5) ? "-" : model.EstCost5;
                #endregion
                _dbContext.tblProject.Add(tblProject);
                _dbContext.SaveChanges();
                model.ProjId = tblProject.ProjId;
                httpResponse.data = model;
                #region Save multiple counties in tblProjCounty
                if (!string.IsNullOrEmpty(model.Counties))
                {
                    string[] arrCounties = model.Counties.Split(',');
                    foreach (var item in arrCounties)
                    {
                        int i = 0;
                        int.TryParse(item, out i);
                        if (i != 0)
                        {
                            TblProjCounty projCounty = new();
                            projCounty.ProjId = model.ProjId;
                            projCounty.CountyId = Convert.ToInt32(item);
                            await _dbContext.TblProjCounty.AddAsync(projCounty);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                #endregion
                #region Save pre bid information
                if (model.preBidInfos != null && model.preBidInfos.Count > 0)
                {
                    foreach (var item in model.preBidInfos)
                    {
                        if ((item.PreBidDate != null || (bool)item.UndecidedPreBid) && item.IsDeleted != true)
                        {

                            tblPreBidInfo preBidInfo = new();
                            string PreBidTime = "00:00";
                            string hValue = "00";
                            if (item.hComp < 10)
                            {
                                hValue = "0" + item.hComp.ToString();
                            }
                            else
                            {
                                hValue = item.hComp.ToString();
                            }
                            if (item.mValue == "PM")
                            {
                                int tComp = 12 + Convert.ToInt32(item.tComp);
                                PreBidTime = tComp.ToString() + ":" + hValue;
                            }
                            else if (item.mValue == "AM")
                            {
                                if (item.tComp < 10)
                                {
                                    PreBidTime = "0" + item.tComp.ToString() + ":" + hValue;
                                }
                            }
                            preBidInfo.PreBidDate = item.PreBidDate;
                            preBidInfo.PreBidTime = PreBidTime;
                            preBidInfo.PST = item.PST;
                            preBidInfo.IsDeleted = false;
                            preBidInfo.ProjId = model.ProjId;
                            preBidInfo.PreBidAnd = item.PreBidAnd;
                            preBidInfo.Mandatory = item.Mandatory;
                            preBidInfo.Location = item.Location;
                            preBidInfo.UndecidedPreBid = item.UndecidedPreBid;
                            await _dbContext.tblPreBidInfo.AddAsync(preBidInfo);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                #endregion
                #region Save EstCost In tblEstCostDatails
                if (model.EstCostDetails != null && model.EstCostDetails.Count > 0)
                {
                    foreach (var item in model.EstCostDetails)
                    {
                        if (item.Removed != true && !string.IsNullOrEmpty(item.EstFrom))
                        {
                            tblEstCostDetails tblEstCost = new tblEstCostDetails();
                            tblEstCost.Removed = false;
                            tblEstCost.EstCostFrom = item.EstFrom;
                            tblEstCost.EstCostTo = item.EstTo;
                            tblEstCost.Description = item.Description;
                            tblEstCost.Projid = model.ProjId;
                            tblEstCost.RangeSign = (item.RangeSign != null) ? item.RangeSign : "0";
                            await _dbContext.tblEstCostDetails.AddAsync(tblEstCost);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                #endregion
                #region Save Entity Data in tblEntity
                List<TblEntity> tblEntities = new List<TblEntity>();
                if (model.Entities != null && model.Entities.Count > 0)
                {

                    foreach (var item in model.Entities)
                    {
                        TblEntity entity = await _dbContext.TblEntity.SingleOrDefaultAsync(x => x.EntityID == item.EntityID);
                        if (entity == null)
                        {
                            if (!string.IsNullOrEmpty(item.EntityName) || !string.IsNullOrEmpty(item.EntityName))
                            {
                                entity = new();
                                entity.ProjNumber = Convert.ToInt32(model.ProjNumber);
                                entity.Projid = model.ProjId;
                                entity.EnityName = item.EntityName;
                                entity.NameId = item.NameId;
                                entity.EntityType = item.EntityType;
                                entity.IsActive = true;
                                entity.chkIssue = item.chkIssue;
                                entity.CompType = 1;
                                await _dbContext.TblEntity.AddAsync(entity);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            entity.ProjNumber = Convert.ToInt32(model.ProjNumber);
                            entity.Projid = model.ProjId;
                            entity.EnityName = item.EntityName;
                            entity.NameId = item.NameId;
                            entity.EntityType = item.EntityType;
                            entity.IsActive = true;
                            entity.chkIssue = item.chkIssue;
                            _dbContext.Entry(entity).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                }
                #endregion
                #region Saveing phl information
                if (model.phlInfo != null && model.phlInfo.Count > 0)
                {
                    foreach (var item in model.phlInfo)
                    {
                        //bool? Undecided = item.Undecided == null ? false : item.Undecided;
                        tblPhlInfo tblPInfo = new();
                        if (!string.IsNullOrEmpty(item.StrContact) && !string.IsNullOrEmpty(item.Company))
                        {
                            tblPInfo.MemId = item.MemId;
                            tblPInfo.ConId = item.ConId == null ? 0 : item.ConId;
                            //tblBid.Uid = string.IsNullOrEmpty(item.Uid) ? "0" : item.Uid;
                            tblPInfo.Note = item.Note;
                            tblPInfo.BidDate = item.BidDate;
                            tblPInfo.tComp = item.tComp;
                            tblPInfo.hComp = item.hComp;
                            tblPInfo.mValue = item.mValue;
                            tblPInfo.BidStatus = item.BidStatus;
                            tblPInfo.IsActive = true;
                            tblPInfo.CompType = 2;
                            tblPInfo.PhlType = 4;
                            tblPInfo.PST = "PST";
                            tblPInfo.ProjId = tblProject.ProjId;
                            await _dbContext.tblPhlInfo.AddAsync(tblPInfo);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                #endregion

                if (model.LocAddr2 != null && model.CountyId != 0)
                {
                    List<TblCounty> tblcounty = await _dbContext.TblCounty.Where(m => m.CountyId == model.CountyId && m.County != model.LocAddr2).ToListAsync();
                    if (tblcounty != null)
                    {
                        TblCounty _tblcounty = await _dbContext.TblCounty.FirstOrDefaultAsync(m => m.CountyId == model.CountyId);
                        {
                            if (_tblcounty != null)
                            {
                                _tblcounty.County = model.LocAddr2;
                                _dbContext.Entry(_tblcounty).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();

                            }
                        }
                    }
                }
                else
                {
                    if (model.LocAddr2 != null)
                    {
                        List<TblCounty> lstTblCounty = await _dbContext.TblCounty.Where(x => x.County == model.LocAddr2 && x.State == model.LocState).ToListAsync();
                        int id = 0;
                        if (lstTblCounty == null || lstTblCounty.Count <= 0)
                        {
                            TblCounty tblCounty = new();
                            tblCounty.County = model.LocAddr2;
                            tblCounty.State = model.LocState;
                            await _dbContext.TblCounty.AddAsync(tblCounty);
                            await _dbContext.SaveChangesAsync();
                            id = tblCounty.CountyId;
                            TblCityCounty tblCityCounty = new();
                            tblCityCounty.City = model.LocCity;
                            tblCityCounty.CountyId = id;
                            await _dbContext.TblCityCounty.AddAsync(tblCityCounty);
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            id = lstTblCounty[0].CountyId;
                        }
                        model.CountyId = id;
                    }

                }
                TblProject tbl = await _dbContext.tblProject.FirstOrDefaultAsync(x => x.ProjId == model.ProjId);
                if (tbl != null)
                {
                    tbl.CountyID = model.CountyId;
                    _dbContext.Entry(tbl).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                List<tblUsedProjNuber> tblusedProjNuber = await _dbContext.tblUsedProjNuber.Where(m => (m.ProjNumber == model.ProjNumber) && (m.IsUsed == false)).ToListAsync();
                if (tblusedProjNuber != null && tblusedProjNuber.Count > 0)
                {
                    foreach (var item in tblusedProjNuber)
                    {
                        item.IsUsed = true;
                        _dbContext.Entry(item).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }

                }

                List<int> Cons = await _dbContext.TblProjNotification.Select(x => x.Id).ToListAsync();
                List<string> EmailsTo = new();
                foreach (int i in Cons)
                {
                    TblProjNotification tblProjNotification = await _dbContext.TblProjNotification.SingleOrDefaultAsync(x => x.Id == i);
                    string temp = tblProjNotification.Email;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        EmailsTo.Add(temp);
                    }
                }
                model.EmailsTo = EmailsTo;

                httpResponse.data = model;
                httpResponse.success = true;


            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// To view addenda pdf content for download on member dashboard
        /// </summary>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<List<string>> AddendaListPdfContent(int ProjId)
        {
            List<string> response = new List<string>();
            try
            {
                response = await _dbContext.TblSubAddenda.Where(x => x.ProjId == ProjId).Select(x => x.PdfFileName).ToListAsync();
                return response;
            }
            catch (Exception Ex)
            {
                return response;
            }
        }
        /// <summary>
        /// Add to calendar from member dashboard
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="BidDate"></param>
        /// <param name="model"></param>
        /// <param name="Ischecked"></param>
        /// <returns></returns>
        public async Task<dynamic> AddToCalendar(int ProjId, DateTime BidDate, DisplayLoginInfo model, bool Ischecked)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            var rowAffected = "0";
            TblCalendarNotice calendar = new();

            calendar = await _dbContext.TblCalendarNotice.FirstOrDefaultAsync(m => m.ProjId == ProjId && m.ConId == model.ConId);
            if (calendar == null)
            {
                try
                {
                    calendar = new();
                    calendar.ProjId = ProjId;
                    calendar.BidDt = BidDate != null ? BidDate.ToString() : "";
                    calendar.MemberId = model.Id;
                    calendar.ConId = model.ConId;
                    calendar.Disable = false;
                    await _dbContext.TblCalendarNotice.AddAsync(calendar);
                    await _dbContext.SaveChangesAsync();
                    httpResponse.success = true;
                    httpResponse.data = calendar.Disable;
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                    httpResponse.success = false;
                }

            }
            else
            {
                try
                {
                    calendar.Disable = !calendar.Disable;
                    _dbContext.Entry(calendar).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    httpResponse.success = true;
                    httpResponse.data = calendar.Disable ?? false;
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                    httpResponse.success = false;
                }
            }
            return httpResponse;
        }
        /// <summary>
        /// Save autosearch from member/findprojecthere
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveSearch(SearchViewModel model, DisplayLoginInfo loginInfo)
        {
            var rowAffected = 0;
            TblAutoSearch autoSearch = new();
            try
            {
                autoSearch.MemId = loginInfo.Id;
                autoSearch.Uid = loginInfo.Uid;
                autoSearch.FromDate = model.strBidDateFrom != null ? Convert.ToDateTime(model.strBidDateFrom) : null;
                autoSearch.ToDate = model.strBidDateTo != null ? Convert.ToDateTime(model.strBidDateTo) : null;
                autoSearch.Keywords = model.SearchText;
                autoSearch.Name = model.Name;
                autoSearch.Wage = model.PrevailingWageFlag == true ? "1" : "0";
                if (model.ProjectTypeIds.Count > 0)
                {
                    autoSearch.TypeB = model.ProjectTypeIds.Contains("1") ? true : false;
                    autoSearch.TypeU = model.ProjectTypeIds.Contains("2") ? true : false;
                    autoSearch.TypeR = model.ProjectTypeIds.Contains("3") ? true : false;
                    autoSearch.TypeV = model.ProjectTypeIds.Contains("4") ? true : false;
                }
                StringBuilder sb = new();
                if (model.ProjectSubTypeIds != null && model.ProjectSubTypeIds.Any())
                {
                    foreach (var item in model.ProjectSubTypeIds)
                    {
                        sb.Append(item + ",");
                    }
                    string fBuilder = sb.ToString();

                    int place = fBuilder.LastIndexOf(',');
                    if (place >= 0)
                    {
                        fBuilder = fBuilder.Remove(place);
                    }

                    autoSearch.ProjSubTypeId = fBuilder;
                }
                sb = new();
                if (model.ProjectestCosts != null && model.ProjectestCosts.Any())
                {
                    foreach (var item in model.ProjectestCosts)
                    {
                        sb.Append(item + ",");
                    }
                    string fBuilder = sb.ToString();

                    int place = fBuilder.LastIndexOf(',');
                    if (place >= 0)
                    {
                        fBuilder = fBuilder.Remove(place);
                    }

                    autoSearch.EstCost = fBuilder;
                }
                sb = new();
                if (model.ProjectScopes != null && model.ProjectScopes.Any())
                {
                    foreach (var item in model.ProjectScopes)
                    {
                        sb.Append(item + ",");
                    }
                    string fBuilder = sb.ToString();

                    int place = fBuilder.LastIndexOf(',');
                    if (place >= 0)
                    {
                        fBuilder = fBuilder.Remove(place);
                    }

                    autoSearch.ProjectScopes = fBuilder;
                }

                sb = new();
                if (model.ProjectStates != null && model.ProjectStates.Any())
                {
                    foreach (var item in model.ProjectStates)
                    {
                        sb.Append(item + ",");
                    }
                    string fBuilder = sb.ToString();
                    int place = fBuilder.LastIndexOf(',');
                    fBuilder = fBuilder.Remove(place);
                    autoSearch.State = fBuilder;
                }
                else if (!string.IsNullOrEmpty(model.State) && model.State != "0")
                {
                    autoSearch.State = model.State;
                }
                autoSearch.Distance = model.Distance.ToString();
                autoSearch.City = model.City;
                autoSearch.GeoState = model.State;
                //autoSearch.EstCost = model.EstCost;
                await _dbContext.TblAutoSearch.AddAsync(autoSearch);
                await _dbContext.SaveChangesAsync();
                rowAffected++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rowAffected;

        }
        /// <summary>
        /// To get autosearch from member/findprojecther
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public async Task<List<SearchViewModel>> GetSavedSearch(DisplayLoginInfo loginInfo)
        {
            List<SearchViewModel> response = new();
            response = await _dbContext.TblAutoSearch.Where(x => x.MemId == loginInfo.Id).Select(x => new SearchViewModel { Id = x.Id, Name = x.Name }).ToListAsync();
            return response;
        }
        /// <summary>
        /// To get value from autosearch from member/findprojecther
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SearchViewModel> GoToSavedSearch(int id)
        {
            SearchViewModel response = new();
            TblAutoSearch autoSearch = new();
            autoSearch = await _dbContext.TblAutoSearch.FirstOrDefaultAsync(x => x.Id == id);
            if (autoSearch != null)
            {
                //response.ProjectSubTypeId = autoSearch.ProjSubTypeId;
                try
                {
                    response.SearchText = autoSearch.Keywords;
                    response.strBidDateFrom = autoSearch.FromDate == null ? "" : autoSearch.FromDate.ToString();
                    response.strBidDateTo = autoSearch.ToDate == null ? "" : autoSearch.ToDate.ToString();
                    response.PrevailingWageFlag = autoSearch.Wage == "1" ? true : false;
                    response.ProjectTypeId = autoSearch.TypeB == true ? 1 : (autoSearch.TypeU == true ? 2 : (autoSearch.TypeR == true ? 3 : (autoSearch.TypeV == true ? 4 : 0)));
                    List<string> lstProjTypeIds = new();
                    List<string> lstState = new();
                    List<string> lstProjSubTypeIds = new();
                    List<string> lstEstCosts = new();
                    List<string> lstScopes = new();

                    if ((bool)autoSearch.TypeB) lstProjTypeIds.Add("1");
                    if ((bool)autoSearch.TypeU) lstProjTypeIds.Add("2");
                    if ((bool)autoSearch.TypeR) lstProjTypeIds.Add("3");
                    if ((bool)autoSearch.TypeV) lstProjTypeIds.Add("4");


                    if (!string.IsNullOrEmpty(autoSearch.State))
                    {
                        if (autoSearch.State.Contains(','))
                        {
                            lstState = autoSearch.State.Split(',').ToList();
                        }
                        else
                        {
                            lstState.Add(autoSearch.State);
                        }
                    }
                    if (!string.IsNullOrEmpty(autoSearch.ProjSubTypeId))
                    {
                        if (autoSearch.ProjSubTypeId.Contains(','))
                        {
                            lstProjSubTypeIds = autoSearch.ProjSubTypeId.Split(',').ToList();
                        }
                        else
                        {

                            lstProjSubTypeIds.Add(autoSearch.ProjSubTypeId);
                        }
                    }
                    if (!string.IsNullOrEmpty(autoSearch.EstCost))
                    {
                        if (autoSearch.EstCost.Contains(','))
                        {
                            lstEstCosts = autoSearch.EstCost.Split(',').ToList();
                        }
                        else
                        {
                            lstEstCosts.Add(autoSearch.EstCost);
                        }
                    }
                    if (!string.IsNullOrEmpty(autoSearch.ProjectScopes))
                    {
                        if (autoSearch.ProjectScopes.Contains(','))
                        {
                            lstScopes = autoSearch.ProjectScopes.Split(',').ToList();
                        }
                        else
                        {
                            lstScopes.Add(autoSearch.ProjectScopes);
                        }
                    }
                    response.City = autoSearch.City;
                    response.State = autoSearch.GeoState;
                    double tempDistance = 0;
                    if (!string.IsNullOrEmpty(autoSearch.Distance))
                        double.TryParse(autoSearch.Distance, out tempDistance);
                    response.Distance = tempDistance;
                    response.ProjectStates = (lstState.Count > 0) ? lstState : null;
                    response.ProjectSubTypeIds = (lstProjSubTypeIds.Count > 0) ? lstProjSubTypeIds : null;
                    response.ProjectTypeIds = (lstProjTypeIds.Count > 0) ? lstProjTypeIds : null;
                    response.ProjectestCosts = (lstEstCosts.Count > 0) ? lstEstCosts : null;
                    response.ProjectScopes = (lstScopes.Count > 0) ? lstScopes : null;
                    response.Name = autoSearch.Name;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return response;
        }
        /// <summary>
        /// Save value for sign up in process when going to step 2 frpm membership/register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveInCompleteSignUpAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblIncompleteSignUp _IncompleteSignUp = await _dbContext.tblIncompleteSignUp.SingleOrDefaultAsync(m => m.ContactEmail == model.ContactEmail);
                if (_IncompleteSignUp != null)
                {
                    _dbContext.Entry(_IncompleteSignUp).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                tblIncompleteSignUp IncompleteSignUp = new();
                IncompleteSignUp.ID = model.ID;
                IncompleteSignUp.Company = model.Company;
                IncompleteSignUp.DBA = model.Dba;
                IncompleteSignUp.MailAddress = model.MailAddress;
                IncompleteSignUp.MailCity = model.MailCity;
                IncompleteSignUp.MailState = model.MailState;
                IncompleteSignUp.MailZip = model.MailZip;
                IncompleteSignUp.BillAddress = model.BillAddress;
                IncompleteSignUp.BillCity = model.BillCity;
                IncompleteSignUp.BillState = model.BillState;
                IncompleteSignUp.BillZip = model.BillZip;
                IncompleteSignUp.CompanyPhone = model.CompanyPhone;
                IncompleteSignUp.FirstName = model.FirstName;
                IncompleteSignUp.LastName = model.LastName;
                IncompleteSignUp.ContactPhone = model.ContactPhone;
                IncompleteSignUp.ContactEmail = model.ContactEmail;
                IncompleteSignUp.ContactPassword = model.ContactPassword;
                IncompleteSignUp.Term = model.hdnTerm;
                IncompleteSignUp.MemberCost = model.MemberCost;
                IncompleteSignUp.MemberType = model.MemberType;
                IncompleteSignUp.BillEmail = model.BillEmail;
                IncompleteSignUp.Extension = model.Extension;
                IncompleteSignUp.DiscountId = model.DiscountId;
                await _dbContext.tblIncompleteSignUp.AddAsync(IncompleteSignUp);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.data = IncompleteSignUp.ID;
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
        /// <summary>
        /// Delete incomplete sign up in process when registration is complted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteInCompleteSignUp(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblIncompleteSignUp IncompleteSignUp = await _dbContext.tblIncompleteSignUp.SingleOrDefaultAsync(m => m.ID == id);
                if (IncompleteSignUp != null)
                {
                    _dbContext.Entry(IncompleteSignUp).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
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
            return response;
        }
        /// <summary>
        /// To get data on member copy center page
        /// </summary>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<List<OrderTables>> GetCopyCenterData(int ConId)
        {
            List<OrderTables> response = new List<OrderTables>();
            List<TblProjOrder> result = await _dbContext.TblProjOrder.Where(x => x.Uid == ConId.ToString()).OrderByDescending(t => t.OrderId).Take(30).ToListAsync();
            List<TblProjOrderDetail> details = new();
            foreach (var x in result)
            {
                OrderTables order = new();
                order.OrderId = x.OrderId;
                order.Viewed = x.Viewed;
                order.ProjId = x.ProjId;
                order.PO = x.Po;
                order.Sz1Qty = x.Sz1Qty;
                order.Sz2Qty = x.Sz2Qty;
                order.Sz3Qty = x.Sz3Qty;
                order.Sz4Qty = x.Sz4Qty;
                order.Sz5Qty = x.Sz5Qty;
                order.Sz6Qty = x.Sz6Qty;
                order.Prints = x.Prints;
                order.Instructions = x.Instructions;
                order.Company = x.Company;
                order.Name = x.Name;
                order.Addr = x.Addr;
                order.CSZ = x.CSZ;
                order.Phone = x.Phone;
                order.Email = x.Email;
                order.DeliveryDt = x.DeliveryDt;
                //order.HowShip = x.HowShip;
                if (int.TryParse(x.HowShip, out int howShipNumeric))
                {
                    // x.HowShip is a numeric value, you can use howShipNumeric as an int
                    //order.HowShip = howShipNumeric.ToString();
                    var list = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptId == howShipNumeric);
                    if (list != null)
                    {
                        int id = list.DelivId;
                        var tbldeliverymaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivId == id);
                        order.HowShip = tbldeliverymaster.DelivName;
                    }
                }
                else
                {
                    // x.HowShip is a string value, you can use x.HowShip as a string
                    order.HowShip = x.HowShip;
                }
                order.GetTblProjs = _dbContext.TblProjOrderDetail.Where(t => t.OrderId == x.OrderId).ToList().Select(m => new OrderDetails
                {
                    Pages = m.Pages
                }).ToList();
                order.OrderDt = x.OrderDt;
                order.DoneDt = x.DoneDt;
                order.ShipDt = x.ShipDt;
                //order.EmailContent = returnurl + x.OrderId;
                response.Add(order);
            }
            return response;
        }
        /// <summary>
        /// To fill address info of the logged in member on copycenter/preview print order form
        /// </summary>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<AutoFillData> AutoFill(int ConId)
        {
            AutoFillData response = new();
            try
            {
                if (ConId > 0)
                {
                    var res = await _dbContext.TblContacts.FirstOrDefaultAsync(m => m.ConId == ConId);
                    if (res != null)
                    {
                        response.Id = res.Id;
                        var comp = _entityRepository.GetEntities().FirstOrDefault(m => m.Id == res.Id);

                        if (comp != null)
                        {
                            response.Company = comp.Company;
                            response.StateCode = _dbContext.TblState.Where(s => s.State == comp.BillState).Count() == 0 ? 0 :
                            _dbContext.TblState.SingleOrDefault(s => s.State == comp.BillState).StateId;
                            response.City = comp.BillCity ?? "";
                            response.Address = comp.BillAddress ?? "";
                            response.Zip = comp.BillZip ?? "";
                        }
                        response.ConID = res.ConId;
                        response.Name = res.Contact ?? "";
                        response.Email = res.Email ?? string.Empty;
                        response.Phone = res.Phone ?? string.Empty;

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// To get data on Member/MemberDirectory
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MemberShipRegistration>> GetMemberDirectoryAsync()
        {
            IEnumerable<MemberShipRegistration> response = new List<MemberShipRegistration>();
            try
            {
                response = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            where c.MainContact == true && m.Inactive == false && m.CheckDirectory == true
                            //join t in _dbContext.TblMemberTypeCounty on m.MemberType equals t.MemberType
                            // where any acondition apply
                            select new
                            {
                                m.Company,
                                c.Contact,
                                m.BillAddress,
                                m.BillCity,
                                m.BillState,
                                m.BillZip,
                                m.CompanyPhone,
                                c.Phone,
                                c.Email,
                                m.Discipline,
                                m.Id,
                                CompanyEmail = m.Email,
                            }).ToList()
                                   .Select(x => new MemberShipRegistration
                                   {
                                       Company = x.Company,
                                       ContactName = x.Contact,
                                       BillCity = x.BillCity,
                                       BillState = x.BillState,
                                       BillAddress = x.BillAddress,
                                       BillZip = x.BillZip,
                                       ContactPhone = string.IsNullOrEmpty(x.CompanyPhone) ? x.Phone : x.CompanyPhone,
                                       Email = string.IsNullOrEmpty(x.CompanyEmail) ? x.Email : x.CompanyEmail,
                                       Discipline = x.Discipline,
                                       ID = x.Id,
                                       //Package = "Not Known"
                                   }).ToList();
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        /// <summary>
        /// To show card on Member/MemberDirectory
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> ShowCard(int Id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                MemberShipRegistration member = new();
                TblDirectoryCheck check = await _dbContext.TblDirectoryCheck.Where(x => x.MemId == Id).FirstOrDefaultAsync();
                if (check != null)
                {
                    TblMember tbl = _entityRepository.GetEntities().Where(x => x.Id == Id).FirstOrDefault();
                    TblContact contact = await _dbContext.TblContacts.Where(x => x.Id == Id && x.MainContact == true).FirstOrDefaultAsync();
                    if (tbl != null)
                    {
                        member.Company = check.Company ? (string.IsNullOrEmpty(tbl.Company) ? "" : tbl.Company) : "";
                        member.Discipline = check.Speciality ? (string.IsNullOrEmpty(tbl.Discipline) ? "" : tbl.Discipline) : "";
                        member.Dba = check.DBA ? (string.IsNullOrEmpty(tbl.Dba) ? "" : tbl.Dba) : "";
                        if (check.BillAddr)
                        {
                            member.ConsBillAddress = string.IsNullOrEmpty(tbl.BillAddress) ? "" : tbl.BillAddress;
                            member.ConsBillAddress = string.IsNullOrEmpty(tbl.BillCity) ? member.ConsBillAddress : (string.IsNullOrEmpty(member.ConsBillAddress) ? tbl.BillCity : member.ConsBillAddress + ", " + tbl.BillCity);
                            member.ConsBillAddress = string.IsNullOrEmpty(tbl.BillState) ? member.ConsBillAddress : (string.IsNullOrEmpty(member.ConsBillAddress) ? tbl.BillState : member.ConsBillAddress + ", " + tbl.BillState);
                            member.ConsBillAddress = string.IsNullOrEmpty(tbl.BillZip) ? member.ConsBillAddress : member.ConsBillAddress + " " + tbl.BillZip;
                        }
                        else
                        {
                            member.ConsBillAddress = "";
                        }
                        if (check.MailAddr)
                        {
                            member.ConsMailAddress = string.IsNullOrEmpty(tbl.MailAddress) ? "" : tbl.MailAddress;
                            member.ConsMailAddress = string.IsNullOrEmpty(tbl.MailCity) ? member.ConsMailAddress : (string.IsNullOrEmpty(member.ConsMailAddress) ? tbl.MailCity : member.ConsMailAddress + ", " + tbl.MailCity);
                            member.ConsMailAddress = string.IsNullOrEmpty(tbl.MailState) ? member.ConsMailAddress : (string.IsNullOrEmpty(member.ConsMailAddress) ? tbl.MailState : member.ConsMailAddress + ", " + tbl.MailState); member.ConsMailAddress = string.IsNullOrEmpty(tbl.MailZip) ? member.ConsMailAddress : member.ConsMailAddress + " " + tbl.MailZip;
                        }
                        else
                        {
                            member.ConsMailAddress = "";
                        }
                        member.CompanyPhone = check.Phone ? (string.IsNullOrEmpty(tbl.CompanyPhone) ? (contact != null ? (string.IsNullOrEmpty(contact.Phone) ? "" : contact.Phone) : "") : tbl.CompanyPhone) : "";
                        member.Email = check.Email ? (string.IsNullOrEmpty(tbl.Email) ? (contact != null ? (string.IsNullOrEmpty(contact.Email) ? "" : contact.Email) : "") : tbl.Email) : "";
                        member.ContactName = check.PrimaryContact ? (contact != null ? (string.IsNullOrEmpty(contact.Contact) ? "" : contact.Contact) : "") : "";
                        if (check.Business)
                        {
                            List<TblMemberDiv> list = await _dbContext.TblMemberDivs.Where(x => x.MemberId == Id && x.IsDeleted == false).ToListAsync();
                            if (list.Count > 0)
                            {
                                StringBuilder sb = new();
                                foreach (var item in list)
                                {
                                    TblWebDiv WebDiv = await _dbContext.TblWebDivs.SingleOrDefaultAsync(x => x.DivNo == item.WebDivId);
                                    string WebDivName = WebDiv.DivName;
                                    string WebDivDisc = WebDiv.DivDesc;
                                    if (WebDivName.Contains(WebDivDisc))
                                    {
                                        WebDivName = WebDivName.Replace(WebDivDisc, "");
                                        WebDivName = WebDivName.Replace(" - ", "");
                                    }
                                    sb.Append(WebDivName + ", ");
                                }
                                member.Div = sb.ToString();
                                int CommaIndex = 0;
                                CommaIndex = member.Div.LastIndexOf(",");
                                member.Div = member.Div.Substring(0, CommaIndex);
                            }
                            else
                            {
                                member.Div = "";
                            }
                        }
                        else
                        {
                            member.Div = "";
                        }
                        if (check.License)
                        {
                            List<tblLicense> licenses = await _dbContext.tblLicense.Where(m => m.MemId == Id).ToListAsync();
                            if (licenses != null)
                            {
                                if (licenses.Count > 0)
                                {
                                    foreach (var item in licenses)
                                    {
                                        TblState tblState = await _dbContext.TblState.SingleOrDefaultAsync(x => x.StateId == item.LicState);
                                        string StateText = tblState.State;
                                        member.License += "(" + StateText + ")" + item.LicNum + " | ";
                                    }
                                    int DelIndex = member.License.LastIndexOf("|") - 1;
                                    member.License = member.License.Substring(0, DelIndex);
                                }
                            }
                            else
                            {
                                member.License = "";
                            }
                        }
                        else
                        {
                            member.License = "";
                        }
                        response.success = true;
                        response.data = member;
                    }


                }
                else
                {
                    member.Company = "";
                    member.Discipline = "";
                    member.CompanyPhone = "";
                    member.Email = "";
                    member.ConsBillAddress = "";
                    member.ConsMailAddress = "";
                    member.License = "";
                    member.Div = "";
                    member.Dba = "";
                    member.ContactName = "";
                    response.success = true;
                    response.data = member;
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
                throw;
            }
            return response;
        }
        /// <summary>
        ///Get Unique UID on saving free trial member 
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="a"></param>
        /// <returns></returns>

        private string GetUID(string UID, int a = 0)
        {
            string Fuid = UID;
            TblContact tbl = _dbContext.TblContacts.Where(x => x.Uid == Fuid).FirstOrDefault();
            if (tbl != null)
            {
                Fuid = Fuid + a.ToString();
                a++;
                GetUID(Fuid, a);
            }
            return Fuid;
        }
        /// <summary>
        /// Save free trial member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveFreeTrialMemberAsync(MemberShipRegistration model)
        {
            List<TblMember> dataList = new();
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblMember tblMember = new();
                TblContact tblContact = new();
                int forUID = _dbContext.TblContacts.OrderByDescending(t => t.ConId).Last().ConId;
                model.ContactUID = forUID.ToString() + "A";
                model.ContactUID = GetUID(model.ContactUID);
                tblMember.Company = model.Company;
                tblMember.RenewalDate = model.RenewalDate;
                tblMember.MailAddress = model.MailAddress;
                tblMember.MailCity = model.MailCity;
                tblMember.MailState = model.MailState;
                tblMember.MailZip = model.MailZip;
                tblMember.InsertDate = DateTime.Now;
                tblMember.MemberCost = 0;
                tblMember.MemberType = 12;
                tblMember.Inactive = false;
                tblMember.RenewalDate = DateTime.Now.AddDays(6);

                var businessEntity = _entityRepository.BusinessEntity_instance(tblMember);
                await _dbContext.BusinessEntities.AddAsync(businessEntity);
                await _dbContext.SaveChangesAsync();

                var address = _entityRepository.Address_instance(tblMember);
                var member = _entityRepository.Member_instance(tblMember);
                await _dbContext.Addresses.AddAsync(address);
                await _dbContext.Members.AddAsync(member);

                await _dbContext.SaveChangesAsync();

                //await _dbContext.TblMembers.AddAsync(tblMember);
                //await _dbContext.SaveChangesAsync();
                tblContact.Id = (_dbContext.BusinessEntities.Any()) ? _dbContext.BusinessEntities.Max(m => m.BusinessEntityId) : 1;
                tblContact.Email = model.Email;
                tblContact.Phone = model.ContactPhone;
                tblContact.Contact = model.FirstName+" "+model.LastName;
                tblContact.Uid = model.ContactUID;
                tblContact.UserId = model.ASPUserId;
                tblContact.Password = model.hdnPass;
                tblContact.FirstName = model.FirstName;
                tblContact.LastName = model.LastName;
                await _dbContext.TblContacts.AddAsync(tblContact);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Your Free Trial starts now. Please check your email.";
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
        /// <summary>
        /// From service
        /// </summary>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectInformation>> GetProjectUpdatePacificAsync(int ConId)
        {
            IEnumerable<ProjectInformation> response = new List<ProjectInformation>();
            try
            {
                List<TblProject> model1 = _dbContext.tblProject.Where(x => x.BidDt >= DateTime.Now && x.ImportDt != null).ToList();
                List<TblProject> model = new();
                foreach (var item in model1)
                {
                    DateTime Dt = DateTime.Now.Date;
                    DateTime ImpDate = Convert.ToDateTime(item.ImportDt).Date;
                    if (Dt == ImpDate)
                    {
                        model.Add(item);
                    }
                }
                response = model.Select(x => new ProjectInformation
                {
                    ProjId = x.ProjId,
                    Title = x.Title,
                    LocAddr2 = x.LocAddr2,
                    LocCity = x.LocCity,
                    LocState = x.LocState,
                    BidDt = x.BidDt,
                    MemberTrack = x.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == x.ProjId && y.ConId == ConId) != null ? (_dbContext.TblTracking.SingleOrDefault(y => (y.ProjId == x.ProjId) && (y.ConId == ConId)).IsTracking) : false) : true
                });
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        /// <summary>
        /// From service
        /// </summary>
        /// <param name="Pids"></param>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectInformation>> GetProjectUpdateAsync(List<int> Pids, int ConId)
        {
            IEnumerable<ProjectInformation> response = new List<ProjectInformation>();
            try
            {
                List<TblProject> model = new();
                foreach (int item in Pids)
                {
                    TblProject tbl = _dbContext.tblProject.Where(x => x.ProjId == item).First();
                    if (tbl != null)
                        model.Add(tbl);
                }

                response = model.Select(x => new ProjectInformation
                {
                    ProjId = x.ProjId,
                    Title = x.Title,
                    LocAddr2 = x.LocAddr2,
                    LocCity = x.LocCity,
                    LocState = x.LocState,
                    BidDt = x.BidDt,
                    MemberTrack = x.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == x.ProjId && y.ConId == ConId) != null ? (_dbContext.TblTracking.SingleOrDefault(y => (y.ProjId == x.ProjId) && (y.ConId == ConId)).IsTracking) : false) : true
                });
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Get member location detail on memberprofile(staff/member) page
        /// </summary>
        /// <param name="MemId"></param>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetMemberLocAsync(int MemId, string SelectedValue = "")
        {
            var response = await (from tab in _dbContext.TblLocList.Where(m => m.MemId == MemId) select tab).ToListAsync();

            var result = new List<SelectListItem>();
            string loc = "";
            foreach (var location in response)
            {
                loc = location.LocAddr;
                loc = string.IsNullOrEmpty(location.LocCity) ? loc : loc + ", " + location.LocCity;
                loc = string.IsNullOrEmpty(location.LocCounty) ? loc : loc + ", " + location.LocCounty;
                loc = string.IsNullOrEmpty(location.LocState) ? loc : loc + ", " + location.LocState;
                loc = string.IsNullOrEmpty(location.LocZip) ? loc : loc + " " + location.LocZip;
                result.Add(new SelectListItem
                {
                    Text = loc,
                    Value = location.LocId.ToString(),
                    Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (location.LocId.ToString() == SelectedValue ? true : false))
                });
            }

            return result;
        }
        /// <summary>
        /// Create Uid when user is added from add new user
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateUid()
        {
            var response = await _dbContext.TblContacts.OrderByDescending(x => x.ConId).FirstOrDefaultAsync();
            string Uid = response.ConId.ToString() + "A";
            var chkresponse = await _dbContext.TblContacts.Where(x => x.Uid == Uid).FirstOrDefaultAsync();
            if (chkresponse == null)
                return Uid;
            else
                return await CreateUid();
        }
        /// <summary>
        /// To edit user from member profile page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> EditUser(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                //var tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Email == model.ContactEmail && m.ConId != model.ConID);
                //if (tblContact != null)
                //{
                //    response.success = false;
                //    response.statusCode = "404";
                //    response.statusMessage = "Email already exist";
                //    return response;
                //}
                var _tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.ConId == model.ConID);
                if (_tblContact != null)
                {
                    _tblContact.Contact = model.ContactName;
                    _tblContact.Email = model.ContactEmail;
                    _tblContact.Phone = model.ContactPhone;
                    //user.Password = model.ContactPassword;
                    _tblContact.LocId = model.LocId;
                    _dbContext.Entry(_tblContact).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "User data updated";
                }
                else
                {
                    response.success = false;
                    response.statusCode = "400";
                    response.statusMessage = "Something went wrong";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// To search radius on findprojecthere
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<dynamic> RadiusSearchAsync(SearchViewModel model, string username)
        {
            HttpResponseDetail<dynamic> details = new();
            TblCityLatLong dataList = new();

            try
            {
                int MemberIDs = (from c in _dbContext.TblContacts where c.Email == username select c.Id).FirstOrDefault();


                int ConIDs = (from c in _dbContext.TblContacts where c.Email == username select c.ConId).FirstOrDefault();
                string userId = Convert.ToString(ConIDs);
                if (model.StateList != null && model.StateList.Count > 0)
                {
                    List<ProjectInformation> response = new List<ProjectInformation>();
                    List<ProjectInformation> responseList = new List<ProjectInformation>();
                    if (model.City != null)
                    {
                        foreach (var item in model.StateList)
                        {
                            string tempState = item;
                            int chkState = 0;
                            int.TryParse(tempState, out chkState);
                            if (chkState != 0)
                            {
                                tempState = GetSelectedStateText(item);
                            }
                            dataList = await _dbContext.TblCityLatLong.SingleOrDefaultAsync(m => m.City == model.City && m.State == tempState);
                            if (dataList != null)
                            {
                                double Latitude = Convert.ToDouble(dataList.Latitude);
                                double Longitude = Convert.ToDouble(dataList.Longitude);
                                double Distance = Convert.ToDouble(model.Distance);
                                SqlParameter param1 = new SqlParameter("@latitude", Latitude);
                                SqlParameter param2 = new SqlParameter("@longitude", Longitude);
                                SqlParameter param3 = new SqlParameter("@Distance", Distance);
                                IEnumerable<TblProject> result = _dbContext.tblProject.FromSqlRaw("exec GetRediousSearch  {0}, {1},{2}", param1, param2, param3).ToList();
                                responseList = result.Select(t => new ProjectInformation
                                {
                                    ImportDt = t.ImportDt,
                                    ProjId = t.ProjId,
                                    ProjNumber = t.ProjNumber,
                                    Publish = Convert.ToBoolean(t.Publish),
                                    SpcChk = Convert.ToBoolean(t.SpcChk),
                                    SpecsOnPlans = Convert.ToBoolean(t.SpecsOnPlans),
                                    Title = t.Title,
                                    LocCity = t.LocCity,
                                    LocState = t.LocState,
                                    LocAddr1 = t.LocAddr1,
                                    LocAddr2 = t.LocAddr2,
                                    ProjTypeIdString = t.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId) != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId).ProjType) : "") : "",
                                    ProjTypeId = t.ProjTypeId != null ? t.ProjTypeId : 0,
                                    ProjSubTypeId = t.ProjSubTypeId != null ? t.ProjSubTypeId : 0,
                                    BidDt = t.BidDt,
                                    EstCost = t.EstCost,
                                    EstCost2 = t.EstCost2,
                                    EstCost3 = t.EstCost3,
                                    EstCost4 = t.EstCost4,
                                    EstCost5 = t.EstCost5,
                                    strBidDt = string.IsNullOrEmpty(t.StrBidDt) ? "" : t.StrBidDt,
                                    ArrivalDt = t.ArrivalDt,
                                    MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs) != null ? (_dbContext.TblTracking.SingleOrDefault(y => (y.ProjId == t.ProjId) && (y.ConId == ConIDs)).IsTracking) : false) : true,
                                    TrackCount = (from c in _dbContext.TblTracking where c.ProjId == t.ProjId && c.IsTracking == true select c.ProjId).Count(),
                                    //MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs && y.MemberId == MemberIDs) != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs && y.MemberId == MemberIDs).IsTracking) : false) : true,
                                    FutureWork = t.FutureWork != null ? Convert.ToBoolean(t.FutureWork) : false,
                                    strBidDt5 = t.StrBidDt5,
                                    MBDCheck = _dbContext.tblBidDateTime.Where(x => x.ProjId == t.ProjId && x.IsDeleted == false && x.PhlId != null).Count() > 0 ? "Y" : "N"
                                }).ToList();
                                response.AddRange(responseList);
                                responseList = new List<ProjectInformation>();
                            }
                            else
                            {
                                responseList = new List<ProjectInformation>();
                            }

                        }
                        response = response.DistinctBy(x => x.ProjId).ToList();
                        details.success = true;
                        if (response != null && response.Count > 0)
                            details.data = response;
                        else
                        {
                            details.data = new List<ProjectInformation>();
                        }
                    }
                }
                else if (model.City != null && model.State != null)
                {
                    dataList = await _dbContext.TblCityLatLong.SingleOrDefaultAsync(m => m.City == model.City && m.State == model.State);
                    if (dataList != null)
                    {
                        double Latitude = Convert.ToDouble(dataList.Latitude);
                        double Longitude = Convert.ToDouble(dataList.Longitude);
                        double Distance = Convert.ToDouble(model.Distance);
                        SqlParameter param1 = new SqlParameter("@latitude", Latitude);
                        SqlParameter param2 = new SqlParameter("@longitude", Longitude);
                        SqlParameter param3 = new SqlParameter("@Distance", Distance);
                        IEnumerable<TblProject> result = _dbContext.tblProject.FromSqlRaw("exec GetRediousSearch  {0}, {1},{2}", param1, param2, param3).ToList();
                        List<ProjectInformation> response = new List<ProjectInformation>();
                        response = result.Select(t => new ProjectInformation
                        {
                            ImportDt = t.ImportDt,
                            ProjId = t.ProjId,
                            ProjNumber = t.ProjNumber,
                            Publish = Convert.ToBoolean(t.Publish),
                            SpcChk = Convert.ToBoolean(t.SpcChk),
                            SpecsOnPlans = Convert.ToBoolean(t.SpecsOnPlans),
                            Title = t.Title,
                            LocCity = t.LocCity,
                            LocState = t.LocState,
                            LocAddr1 = t.LocAddr1,
                            LocAddr2 = t.LocAddr2,
                            ProjTypeId = t.ProjTypeId != null ? t.ProjTypeId : 0,
                            ProjSubTypeId = t.ProjSubTypeId != null ? t.ProjSubTypeId : 0,
                            ProjTypeIdString = t.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId) != null ? (_dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == t.ProjTypeId).ProjType) : "") : "",
                            BidDt = t.BidDt,
                            strBidDt = string.IsNullOrEmpty(t.StrBidDt) ? "" : t.StrBidDt,
                            ArrivalDt = t.ArrivalDt,
                            MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs) != null ? (_dbContext.TblTracking.SingleOrDefault(y => (y.ProjId == t.ProjId) && (y.ConId == ConIDs)).IsTracking) : false) : true,
                            TrackCount = (from c in _dbContext.TblTracking where c.ProjId == t.ProjId && c.IsTracking == true select c.ProjId).Count(),
                            //MemberTrack = t.ProjId != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs && y.MemberId == MemberIDs) != null ? (_dbContext.TblTracking.SingleOrDefault(y => y.ProjId == t.ProjId && y.ConId == ConIDs && y.MemberId == MemberIDs).IsTracking) : false) : true,
                            FutureWork = t.FutureWork != null ? Convert.ToBoolean(t.FutureWork) : false
                        }).ToList();
                        details.success = true;
                        details.data = response;
                    }
                    else
                    {
                        details.data = new List<ProjectInformation>();
                    }
                }
                else
                {
                    details.data = new List<ProjectInformation>();
                }
            }
            catch (Exception ex)
            {
                details.success = false;
                details.statusCode = "404";
                details.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return details;
        }
        /// <summary>
        /// To get content of faq page
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TblFAQ>> GetFaqAsync()
        {
            IEnumerable<TblFAQ> response = new List<TblFAQ>();
            try
            {
                response = await _dbContext.TblFAQ.Where(x => x.IsActive == true).Select(x => new TblFAQ
                {
                    Id = x.Id,
                    Question = x.Question,
                    Answer = x.Answer
                })
                    .ToListAsync();
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        public async Task<MemberShipRegistration> GetTrialMemberAsync(int id)
        {
            MemberShipRegistration response = new MemberShipRegistration();
            try
            {
                TblMember tbl = _entityRepository.GetEntities().SingleOrDefault(t => t.Id == id);
                if (tbl != null)
                {
                    TblContact contact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.Id == tbl.Id);
                    response.ID = tbl.Id;
                    response.Company = tbl.Company;
                    response.MailAddress = tbl.MailAddress;
                    response.MailCity = tbl.MailCity;
                    response.MailState = tbl.MailState;
                    response.MailZip = tbl.MailZip;
                    response.Email = contact.Email;
                    response.FirstName = contact.Contact;
                    response.ContactPhone = contact.Phone;
                    response.ConID = contact.ConId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<MemberShipRegistration> UpdateTrialMembershipAsync(MemberShipRegistration model)
        {
            MemberShipRegistration response = new MemberShipRegistration();
            try
            {
                model.BillState = GetSelectedStateText(model.BillState);
                model.MailState = GetSelectedStateText(model.MailState);
                model.ContactName = model.FirstName + " " + model.LastName;
                var rowsAffected = 0;
                model.InsertDate = DateTime.Now;
                TblMember tblMember = new();
                TblContact tblContact = new();
                if (model.Inactive == false)
                {
                    if (model.hdnTerm == "Yearly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddYears(1).AddDays(-1) : null;
                    if (model.hdnTerm == "Quaterly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(4).AddDays(-1) : null;
                    if (model.hdnTerm == "Monthly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(1).AddDays(-1) : null;
                    if (model.hdnTerm == "Free Trial")
                        model.RenewalDate = DateTime.Now.AddDays(364);
                }
                tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.ConId == model.ConID);
                if (tblContact != null)
                {
                    tblContact.Id = model.ID;
                    tblContact.Contact = model.ContactName;
                    tblContact.Phone = model.ContactPhone;
                    tblContact.Email = model.ContactEmail;
                    tblContact.MainContact = true;
                    //tblContact.Uid = await CreateUid();
                    _dbContext.Entry(tblContact).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.ID);
                if (tblMember != null)
                {
                    tblMember.InsertDate = model.InsertDate;
                    tblMember.Company = model.Company;
                    tblMember.Inactive = false;
                    tblMember.BillAddress = model.BillAddress;
                    tblMember.BillCity = model.BillCity;
                    tblMember.BillState = model.BillState;
                    tblMember.BillZip = model.BillZip;
                    tblMember.LastPayDate = model.LastPayDate;
                    tblMember.RenewalDate = model.RenewalDate;
                    tblMember.Term = model.hdnTerm;
                    tblMember.MemberType = Convert.ToInt32(model.MemberType);
                    tblMember.MemberCost = Convert.ToInt32(model.MemberCost);
                    tblMember.Dba = model.Dba;
                    tblMember.MailAddress = model.MailAddress;
                    tblMember.MailCity = model.MailCity;
                    tblMember.MailState = model.MailState;
                    tblMember.MailZip = model.MailZip;
                    tblMember.CheckDirectory = true;
                    _dbContext.Entry(tblMember).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> UpdateInCompleteSignUpAsync(MemberShipRegistration model, int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblIncompleteSignUp tblIncomplete = await _dbContext.tblIncompleteSignUp.SingleOrDefaultAsync(m => m.ID == id);
                if (tblIncomplete != null)
                {
                    tblIncomplete.MemberCost = model.MemberCost;
                    tblIncomplete.UserId = model.ASPUserId;
                    _dbContext.Entry(tblIncomplete).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
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
        public async Task<dynamic> RemoveFromDashboardProj(int ProjId, DisplayLoginInfo info)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblTracking tracks = await _dbContext.TblTracking.Where(M => M.ConId == info.ConId && M.MemberId == info.Id && M.ProjId == ProjId).SingleAsync(s => s.ProjId == ProjId);
                if (tracks != null)
                {
                    tracks.IsTracking = false;
                    _dbContext.TblTracking.Update(tracks);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Project removed from dashboard";
                }
                else
                {
                    response.success = true;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }

            }
            catch (Exception Ex)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = Ex.Message;
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        public async Task<dynamic> AddBiddingProj(string Id, DisplayLoginInfo info)
        {
            int ProjId = Convert.ToInt32(Id);
            string userId = Convert.ToString(info.ConId);
            try
            {
                var tbl2 = await _dbContext.TblBidStatus.Where(M => M.Projid == ProjId && (M.Uid == info.Uid || M.Company == info.Id.ToString() || M.Contact == info.ConId.ToString())).OrderByDescending(t => t.Projid).Take(200).ToListAsync();
                if (tbl2.Count == 0)
                {
                    TblBidStatus BidSt = new TblBidStatus();
                    BidSt.Uid = info.Uid;
                    BidSt.Projid = ProjId;
                    BidSt.Bidding = true;
                    BidSt.Undecided = false;
                    BidSt.Contact = info.ConId.ToString();
                    BidSt.Company = info.Id.ToString();
                    BidSt.CompType = 1;
                    _dbContext.TblBidStatus.Add(BidSt);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    TblBidStatus BidStatus = await _dbContext.TblBidStatus.Where(M => (M.Contact == info.ConId.ToString() || M.Uid == info.Uid || M.Company == info.Id.ToString()) && M.Projid == ProjId).SingleAsync(s => s.Projid == ProjId);
                    if (BidStatus.Bidding == true)
                    {
                        BidStatus.Uid = info.Uid;
                        BidStatus.Projid = ProjId;
                        BidStatus.Undecided = true;
                        BidStatus.Bidding = false;
                        BidStatus.Contact = info.ConId.ToString();
                        BidStatus.Company = info.Id.ToString();
                        BidStatus.CompType = 1;
                        _dbContext.TblBidStatus.Update(BidStatus);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        BidStatus.Uid = info.Uid;
                        BidStatus.Projid = ProjId;
                        BidStatus.Undecided = false;
                        BidStatus.Bidding = true;
                        BidStatus.Contact = info.ConId.ToString();
                        BidStatus.Company = info.Id.ToString();
                        BidStatus.CompType = 1;
                        _dbContext.TblBidStatus.Update(BidStatus);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return ProjId;
        }
        public async Task<dynamic> EditSaveSearch(SearchViewModel model, DisplayLoginInfo loginInfo)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblAutoSearch tblAutoSearch = await _dbContext.TblAutoSearch.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (tblAutoSearch != null)
                {
                    tblAutoSearch.FromDate = model.strBidDateFrom != null ? Convert.ToDateTime(model.strBidDateFrom) : null;
                    tblAutoSearch.ToDate = model.strBidDateTo != null ? Convert.ToDateTime(model.strBidDateTo) : null;
                    tblAutoSearch.Keywords = model.SearchText;
                    tblAutoSearch.Wage = model.PrevailingWageFlag == true ? "1" : "0";
                    if (model.ProjectTypeIds.Count > 0)
                    {
                        tblAutoSearch.TypeB = model.ProjectTypeIds.Contains("1") ? true : false;
                        tblAutoSearch.TypeU = model.ProjectTypeIds.Contains("2") ? true : false;
                        tblAutoSearch.TypeR = model.ProjectTypeIds.Contains("3") ? true : false;
                        tblAutoSearch.TypeV = model.ProjectTypeIds.Contains("4") ? true : false;
                    }
                    StringBuilder sb = new();
                    if (model.ProjectSubTypeIds != null && model.ProjectSubTypeIds.Any())
                    {
                        foreach (var item in model.ProjectSubTypeIds)
                        {
                            sb.Append(item + ",");
                        }
                        string fBuilder = sb.ToString();

                        int place = fBuilder.LastIndexOf(',');
                        if (place >= 0)
                        {
                            fBuilder = fBuilder.Remove(place);
                        }

                        tblAutoSearch.ProjSubTypeId = fBuilder;
                    }
                    else
                    {
                        tblAutoSearch.ProjSubTypeId = null;
                    }
                    sb = new();
                    if (model.ProjectestCosts != null && model.ProjectestCosts.Any())
                    {
                        foreach (var item in model.ProjectestCosts)
                        {
                            sb.Append(item + ",");
                        }
                        string fBuilder = sb.ToString();

                        int place = fBuilder.LastIndexOf(',');
                        if (place >= 0)
                        {
                            fBuilder = fBuilder.Remove(place);
                        }

                        tblAutoSearch.EstCost = fBuilder;
                    }
                    else
                    {
                        tblAutoSearch.EstCost = null;
                    }
                    sb = new();
                    if (model.ProjectScopes != null && model.ProjectScopes.Any())
                    {
                        foreach (var item in model.ProjectScopes)
                        {
                            sb.Append(item + ",");
                        }
                        string fBuilder = sb.ToString();

                        int place = fBuilder.LastIndexOf(',');
                        if (place >= 0)
                        {
                            fBuilder = fBuilder.Remove(place);
                        }

                        tblAutoSearch.ProjectScopes = fBuilder;
                    }
                    else
                    {
                        tblAutoSearch.ProjectScopes = null;
                    }
                    tblAutoSearch.Name = model.Name;
                    sb = new();
                    if (model.ProjectStates != null && model.ProjectStates.Any())
                    {
                        foreach (var item in model.ProjectStates)
                        {
                            sb.Append(item + ",");
                        }
                        string fBuilder = sb.ToString();
                        int place = fBuilder.LastIndexOf(',');
                        fBuilder = fBuilder.Remove(place);
                        tblAutoSearch.State = fBuilder;
                    }
                    else
                    {
                        tblAutoSearch.State = null;
                    }

                    tblAutoSearch.Distance = model.Distance.ToString();
                    tblAutoSearch.City = model.City;
                    tblAutoSearch.GeoState = model.State;
                    //tblAutoSearch.EstCost = model.EstCost;
                    _dbContext.Entry(tblAutoSearch).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Search details updated successfully";

                }

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
        public async Task<dynamic> DeleteSaveSearchAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblAutoSearch tblAutoSearch = await _dbContext.TblAutoSearch.SingleOrDefaultAsync(m => m.Id == id);
                if (tblAutoSearch != null)
                {
                    _dbContext.Entry(tblAutoSearch).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Search details deleted successfully";
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



        public async Task<List<TblProject>> GetFilteredProjectsData(SearchViewModel model)
        {
            List<TblProject> response = new();
            List<TblProject> responseList = new();
            List<TblProject> responseStateFilter = new();
            List<TblProject> responseFilterObj = new();
            try
            {
                if (model != null)
                {
                    DateTime BidDateFrom = string.IsNullOrEmpty(model.strBidDateFrom) ? Convert.ToDateTime(_dbContext.tblProject.Min(x => x.BidDt)) : Convert.ToDateTime(model.strBidDateFrom);
                    DateTime BidDateTo = string.IsNullOrEmpty(model.strBidDateTo) ? Convert.ToDateTime(_dbContext.tblProject.Max(x => x.BidDt)) : Convert.ToDateTime(model.strBidDateTo);

                    if (string.IsNullOrEmpty(model.strBidDateFrom) && string.IsNullOrEmpty(model.strBidDateTo))
                        responseList = await _dbContext.tblProject.Where(m => (m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo || (m.BidDt == null)) && m.Publish == true && m.IsActive != false && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0").OrderByDescending(t => t.ProjId).Take(1000).ToListAsync();
                    else
                        responseList = await _dbContext.tblProject.Where(m => (m.BidDt >= BidDateFrom && m.BidDt <= BidDateTo) && m.Publish == true && m.IsActive != false && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0").OrderByDescending(t => t.ProjId).Take(1000).ToListAsync();


                    responseStateFilter.AddRange(responseList);

                    //if (model.ProjectTypeIds != null && model.ProjectTypeIds.Count > 0)
                    //{
                    //    List<TblProject> responseProjectTypeFilter = new ();

                    //    foreach (var projectTypeId in model.ProjectTypeIds)
                    //    {
                    //        var proj = responseStateFilter.Where(m => m.ProjTypeId == Convert.ToInt32(projectTypeId)).ToList();

                    //        if (proj != null)
                    //            responseProjectTypeFilter.AddRange(proj);
                    //    }

                    //    responseList = responseProjectTypeFilter;


                    //    // Filter by ProjectSubTypeIds
                    //    if (model.ProjectSubTypeIds != null && model.ProjectSubTypeIds.Count > 0)
                    //    {
                    //        List<TblProject> responseProjectSubTypeFilter = new();

                    //        foreach (var projectSubTypeId in model.ProjectSubTypeIds)
                    //        {
                    //            var proj = responseList.Where(m => m.ProjSubTypeId == Convert.ToInt32(projectSubTypeId)).ToList();

                    //            if (proj != null)
                    //                responseProjectSubTypeFilter.AddRange(proj);
                    //        }

                    //        responseStateFilter = responseProjectSubTypeFilter;
                    //    }
                    //    else
                    //    {
                    //        responseStateFilter = responseList;
                    //    }
                    //}
                    //responseList = responseStateFilter;

                    if (model.ProjectScopes != null && model.ProjectScopes.Any() && responseStateFilter != null)
                    {
                        responseStateFilter = responseStateFilter
                            .Where(item => item.ProjScope != null && model.ProjectScopes.Any(scope => item.ProjScope.Split(',').Contains(scope.Trim())))
                            .ToList();
                    }

                    if (model.PrevailingWageFlag)
                        responseFilterObj = responseStateFilter.Where(m => m.PrevailingWage == model.PrevailingWageFlag).ToList();
                    else
                        responseFilterObj.AddRange(responseStateFilter);

                    if (!string.IsNullOrEmpty(model.SearchText))
                        response.AddRange(responseFilterObj.Where(m => ((!string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0")
                        && m.ProjNumber.Contains(model.SearchText.ToLower())) || (!string.IsNullOrEmpty(m.Title) &&
                        m.Title.ToLower().Contains(model.SearchText.ToLower()))));
                    else
                        response.AddRange(responseFilterObj);
                    responseFilterObj = new();
                    if (model.EstCost != null && model.EstCost != "0")
                    {
                        int minRange = 0;
                        int maxRange = 0;
                        if (model.EstCost.Contains('-'))
                        {
                            string[] costArr = model.EstCost.Split('-');
                            int.TryParse(costArr[0], out minRange);
                            int.TryParse(costArr[1], out maxRange);
                        }
                        else
                        {
                            minRange = int.Parse(model.EstCost);
                            maxRange = int.MaxValue;
                        }
                        if (response != null && response.Count > 0)
                        {
                            foreach (var item in response)
                            {
                                int mnEst = 0;
                                int mxEst = 0;
                                bool Added = false;
                                if (!string.IsNullOrEmpty(item.EstCost) && item.EstCost.Contains('-'))
                                {
                                    string[] costArr = item.EstCost.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[1], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost))
                                {
                                    int.TryParse(item.EstCost, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost2) && item.EstCost2.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost2.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost2) && Added == false)
                                {
                                    int.TryParse(item.EstCost2, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost3) && item.EstCost3.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost3.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost3) && Added == false)
                                {
                                    int.TryParse(item.EstCost3, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost4) && item.EstCost4.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost4.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost4) && Added == false)
                                {
                                    int.TryParse(item.EstCost4, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.EstCost5) && item.EstCost5.Contains('-') && Added == false)
                                {
                                    string[] costArr = item.EstCost5.Split('-');
                                    int.TryParse(costArr[0], out mnEst);
                                    int.TryParse(costArr[0], out mxEst);
                                    if (mnEst > 0 || mxEst > 0)
                                    {
                                        if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                        {
                                            if (mnEst >= minRange || maxRange >= mxEst)
                                            {
                                                responseFilterObj.Add(item);
                                                Added = true;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(item.EstCost5) && Added == false)
                                {
                                    int.TryParse(item.EstCost5, out mnEst);
                                    if (mnEst > 0)
                                    {
                                        if (mnEst >= minRange && maxRange >= mnEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                        }
                        response = new();
                        response.AddRange(responseFilterObj);
                    }
                }
                else
                {
                    DateTime BidDateFrom = Convert.ToDateTime(_dbContext.tblProject.Min(x => x.BidDt));
                    DateTime BidDateTo = Convert.ToDateTime(_dbContext.tblProject.Max(x => x.BidDt));

                    response = await _dbContext.tblProject
                        .Where(m => (m.IsActive == null || m.IsActive == true)
                                    && m.Publish == true
                                    && (
                                        (m.BidDt != null && m.BidDt >= DateTime.Now.Date)
                                        || (m.BidDt >= DateTime.Now.AddMonths(-18) && m.BidDt < DateTime.Now.Date)
                                        || (m.BidDt == null)
                                    )
                                    && !string.IsNullOrEmpty(m.ProjNumber) && m.ProjNumber != "0")
                        .OrderByDescending(t => t.ProjId)
                        .Take(2000)
                        .ToListAsync();
                }
                if (response != null && response.Count > 0)
                {
                    var futureProj = response.Where(m => (m.BidDt == null || m.BidDt >= DateTime.Now.Date) && m.FutureWork == true).ToList();
                    response.AddRange(futureProj);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;

        }


        private SolrQuery CreateSolrQuery(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return null;
            }

            string query = "";

            var terms = userInput.Split(new[] { ',', '&' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(term => term.Trim())
                                 .Where(term => !string.IsNullOrEmpty(term))
                                 .ToList();

            bool isAndSearch = userInput.Contains("&");
            bool isOrSearch = userInput.Contains(",");

            List<string> queryParts = new List<string>();

            foreach (var term in terms)
            {
                if (term.StartsWith("\"") && term.EndsWith("\""))
                {
                    queryParts.Add($"content:{term}");
                }
                else if (term.StartsWith("*") || term.EndsWith("*"))
                {
                    queryParts.Add($"content:{term}");
                }
                else
                {
                    queryParts.Add($"content:*{term}*");
                }
            }

            if (isAndSearch && isOrSearch)
            {
                //throw new InvalidOperationException("Cannot mix commas (OR) and ampersands (AND) in the query.");
            }
            else if (isAndSearch)
            {
                query = string.Join(" AND ", queryParts);
            }
            else
            {
                query = string.Join(" OR ", queryParts);
            }

            return new SolrQuery(query);
        }




    }
}
