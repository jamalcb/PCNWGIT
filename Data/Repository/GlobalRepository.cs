using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    /// <summary>
    /// Global Partail controller of globalMasterController 
    /// </summary>
    public class GlobalRepository : IGlobalRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<GlobalRepository> _logger;
        private readonly string _connectionString;
        public string url;
        private readonly IWebHostEnvironment _Environment;
        private readonly IEntityRepository _entityRepository;

        public GlobalRepository(ApplicationDbContext dbContext, ILogger<GlobalRepository> logger, IWebHostEnvironment Environment, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            url = "";
            _Environment = Environment;
            _entityRepository = entityRepository;
        }
        #region globalmaster/RenewalProjection (GlobalPartialController)
        /// <summary>
        /// Get Data for GlobalMaster/RenewalProjection (GlobalPartialController)(Income Projection)
        /// </summary>
        /// <returns></returns>
        public async Task<List<SuperAdminViewModel>> GetRenewProj()
        {
            List<SuperAdminViewModel> model = new List<SuperAdminViewModel>();
            try
            {
                var ParamYear = new SqlParameter("@year", DateTime.Now.Year.ToString());
                var ParamMonth = new SqlParameter("@month", DateTime.Now.Month.ToString());
                IEnumerable<TblMember> result = await _dbContext.Set<TblMember>().FromSqlRaw($"sp_GetRenewProj @year, @month", ParamYear, ParamMonth).ToListAsync();
                model = result.Select(x => new SuperAdminViewModel
                {
                    Company = x.Company,
                    StartDate = x.InsertDate == null ? "" : Convert.ToDateTime(x.InsertDate).ToString("MM/dd/yyyy"),
                    RenewalDate = x.RenewalDate == null ? "" : Convert.ToDateTime(x.RenewalDate).ToString("MM/dd/yyyy"),
                    Term = string.IsNullOrEmpty(x.Term) ? "" : x.Term,
                    MemberShip = x.MemberCost == null ? 0 : x.MemberCost,
                    Option = x.MemberType == null ? "" : "Option " + x.MemberType.ToString(),
                    Status = x.Inactive == false ? "InActive" : "Active",
                }).ToList();
            }
            catch (Exception Ex)
            {

                _logger.LogWarning(Ex.Message);
            }
            return model;
        }
        /// <summary>
        /// Sort Data according to given date range for GlobalMaster/RenewalProjection (GlobalPartialController)(Income Projection)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<SuperAdminViewModel>> GetRenewProjRange(DateTime startDate, DateTime endDate)
        {
            List<SuperAdminViewModel> model = new List<SuperAdminViewModel>();
            List<TblProject> tblProjects = new List<TblProject>();
            try
            {
                IEnumerable<TblMember> result = _entityRepository.GetEntities().Where(x => x.RenewalDate != null && (x.RenewalDate >= startDate && x.RenewalDate <= endDate)).ToList();
                model = result.Select(x => new SuperAdminViewModel
                {
                    Company = x.Company,
                    StartDate = x.InsertDate == null ? "" : Convert.ToDateTime(x.InsertDate).ToString("MM/dd/yyyy"),
                    RenewalDate = x.RenewalDate == null ? "" : Convert.ToDateTime(x.RenewalDate).ToString("MM/dd/yyyy"),
                    Term = string.IsNullOrEmpty(x.Term) ? "" : x.Term,
                    MemberShip = x.MemberCost == null ? 0 : x.MemberCost,
                    Option = x.MemberType == null ? "" : "Option " + x.MemberType.ToString(),
                    Status = x.Inactive == false ? "InActive" : "Active",
                }).ToList();
            }
            catch (Exception Ex)
            {

                _logger.LogWarning(Ex.Message);
            }
            return model;
        }
        /// <summary>
        /// Sort Data according to month for GlobalMaster/RenewalProjection (GlobalPartialController)(Income Projection)
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<List<SuperAdminViewModel>> GetRenewProjMonth(string month, string year)
        {
            List<SuperAdminViewModel> model = new List<SuperAdminViewModel>();
            try
            {
                var ParamYear = new SqlParameter("@year", year);
                var ParamMonth = new SqlParameter("@month", month);
                IEnumerable<TblMember> result = await _dbContext.Set<TblMember>().FromSqlRaw($"sp_GetRenewProj @year, @month", ParamYear, ParamMonth).ToListAsync();
                model = result.Select(x => new SuperAdminViewModel
                {
                    Company = x.Company,
                    StartDate = x.InsertDate == null ? "" : Convert.ToDateTime(x.InsertDate).ToString("MM/dd/yyyy"),
                    RenewalDate = x.RenewalDate == null ? "" : Convert.ToDateTime(x.RenewalDate).ToString("MM/dd/yyyy"),
                    Term = string.IsNullOrEmpty(x.Term) ? "" : x.Term,
                    MemberShip = x.MemberCost == null ? 0 : x.MemberCost,
                    Option = x.MemberType == null ? "" : "Option " + x.MemberType.ToString(),
                    Status = x.Inactive == false ? "InActive" : "Active",
                }).ToList();
            }
            catch (Exception Ex)
            {

                _logger.LogWarning(Ex.Message);
            }
            return model;
        }
        #endregion
        /// <summary>
        /// Get data for GlobalMaster/ManagePricing (Manage Pricing)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentInfo>> GetMembershipPackage()
        {
            IEnumerable<PaymentInfo> response = new List<PaymentInfo>();
            var result = await (from tab in _dbContext.tblMemberShipSubPlans
                                join mplan in _dbContext.tblMemberShipPlans
                                on tab.MemberShipPlanId equals mplan.MemberShipPlanId
                                select new
                                {
                                    PlanName = mplan.MemberShipPlanName,
                                    PlanId = tab.MemberShipPlanId,
                                    SubPlanActive = tab.Active,
                                    SubPlanId = tab.SubMemberShipPlanId,
                                    MPrice = tab.MonthlyPrice,
                                    YPrice = tab.YearlyPrice,
                                    QPrice = tab.QuarterlyPrice,
                                    SubPlanName = tab.SubMemberShipPlanName
                                }).ToListAsync();
            response = result.Select(x => new PaymentInfo
            {
                MemberShipPlanName = x.PlanName,
                SubMemberShipPlanName = x.SubPlanName,
                MemberShipPlanId = x.PlanId,
                MonthlyPrice = decimal.Round(x.MPrice, 2, MidpointRounding.AwayFromZero),
                YearlyPrice = decimal.Round(x.YPrice, 2, MidpointRounding.AwayFromZero),
                QuarterlyPrice = decimal.Round(x.QPrice, 2, MidpointRounding.AwayFromZero),
                SubMemberShipPlanId = x.SubPlanId,
                Active = x.SubPlanActive
            });
            return response;
        }
        /// <summary>
        /// Update package data from View GlobalMaster/ManagePricing
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SavePackageData(PaymentInfo model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblMemberShipSubPlans subPlans = await _dbContext.tblMemberShipSubPlans.FirstOrDefaultAsync(x => x.SubMemberShipPlanId == model.SubMemberShipPlanId);
                if (subPlans == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    subPlans.MonthlyPrice = model.MonthlyPrice;
                    subPlans.QuarterlyPrice = model.QuarterlyPrice;
                    subPlans.YearlyPrice = model.YearlyPrice;
                    _dbContext.Entry(subPlans).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Get Package on load of GlobalMaster/ManageCounties view for package select
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CountyDescriptionViewModel>> GetPackage()
        {
            IEnumerable<CountyDescriptionViewModel> response = new List<CountyDescriptionViewModel>();
            var result = await (from tab in _dbContext.TblMemberTypeCounty
                                select new
                                {
                                    Id = tab.MemberType,
                                    Package = tab.Package
                                }
                                ).Distinct().ToListAsync();
            response = result.Select(x => new CountyDescriptionViewModel
            {
                MemberType = x.Id,
                PackageName = x.Package
            });
            return response;
        }
        /// <summary>
        /// Get list of all counties GlobalMaster/ManageCounties and also on Project/EditProjectInfo multiple counties
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TblCounty>> GetCounties()
        {
            IEnumerable<TblCounty> response = new List<TblCounty>();
            response = await (from tab in _dbContext.TblCounty
                              select tab
                                ).ToListAsync();
            return response;
        }
        /// <summary>
        /// Manage counties on package select of GlobalMaster/ManageCounties view 
        /// </summary>
        /// <param name="MemberType"></param>
        /// <returns></returns>
        public async Task<List<int?>> OnPackageSelect(int MemberType)
        {
            List<int?> tbl = await (from tab in _dbContext.TblMemberTypeCounty
                                    where tab.MemberType == MemberType && tab.isActive == true
                                    select tab.CountyID
                                ).ToListAsync();
            return tbl;
        }
        /// <summary>
        /// Update counties for selected package of GlobalMaster/ManageCounties 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Package"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateCounty(List<ManageCountyViewModel> model, string Package)
        {
            var response = 0;
            try
            {
                foreach (var item in model)
                {
                    TblMemberTypeCounty result = await _dbContext.TblMemberTypeCounty.SingleOrDefaultAsync(m => m.CountyID == item.CountyId && m.MemberType == item.MemberTypeId);
                    if (result != null)
                    {
                        if (result.isActive != item.isActive)
                        {
                            result.isActive = item.isActive;
                            _dbContext.Entry(result).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                            response++;
                        }
                    }
                    else
                    {
                        if (item.isActive)
                        {
                            result = new();
                            result.Package = Package;
                            result.isActive = item.isActive;
                            result.MemberType = item.MemberTypeId;
                            result.CountyID = item.CountyId;
                            _dbContext.TblMemberTypeCounty.Add(result);
                            await _dbContext.SaveChangesAsync();
                            response++;
                        }
                    }
                }
                response++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Get data for GlobalMaster/ManageCountyContent (Manage County Content)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CountyDescriptionViewModel>> GetCountyText()
        {
            IEnumerable<CountyDescriptionViewModel> response = new List<CountyDescriptionViewModel>();
            var result = await (from tab in _dbContext.tblPaymentCardDetail
                                select tab
                                ).ToListAsync();
            response = result.Select(x => new CountyDescriptionViewModel
            {
                Id = x.Id,
                UserText = x.UserText,
                RegionText = x.RegionText,
                PackageName = x.PackageName,
                RegionHead = x.RegionHead,
                MemberType = x.MemberType

            });
            return response;
        }
        /// <summary>
        /// Save data for GlobalMaster/ManageCountyContent (Manage County Content)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveStateText(CountyDescriptionViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblPaymentCardDetail subPlans = await _dbContext.tblPaymentCardDetail.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (subPlans == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    subPlans.RegionText = model.RegionText;
                    _dbContext.Entry(subPlans).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Get Data for GlobalMaster/ManageCopyCenterPricing (Copy Center Price)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CopyCenterAdminViewModel>> GetCopyCenterDetail()
        {
            IEnumerable<CopyCenterAdminViewModel> response = new List<CopyCenterAdminViewModel>();
            List<tblProjOrdChrgsDetails> result = await _dbContext.tblProjOrdChrgsDetails.ToListAsync();
            response = result.Select(x => new CopyCenterAdminViewModel
            {
                Id = x.Id,
                SizeName = x.SizeName,
                Size = x.Size,
                MemberPrice = decimal.Round(Convert.ToDecimal(x.MemberPrice), 2, MidpointRounding.AwayFromZero),
                NonMemberPrice = decimal.Round(Convert.ToDecimal(x.NonMemberPrice), 2, MidpointRounding.AwayFromZero),
                ColorMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorMemberPrice), 2, MidpointRounding.AwayFromZero),
                ColorNonMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorNonMemberPrice), 2, MidpointRounding.AwayFromZero),
                isActive = x.isActive == true ? "Active" : "Inactive"
            }
                );
            return response;
        }
        /// <summary>
        /// Get data for GlobalMaster/ManageCopyCenterSize (Copy Center Size) 
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetAllCopyCenterSizeListAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<CopyCenterAdminViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.tblProjOrdChrgsDetails.Select(x => new CopyCenterAdminViewModel
                {
                    Id = x.Id,
                    SizeName = x.SizeName,
                    Size = x.Size,
                    MemberPrice = decimal.Round(Convert.ToDecimal(x.MemberPrice), 2, MidpointRounding.AwayFromZero),
                    NonMemberPrice = decimal.Round(Convert.ToDecimal(x.NonMemberPrice), 2, MidpointRounding.AwayFromZero),
                    ColorMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorMemberPrice), 2, MidpointRounding.AwayFromZero),
                    ColorNonMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorNonMemberPrice), 2, MidpointRounding.AwayFromZero),
                    isActive = x.isActive == true ? "Active" : "Inactive"
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "All data bind";
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
        /// Switch Active/Inactive for GlobalMaster/ManageCopyCenterSize (Copy Center Size)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetActiveSizeListAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<CopyCenterAdminViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.tblProjOrdChrgsDetails.Where(m => m.isActive == true).Select(x => new CopyCenterAdminViewModel
                {
                    Id = x.Id,
                    SizeName = x.SizeName,
                    Size = x.Size,
                    MemberPrice = decimal.Round(Convert.ToDecimal(x.MemberPrice), 2, MidpointRounding.AwayFromZero),
                    NonMemberPrice = decimal.Round(Convert.ToDecimal(x.NonMemberPrice), 2, MidpointRounding.AwayFromZero),
                    ColorMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorMemberPrice), 2, MidpointRounding.AwayFromZero),
                    ColorNonMemberPrice = decimal.Round(Convert.ToDecimal(x.ColorNonMemberPrice), 2, MidpointRounding.AwayFromZero),
                    isActive = x.isActive == true ? "Active" : "Inactive"
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Active data bind";
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
        /// Edit size for GlobalMaster/ManageCopyCenterSize (Copy Center Size)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> EditPageSize(CopyCenterAdminViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblProjOrdChrgsDetails tblProjOrdChrgsDetails = await _dbContext.tblProjOrdChrgsDetails.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (tblProjOrdChrgsDetails == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    tblProjOrdChrgsDetails.Size = model.Size;
                    tblProjOrdChrgsDetails.isActive = model.isActive == "Active" ? true : false;
                    _dbContext.Entry(tblProjOrdChrgsDetails).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Save new size for GlobalMaster/ManageCopyCenterSize (Copy Center Size)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SavePageSize(CopyCenterAdminViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblProjOrdChrgsDetails tblProjOrdChrgsDetails = new();
                tblProjOrdChrgsDetails.SizeName = model.SizeName;
                tblProjOrdChrgsDetails.Size = model.Size;
                tblProjOrdChrgsDetails.MemberPrice = model.MemberPrice;
                tblProjOrdChrgsDetails.NonMemberPrice = model.NonMemberPrice;
                tblProjOrdChrgsDetails.isActive = true;

                if (tblProjOrdChrgsDetails == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    tblProjOrdChrgsDetails.Size = model.Size;
                    tblProjOrdChrgsDetails.isActive = model.isActive == "Active" ? true : false;
                    await _dbContext.tblProjOrdChrgsDetails.AddAsync(tblProjOrdChrgsDetails);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Edit price for GlobalMaster/ManageCopyCenterPricing (Copy Center Price)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> EditPagePrice(CopyCenterAdminViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblProjOrdChrgsDetails tblProjOrdChrgsDetails = await _dbContext.tblProjOrdChrgsDetails.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (tblProjOrdChrgsDetails == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    tblProjOrdChrgsDetails.MemberPrice = model.MemberPrice;
                    tblProjOrdChrgsDetails.NonMemberPrice = model.NonMemberPrice;
                    tblProjOrdChrgsDetails.ColorMemberPrice = model.ColorMemberPrice;
                    tblProjOrdChrgsDetails.ColorNonMemberPrice = model.ColorNonMemberPrice;
                    tblProjOrdChrgsDetails.isActive = model.isActive == "Active" ? true : false;
                    _dbContext.Entry(tblProjOrdChrgsDetails).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Get data for GlobalMaster/ManageDiscount
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DiscountViewModel>> GetDiscountDetails()
        {
            IEnumerable<DiscountViewModel> response = new List<DiscountViewModel>();
            List<tblDiscount> tblDiscount = await _dbContext.tblDiscount.ToListAsync();
            response = tblDiscount.Select(x => new DiscountViewModel
            {
                DiscountId = x.DiscountId,
                DiscountRate = x.DiscountRate,
                StartDate = x.StartDate,
                Description = x.Description,
                EndDate = x.EndDate,
                isActive = x.isActive == true ? "Active" : "Inactive"
            });
            return response;
        }
        /// <summary>
        /// Add new discount detail from GlobalMaster/ManageDiscount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveDiscount(DiscountViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                List<tblDiscount> list = await _dbContext.tblDiscount.ToListAsync();
                if (list.Count > 0)
                {
                    List<tblDiscount> overlappingRange = list.Where(dr => (model.StartDate <= dr.EndDate) && (model.EndDate >= dr.StartDate)).ToList();

                    if (overlappingRange != null && overlappingRange.Count > 0)
                    {
                        response.success = false;
                        response.statusCode = "404";
                        response.statusMessage = "There is already a message set at given time range.";
                        return response;
                    }
                }
                tblDiscount tblDiscount = new();
                tblDiscount.DiscountRate = model.DiscountRate;
                tblDiscount.StartDate = model.StartDate;
                tblDiscount.EndDate = model.EndDate;
                tblDiscount.Description = model.Description;
                tblDiscount.isActive = model.isActive == "Active" ? true : false;
                await _dbContext.tblDiscount.AddAsync(tblDiscount);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Data saved successfully";


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
        /// Update/Edit discount detail from GlobalMaster/ManageDiscount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDiscount(DiscountViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                List<tblDiscount> list = await _dbContext.tblDiscount.ToListAsync();
                if (list.Count > 0)
                {
                    List<tblDiscount> overlappingRange = list.Where(dr => (model.StartDate <= dr.EndDate) && (model.EndDate >= dr.StartDate)).ToList();

                    if (overlappingRange != null && overlappingRange.Count > 0)
                    {
                        response.success = false;
                        response.statusCode = "404";
                        response.statusMessage = "There is already a message set at given time range.";
                        return response;
                    }
                }

                tblDiscount tblDiscount = await _dbContext.tblDiscount.FirstOrDefaultAsync(x => x.DiscountId == model.DiscountId);
                if (tblDiscount == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    tblDiscount.DiscountRate = model.DiscountRate;
                    tblDiscount.StartDate = model.StartDate;
                    tblDiscount.EndDate = model.EndDate;
                    tblDiscount.Description = model.Description;
                    tblDiscount.isActive = model.isActive == "Active" ? true : false;
                    _dbContext.Entry(tblDiscount).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Update/Edit discount detail from GlobalMaster/ManageBulletPoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveBulletPoints(CountyDescriptionViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblPaymentCardDetail subPlans = await _dbContext.tblPaymentCardDetail.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (subPlans == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    subPlans.UserText = model.UserText;
                    subPlans.RegionHead = model.RegionHead;
                    _dbContext.Entry(subPlans).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
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
        /// Get details from GlobalMaster/DailyMailer (Daily Mailer)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DailyMailerViewModel>> DailyMailer()
        {
            IEnumerable<DailyMailerViewModel> response = new List<DailyMailerViewModel>();
            List<TblDailyMailer> results = new List<TblDailyMailer>();
            results = await (from tab in _dbContext.TblDailyMailer
                             select tab
                                ).ToListAsync();
            response = results.Select(x => new DailyMailerViewModel
            {
                Id = x.Id,
                MailerPath = x.MailerPath,
                MailerText = x.MailerText,
                IsActive = x.IsActive,
                //ImageUrl = Path.Combine(rootpath , x.MailerPath),
                //ImageUrl = rootpath+ x.MailerPath,
            });

            return response;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> DailyMailerListAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblDailyMailer> dataList = new();
            try
            {
                dataList = await _dbContext.TblDailyMailer.Select(x => new TblDailyMailer
                {
                    Id = x.Id,
                    MailerText = x.MailerText,
                    IsActive = x.IsActive,
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Daily mailer data bind successfully";
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
        /// Update/Edit details from GlobalMaster/DailyMailer (Daily Mailer)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDailyMailer(DailyMailerViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblDailyMailer TblDailyMailer = await _dbContext.TblDailyMailer.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (TblDailyMailer == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    string wwwRootPath = _Environment.WebRootPath;
                    string fileName = Path.GetFileName(model.MailerPath);
                    string extension = Path.GetExtension(model.MailerPath);

                    string mailerPath = "/MailerImage/" + fileName;
                    TblDailyMailer.MailerPath = mailerPath;
                    TblDailyMailer.MailerText = model.MailerText;
                    TblDailyMailer.IsActive = model.IsActive;
                    _dbContext.Entry(TblDailyMailer).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data updated successfully";
                    response.data = mailerPath;
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
        /// Save details from GlobalMaster/DailyMailer (Daily Mailer)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveDailyMailer(DailyMailerViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            TblDailyMailer TblDailyMailer = new();
            try
            {
                string wwwRootPath = _Environment.WebRootPath;
                string fileName = Path.GetFileName(model.MailerPath);
                string extension = Path.GetExtension(model.MailerPath);

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    string mailerPath = "/MailerImage/" + fileName;
                    TblDailyMailer.MailerPath = mailerPath;
                    TblDailyMailer.MailerText = model.MailerText;
                    TblDailyMailer.IsActive = model.IsActive;
                    _dbContext.TblDailyMailer.Add(TblDailyMailer);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Data saved successfully";
                }
                else
                {
                    response.success = false;
                    response.statusCode = "400";
                    response.statusMessage = "Please upload only jpg, jpeg, png file.";
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
        /// Remove image from GlobalMaster/DailyMailer (Daily Mailer)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public async Task<dynamic> RemoveImage(int Id, string Path)
        {
            HttpResponseDetail<dynamic> response = new();
            string rootPath = _Environment.WebRootPath;
            try
            {
                TblDailyMailer subPlans = await _dbContext.TblDailyMailer.FirstOrDefaultAsync(x => x.Id == Id);
                if (subPlans == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    subPlans.MailerPath = "";
                    _dbContext.Entry(subPlans).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Image removed successfully";
                    if (File.Exists(rootPath + Path))
                    {
                        File.Delete(rootPath + Path);
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
    }
}
