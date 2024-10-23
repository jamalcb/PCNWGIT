using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PCNW.Data.ADO;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using PCNW.ViewModel;
using System.Data;
using System.Globalization;
namespace PCNW.Data.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CommonRepository> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly string _connectionString;
        public string url;
        private readonly IEntityRepository _entityRepository;

        public CommonRepository(ApplicationDbContext dbContext, ILogger<CommonRepository> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            url = "";
            _entityRepository = entityRepository;
        }
        /// <summary>
        /// Get the currently available project bidding project fro index
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCurrentBiddingProject()
        {
            Int32 biddingProjectCount = 0;
            DataTable dt = new();
            try
            {
                Dictionary<string, object> Params = new();
                MyConnection con = new MyConnection(_connectionString);
                con.cmd.CommandText = "sp_GetBiddingProjectCount";
                con.cmd.CommandType = CommandType.StoredProcedure;
                con.cmd.Parameters.AddWithValue("@SpType", 0);
                SqlParameter p3 = new SqlParameter("@msg", SqlDbType.VarChar, 200);
                p3.Direction = ParameterDirection.Output;
                con.cmd.Parameters.Add(p3);
                try
                {
                    con.Open();
                    SqlDataAdapter adp = new SqlDataAdapter(con.cmd);
                    adp.Fill(dt);
                    con.Close();
                    string msg = con.cmd.Parameters["@msg"].Value.ToString();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
                finally
                {
                    con.Close();
                    con.cmd.Dispose();
                }
                //var result = await GeneralMethods.DynamicListFromSqlAsync(_dbContext, "sp_GetBiddingProjectCount", Params);
                //biddingProjectCount = result.Count();
                if (dt != null)
                    if (dt.Rows.Count > 0)
                        biddingProjectCount = dt.Rows[0][0] is DBNull ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                biddingProjectCount = 0;
            }
            return biddingProjectCount;
        }
        /// <summary>
        /// Check for unique email address for email field
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string CheckUniqueEmail(string email)
        {
            return _dbContext.Users.Any(x => x.Email != email) ?
                    "true" : string.Format("an account for address {0} already exists.", email);
        }
        /// <summary>
        /// Get user information of logged in member from windows service
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public DisplayLoginInfo GetUserInfo(string email)
        {
            DisplayLoginInfo response = new();
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    var res = _dbContext.TblContacts.FirstOrDefault(m => m.Email == email && (m.CompType == 1 || m.CompType == 4));
                    if (res != null)
                    {
                        res.Id = (res.CompType == 4) ? 0 : res.Id;
                        if (res.Id != 0)
                        {
                            response.Id = res.Id;
                            var comp = (_entityRepository.GetEntities().Any(m => m.Id == res.Id)) ? _entityRepository.GetEntities().FirstOrDefault(m => m.Id == res.Id).Company : _dbContext.BusinessEntities.FirstOrDefault(m => m.BusinessEntityId == res.Id).BusinessEntityName;

                            TblMember tbl = _entityRepository.GetEntities().SingleOrDefault(t => t.Id == res.Id);
                            if (tbl.Inactive == true)
                            {
                                response.InActive = true;
                            }
                            else if (tbl.RenewalDate != null)
                            {
                                DateTime lastpaydate = Convert.ToDateTime(tbl.RenewalDate);
                                DateTime currentdate = DateTime.Now.Date;
                                if (currentdate > lastpaydate)
                                {
                                    tbl.Inactive = true;
                                    _entityRepository.UpdateEntity(tbl);
                                    response.InActive = true;
                                }
                                else
                                {
                                    response.InActive = false;
                                }
                            }
                            else
                            {
                                response.InActive = false;
                            }
                            if (comp != null)
                            {
                                response.Company = comp;
                            }
                        }
                        response.ConId = res.ConId;
                        response.Name = res.Contact ?? "Member";
                        response.Email = res.Email ?? string.Empty;
                        response.Phone = res.Phone ?? string.Empty;
                        response.Uid = res.Uid ?? string.Empty;
                        var user1 = _userManager.FindByEmailAsync(email);
                        var role = _userManager.GetRolesAsync(user1.Result);
                        if (role != null)
                        {
                            response.Role = role.Result.ToString();
                        }
                        else
                        {
                            response.Role = "";
                        }
                    }
                    else
                    {
                        var user = _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            response.Name = email;
                            response.Email = email;
                            var user1 = _userManager.FindByEmailAsync(email);
                            var role = _userManager.GetRolesAsync(user1.Result);
                            if (role != null)
                            {
                                response.Role = role.Result.ToString();
                            }
                            else
                            {
                                response.Role = "";
                            }
                        }
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
        /// No use
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<List<MemberDashboardViewModel>> GetDashboardInfo(string username)
        {
            List<MemberDashboardViewModel> result = new();
            var paramUserName = new SqlParameter("@username", username);
            try
            {
                result = await _dbContext.MemberDashboardViewModel.FromSqlRaw($"sp_GetYourProjectDashboadInfo @username", paramUserName).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// Record login activity in TblLogActivity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> GetLogActivityAsync(LoginViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                List<TblLogActivity> dataList = await _dbContext.TblLogActivity.OrderByDescending(x => x.Key).Take(1).ToListAsync();

                TblLogActivity tblLogActivity = new();
                tblLogActivity.UserName = model.Email;
                tblLogActivity.LoginTime = DateTime.Now;
                tblLogActivity.LoginFlag = true;
                tblLogActivity.Key = GetRandomKey();
                await _dbContext.TblLogActivity.AddAsync(tblLogActivity);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Login Activity created successfully";
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
        /// Record logout activity in TblLogActivity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> GetLogOutActivityAsync(string name, bool IsAutoLogout)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                if (IsAutoLogout != false)
                {
                    List<TblLogActivity> dataList1 = await _dbContext.TblLogActivity.Where(x => x.UserName == name).OrderByDescending(x => x.Id).Take(1).ToListAsync();
                    TblLogActivity _tblLogActivity = await _dbContext.TblLogActivity.SingleOrDefaultAsync(m => m.Id == dataList1[0].Id);
                    if (_tblLogActivity != null)
                    {
                        _tblLogActivity.LoginFlag = false;
                        _tblLogActivity.LastActivity = DateTime.Now;
                        _tblLogActivity.IsAutoLogout = true;
                        _dbContext.Entry(_tblLogActivity).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    List<TblLogActivity> dataList = await _dbContext.TblLogActivity.Where(x => x.UserName == name).OrderByDescending(x => x.Id).Take(1).ToListAsync();
                    TblLogActivity tblLogActivity = await _dbContext.TblLogActivity.SingleOrDefaultAsync(m => m.Id == dataList[0].Id);
                    if (tblLogActivity != null)
                    {
                        tblLogActivity.LoginFlag = false;
                        tblLogActivity.LastActivity = DateTime.Now;
                        _dbContext.Entry(tblLogActivity).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
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

        #region Function 
        /// <summary>
        /// Generating random key
        /// </summary>
        /// <returns></returns>
        public string GetRandomKey()
        {
            Random res = new Random();
            // String of alphabets 
            String str = "0123456789abcdefghijklmnopqrstuvwxyz";
            int size = 15;
            // Initializing the empty string
            String ran = "";
            for (int i = 0; i < size; i++)
            {
                // Selecting a index randomly
                int x = res.Next(26);
                // Appending the character at the 
                // index to the random string.
                ran = ran + str[x];
            }
            return ran;
        }
        #endregion
        /// <summary>
        /// Get details of copy center price
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCopyCenterPriceDetail()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblProjOrdChrgsDetails> tblProjOrdChrgs = new();
            try
            {
                tblProjOrdChrgs = await _dbContext.tblProjOrdChrgsDetails.Where(x => x.isActive == true).ToListAsync();
                response.data = tblProjOrdChrgs;
                response.success = true;
                response.statusMessage = "Data populated successfully";
                response.statusCode = "200";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = "Something went wrong";
                response.statusCode = "400";
                response.data = ex.Message;
            }
            return response;
        }
        /// <summary>
        /// To implemnet reorder functionality from member and staff copy center dashbaord
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<dynamic> Reorder(int OrderId)
        {
            HttpResponseDetail<dynamic> response = new();
            TblProjOrder projOrder = await _dbContext.TblProjOrder.SingleOrDefaultAsync(x => x.OrderId == OrderId);
            List<tblProjOrdChrgsDetails> tblProjOrdChrgs = new();
            if (projOrder != null)
            {
                try
                {
                    TblProjOrder order = new();
                    order = projOrder;
                    order.OrderId = 0;
                    order.ShipDt = null;
                    order.DoneDt = null;
                    order.Viewed = false;
                    order.OrderDt = DateTime.Now;
                    await _dbContext.TblProjOrder.AddAsync(order);
                    await _dbContext.SaveChangesAsync();
                    List<TblProjOrderDetail> details = _dbContext.TblProjOrderDetail.Where(x => x.OrderId == OrderId).ToList();
                    foreach (var det in details)
                    {
                        TblProjOrderDetail orderDetail = new();
                        orderDetail.OrderId = order.OrderId;
                        orderDetail.FileName = det.FileName;
                        orderDetail.Pages = det.Pages;
                        orderDetail.Copies = det.Copies;
                        orderDetail.Size = det.Size;
                        orderDetail.Price = det.Price;
                        await _dbContext.TblProjOrderDetail.AddAsync(orderDetail);
                        await _dbContext.SaveChangesAsync();
                    }
                    List<int> lst = new List<int>();
                    lst.Add(OrderId);
                    lst.Add(order.OrderId);
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Project reordered successfully";
                    response.data = lst;
                }
                catch (Exception ex)
                {
                    response.success = false;
                    response.statusMessage = "Something went wrong";
                    response.statusCode = "400";
                    response.data = ex.Message;
                }
            }
            return response;
        }
        /// <summary>
        /// Saving directory information from memberprofile page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveDirectory(TblDirectoryCheck model)
        {
            HttpResponseDetail<dynamic> response = new();
            if (model.DirId == 0)
            {
                try
                {
                    await _dbContext.TblDirectoryCheck.AddAsync(model);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusMessage = "Directory data saved successfully";
                    response.statusCode = "200";
                }
                catch (Exception ex)
                {
                    response.success = false;
                    response.statusMessage = "Something went wrong";
                    response.statusCode = "400";
                    response.data = ex.Message;
                }
            }
            else
            {
                try
                {
                    _dbContext.Entry(model).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusMessage = "Directory data updated successfully";
                    response.statusCode = "200";
                }
                catch (Exception ex)
                {
                    response.success = false;
                    response.statusMessage = "Something went wrong";
                    response.statusCode = "400";
                    response.data = ex.Message;
                }

            }
            return response;
        }
        /// <summary>
        /// Saving license information from memberprofile page
        /// </summary>
        /// <param name="lstState"></param>
        /// <param name="LicNum"></param>
        /// <param name="LicDesc"></param>
        /// <param name="MemId"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveLic(List<int> lstState, string LicNum, string LicDesc, int MemId)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                if (lstState != null)
                {
                    foreach (int item in lstState)
                    {
                        tblLicense license = new();
                        license.LicNum = LicNum;
                        license.LicDesc = LicDesc;
                        license.MemId = MemId;
                        license.LicState = item;
                        await _dbContext.tblLicense.AddAsync(license);
                        await _dbContext.SaveChangesAsync();
                        response.success = true;
                        response.statusMessage = "License data saved successfully";
                        response.statusCode = "200";
                    }
                }
            }
            catch (Exception Ex)
            {
                response.success = false;
                response.statusMessage = "Something went wrong";
                response.statusCode = "400";
                response.data = Ex.Message;
            }
            return response;
        }
        /// <summary>
        /// Saving location information from memberprofile page
        /// </summary>
        /// <param name="lstState"></param>
        /// <param name="LicNum"></param>
        /// <param name="LicDesc"></param>
        /// <param name="MemId"></param>
        /// <returns></returns>
        public async Task<dynamic> AddLocation(TblLocList model)
        {
            HttpResponseDetail<dynamic> response = new();

            try
            {
                await _dbContext.TblLocList.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusMessage = "Loaction added successfully";
                response.statusCode = "200";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = "Something went wrong";
                response.statusCode = "400";
                response.data = ex.Message;
            }

            return response;
        }
        /// <summary>
        /// Get information of logged in member
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<DisplayLoginInfo> GetUserInfoAsync(string email)
        {
            DisplayLoginInfo response = new();
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    var res = await _dbContext.TblContacts.FirstOrDefaultAsync(m => m.Email == email);
                    if (res != null)
                    {
                        response.Id = res.Id;
                        TblMember tblMember = _entityRepository.GetEntities().FirstOrDefault(m => m.Id == res.Id);
                        string comp = string.Empty;
                        if (tblMember != null)
                        {
                            comp = tblMember != null ? tblMember.Company : string.Empty;
                        }

                        if (comp != null)
                        {
                            response.Company = comp;
                        }
                        response.ConId = res.ConId;
                        response.Name = res.Contact ?? "Member";
                        response.Email = res.Email ?? string.Empty;
                        response.Phone = res.Phone ?? string.Empty;
                        response.Uid = res.Uid ?? string.Empty;
                    }
                    else
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            response.Name = email;
                            response.Email = email;
                        }
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
        /// Saving member and non member project
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
                tblProject.Title = model.Title;
                tblProject.BidDt = model.BidDt;
                tblProject.BidDt2 = model.BidDt2;
                tblProject.BidDt3 = model.BidDt3;
                tblProject.BidDt4 = model.BidDt4;
                tblProject.BidDt5 = model.BidDt5;
                tblProject.ArrivalDt = DateTime.Now;
                tblProject.LocAddr1 = model.LocAddr1;
                tblProject.LocCity = model.LocCity;
                tblProject.LocState = model.LocState;
                tblProject.PreBidDt = model.PreBidDt;
                tblProject.PreBidDt2 = model.PreBidDt2;
                tblProject.PreBidDt3 = model.PreBidDt3;
                tblProject.PreBidDt4 = model.PreBidDt4;
                tblProject.PreBidDt5 = model.PreBidDt5;
                tblProject.PreBidLoc = model.PreBidLoc;
                tblProject.PreBidLoc2 = model.PreBidLoc2;
                tblProject.PreBidLoc3 = model.PreBidLoc3;
                tblProject.PreBidLoc4 = model.PreBidLoc4;
                tblProject.PreBidLoc5 = model.PreBidLoc5;
                tblProject.LocZip = model.LocZip;
                tblProject.EstCost = model.EstCost;
                tblProject.ProjNote = "This project was uploaded by " + model.ContactName + " of " + model.ContactMember + " on " + DateTime.Now.ToString("MM/dd/yyyy");
                tblProject.SpecsOnPlans = model.SpecsOnPlans;
                tblProject.SpcChk = model.SpcChk;
                tblProject.PrevailingWage = model.PrevailingWage;
                tblProject.ProjTypeId = Convert.ToInt32(model.ProjTypeId);
                tblProject.ProjSubTypeId = Convert.ToInt32(model.ProjSubTypeId);
                tblProject.ProjScope = model.ProjScope;
                tblProject.StrBidDt = model.strBidDt;
                tblProject.StrBidDt2 = model.strBidDt2;
                tblProject.StrBidDt3 = model.strBidDt3;
                tblProject.StrBidDt4 = model.strBidDt4;
                tblProject.StrBidDt5 = model.strBidDt5;
                tblProject.LocAddr2 = model.LocAddr2;
                tblProject.createdDate = DateTime.Now;
                tblProject.createdBy = model.ContactName;
                tblProject.memberId = model.memberId;
                _dbContext.tblProject.Add(tblProject);
                _dbContext.SaveChanges();
                model.ProjId = tblProject.ProjId;
                httpResponse.data = model;
                if (model.EIList != null)
                {
                    for (int i = 0; i < model.EIList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(model.EIList[i].EntityName) && !string.IsNullOrEmpty(model.EIList[i].EntityType))
                        {
                            TblEntity entity = new();
                            entity.ProjNumber = Convert.ToInt32(model.ProjNumber);
                            entity.Projid = model.ProjId;
                            entity.EnityName = model.EIList[i].EntityName;
                            entity.EntityType = model.EIList[i].EntityType;
                            _dbContext.TblEntity.Add(entity);
                            _dbContext.SaveChanges();
                        }
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
        /// Confirm bidding for project
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="usern"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<dynamic> ConfirmBiddingProj(string Id, string usern, string status)
        {
            HttpResponseDetail<dynamic> response = new();
            int ConIDs = (from c in _dbContext.TblContacts where c.Email == usern && c.CompType == 1 select c.ConId).FirstOrDefault();
            int Ids = (from c in _dbContext.TblContacts where c.Email == usern && c.CompType == 1 select c.Id).FirstOrDefault();
            string UID = (from c in _dbContext.TblContacts where c.Email == usern select c.Uid).FirstOrDefault();
            int ProjId = Convert.ToInt32(Id);
            string userId = Convert.ToString(ConIDs);

            try
            {

                TblBidStatus BidStatus = await _dbContext.TblBidStatus.FirstOrDefaultAsync(M => M.Uid == UID && M.Projid == ProjId);
                if (BidStatus != null)
                {
                    if (status == "N")
                    {
                        BidStatus.Bidding = false;
                        _dbContext.TblBidStatus.Update(BidStatus);
                        await _dbContext.SaveChangesAsync();
                        response.success = true;
                        response.statusMessage = "Thanks for confirmation";
                        response.data = "You have opted out from bidding for this project";
                        response.statusCode = "200";
                    }
                    else if (status == "Y")
                    {
                        BidStatus.Bidding = true;
                        _dbContext.TblBidStatus.Update(BidStatus);
                        await _dbContext.SaveChangesAsync();
                        response.success = true;
                        response.statusMessage = "Thanks for confirmation";
                        response.data = "Thanks for confirmation";
                        response.statusCode = "200";
                    }
                }
                else
                {
                    BidStatus = new();
                    if (status == "N")
                    {
                        response.success = true;
                        response.statusMessage = "Thanks for confirmation";
                        response.data = "You have opted out from bidding for this project";
                        response.statusCode = "200";
                    }
                    else if (status == "Y")
                    {
                        BidStatus.Uid = UID;
                        BidStatus.Contact = ConIDs.ToString();
                        BidStatus.Company = Ids.ToString();
                        BidStatus.Projid = ProjId;
                        BidStatus.Bidding = true;
                        BidStatus.CompType = 1;
                        await _dbContext.AddAsync(BidStatus);
                        await _dbContext.SaveChangesAsync();
                        response.success = true;
                        response.statusMessage = "Thanks for confirmation";
                        response.data = "Thanks for confirmation";
                        response.statusCode = "200";
                    }
                }
                //BidStatus.Bidding = true;
                //_dbContext.TblBidStatus.Update(BidStatus);
                //await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = "Something went wrong";
                response.statusCode = "400";
                response.data = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Function to determine whether the member profile will be ahown in directory or not
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDirectoryAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.ID);
                if (tblMember != null)
                {
                    tblMember.CheckDirectory = model.CheckDirectory;
                    //    _dbContext.Entry(tblMember).State = EntityState.Modified;
                    //    await _dbContext.SaveChangesAsync();
                    _entityRepository.UpdateEntity(tblMember);
                }

                tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.ID);
                response.success = true;
                response.statusCode = "200";
                if (tblMember.CheckDirectory == true)
                {
                    response.statusMessage = "Include in Member Directory";
                }
                else
                {
                    response.statusMessage = "Do not Include in Member Directory";
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
        /// <summary>
        /// To make or remove member as admin from member profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> AdminUserContactAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Id == model.ID && m.MainContact == true);
                if (tblContact != null)
                {
                    tblContact.MainContact = false;
                    _dbContext.Entry(tblContact).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                TblContact _tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Id == model.ID && m.Email == model.Email);
                if (_tblContact != null)
                {
                    _tblContact.MainContact = true;
                    _tblContact.Daily = true;
                    _dbContext.Entry(_tblContact).State = EntityState.Modified;
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
        /// To make or remove daily project facilty to user from member profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> MemberUserDailyReportAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Id == model.ID && m.Email == model.Email);
                if (tblContact != null)
                {
                    tblContact.Daily = model.Daily;
                    _dbContext.Entry(tblContact).State = EntityState.Modified;
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
        /// To get value from incomplete sign up
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<tblIncompleteSignUp> GetSignUpData(int Id)
        {
            tblIncompleteSignUp tbl = new();
            tbl = await _dbContext.tblIncompleteSignUp.SingleOrDefaultAsync(x => x.ID == Id);
            return tbl;
        }
        /// <summary>
        /// Get project by project number
        /// </summary>
        /// <param name="projNo"></param>
        /// <returns></returns>
        public async Task<TblProject> GetProjByNumber(string projNo)
        {
            TblProject tbl = new();
            tbl = await _dbContext.tblProject.Where(x => x.ProjNumber == projNo).FirstOrDefaultAsync();
            return tbl;
        }
        /// <summary>
        /// Check for existance of addenda folder
        /// </summary>
        /// <param name="AddendaNo"></param>
        /// <param name="ProjId"></param>
        /// <param name="parentFolder"></param>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public async Task<TblAddenda> CheckAddenda(string AddendaNo, int ProjId, string parentFolder, int ParentId)
        {
            TblAddenda tbl = new();
            tbl = await _dbContext.TblAddenda.Where(x => x.AddendaNo == AddendaNo && x.ProjId == ProjId && x.ParentFolder == parentFolder && x.ParentId == ParentId).FirstOrDefaultAsync();
            return tbl;
        }
        /// <summary>
        /// Check for existance of addenda filde
        /// </summary>
        /// <param name="AddendaNo"></param>
        /// <param name="ProjId"></param>
        /// <param name="parentFolder"></param>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public async Task<tblSubAddenda> CheckSubAddenda(tblSubAddenda tblSub)
        {
            tblSubAddenda tbl = new();
            tbl = await _dbContext.TblSubAddenda.Where(x => x.AddendaId == tblSub.AddendaId && x.ProjId == tblSub.ProjId && x.ParentFolder == tblSub.ParentFolder && x.PdfFileName == tblSub.PdfFileName).FirstOrDefaultAsync();
            return tbl;
        }
        /// <summary>
        /// Populate sub addenda table
        /// </summary>
        /// <param name="tblSub"></param>
        /// <returns></returns>
        public async Task<dynamic> PopulateSubAddenda(tblSubAddenda tblSub)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                await _dbContext.TblSubAddenda.AddAsync(tblSub);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.TempValue = tblSub.SubAddendaId;
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
        /// Populate addenda table
        /// </summary>
        /// <param name="tblSub"></param>
        /// <returns></returns>
        public async Task<dynamic> PopulateAddenda(TblAddenda tblAdd)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblAdd.InsertDt = DateTime.Now;
                await _dbContext.TblAddenda.AddAsync(tblAdd);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.TempValue = tblAdd.AddendaId;
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
        /// Get card details
        /// </summary>
        /// <returns></returns>
        public async Task<List<tblPaymentCardDetail>> GetPacificCardDetailsAsync()
        {
            List<tblPaymentCardDetail> response = await _dbContext.tblPaymentCardDetail.ToListAsync();
            return response;
        }
        /// <summary>
        /// Get special message in stripe on index page
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetSpecialMsgAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblSpecialMsg> dataList = new();
            try
            {
                var now = DateTime.Now;
                DateTime Today = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                if (DateTime.TryParseExact(Today.ToString("MM/dd/yyyy HH:mm:ss"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    Today = parsedDate;
                }
                dataList = await _dbContext.TblSpecialMsg
                    .Where(m => m.StartDate <= Today && m.EndDate >= Today && m.Type == "Maintenance" && m.IsActive == true).ToListAsync();
                response.data = dataList;
                //}
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Special Message data saved successfully";
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
        /// Get special message for Maintenance in stripe on index page
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetSpecialMsgMainAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblSpecialMsg> dataList = new();
            try
            {
                var now = DateTime.Now;
                DateTime Today = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                if (DateTime.TryParseExact(Today.ToString("MM/dd/yyyy HH:mm:ss"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    Today = parsedDate;
                }
                dataList = await _dbContext.TblSpecialMsg
                    .Where(m => m.StartDate <= Today && m.EndDate >= Today && m.Type == "Marketing" && m.IsActive == true).ToListAsync();
                response.data = dataList;
                //}
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Special Message data saved successfully";
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
        /// To get state daetails by state name
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<dynamic> CheckStateAsync(string State)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                response.data = await _dbContext.TblState.Where(m => m.State == State).ToListAsync();
                response.data = response.data == null ? "" : response.data;
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
        /// <summary>
        /// Saving data from sent project files
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SendProjectFilesAsync(MemberProjectInfo model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            try
            {
                if (model.ProjNumber != null)
                {

                    TblProject _tblProj = await _dbContext.tblProject.SingleOrDefaultAsync(m => m.ProjNumber == model.ProjNumber);
                    if (_tblProj != null)
                    {
                        var year = DateTime.Now.ToString("yy");
                        var month = DateTime.Now.ToString("MM");
                        var projnum = year + month;
                        List<tblUsedProjNuber> dataList = _dbContext.tblUsedProjNuber.Where(m => m.IsUsed == false && m.ProjNumber.Contains(projnum)).ToList();
                        if (dataList != null && dataList.Count > 0)
                        {
                            model.ProjNumber = dataList[0].ProjNumber;
                            tblUsedProjNuber tblUsedProjNuber = _dbContext.tblUsedProjNuber.SingleOrDefault(x => x.Id == dataList[0].Id);
                            if (tblUsedProjNuber != null)
                            {
                                tblUsedProjNuber.IsUsed = true;
                                _dbContext.Entry(tblUsedProjNuber).State = EntityState.Modified;
                                _dbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            IEnumerable<TblProjectCode> result = _dbContext.TblProjectCode.FromSqlRaw("sp_GetProjectNumber").ToList();
                            foreach (var item in result)
                            {
                                model.ProjNumber = item.ProjNumber.ToString();
                            }
                        }

                    }
                }
                TblProject tblprojectnum = await _dbContext.tblProject.SingleOrDefaultAsync(m => m.createdBy == model.ContactName && m.Title == model.Title);
                if (tblprojectnum == null)
                {
                    TblProject tblProject = new();
                    tblProject.ArrivalDt = DateTime.Now;
                    tblProject.ProjNumber = model.ProjNumber;
                    tblProject.BackProjNumber = model.ProjNumber;
                    tblProject.Title = model.Title;
                    tblProject.createdDate = DateTime.Now;
                    tblProject.createdBy = model.ContactName;
                    tblProject.memberId = -1;
                    _dbContext.tblProject.Add(tblProject);
                    _dbContext.SaveChanges();

                    httpResponse.data = model;
                    httpResponse.success = true;
                    httpResponse.statusMessage = "Project uploaded";

                }
                else
                {

                    httpResponse.success = false;
                    httpResponse.statusMessage = "This project name already exist";
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<Int64> GetProjectCodeAsync()
        {
            Int64 response = 0;
            try
            {
                var year = DateTime.Now.ToString("yy");
                var month = DateTime.Now.ToString("MM");
                var projnum = year + month;
                List<tblUsedProjNuber> dataList = _dbContext.tblUsedProjNuber.Where(m => m.IsUsed == false && m.ProjNumber.Contains(projnum)).ToList();
                if (dataList != null && dataList.Count > 0)
                {
                    response = Convert.ToInt64(dataList[0].ProjNumber);
                    tblUsedProjNuber tblUsedProjNuber = _dbContext.tblUsedProjNuber.SingleOrDefault(x => x.Id == dataList[0].Id);
                    if (tblUsedProjNuber != null)
                    {
                        tblUsedProjNuber.IsUsed = true;
                        _dbContext.Entry(tblUsedProjNuber).State = EntityState.Modified;
                        _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    IEnumerable<TblProjectCode> result = _dbContext.TblProjectCode.FromSqlRaw("sp_GetProjectNumber").ToList();
                    foreach (var item in result)
                    {
                        response = item.ProjNumber;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        /// <summary>
        /// To get copy center pricing
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCopyCenterPriceListAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblProjOrdChrgsDetails> dataList = new();
            try
            {
                dataList = await _dbContext.tblProjOrdChrgsDetails.Where(m => m.isActive == true)
                    .Select(x => new tblProjOrdChrgsDetails
                    {
                        Id = x.Id,
                        SizeName = x.SizeName,
                        Size = x.Size,
                        MemberPrice = decimal.Round(Convert.ToDecimal(x.MemberPrice), 2, MidpointRounding.AwayFromZero),
                        NonMemberPrice = decimal.Round(Convert.ToDecimal(x.NonMemberPrice), 2, MidpointRounding.AwayFromZero),
                        ColorMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorMemberPrice), 2, MidpointRounding.AwayFromZero),
                        ColorNonMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorNonMemberPrice), 2, MidpointRounding.AwayFromZero),
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Copy Center Price data bind successfully";
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
        /// Get delivery option details in copy center print order form
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetDeliveryListAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblDeliveryMaster> tblDeliveryMaster = new();
            List<tblDeliveryOption> tblDeliveryOption = new();
            try
            {
                tblDeliveryMaster = await _dbContext.tblDeliveryMaster.Where(m => m.IsActive == true)
                    .Select(x => new tblDeliveryMaster
                    {
                        DelivId = x.DelivId,
                        DelivName = x.DelivName
                    }).ToListAsync();
                tblDeliveryOption = await _dbContext.tblDeliveryOption.Where(m => m.IsActive == true)
                    .Select(x => new tblDeliveryOption
                    {
                        DelivOptId = x.DelivOptId,
                        DelivId = x.DelivId,
                        DelivOptName = x.DelivOptName
                    }).ToListAsync();
                Dictionary<string, List<tblDeliveryOption>> keyValuePairs = new Dictionary<string, List<tblDeliveryOption>>();
                foreach (var item in tblDeliveryMaster)
                {
                    keyValuePairs.Add(item.DelivName, await _dbContext.tblDeliveryOption.Where(m => m.DelivId == item.DelivId && m.IsActive == true).ToListAsync());

                }
                var datalist = keyValuePairs;
                for (int i = 0; i < tblDeliveryMaster.Count; i++)
                {
                    var data = keyValuePairs[tblDeliveryMaster[i]?.DelivName];
                }
                response.data = datalist;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Copy Center Delivery dropdown data bind successfully";
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
        /// Get Member details when ordering from staff print order form of copy center pages
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MemberShipRegistration> GetMemberAsync(int id)
        {
            MemberShipRegistration tbl = new();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().FirstOrDefault(t => t.Id == id);
                TblContact tblContact = await _dbContext.TblContacts.FirstOrDefaultAsync(t => t.Id == id && t.MainContact == true);
                if (tblContact != null)
                {
                    tbl.ID = tblMember.Id;
                    tbl.MemID = tblMember.MemID;
                    tbl.Company = tblMember.Company;
                    if (string.IsNullOrEmpty(tblMember.CompanyPhone))
                    {
                        tbl.CompanyPhone = tblContact.Phone;
                    }
                    else
                    {
                        tbl.CompanyPhone = tblMember.CompanyPhone;

                    }
                    tbl.Email = string.IsNullOrEmpty(tblContact.Email) ? "notavailable@pcnw.com" : tblContact.Email;
                    tbl.BillAddress = string.IsNullOrEmpty(tblMember.BillAddress) ? "No Information" : tblMember.BillAddress;
                    tbl.BillCity = string.IsNullOrEmpty(tblMember.BillCity) ? "No Information" : tblMember.BillCity;
                    tbl.BillZip = string.IsNullOrEmpty(tblMember.BillZip) ? "No Information" : tblMember.BillZip;
                    if (!string.IsNullOrEmpty(tblContact.Contact))
                    {
                        if (tblContact.Contact.Contains(' '))
                        {
                            string[] cntArr = tblContact.Contact.Split(' ');
                            tbl.FirstName = cntArr[0];
                            tbl.LastName = cntArr[1];
                        }
                        else
                        {
                            tbl.FirstName = tblContact.Contact;
                            tbl.LastName = tblContact.Contact;
                        }

                    }
                    else
                    {
                        tbl.FirstName = "Not Available";
                        tbl.LastName = "Not Available";
                    }
                }
                else
                {
                    tblContact = await _dbContext.TblContacts.FirstOrDefaultAsync(t => t.Id == id);
                    if (tblContact != null)
                    {
                        tbl.Company = tblMember.Company;
                        if (string.IsNullOrEmpty(tblMember.CompanyPhone))
                        {
                            tbl.CompanyPhone = tblContact.Phone;
                        }
                        else
                        {
                            tbl.CompanyPhone = tblMember.CompanyPhone;

                        }
                        tbl.Email = string.IsNullOrEmpty(tblContact.Email) ? "notavailable@pcnw.com" : tblContact.Email;
                        tbl.BillAddress = tblMember.BillAddress;
                        tbl.BillCity = tblMember.BillCity;
                        tbl.BillZip = tblMember.BillZip;
                        if (!string.IsNullOrEmpty(tblContact.Contact))
                        {
                            if (tblContact.Contact.Contains(' '))
                            {
                                string[] cntArr = tblContact.Contact.Split(' ');
                                tbl.FirstName = cntArr[0];
                                tbl.LastName = cntArr[1];
                            }
                            else
                            {
                                tbl.FirstName = tblContact.Contact;
                                tbl.LastName = tblContact.Contact;
                            }

                        }
                        else
                        {
                            tbl.FirstName = "Not Available";
                            tbl.LastName = "Not Available";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tbl = null;
                _logger.LogWarning(ex.Message);
            }
            return tbl;
        }
        /// <summary>
        /// Update quickbook id in tblMember
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memId"></param>
        /// <returns></returns>
        public async Task UpdateMemberMemId(int id, int memId)
        {
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().FirstOrDefault(t => t.Id == id);
                tblMember.MemID = memId;
                //_dbContext.Entry(tblMember).State = EntityState.Modified;
                _entityRepository.UpdateEntity(tblMember);

            }
            catch (Exception)
            {

                throw;
            }


        }
        /// <summary>
        /// Update password in tblcontact when resetting the password from resetpassword UI
        /// </summary>
        /// <param name="resetpassword"></param>
        /// <returns></returns>
        public async Task<dynamic> PasswordUpdateAsync(Resetpassword resetpassword)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Email == resetpassword.Email);
                if (tblContact != null)
                {
                    tblContact.Password = resetpassword.ConfirmPassword;
                    _dbContext.Entry(tblContact).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Password updated successfully";
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
        /// Saving staff member from managestaff UI of administration controller
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveStaffMember(StaffManageViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.ConId == model.ConId);
                bool changeFlag = false;
                if (tblContact != null)
                {
                    tblContact.Contact = model.Contact;
                    if (model.Email.Trim().ToLower() != tblContact.Email.Trim().ToLower())
                    {
                        response.Value = tblContact.Email;
                    }
                    var businessobj = await _dbContext.BusinessEntities.SingleOrDefaultAsync(m => m.BusinessEntityId == tblContact.Id);
                    if (businessobj != null)
                    {
                        businessobj.BusinessEntityEmail = model.Email;
                        businessobj.BusinessEntityName = model.Contact;
                        businessobj.BusinessEntityPhone = model.Phone;
                        businessobj.IsArchitect = false;
                        businessobj.IsContractor = false;
                        businessobj.IsMember = false;
                    };
                    _dbContext.Entry(businessobj).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();


                    tblContact.Email = model.Email;
                    tblContact.Phone = model.Phone;
                    tblContact.CompType = 4;
                    tblContact.Password = model.Password;
                    tblContact.Active = model.Active;
                    _dbContext.Entry(tblContact).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Staff member update succesfully";
                    response.data = tblContact;
                    if (tblContact.Active == true)
                    {
                        response.TempValue = 1;
                    }
                    else
                    {
                        response.TempValue = 0;
                    }
                }
                else
                {
                    var businessobj = new BusinessEntity()
                    {
                        BusinessEntityEmail = model.Email,
                        BusinessEntityName = model.Contact,
                        BusinessEntityPhone = model.Phone,
                        IsArchitect = false,
                        IsContractor = false,
                        IsMember = false
                    };
                    await _dbContext.BusinessEntities.AddAsync(businessobj);
                    await _dbContext.SaveChangesAsync();

                    tblContact = new();
                    tblContact.Id = (_dbContext.BusinessEntities.Any()) ? _dbContext.BusinessEntities.Max(m => m.BusinessEntityId) : 1;
                    tblContact.Contact = model.Contact;
                    tblContact.Email = model.Email;
                    tblContact.Phone = model.Phone;
                    tblContact.CompType = 4;
                    tblContact.Active = model.Active;
                    tblContact.Password = model.Password;
                    await _dbContext.AddAsync(tblContact);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Staff member added succesfully";
                    response.data = tblContact;
                    if (tblContact.Active == true)
                    {
                        response.TempValue = 1;
                    }
                    else
                    {
                        response.TempValue = 0;
                    }
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
        /// <summary>
        /// Getting staff member from managestaff UI of administration controller
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetStaffMember()
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                List<TblContact> tblContact = new List<TblContact>();
                List<StaffManageViewModel> staffManages = new();
                tblContact = await _dbContext.TblContacts.Where(x => x.CompType == 4).ToListAsync();
                staffManages = tblContact.Select(x => new StaffManageViewModel
                {
                    ConId = x.ConId,
                    Contact = x.Contact,
                    Email = x.Email,
                    Phone = x.Phone,
                    Active = x.Active
                }).ToList();
                response.success = true;
                response.statusCode = "200";
                response.data = staffManages;
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
        /// Getting data on editstaff popup from managestaff UI of administration controller
        /// </summary>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<dynamic> GetEditData(int ConId)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblContact = new TblContact();
                StaffManageViewModel staffManages = new();
                tblContact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.ConId == ConId);
                staffManages.ConId = tblContact.ConId;
                staffManages.Contact = tblContact.Contact;
                staffManages.Email = tblContact.Email;
                staffManages.Phone = tblContact.Phone;
                staffManages.Active = tblContact.Active;
                staffManages.Password = tblContact.Password;
                response.success = true;
                response.statusCode = "200";
                response.data = staffManages;
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
