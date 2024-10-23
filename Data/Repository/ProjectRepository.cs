using ChargeBee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using System.Data;
using System.Globalization;

namespace PCNW.Data.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProjectRepository> _logger;
        private readonly string _connectionString;
        private readonly IEntityRepository _entityRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public ProjectRepository(UserManager<IdentityUser> userManager,ApplicationDbContext dbContext, ILogger<ProjectRepository> logger, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            _entityRepository = entityRepository;
            _userManager = userManager;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetProjecttype(string SelectedValue = "")
        {
            var response = await (from tab in _dbContext.TblProjType select tab).ToListAsync();
            var result = response.ToList().OrderBy(m => m.SortOrder).Select(x => new SelectListItem
            {
                Text = x.ProjType,
                Value = x.ProjTypeId.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.ProjTypeId.ToString() == SelectedValue ? true : false))
            }).ToList();
            return result;
        }
        /// <summary>
        /// Getting Project Number
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
        /// Saving Project from editprojectinfo page 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveProjectInfoAsync(ProjectInformation model)
        {
            var rowAffected = 0;
            TblProject tblProject = new();
            try
            {
                tblProject.ProjNumber = model.ProjNumber;
                tblProject.BackProjNumber = model.ProjNumber;
                tblProject.Title = model.Title;
                tblProject.ArrivalDt = model.ArrivalDt;
                tblProject.LocAddr1 = model.LocAddr1;
                tblProject.LocCity = model.LocCity;
                tblProject.LocState = model.LocState;
                tblProject.LocZip = model.LocZip;
                tblProject.LocAddr2 = model.LocAddr2;
                tblProject.Latitude = model.Latitude;
                tblProject.Longitude = model.Longitude;
                tblProject.ProjNote = model.ProjNote;
                tblProject.InternalNote = model.InternalNote;
                tblProject.Story = model.Story;
                tblProject.SpecsOnPlans = model.SpecsOnPlans;
                tblProject.SpcChk = model.SpcChk;
                tblProject.PrevailingWage = model.PrevailingWage;
                tblProject.FutureWork = model.FutureWork;
                tblProject.ProjTypeId = Convert.ToInt32(model.ProjTypeId);
                tblProject.ProjSubTypeId = Convert.ToInt32(model.ProjSubTypeId);
                if (!string.IsNullOrEmpty(model.ProjScope))
                {
                    model.ProjScope = model.ProjScope.Substring(0, model.ProjScope.Length - 1);
                }
                tblProject.Brnote = model.BRNote;
                tblProject.Phlnote = model.PHLnote;
                tblProject.ProjScope = model.ProjScope;
                tblProject.CompleteDt = model.CompleteDt;
                tblProject.IssuingOffice = model.IssuingOffice;
                tblProject.createdDate = DateTime.Now;
                tblProject.createdBy = model.Contact;
                tblProject.IsActive = true;
                tblProject.BidBond = model.BidBond;
                tblProject.BidDt = model.BidDt;
                tblProject.LastBidDt = model.LastBidDt;
                string OBidTime = "";
                string hmValue = "00";
                if (model.hComp < 10)
                {
                    hmValue = "0" + model.hComp.ToString();
                }
                else
                {
                    hmValue = model.hComp.ToString();
                }
                if (model.mValue == "PM" && model.tComp != 0)
                {
                    int OtComp = 00;
                    if (model.tComp != 12)
                    {
                        OtComp = 12 + Convert.ToInt32(model.tComp);
                    }
                    else
                    {
                        OtComp = Convert.ToInt32(model.tComp);
                    }
                    OBidTime = OtComp.ToString() + ":" + hmValue;
                }
                else if (model.mValue == "AM" && model.tComp != 0)
                {
                    if (model.tComp < 10)
                    {
                        OBidTime = "0" + model.tComp.ToString() + ":" + hmValue;
                    }
                    else if (model.tComp == 12)
                    {
                        OBidTime = "00:" + hmValue;
                    }
                    else
                    {
                        OBidTime = model.tComp.ToString() + ":" + hmValue;
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
                                    //model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                    //model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                    //model.EstCost = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;
                                    if (!string.IsNullOrEmpty(model.EstCostDetails[i].EstTo))
                                    {
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains(',') ? model.EstCostDetails[i].EstTo.Replace(",", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCostDetails[i].EstTo = model.EstCostDetails[i].EstTo.Contains('$') ? model.EstCostDetails[i].EstTo.Replace("$", "") : model.EstCostDetails[i].EstTo;
                                        model.EstCost = model.EstCostDetails[i].EstFrom + "-" + model.EstCostDetails[i].EstTo;
                                    }
                                    else
                                    {
                                        model.EstCost = model.EstCostDetails[i].EstFrom + "-";

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
                                        model.EstCost2 = model.EstCostDetails[i].EstFrom + "-";

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
                                        model.EstCost3 = model.EstCostDetails[i].EstFrom + "-";

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
                                        model.EstCost4 = model.EstCostDetails[i].EstFrom + "-";

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
                                        model.EstCost5 = model.EstCostDetails[i].EstFrom + "-";

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
                tblProject.AddendaNote = model.AddendaNote;
                tblProject.PrebidNote = model.PrebidNote;
                #endregion
                await _dbContext.tblProject.AddAsync(tblProject);
                rowAffected = await _dbContext.SaveChangesAsync();

                model.ProjId = tblProject.ProjId;
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
                        if (item.PreBidDate != null && item.IsDeleted != true)
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
                            entity.CompType = item.CompType;
                            _dbContext.Entry(entity).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                }
                #endregion
                #region Saving phl information
                if (model.phlInfo != null && model.phlInfo.Count > 0)
                {
                    foreach (var item in model.phlInfo)
                    {
                        bool? decider = item.IsActive == null ? true : item.IsActive;
                        if (!string.IsNullOrEmpty(item.Company) && Convert.ToBoolean(decider) == true)
                        {
                            tblPhlInfo info = new();
                            info.ProjId = tblProject.ProjId;
                            info.MemId = item.MemId;
                            info.ConId = item.ConId;
                            info.CompType = 2;
                            info.PST = "PT";
                            if (item.BidDate != null)
                            {
                                info.BidDate = item.BidDate;
                                info.mValue = item.mValue;
                                info.hComp = item.hComp;
                                info.tComp = item.tComp;
                            }
                            info.IsActive = true;
                            info.PhlType = item.PhlType;
                            info.BidStatus = item.BidStatus;
                            info.Note = item.Note;
                            await _dbContext.tblPhlInfo.AddAsync(info);
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

            }
            catch (Exception ex)
            {
                model.CallBack = true;
                _logger.LogWarning(ex.Message);
            }
            rowAffected = tblProject.ProjId;
            return rowAffected;
        }
        /// <summary>
        /// Updating Project from editprojectinfo page 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateProjectInfoAsync(ProjectInformation model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            TblProject tblProject = new();
            TblProject _tblProject = new();
            tblProject = await _dbContext.tblProject.SingleOrDefaultAsync(x => x.ProjId == model.ProjId);
            _tblProject = tblProject;
            if (tblProject != null)
            {
                try
                {
                    tblProject.ProjNumber = model.ProjNumber;
                    tblProject.BackProjNumber = model.ProjNumber;
                    tblProject.Title = model.Title;
                    tblProject.ArrivalDt = model.ArrivalDt;
                    tblProject.LocAddr1 = model.LocAddr1;
                    tblProject.LocCity = model.LocCity;
                    tblProject.LocState = model.LocState;
                    tblProject.LocZip = model.LocZip;
                    tblProject.LocAddr2 = model.LocAddr2;
                    int costCount = 0;
                    bool costflag = true;
                    if (model.EstCostDetails != null && model.EstCostDetails.Count > 0)
                    {
                        foreach (var item in model.EstCostDetails)
                        {
                            if (!string.IsNullOrEmpty(item.Id))
                            {
                                tblEstCostDetails tblEst = await _dbContext.tblEstCostDetails.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(item.Id) && x.Projid == model.ProjId);
                                if (tblEst != null)
                                {
                                    if (item.Removed == true)
                                    {
                                        tblEst.Removed = true;
                                        _dbContext.Entry(tblEst).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(item.EstFrom))
                                        {
                                            tblEst.Removed = true;
                                            _dbContext.Entry(tblEst).State = EntityState.Modified;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            tblEst.Removed = false;
                                            tblEst.Projid = model.ProjId;
                                            tblEst.EstCostFrom = item.EstFrom.Contains(',') ? item.EstFrom.Replace(",", "") : item.EstFrom;
                                            tblEst.EstCostFrom = item.EstFrom.Contains('$') ? item.EstFrom.Replace("$", "") : item.EstFrom;
                                            //tblEst.EstCostTo = item.EstTo.Contains(',') ? item.EstTo.Replace(",", "") : item.EstTo;
                                            //tblEst.EstCostTo = item.EstTo.Contains('$') ? item.EstTo.Replace("$", "") : item.EstTo;
                                            if (!string.IsNullOrEmpty(item.EstTo))
                                            {
                                                tblEst.EstCostTo = item.EstTo.Contains(',') ? item.EstTo.Replace(",", "") : item.EstTo;
                                                tblEst.EstCostTo = item.EstTo.Contains('$') ? item.EstTo.Replace("$", "") : item.EstTo;
                                            }
                                            tblEst.RangeSign = item.RangeSign;
                                            tblEst.Description = item.Description;
                                            _dbContext.Entry(tblEst).State = EntityState.Modified;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (item.Removed != true)
                                {
                                    if (!string.IsNullOrEmpty(item.EstFrom))
                                    {
                                        tblEstCostDetails tblEstCost = new tblEstCostDetails();
                                        tblEstCost.Removed = false;
                                        tblEstCost.EstCostFrom = item.EstFrom.Contains(',') ? item.EstFrom.Replace(",", "") : item.EstFrom;
                                        tblEstCost.EstCostFrom = item.EstFrom.Contains('$') ? item.EstFrom.Replace("$", "") : item.EstFrom;
                                        if (!string.IsNullOrEmpty(item.EstTo))
                                        {
                                            tblEstCost.EstCostTo = item.EstTo.Contains(',') ? item.EstTo.Replace(",", "") : item.EstTo;
                                            tblEstCost.EstCostTo = item.EstTo.Contains('$') ? item.EstTo.Replace("$", "") : item.EstTo;
                                        }
                                        //tblEstCost.EstCostFrom = item.EstFrom;
                                        //tblEstCost.EstCostTo = item.EstTo;
                                        tblEstCost.Description = item.Description;
                                        tblEstCost.RangeSign = item.RangeSign;
                                        tblEstCost.Projid = model.ProjId;
                                        await _dbContext.tblEstCostDetails.AddAsync(tblEstCost);
                                        await _dbContext.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
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
                                            model.EstCost = model.EstCostDetails[i].EstFrom + "-";

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
                                            model.EstCost2 = model.EstCostDetails[i].EstFrom + "-";

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
                                            model.EstCost3 = model.EstCostDetails[i].EstFrom + "-";

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
                                            model.EstCost4 = model.EstCostDetails[i].EstFrom + "-";

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
                                            model.EstCost5 = model.EstCostDetails[i].EstFrom + "-";

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
                    tblProject.EstCost = string.IsNullOrEmpty(model.EstCost) ? "-" : model.EstCost;
                    tblProject.EstCost2 = string.IsNullOrEmpty(model.EstCost2) ? "-" : model.EstCost2;
                    tblProject.EstCost3 = string.IsNullOrEmpty(model.EstCost3) ? "-" : model.EstCost3;
                    tblProject.EstCost4 = string.IsNullOrEmpty(model.EstCost4) ? "-" : model.EstCost4;
                    tblProject.EstCost5 = string.IsNullOrEmpty(model.EstCost5) ? "-" : model.EstCost5;
                    tblProject.ProjNote = model.ProjNote;
                    tblProject.InternalNote = model.InternalNote;
                    tblProject.Story = model.Story;
                    tblProject.SpecsOnPlans = model.SpecsOnPlans;
                    tblProject.SpcChk = model.SpcChk;
                    tblProject.PrevailingWage = model.PrevailingWage;
                    tblProject.FutureWork = model.FutureWork;
                    tblProject.ProjTypeId = Convert.ToInt32(model.ProjTypeId);
                    tblProject.ProjSubTypeId = Convert.ToInt32(model.ProjSubTypeId);
                    tblProject.BidBond = model.BidBond;
                    tblProject.Brnote = model.BRNote;
                    tblProject.Phlnote = model.PHLnote;
                    if (!string.IsNullOrEmpty(model.ProjScope))
                    {
                        if (model.ProjScope.EndsWith(","))
                            model.ProjScope = model.ProjScope.Substring(0, model.ProjScope.LastIndexOf(','));
                    }
                    tblProject.BidDt2 = model.BidDt2;
                    tblProject.StrBidDt2 = model.strBidDt2;
                    tblProject.StrBidDt4 = model.strBidDt4;
                    tblProject.ProjScope = model.ProjScope;
                    tblProject.CompleteDt = model.CompleteDt;
                    tblProject.IssuingOffice = model.IssuingOffice;
                    tblProject.BidDt = model.BidDt;
                    tblProject.LastBidDt = model.LastBidDt;
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
                    if (model.mValue == "PM" && model.tComp != 0)
                    {
                        int OtComp = 00;
                        if (model.tComp != 12)
                        {
                            OtComp = 12 + Convert.ToInt32(model.tComp);
                        }
                        else
                        {
                            OtComp = Convert.ToInt32(model.tComp);
                        }
                        OBidTime = OtComp.ToString() + ":" + hmValue;
                    }
                    else if (model.mValue == "AM" && model.tComp != 0)
                    {
                        if (model.tComp < 10)
                        {
                            OBidTime = "0" + model.tComp.ToString() + ":" + hmValue;
                        }
                        else if (model.tComp == 12)
                        {
                            OBidTime = "00:" + hmValue;
                        }
                        else
                        {
                            OBidTime = model.tComp.ToString() + ":" + hmValue;
                        }
                    }
                    model.strBidDt5 = OBidTime;
                    tblProject.StrBidDt5 = OBidTime;
                    tblProject.StrBidDt = Convert.ToDateTime(model.BidDt).ToString("MMM dd yyyy") + " " + model.strBidDt5 + " " + model.strBidDt3;
                    tblProject.StrBidDt = model.Undecided == "TBD" ? "TBD" : tblProject.StrBidDt;
                    tblProject.Undecided = model.Undecided;
                    tblProject.StrAddenda = model.strAddenda;
                    tblProject.AddendaNote = model.AddendaNote;
                    tblProject.PrebidNote = model.PrebidNote;
                    tblProject.SubApprov = model.SubApprov;


                    _dbContext.Entry(tblProject).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    List<int> chkEntity = new();
                    List<int> toDel = new();
                    model.ProjId = tblProject.ProjId;
                    if (model.Entities != null && model.Entities.Count > 0)
                    {

                        foreach (var item in model.Entities)
                        {
                            if (item.EntityID == 0)
                            {
                                if (!string.IsNullOrEmpty(item.EntityName) && !string.IsNullOrEmpty(item.EntityType) && item.IsActive != false)
                                {
                                    TblEntity entity = new();
                                    entity.ProjNumber = Convert.ToInt32(model.ProjNumber);
                                    entity.Projid = model.ProjId;
                                    entity.EnityName = item.EntityName;
                                    entity.NameId = item.NameId;
                                    entity.EntityType = item.EntityType;
                                    entity.IsActive = true;
                                    entity.chkIssue = item.chkIssue;
                                    entity.CompType = item.CompType;
                                    await _dbContext.TblEntity.AddAsync(entity);
                                    await _dbContext.SaveChangesAsync();
                                    chkEntity.Add(entity.EntityID);
                                }
                            }
                            else
                            {
                                if (item.IsActive == false)
                                {
                                    TblEntity entity = await _dbContext.TblEntity.SingleOrDefaultAsync(x => x.EntityID == item.EntityID);
                                    if (entity != null)
                                    {
                                        entity.ProjNumber = Convert.ToInt32(model.ProjNumber);
                                        entity.Projid = model.ProjId;
                                        entity.EnityName = item.EntityName;
                                        entity.NameId = item.NameId;
                                        entity.EntityType = item.EntityType;
                                        entity.IsActive = false;
                                        entity.chkIssue = false;
                                        entity.CompType = item.CompType;
                                        _dbContext.Entry(entity).State = EntityState.Modified;
                                        _dbContext.SaveChanges();
                                        chkEntity.Add(entity.EntityID);
                                    }
                                }
                                else
                                {
                                    TblEntity entity = await _dbContext.TblEntity.SingleOrDefaultAsync(x => x.EntityID == item.EntityID);
                                    if (entity != null)
                                    {
                                        if (!string.IsNullOrEmpty(item.EntityName) && !string.IsNullOrEmpty(item.EntityType))
                                        {
                                            entity.ProjNumber = Convert.ToInt32(model.ProjNumber);
                                            entity.Projid = model.ProjId;
                                            entity.EnityName = item.EntityName;
                                            entity.NameId = item.NameId;
                                            entity.EntityType = item.EntityType;
                                            entity.IsActive = true;
                                            entity.chkIssue = item.chkIssue;
                                            entity.CompType = item.CompType;
                                            _dbContext.Entry(entity).State = EntityState.Modified;
                                            _dbContext.SaveChanges();
                                            chkEntity.Add(entity.EntityID);
                                        }
                                        else
                                        {
                                            entity.IsActive = false;
                                            entity.chkIssue = false;
                                            _dbContext.Entry(entity).State = EntityState.Modified;
                                            _dbContext.SaveChanges();
                                            chkEntity.Add(entity.EntityID);
                                        }
                                    }
                                }
                            }

                        }

                    }

                    if (model.AddendaInformation != null)
                    {
                        int AddendaLength = model.AddendaInformation.Count;
                        for (int i = 0; i < AddendaLength; i++)
                        {
                            TblAddenda addenda = await _dbContext.TblAddenda.SingleOrDefaultAsync(x => (x.AddendaNo == model.AddendaInformation[i].AddendaNo) && (x.AddendaId == model.AddendaInformation[i].AddendaId) && (x.ProjId == model.ProjId));
                            if (addenda != null)
                            {
                                addenda.IssueDt = model.AddendaInformation[i].IssueDt;
                                _dbContext.Entry(addenda).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    httpResponse.TempValue++;
                    if (tblProject.BidDt != model.HiddenBidDt)
                    {
                        List<int> Cons = await _dbContext.TblTracking.Where(x => x.ProjId == model.ProjId).Select(x => x.ConId).ToListAsync();
                        List<string> EmailsTo = new();
                        foreach (int i in Cons)
                        {
                            TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(x => x.ConId == i);
                            string temp = tblContact.Email;
                            if (!string.IsNullOrEmpty(temp))
                            {
                                EmailsTo.Add(temp);
                            }
                        }
                        model.EmailsTo = EmailsTo;
                    }
                    if (model.LocAddr2 != null && model.CountyId != 0)
                    {
                        var tblcounty = await _dbContext.TblCounty.SingleOrDefaultAsync(m => m.CountyId == model.CountyId && m.County != model.LocAddr2);
                        if (tblcounty != null)
                        {
                            TblCounty _tblcounty = await _dbContext.TblCounty.SingleOrDefaultAsync(m => m.CountyId == model.CountyId);
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
                    if (model.pInfo != null && model.pInfo.Count > 0)
                    {
                        foreach (var item in model.pInfo)
                        {
                            bool? decider = item.Undecided == null ? false : item.Undecided;
                            if (item.ID == 0)
                            {
                                TblBidStatus tblBid = new();
                                if (!string.IsNullOrEmpty(item.Contact) && !string.IsNullOrEmpty(item.Company) && !Convert.ToBoolean(decider))
                                {
                                    tblBid.Company = item.memId.ToString();
                                    tblBid.Contact = item.Contact;
                                    tblBid.Uid = string.IsNullOrEmpty(item.Uid) ? "0" : item.Uid;
                                    tblBid.Projid = model.ProjId;
                                    tblBid.Bidding = item.bidding;
                                    tblBid.PHLNote = item.PHLNote;
                                    tblBid.PHLType = item.PHLType;
                                    tblBid.CompType = 2;
                                    tblBid.Undecided = false;
                                    await _dbContext.TblBidStatus.AddAsync(tblBid);
                                    await _dbContext.SaveChangesAsync();
                                    item.ID = tblBid.Id;
                                    if (item.phlBid1 != null)
                                    {
                                        if (item.phlBid1.BidDate != null)
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid1.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid1.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid1.hComp.ToString();
                                            }
                                            if (item.phlBid1.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid2.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid1.mValue == "AM")
                                            {
                                                if (item.phlBid1.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid1.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            tblBidDateTime tblBidDate = new();
                                            tblBidDate.BidDateTime = BidTime;
                                            tblBidDate.BidDate = item.phlBid1.BidDate;
                                            tblBidDate.ProjId = model.ProjId;
                                            tblBidDate.PST = item.phlBid1.PST;
                                            tblBidDate.IsDeleted = false;
                                            tblBidDate.Isextensions = false;
                                            tblBidDate.ExtNum = 0;
                                            tblBidDate.PhlId = item.ID;
                                            tblBidDate.CreatedDate = DateTime.Now;
                                            await _dbContext.tblBidDateTime.AddAsync(tblBidDate);
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                    if (item.phlBid2 != null)
                                    {
                                        if (item.phlBid2.BidDate != null)
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid2.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid2.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid2.hComp.ToString();
                                            }
                                            if (item.phlBid2.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid2.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid2.mValue == "AM")
                                            {
                                                if (item.phlBid2.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid2.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            tblBidDateTime tblBidDate = new();
                                            tblBidDate.BidDateTime = BidTime;
                                            tblBidDate.BidDate = item.phlBid2.BidDate;
                                            tblBidDate.ProjId = model.ProjId;
                                            tblBidDate.PST = item.phlBid2.PST;
                                            tblBidDate.IsDeleted = false;
                                            tblBidDate.Isextensions = true;
                                            tblBidDate.ExtNum = 1;
                                            tblBidDate.PhlId = item.ID;
                                            tblBidDate.CreatedDate = DateTime.Now;
                                            await _dbContext.tblBidDateTime.AddAsync(tblBidDate);
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                    if (item.phlBid3 != null)
                                    {
                                        if (item.phlBid3.BidDate != null)
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid3.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid3.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid3.hComp.ToString();
                                            }
                                            if (item.phlBid3.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid3.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid3.mValue == "AM")
                                            {
                                                if (item.phlBid3.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid3.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            tblBidDateTime tblBidDate = new();
                                            tblBidDate.BidDateTime = BidTime;
                                            tblBidDate.BidDate = item.phlBid3.BidDate;
                                            tblBidDate.ProjId = model.ProjId;
                                            tblBidDate.PST = item.phlBid3.PST;
                                            tblBidDate.IsDeleted = false;
                                            tblBidDate.Isextensions = true;
                                            tblBidDate.ExtNum = 2;
                                            tblBidDate.PhlId = item.ID;
                                            tblBidDate.CreatedDate = DateTime.Now;
                                            await _dbContext.tblBidDateTime.AddAsync(tblBidDate);
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                TblBidStatus tblBid = await _dbContext.TblBidStatus.FirstOrDefaultAsync(x => x.Id == item.ID);
                                if (!string.IsNullOrEmpty(item.Contact) && !string.IsNullOrEmpty(item.Company))
                                {
                                    if (tblBid.Company == item.Company)
                                        tblBid.CompType = item.CompType;
                                    else
                                    {
                                        tblBid.CompType = 2;
                                    }
                                    tblBid.Company = item.memId.ToString();
                                    tblBid.Contact = item.Contact;
                                    tblBid.Uid = string.IsNullOrEmpty(item.Uid) ? "0" : item.Uid;
                                    tblBid.Projid = model.ProjId;
                                    tblBid.Bidding = item.bidding;
                                    tblBid.PHLNote = item.PHLNote;
                                    tblBid.PHLType = item.PHLType;
                                    tblBid.Undecided = item.Undecided == null ? false : item.Undecided;
                                    _dbContext.Entry(tblBid).State = EntityState.Modified;
                                    await _dbContext.SaveChangesAsync();
                                    if (item.Undecided != true)
                                    {
                                        if (item.phlBid1.ID == 0)
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid1.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid1.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid1.hComp.ToString();
                                            }
                                            if (item.phlBid1.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid1.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid1.mValue == "AM")
                                            {
                                                if (item.phlBid1.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid1.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            if (item.phlBid1.BidDate != null)
                                            {
                                                tblBidDateTime tblBidDate = new();
                                                tblBidDate.BidDateTime = BidTime;
                                                tblBidDate.BidDate = item.phlBid1.BidDate;
                                                tblBidDate.ProjId = model.ProjId;
                                                tblBidDate.PST = item.phlBid1.PST;
                                                tblBidDate.IsDeleted = false;
                                                tblBidDate.Isextensions = false;
                                                tblBidDate.ExtNum = 0;
                                                tblBidDate.PhlId = item.ID;
                                                tblBidDate.CreatedDate = DateTime.Now;
                                                await _dbContext.tblBidDateTime.AddAsync(tblBidDate);
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid1.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid1.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid1.hComp.ToString();
                                            }
                                            if (item.phlBid1.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid1.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid1.mValue == "AM")
                                            {
                                                if (item.phlBid1.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid1.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            tblBidDateTime tblBidDate = await _dbContext.tblBidDateTime.FirstOrDefaultAsync(x => x.ID == item.phlBid1.ID);
                                            if (tblBidDate != null)
                                            {
                                                if (item.phlBid1.BidDate != null)
                                                {
                                                    tblBidDate.BidDate = item.phlBid1.BidDate;
                                                    tblBidDate.PST = item.phlBid1.PST;
                                                    tblBidDate.IsDeleted = false;
                                                    tblBidDate.Isextensions = false;
                                                    tblBidDate.ExtNum = 0;
                                                    tblBidDate.BidDateTime = BidTime;
                                                    tblBidDate.PhlId = item.ID;
                                                    tblBidDate.ProjId = model.ProjId;
                                                    _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    tblBidDate.IsDeleted = true;
                                                    _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                            }
                                            else
                                            {
                                                tblBidDate.IsDeleted = true;
                                                _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        if (item.phlBid2.ID == 0)
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid2.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid2.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid2.hComp.ToString();
                                            }
                                            if (item.phlBid2.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid2.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid2.mValue == "AM")
                                            {
                                                if (item.phlBid2.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid2.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            if (item.phlBid2.BidDate != null)
                                            {
                                                tblBidDateTime tblBidDate = new();
                                                tblBidDate.BidDateTime = BidTime;
                                                tblBidDate.BidDate = item.phlBid2.BidDate;
                                                tblBidDate.ProjId = model.ProjId;
                                                tblBidDate.PST = item.phlBid2.PST;
                                                tblBidDate.IsDeleted = false;
                                                tblBidDate.Isextensions = true;
                                                tblBidDate.ExtNum = 1;
                                                tblBidDate.PhlId = item.ID;
                                                tblBidDate.CreatedDate = DateTime.Now;
                                                await _dbContext.tblBidDateTime.AddAsync(tblBidDate);
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid2.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid2.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid2.hComp.ToString();
                                            }
                                            if (item.phlBid2.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid2.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid2.mValue == "AM")
                                            {
                                                if (item.phlBid2.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid2.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            tblBidDateTime tblBidDate = await _dbContext.tblBidDateTime.FirstOrDefaultAsync(x => x.ID == item.phlBid2.ID);
                                            if (tblBidDate != null)
                                            {
                                                if (item.phlBid2.BidDate != null)
                                                {
                                                    tblBidDate.BidDate = item.phlBid2.BidDate;
                                                    tblBidDate.PST = item.phlBid2.PST;
                                                    tblBidDate.IsDeleted = false;
                                                    tblBidDate.Isextensions = true;
                                                    tblBidDate.ExtNum = 1;
                                                    tblBidDate.BidDateTime = BidTime;
                                                    tblBidDate.PhlId = item.ID;
                                                    tblBidDate.ProjId = model.ProjId;
                                                    _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    tblBidDate.IsDeleted = true;
                                                    _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                            }
                                            else
                                            {
                                                tblBidDate.IsDeleted = true;
                                                _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        if (item.phlBid3.ID == 0)
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid3.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid3.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid3.hComp.ToString();
                                            }
                                            if (item.phlBid3.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid3.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid3.mValue == "AM")
                                            {
                                                if (item.phlBid3.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid3.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            if (item.phlBid3.BidDate != null)
                                            {
                                                tblBidDateTime tblBidDate = new();
                                                tblBidDate.BidDateTime = BidTime;
                                                tblBidDate.BidDate = item.phlBid3.BidDate;
                                                tblBidDate.ProjId = model.ProjId;
                                                tblBidDate.PST = item.phlBid3.PST;
                                                tblBidDate.IsDeleted = false;
                                                tblBidDate.Isextensions = true;
                                                tblBidDate.ExtNum = 2;
                                                tblBidDate.PhlId = item.ID;
                                                tblBidDate.CreatedDate = DateTime.Now;
                                                await _dbContext.tblBidDateTime.AddAsync(tblBidDate);
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            string BidTime = "00:00";
                                            string hValue = "00";
                                            if (item.phlBid3.hComp < 10)
                                            {
                                                hValue = "0" + item.phlBid3.hComp.ToString();
                                            }
                                            else
                                            {
                                                hValue = item.phlBid3.hComp.ToString();
                                            }
                                            if (item.phlBid3.mValue == "PM")
                                            {
                                                int tComp = 12 + Convert.ToInt32(item.phlBid3.tComp);
                                                BidTime = tComp.ToString() + ":" + hValue;
                                            }
                                            else if (item.phlBid3.mValue == "AM")
                                            {
                                                if (item.phlBid3.tComp < 10)
                                                {
                                                    BidTime = "0" + item.phlBid3.tComp.ToString() + ":" + hValue;
                                                }
                                            }
                                            tblBidDateTime tblBidDate = await _dbContext.tblBidDateTime.FirstOrDefaultAsync(x => x.ID == item.phlBid3.ID);
                                            if (tblBidDate != null)
                                            {
                                                if (item.phlBid3.BidDate != null)
                                                {
                                                    tblBidDate.BidDate = item.phlBid3.BidDate;
                                                    tblBidDate.PST = item.phlBid3.PST;
                                                    tblBidDate.IsDeleted = false;
                                                    tblBidDate.Isextensions = true;
                                                    tblBidDate.ExtNum = 2;
                                                    tblBidDate.BidDateTime = BidTime;
                                                    tblBidDate.PhlId = item.ID;
                                                    tblBidDate.ProjId = model.ProjId;
                                                    _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    tblBidDate.IsDeleted = true;
                                                    _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                            }
                                            else
                                            {
                                                tblBidDate.IsDeleted = true;
                                                _dbContext.Entry(tblBidDate).State = EntityState.Modified;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tblBid.Undecided = true;
                                        _dbContext.Entry(tblBid).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();
                                        List<tblBidDateTime> toDelBid = await _dbContext.tblBidDateTime.Where(x => x.PhlId == tblBid.Id).ToListAsync();
                                        if (toDelBid != null && toDelBid.Count > 0)
                                        {
                                            foreach (var bdt in toDelBid)
                                            {
                                                tblBidDateTime dltBid = await _dbContext.tblBidDateTime.FirstOrDefaultAsync(x => x.ID == bdt.ID);
                                                dltBid.IsDeleted = true;
                                                _dbContext.Entry(dltBid).State = EntityState.Modified;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (model.preBidInfos != null && model.preBidInfos.Count > 0)
                        {
                            foreach (var item in model.preBidInfos)
                            {
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
                                bool chkDeleted = item.IsDeleted == null ? false : item.IsDeleted;
                                if (item.Id == 0)
                                {
                                    tblPreBidInfo bidInfo = new();
                                    if (item.PreBidDate != null && !chkDeleted)
                                    {
                                        bidInfo.Location = item.Location;
                                        bidInfo.PST = "PT";
                                        bidInfo.PreBidDate = item.PreBidDate;
                                        bidInfo.PreBidTime = PreBidTime;
                                        bidInfo.PreBidAnd = item.PreBidAnd;
                                        bidInfo.ProjId = model.ProjId;
                                        bidInfo.IsDeleted = chkDeleted;
                                        bidInfo.UndecidedPreBid = item.UndecidedPreBid;
                                        await _dbContext.AddAsync(bidInfo);
                                        await _dbContext.SaveChangesAsync();
                                    }

                                }
                                else
                                {
                                    tblPreBidInfo bidInfo = await _dbContext.tblPreBidInfo.FirstOrDefaultAsync(x => x.Id == item.Id);
                                    if (bidInfo != null)
                                    {
                                        if (item.PreBidDate != null || (bool)item.UndecidedPreBid)
                                        {
                                            bidInfo.Location = item.Location;
                                            bidInfo.PST = "PT";
                                            bidInfo.PreBidDate = item.PreBidDate;
                                            bidInfo.PreBidTime = PreBidTime;
                                            bidInfo.PreBidAnd = item.PreBidAnd;
                                            bidInfo.IsDeleted = chkDeleted;
                                            bidInfo.UndecidedPreBid = item.UndecidedPreBid;
                                            _dbContext.Entry(bidInfo).State = EntityState.Modified;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            bidInfo.IsDeleted = true;
                                            _dbContext.Entry(bidInfo).State = EntityState.Modified;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                        httpResponse.data = model;
                        httpResponse.success = true;
                    }
                    if (model.phlInfo != null && model.phlInfo.Count > 0)
                    {
                        foreach (var item in model.phlInfo)
                        {
                            bool? decider = item.IsActive == null ? true : item.IsActive;
                            if (item.Id == 0)
                            {
                                if (!string.IsNullOrEmpty(item.Company) && Convert.ToBoolean(decider) == true)
                                {
                                    tblPhlInfo info = new();
                                    info.ProjId = tblProject.ProjId;
                                    info.MemId = item.MemId;
                                    info.ConId = item.ConId;
                                    info.CompType = 2;
                                    info.PST = "PT";
                                    if (item.BidDate != null)
                                    {
                                        info.BidDate = item.BidDate;
                                        info.mValue = item.mValue;
                                        info.hComp = item.hComp;
                                        info.tComp = item.tComp;
                                    }
                                    info.IsActive = true;
                                    info.PhlType = item.PhlType;
                                    info.BidStatus = item.BidStatus;
                                    info.Note = item.Note;
                                    await _dbContext.tblPhlInfo.AddAsync(info);
                                    await _dbContext.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                tblPhlInfo info = await _dbContext.tblPhlInfo.FirstOrDefaultAsync(x => x.Id == item.Id);
                                if (info != null)
                                {
                                    if (!string.IsNullOrEmpty(item.Company) && Convert.ToBoolean(decider) == true)
                                    {
                                        info.ProjId = tblProject.ProjId;
                                        info.MemId = item.MemId;
                                        info.ConId = item.ConId;
                                        info.CompType = 2;
                                        info.PST = "PT";
                                        if (item.BidDate != null)
                                        {
                                            info.BidDate = item.BidDate;
                                            info.mValue = item.mValue;
                                            info.hComp = item.hComp;
                                            info.tComp = item.tComp;
                                        }
                                        info.IsActive = true;
                                        info.PhlType = item.PhlType;
                                        info.BidStatus = item.BidStatus;
                                        info.Note = item.Note;
                                        _dbContext.Entry(info).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        info.IsActive = false;
                                        _dbContext.Entry(info).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    info = new();
                                    info.ProjId = tblProject.ProjId;
                                    info.MemId = item.MemId;
                                    info.ConId = item.ConId;
                                    info.CompType = 2;
                                    info.PST = "PT";
                                    if (item.BidDate != null)
                                    {
                                        info.BidDate = item.BidDate;
                                        info.mValue = item.mValue;
                                        info.hComp = item.hComp;
                                        info.tComp = item.tComp;
                                    }
                                    info.IsActive = true;
                                    info.PhlType = item.PhlType;
                                    info.BidStatus = item.BidStatus;
                                    info.Note = item.Note;
                                    await _dbContext.tblPhlInfo.AddAsync(info);
                                    await _dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    List<int> currCounties = await _dbContext.TblProjCounty.Where(x => x.ProjId == model.ProjId).Select(x => x.ProjCountyId).ToListAsync();
                    List<int> actCounties = new();
                    List<int> toDelCounties = new();
                    if (!string.IsNullOrEmpty(model.Counties))
                    {
                        string[] arrCounties = model.Counties.Split(',');
                        foreach (var item in arrCounties)
                        {
                            int i = 0;
                            int.TryParse(item, out i);
                            if (i != 0)
                            {
                                TblProjCounty projCounty = await _dbContext.TblProjCounty.FirstOrDefaultAsync(x => x.CountyId == i && x.ProjId == model.ProjId);
                                if (projCounty == null)
                                {
                                    projCounty = new();
                                    projCounty.ProjId = model.ProjId;
                                    projCounty.CountyId = Convert.ToInt32(item);
                                    await _dbContext.TblProjCounty.AddAsync(projCounty);
                                    await _dbContext.SaveChangesAsync();
                                }
                                actCounties.Add(projCounty.ProjCountyId);
                            }
                        }
                        if (actCounties != null && actCounties.Count > 0)
                        {
                            if (currCounties != null && currCounties.Count > 0)
                            {
                                foreach (var item in currCounties)
                                {
                                    int idx = 0;
                                    foreach (var act in actCounties)
                                    {
                                        if (item == act)
                                            idx++;
                                    }
                                    if (idx == 0)
                                    {
                                        TblProjCounty projCounty = await _dbContext.TblProjCounty.FirstOrDefaultAsync(x => x.ProjCountyId == item);
                                        if (projCounty != null)
                                        {
                                            _dbContext.Entry(projCounty).State = EntityState.Deleted;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (currCounties != null && currCounties.Count > 0)
                            {
                                foreach (var item in currCounties)
                                {
                                    TblProjCounty projCounty = await _dbContext.TblProjCounty.FirstOrDefaultAsync(x => x.ProjCountyId == item);
                                    if (projCounty != null)
                                    {
                                        _dbContext.Entry(projCounty).State = EntityState.Deleted;
                                        await _dbContext.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (currCounties != null && currCounties.Count > 0)
                        {
                            foreach (var item in currCounties)
                            {
                                TblProjCounty projCounty = await _dbContext.TblProjCounty.FirstOrDefaultAsync(x => x.ProjCountyId == item);
                                if (projCounty != null)
                                {
                                    _dbContext.Entry(projCounty).State = EntityState.Deleted;
                                    await _dbContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    if (model.preBidInfos != null && model.preBidInfos.Count > 0)
                    {

                        foreach (var item in model.preBidInfos)
                        {
                            if (item.Id == 0)
                            {
                                if (item.PreBidDate != null || (bool)item.UndecidedPreBid)
                                {

                                    string BidTime = "00:00";
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
                                        BidTime = tComp.ToString() + ":" + hValue;
                                    }
                                    else if (item.mValue == "AM")
                                    {
                                        if (item.tComp < 10)
                                        {
                                            BidTime = "0" + item.tComp.ToString() + ":" + hValue;
                                        }
                                    }
                                    tblPreBidInfo info = new();
                                    tblBidDateTime tblBidDate = new();
                                    info.PreBidDate = item.PreBidDate;
                                    info.PreBidTime = BidTime;
                                    info.Location = item.Location;
                                    info.PST = item.PST;
                                    info.Mandatory = item.Mandatory;
                                    info.PreBidAnd = item.PreBidAnd;
                                    info.UndecidedPreBid = item.UndecidedPreBid;
                                    info.ProjId = model.ProjId;
                                    await _dbContext.tblPreBidInfo.AddAsync(info);
                                    await _dbContext.SaveChangesAsync();

                                }
                            }
                            else
                            {
                                if (item.PreBidDate != null || (bool)item.UndecidedPreBid || (bool)item.IsDeleted)
                                {

                                    string BidTime = "00:00";
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
                                        BidTime = tComp.ToString() + ":" + hValue;
                                    }
                                    else if (item.mValue == "AM")
                                    {
                                        if (item.tComp < 10)
                                        {
                                            BidTime = "0" + item.tComp.ToString() + ":" + hValue;
                                        }
                                    }
                                    tblPreBidInfo info = new();
                                    info.Id = item.Id;
                                    info.PreBidDate = item.PreBidDate;
                                    info.PreBidTime = BidTime;
                                    info.Location = item.Location;
                                    info.PST = item.PST;
                                    info.Mandatory = item.Mandatory;
                                    info.PreBidAnd = item.PreBidAnd;
                                    info.IsDeleted = item.IsDeleted;
                                    info.UndecidedPreBid = item.UndecidedPreBid;
                                    info.ProjId = model.ProjId;
                                    _dbContext.tblPreBidInfo.Update(info);
                                    await _dbContext.SaveChangesAsync();

                                }
                            }

                        }
                    }
                    httpResponse.data = "Yes";
                }
                catch (Exception ex)
                {
                    model.CallBack = true;
                    _logger.LogWarning(ex.Message);
                }
            }
            return httpResponse;
        }
        /// <summary>
        /// Get EditProjectInfo page content in edit mode
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tbls"></param>
        /// <returns></returns>
        public async Task<ProjectInformation> GetProjectdetail(int id, IEnumerable<TblCounty> tbls)
        {
            ProjectInformation information = new();
            try
            {
                //   var result = await _dbContext.tblProject.Where(m=>m.ProjId == id).ToListAsync();
                var response = await _dbContext.tblProject.SingleOrDefaultAsync(x => x.ProjId == id);
                information.ProjId = response.ProjId;
                information.Title = response.Title;
                information.ArrivalDt = response.ArrivalDt;
                information.ProjNumber = response.ProjNumber;
                information.LocAddr1 = response.LocAddr1;
                information.LocCity = response.LocCity;
                information.LocState = response.LocState;
                information.LocZip = response.LocZip;
                information.ProjSubTypeId = response.ProjSubTypeId == null ? 0 : response.ProjSubTypeId;
                information.ProjSubTypeIdString = information.ProjSubTypeId != 0 ? _dbContext.TblProjSubType.FirstOrDefault(x => x.ProjSubTypeID == information.ProjSubTypeId).ProjSubType : "";
                information.ProjTypeId = response.ProjTypeId == null ? 0 : response.ProjTypeId;
                information.ProjTypeIdString = information.ProjTypeId != 0 ? _dbContext.TblProjType.FirstOrDefault(x => x.ProjTypeId == information.ProjTypeId).ProjType : "";
                //var response = await (from tab in _dbContext.TblProjType.SingleOrDefaultAsync(x => x.ProjTypeId == information.ProjTypeId) select new {tab.ProjType});
                information.FutureWork = Convert.ToBoolean(response.FutureWork);
                information.BidDt = response.BidDt;
                information.HiddenBidDt = response.BidDt;
                DateTime dateTimeValue;
                if (!string.IsNullOrEmpty(response.LastBidDt))
                {
                    string lastBidDt = response.LastBidDt.Trim();
                    lastBidDt = lastBidDt.Replace("PT", "").Trim(); 

                    string[] formats = { "MMM d yyyy HH:mm", "MMM d - h:mm tt", "HH:mm" }; 

                    if (!DateTime.TryParseExact(lastBidDt, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue))
                    {
                        dateTimeValue = DateTime.MinValue;
                    }
                }
                else
                {
                    dateTimeValue = DateTime.MinValue;
                }

                information.LastBidDt = dateTimeValue == DateTime.MinValue ? "" : dateTimeValue.ToString("HH:mm");

                //information.BidDt3 = response.BidDt3;
                //information.BidDt4 = response.BidDt4;
                //information.BidDt5 = response.BidDt5;
                information.strBidDt = response.StrBidDt;
                information.strBidDt2 = response.StrBidDt2;
                information.strBidDt3 = response.StrBidDt3;
                information.strBidDt4 = response.StrBidDt4;
                information.strBidDt5 = response.StrBidDt5;
                information.PreBidDt = response.PreBidDt;
                information.PreBidDt2 = response.PreBidDt2;
                information.PreBidDt3 = response.PreBidDt3;
                information.PreBidDt4 = response.PreBidDt4;
                information.PreBidDt5 = response.PreBidDt5;
                information.PreBidLoc = response.PreBidLoc;
                information.PreBidLoc2 = response.PreBidLoc2;
                information.PreBidLoc3 = response.PreBidLoc3;
                information.PreBidLoc4 = response.PreBidLoc4;
                information.PreBidLoc5 = response.PreBidLoc5;
                information.EstCost = response.EstCost;
                information.EstCost2 = response.EstCost2;
                information.EstCost3 = response.EstCost3;
                information.EstCost4 = response.EstCost4;
                information.EstCost5 = response.EstCost5;
                information.BidBond = response.BidBond;
                information.PHLnote = response.Phlnote;
                information.BRNote = response.Brnote;
                information.strAddenda = response.StrAddenda;
                information.BidDt2 = response.BidDt2;
                information.AddendaNote = response.AddendaNote;
                information.PrebidNote = response.PrebidNote;
                information.SubApprov = response.SubApprov;
                information.EstCostDetails = new();
                int a = 0;
                int b = 0;
                List<tblEstCostDetails> tblEstCostDetails = await _dbContext.tblEstCostDetails.Where(x => x.Projid == id && x.Removed != true).ToListAsync();

                if (tblEstCostDetails != null && tblEstCostDetails.Count > 0)
                {
                    information.EstCostDetails = tblEstCostDetails.Where(x => x.Projid == id).Select(x => new EstCostInfo
                    {
                        EstFrom = x.EstCostFrom,
                        EstTo = x.EstCostTo,
                        Id = x.Id.ToString(),
                        Description = x.Description,
                        RangeSign = x.RangeSign

                    }).ToList();
                }
                else
                {
                    if (information.EstCost != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost.Contains('-'))
                        {
                            string[] EstCost = information.EstCost.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost))
                            {
                                costInfo.EstFrom = information.EstCost;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }

                    }
                    if (information.EstCost2 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost2.Contains('-'))
                        {
                            string[] EstCost = information.EstCost2.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(information.EstCost2))
                            {
                                costInfo.EstFrom = information.EstCost2;
                                information.EstCostDetails.Add(costInfo);
                            }

                        }

                    }
                    if (information.EstCost3 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost3.Contains('-'))
                        {
                            string[] EstCost = information.EstCost3.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost3))
                            {
                                costInfo.EstFrom = information.EstCost3;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                    }
                    if (information.EstCost4 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost4.Contains('-'))
                        {
                            string[] EstCost = information.EstCost4.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost4))
                            {
                                costInfo.EstFrom = information.EstCost4;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                    }
                    if (information.EstCost5 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost5.Contains('-'))
                        {
                            string[] EstCost = information.EstCost5.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost5))
                            {
                                costInfo.EstFrom = information.EstCost5;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                    }
                }
                information.ProjNote = response.ProjNote;
                information.InternalNote = response.InternalNote;
                information.Story = response.Story;
                information.SpecsOnPlans = Convert.ToBoolean(response.SpecsOnPlans);
                information.SpcChk = Convert.ToBoolean(response.SpcChk);
                information.PrevailingWage = Convert.ToBoolean(response.PrevailingWage);
                information.ProjScope = response.ProjScope;
                information.CompleteDt = response.CompleteDt;
                information.IssuingOffice = response.IssuingOffice;
                string StrCounty = response.CountyID <= 0 ? "" : (_dbContext.TblCounty.FirstOrDefault(x => x.CountyId == response.CountyID) != null ? _dbContext.TblCounty.FirstOrDefault(x => x.CountyId == response.CountyID).County : "");
                information.LocAddr2 = response.LocAddr2 != null ? (StrCounty == "" ? response.LocAddr2 : StrCounty) : "";
                information.CountyId = response.CountyID;
                information.Publish = Convert.ToBoolean(response.Publish);
                information.BoolUndecided = response.Undecided == "TBD" ? true : false;
                information.Undecided = response.Undecided;
                information.BoolPreUndecided = response.StrBidDt5 == "TBD" ? true : false;
                var tblEntities = await (_dbContext.TblEntity.Where(x => x.Projid == id && x.IsActive != false).Select(ent =>
                new
                {
                    EntityID = ent.EntityID,
                    EntityName = ent.EnityName,
                    EntityType = ent.EntityType,
                    Projid = ent.Projid,
                    ProjNumber = ent.ProjNumber,
                    NameId = ent.NameId,
                    chkIssue = ent.chkIssue,
                    CompType = ent.CompType
                }
                )).ToListAsync();
                var counties = await _dbContext.TblProjCounty.Where(m => m.ProjId == id).ToListAsync();
                if (counties != null && counties.Count > 0)
                {
                    information.Counties = string.Join(",", counties.Select(c => c.CountyId));
                    var countyIds = counties.Select(c => c.CountyId).ToList();
                    // Check if countyIds contains only -1
                    bool hasAllCounties = countyIds.Count == 1 && countyIds.Contains(-1);

                    // Check if countyIds contains only -2
                    bool hasAllWACounties = countyIds.Count == 1 && countyIds.Contains(-2);

                    // Check if countyIds contains other IDs
                    bool hasOtherCounties = countyIds.Any(id => id != -1 && id != -2);

                    // Generate text based on the combinations
                    string countiesText = string.Empty;
                    if (hasAllCounties)
                    {
                        information.BoolAllOr = true;
                        countiesText = "All OR Counties";
                    }
                    else if (hasAllWACounties)
                    {
                        information.BoolAllWa = true;
                        countiesText = "All WA Counties";
                    }
                    else if (hasOtherCounties)
                    {
                        // Separate -1 and -2 from other IDs
                        var otherCountyIds = countyIds.Where(id => id != -1 && id != -2).ToList();

                        // Fetch texts for other counties
                        var otherCountyTexts = await _dbContext.TblCounty
                            .Where(c => otherCountyIds.Contains(c.CountyId))
                            .Select(c => c.County)
                            .ToListAsync();

                        // If -1 or -2 is also present, add their corresponding text
                        if (countyIds.Contains(-1))
                        {
                            information.BoolAllOr = true;
                            otherCountyTexts.Insert(0, "All OR Counties");
                        }
                        if (countyIds.Contains(-2))
                        {
                            information.BoolAllWa = true;
                            otherCountyTexts.Insert(0, "All WA Counties");
                        }

                        countiesText = string.Join(",", otherCountyTexts);
                    }
                    else
                    {
                        // Handle the case when only -1 and -2 are present
                        if (countyIds.Contains(-1) && countyIds.Contains(-2))
                        {
                            information.BoolAllWa = true;
                            information.BoolAllOr = true;
                            countiesText = "All OR Counties, All WA Counties";
                        }
                    }

                    // Set the text in the information object
                    information.strCounties = countiesText;

                }

                information.Entities = new();
                if (tblEntities != null && tblEntities.Count > 0)
                {
                    foreach (var item in tblEntities)
                    {
                        EntityInformation entity = new();
                        int typeId = 0;
                        int.TryParse(item.EntityType, out typeId);
                        entity.EntityID = item.EntityID;
                        entity.EntityName = item.EntityName;
                        entity.EntityType = item.EntityType;
                        entity.NameId = Convert.ToInt32(item.NameId);
                        entity.CompType = item.CompType;
                        entity.chkIssue = item.chkIssue;
                        if (typeId != 0)
                        {
                            entity.EntityTypeString = await _dbContext.TblEntityType.Where(x => x.EntityID == typeId).Select(x => x.EntityType).FirstOrDefaultAsync();
                        }
                        else
                        {
                            entity.EntityTypeString = "";
                        }
                        information.Entities.Add(entity);
                    }
                }
                var biddate = await (_dbContext.tblBidDateTime.Where(x => x.ProjId == id && x.IsDeleted == false).OrderByDescending(x => x.BidDate).Select(ent =>
                new
                {
                    BidDate = ent.BidDate,
                    PST = ent.PST,
                    ProjId = ent.ProjId,
                    id = ent.ID,
                    IsExt = ent.Isextensions,
                    phlId = ent.PhlId,
                    AddId = ent.AddendaId,
                    CreatedDate = ent.CreatedDate,
                    BidTime = ent.BidDateTime,
                    ExtNum = ent.ExtNum
                }
                )).ToListAsync();
                information.BidDateTimes = biddate.Select(x => new BidDateInformation
                {
                    BidDate = x.BidDate,
                    PST = x.PST,
                    ID = x.id,
                    Isextensions = x.IsExt,
                    PhlId = x.phlId,
                    AddendaId = x.AddId,
                    BidDateTime = x.BidTime,
                    CreatedDate = x.CreatedDate,
                    ExtNum = x.ExtNum
                }).ToList();
                information.pInfo = new();
                information.phlInfo = new();
                List<tblPhlInfo> phlInfos = await _dbContext.tblPhlInfo.Where(x => x.ProjId == id && x.IsActive == true && x.CompType == 2).ToListAsync();
                List<TblBidStatus> statuses = await _dbContext.TblBidStatus.Where(x => x.Projid == id && x.Undecided != true && x.CompType == 2).ToListAsync();
                int MBDCheck = 0;
                if (phlInfos != null)
                {
                    PlanHLInformation info = new();
                    foreach (var item in phlInfos)
                    {
                        info = new();
                        info.Id = item.Id;
                        info.PhlType = item.PhlType ?? 3;
                        info.Note = item.Note;
                        info.hComp = string.IsNullOrEmpty(item.hComp) ? "00" : item.hComp;
                        info.tComp = string.IsNullOrEmpty(item.tComp) ? "00" : item.tComp;
                        info.mValue = string.IsNullOrEmpty(item.mValue) ? "AM" : item.mValue;
                        info.BidDate = item.BidDate;
                        info.MemId = item.MemId;
                        info.ConId = item.ConId;
                        info.BidStatus = item.BidStatus;
                        if (item.MemId != 0)
                        {
                            var contractor = await _dbContext.BusinessEntities.FirstOrDefaultAsync(x => x.BusinessEntityId == item.MemId);
                            info.Company = contractor.BusinessEntityName;
                        }
                        else
                        {
                            info.Company = "";
                        }
                        if (item.ConId != 0)
                        {
                            TblContact contact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.ConId == item.ConId);
                            info.StrContact = string.IsNullOrEmpty(contact.Contact) ? "" : contact.Contact;
                            info.contactEmail = string.IsNullOrEmpty(contact.Email) ? "" : contact.Email;
                            info.contactPhone = string.IsNullOrEmpty(contact.Phone) ? "" : contact.Phone;
                        }
                        else
                        {
                            info.StrContact = "";
                            info.contactEmail = "";
                            info.contactPhone = "";
                        }
                        info.CompType = 2;
                        information.phlInfo.Add(info);
                    }
                }
                List<tblPreBidInfo> prebidInfos = await _dbContext.tblPreBidInfo.Where(x => x.ProjId == id && x.IsDeleted == false).ToListAsync();
                information.preBidInfos = new List<PreBidInfo>();
                if (prebidInfos != null)
                {
                    //PreBidInfo info = new();
                    foreach (var item in prebidInfos)
                    {
                        if (item.PreBidTime != null || (bool)item.UndecidedPreBid)
                        {
                            string PreTime = item.PreBidTime;
                            string[] timeParts = PreTime.Split(':');

                            // Check if there are exactly two parts after splitting
                            if (timeParts.Length == 2)
                            {
                                string hCompstr = timeParts[0];
                                int hValue = 0;
                                int tValue = 0;
                                string mValue = "";
                                if (int.TryParse(hCompstr, out int hComp))
                                {
                                    string tCompstr = timeParts[1];
                                    if (int.TryParse(tCompstr, out int tComp))
                                    {

                                        if (hComp > 12)
                                        {
                                            mValue = "PM";
                                            tValue = tComp - 12;
                                            //PreBidTime = tComp.ToString() + ":" + hValue;
                                        }
                                        else
                                        {
                                            mValue = "AM";
                                            tValue = tComp;
                                        }

                                        if (hComp < 10)
                                        {
                                            string newhValue = "0" + hComp.ToString();
                                            if (int.TryParse(newhValue, out hValue))
                                            {
                                                // Conversion successful

                                            }
                                        }
                                        else
                                        {
                                            string newhValue = hComp.ToString();
                                            if (int.TryParse(newhValue, out hValue))
                                            {
                                                // Conversion successful

                                            }
                                        }
                                        // Now you can use hComp, tComp, and mValue as needed.
                                    }
                                    else
                                    {
                                        // Handle the case where tCompstr couldn't be parsed as an integer.
                                    }
                                    PreBidInfo newpreinfo = new();
                                    newpreinfo.Id = item.Id;
                                    newpreinfo.PreBidDate = item.PreBidDate;
                                    newpreinfo.hComp = hValue;
                                    newpreinfo.tComp = tValue;
                                    newpreinfo.mValue = mValue;
                                    newpreinfo.PST = item.PST;
                                    newpreinfo.PreBidAnd = item.PreBidAnd;
                                    newpreinfo.Mandatory = item.Mandatory;
                                    newpreinfo.Location = item.Location;
                                    newpreinfo.PreBidTime = item.PreBidTime;
                                    newpreinfo.UndecidedPreBid = item.UndecidedPreBid;
                                    information.preBidInfos.Add(newpreinfo);

                                }
                                else
                                {
                                    // Handle the case where hCompstr couldn't be parsed as an integer.
                                }
                            }
                            else
                            {
                                //("Invalid time format");
                            }



                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return information;
        }
        /// <summary>
        /// Populating Addenda table and Getting value from addenda table
        /// </summary>
        /// <param name="AddendaS3List"></param>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public List<AddendaInfo> SetNGetAddendaFolder(List<string> AddendaS3List, int ProjId)
        {
            List<AddendaInfo> AddendaList = new();
            foreach (string s3 in AddendaS3List)
            {
                TblAddenda addendum = new();
                addendum = _dbContext.TblAddenda.Where(x => (x.AddendaNo == s3) && (x.ProjId == ProjId)).FirstOrDefault();
                if (addendum == null)
                {
                    addendum = new();
                    addendum.ProjId = ProjId;
                    addendum.AddendaNo = s3;
                    addendum.InsertDt = DateTime.Now;
                    _dbContext.TblAddenda.Add(addendum);
                    _dbContext.SaveChanges();
                }
            }
            List<TblAddenda> chkTblAddenda = _dbContext.TblAddenda.Where(x => x.ProjId == ProjId && x.Deleted == false && x.ParentFolder == null).ToList();
            List<TblAddenda> tblAddenda = _dbContext.TblAddenda.Where(x => x.ProjId == ProjId && x.Deleted == false && x.ParentFolder == null).ToList();
            AddendaList = tblAddenda.Select(x => new AddendaInfo
            {
                IssueDt = x.IssueDt,
                AddendaNo = x.AddendaNo,
                AddendaId = x.AddendaId,
                ProjId = x.ProjId
            }).ToList();

            return AddendaList;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectInformation>> GetSearchFindProjectsAsync(string searchTxt)
        {
            IEnumerable<ProjectInformation> response = new List<ProjectInformation>();
            List<TblProject> modell = new();
            try
            {
                modell = await _dbContext.tblProject.Where(M => (M.Title.Contains(searchTxt))).OrderByDescending(t => t.ProjId).Take(200).ToListAsync();
                response = modell.Select(t => new ProjectInformation
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
                    ArrivalDt = t.ArrivalDt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;

        }
        /// <summary>
        /// Get Project Preview page content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tbls"></param>
        /// <returns></returns>
        public async Task<ProjectInformation> GetProjectPreview(int id)
        {

            ProjectInformation information = new();
            try
            {
                //   var result = await _dbContext.tblProject.Where(m=>m.ProjId == id).ToListAsync();
                var response = await _dbContext.tblProject.SingleOrDefaultAsync(x => x.ProjId == id);
                if(response is null)
                {
                    return null;
                }
                information.ProjId = response.ProjId;
                information.Title = response.Title;
                information.ArrivalDt = response.ArrivalDt;
                information.ProjNumber = response.ProjNumber;
                information.LocAddr1 = response.LocAddr1;
                information.LocCity = response.LocCity;
                information.LocState = response.LocState;
                information.LocZip = response.LocZip;
                information.ProjSubTypeId = response.ProjSubTypeId == null ? 0 : response.ProjSubTypeId;
                information.ProjSubTypeIdString = information.ProjSubTypeId != 0 ? _dbContext.TblProjSubType.FirstOrDefault(x => x.ProjSubTypeID == information.ProjSubTypeId).ProjSubType : "";
                information.ProjTypeId = response.ProjTypeId == null ? 0 : response.ProjTypeId;
                information.ProjTypeIdString = information.ProjTypeId != 0 ? _dbContext.TblProjType.FirstOrDefault(x => x.ProjTypeId == information.ProjTypeId).ProjType : "";
                //var response = await (from tab in _dbContext.TblProjType.SingleOrDefaultAsync(x => x.ProjTypeId == information.ProjTypeId) select new {tab.ProjType});
                information.FutureWork = Convert.ToBoolean(response.FutureWork);
                information.BidDt = response.BidDt;
                information.BidDt2 = response.BidDt2;
                information.BidDt3 = response.BidDt3;
                information.BidDt4 = response.BidDt4;
                information.BidDt5 = response.BidDt5;
                information.strBidDt = response.StrBidDt;
                information.strBidDt2 = response.StrBidDt2;
                information.strBidDt3 = response.StrBidDt3;
                information.strBidDt4 = response.StrBidDt4;
                information.strBidDt5 = response.StrBidDt5;
                information.PreBidDt = response.PreBidDt;
                information.PreBidDt2 = response.PreBidDt2;
                information.PreBidDt3 = response.PreBidDt3;
                information.PreBidDt4 = response.PreBidDt4;
                information.PreBidDt5 = response.PreBidDt5;
                information.PreBidLoc = response.PreBidLoc;
                information.PreBidLoc2 = response.PreBidLoc2;
                information.PreBidLoc3 = response.PreBidLoc3;
                information.PreBidLoc4 = response.PreBidLoc4;
                information.PreBidLoc5 = response.PreBidLoc5;
                information.EstCost = response.EstCost;
                information.BRNote = response.Brnote;
                information.PHLnote = response.Phlnote;
                information.BidBond = response.BidBond;
                information.strAddenda = response.StrAddenda;
                information.EstCost = response.EstCost;
                information.EstCost2 = response.EstCost2;
                information.EstCost3 = response.EstCost3;
                information.EstCost4 = response.EstCost4;
                information.EstCost5 = response.EstCost5;
                information.AddendaNote = response.AddendaNote;
                information.PrebidNote = response.PrebidNote;
                information.MemberTrack = _dbContext.TblTracking.Where(x => x.ProjId == id && x.IsTracking == true).Count() > 0 ? true : false;

                information.EstCostDetails = new();
                information.preBidInfos = await _dbContext.tblPreBidInfo.Where(x => x.ProjId == id && x.IsDeleted != true).Select(x => new PreBidInfo
                {
                    Id = x.Id,
                    PreBidAnd = x.PreBidAnd,
                    Mandatory = x.Mandatory,
                    PST = x.PST,
                    PreBidDate = x.PreBidDate,
                    UndecidedPreBid = x.UndecidedPreBid,
                    Location = x.Location
                }).ToListAsync();
                int a = 0;
                int b = 0;
                List<tblEstCostDetails> tblEstCostDetails = await _dbContext.tblEstCostDetails.Where(x => x.Projid == id && x.Removed != true).ToListAsync();
                if (tblEstCostDetails != null && tblEstCostDetails.Count > 0)
                {
                    information.EstCostDetails = tblEstCostDetails.Where(x => x.Projid == id).Select(x => new EstCostInfo
                    {
                        EstFrom = x.EstCostFrom,
                        EstTo = x.EstCostTo,
                        Id = x.Id.ToString(),
                        Description = x.Description,
                        RangeSign = x.RangeSign

                    }).ToList();
                }
                else
                {
                    if (information.EstCost != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost.Contains('-'))
                        {
                            string[] EstCost = information.EstCost.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost))
                            {
                                costInfo.EstFrom = information.EstCost;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }

                    }
                    if (information.EstCost2 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost2.Contains('-'))
                        {
                            string[] EstCost = information.EstCost2.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(information.EstCost2))
                            {
                                costInfo.EstFrom = information.EstCost2;
                                information.EstCostDetails.Add(costInfo);
                            }

                        }

                    }
                    if (information.EstCost3 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost3.Contains('-'))
                        {
                            string[] EstCost = information.EstCost3.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost3))
                            {
                                costInfo.EstFrom = information.EstCost3;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                    }
                    if (information.EstCost4 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost4.Contains('-'))
                        {
                            string[] EstCost = information.EstCost4.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost4))
                            {
                                costInfo.EstFrom = information.EstCost4;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                    }
                    if (information.EstCost5 != null)
                    {
                        EstCostInfo costInfo = new();
                        if (information.EstCost5.Contains('-'))
                        {
                            string[] EstCost = information.EstCost5.Split('-');
                            if (!string.IsNullOrEmpty(EstCost[0]))
                            {
                                costInfo.EstFrom = EstCost[0];
                                costInfo.EstTo = EstCost[1];
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(information.EstCost5))
                            {
                                costInfo.EstFrom = information.EstCost5;
                                information.EstCostDetails.Add(costInfo);
                            }
                        }
                    }
                }
                information.ProjNote = response.ProjNote;
                information.InternalNote = response.InternalNote;
                information.Story = response.Story;
                information.SpecsOnPlans = Convert.ToBoolean(response.SpecsOnPlans);
                information.SpcChk = Convert.ToBoolean(response.SpcChk);
                information.PrevailingWage = Convert.ToBoolean(response.PrevailingWage);
                information.ProjScope = response.ProjScope;
                information.CompleteDt = response.CompleteDt;
                information.IssuingOffice = response.IssuingOffice;
                information.LocAddr2 = response.LocAddr2;
                List<TblEntity> entityList = await _dbContext.TblEntity.Where(x => x.Projid == id && x.IsActive != false).ToListAsync();
                List<EntityInformation> entityInformationList = new List<EntityInformation>();
                if (entityList.Count > 0)
                {
                    foreach (var item in entityList)
                    {
                        EntityInformation entity = new();
                        entity.Projid = item.Projid;
                        entity.ProjNumber = item.ProjNumber;
                        entity.EntityName = item.EnityName;
                        var ENTITYid = 0;
                        if (int.TryParse(item.EntityType, out ENTITYid))
                        {
                            var entitydes = await _dbContext.TblEntityType.FirstOrDefaultAsync(m => m.EntityID == ENTITYid);
                            entity.EntityType = entitydes != null ? entitydes.EntityType : "";
                        }
                        else
                        {
                            entity.EntityType = item.EntityType;
                        }
                        entity.chkIssue = item.chkIssue;
                        TblMember member = _entityRepository.GetEntities().SingleOrDefault(x => x.Id == item.NameId);
                        if (member != null)
                        {
                            entity.EntAddr = String.IsNullOrEmpty(member.BillAddress) ? "" : member.BillAddress;
                            string city = String.IsNullOrEmpty(member.BillCity) ? "" : member.BillCity;
                            string state = String.IsNullOrEmpty(member.BillState) ? "" : member.BillState;
                            string zip = String.IsNullOrEmpty(member.BillZip) ? "" : member.BillZip;

                            entity.EntAddr2 = $"{city}, {state} {zip}".TrimEnd(',', ' '); // TrimEnd to remove trailing commas or spaces

                            entity.EntFax = String.IsNullOrEmpty(member.Fax) ? "" : member.Fax;
                            TblContact Contacts = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.Id == member.Id && x.MainContact == true);
                            if (Contacts != null)
                            {
                                entity.EntContact = Contacts.Contact;
                                entity.EntPhone = Contacts.Phone;
                                entity.EntMail = Contacts.Email;
                            }
                            else
                            {
                                entity.EntContact = "";
                                entity.EntPhone = "";
                            }
                        }
                        else
                        {
                            entity.EntAddr = "";
                            entity.EntContact = "";
                            entity.EntPhone = "";
                            entity.EntFax = "";
                        }

                        entityInformationList.Add(entity);
                    }
                }

                information.Entities = entityInformationList;
                var biddate = await (_dbContext.tblBidDateTime.Where(x => x.ProjId == id).Select(ent =>
                new
                {
                    BidDate = ent.BidDate,
                    ProjId = ent.ProjId
                }
                )).ToListAsync();
                information.BidDateTimes = biddate.Select(x => new BidDateInformation
                {
                    BidDate = x.BidDate
                }).ToList();
                List<TblBidStatus> statuses = await _dbContext.TblBidStatus.Where(x => x.Projid == id && x.Undecided != true && x.CompType == 2).ToListAsync();
                information.pInfo = new();
                information.phlInfo = new();
                List<tblPhlInfo> phlInfos = await _dbContext.tblPhlInfo.Where(x => x.ProjId == id && x.IsActive == true).ToListAsync();
                if (phlInfos != null && phlInfos.Count > 0)
                {
                    foreach (var item in phlInfos)
                    {
                        PlanHLInformation planHL = new();
                        planHL.BidDate = item.BidDate;
                        planHL.ConId = item.ConId;
                        planHL.MemId = item.MemId;
                        planHL.BidStatusString = "No Selection";
                        if (item.BidStatus != 0)
                        {
                            tblBidOption bidOption = await _dbContext.tblBidOption.SingleOrDefaultAsync(x => x.Id == item.BidStatus);
                            if (bidOption != null)
                            {
                                planHL.BidStatusString = bidOption.BidOption;
                            }
                        }
                        planHL.PHLTypeString = "Prime";
                        if (item.PhlType != 0)
                        {
                            TblPHLType type = await _dbContext.TblPHLType.SingleOrDefaultAsync(x => x.PHLID == item.PhlType);
                            if (type != null)
                            {
                                planHL.PHLTypeString = type.PHLType;
                            }
                        }
                        if (item.MemId != 0)
                        {
                            var contractor = await _dbContext.BusinessEntities.FirstOrDefaultAsync(x => x.BusinessEntityId == item.MemId);
                            if (contractor != null)
                            {
                                planHL.Company = contractor.BusinessEntityName;
                            }
                            else
                            {
                                planHL.Company = "";
                            }
                        }
                        else
                        {
                            planHL.Company = "";
                        }
                        if (item.ConId != 0)
                        {
                            TblContact contractor = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.ConId == item.ConId);
                            if (contractor != null)
                            {
                                planHL.contactEmail = !string.IsNullOrEmpty(contractor.Email) ? contractor.Email : "";
                                planHL.contactPhone = !string.IsNullOrEmpty(contractor.Phone) ? contractor.Phone : "";
                                planHL.StrContact = !string.IsNullOrEmpty(contractor.Email) ? contractor.Email : "";
                                planHL.StrContact = !string.IsNullOrEmpty(planHL.StrContact) ? (!string.IsNullOrEmpty(contractor.Phone) ? planHL.StrContact + ", " + contractor.Phone : planHL.StrContact) : (!string.IsNullOrEmpty(contractor.Phone) ? contractor.Phone : "");
                            }
                            else

                            {
                                planHL.StrContact = "";
                            }
                        }
                        else
                        {
                            planHL.StrContact = "";
                        }
                        information.phlInfo.Add(planHL);
                    }
                }
                if (statuses != null && statuses.Count > 0)
                {
                    foreach (var item in statuses)
                    {
                        PHLInformation pHL = new();
                        pHL.ID = item.Id;
                        pHL.PHLType = item.PHLType ?? 3;
                        TblPHLType type = await _dbContext.TblPHLType.SingleOrDefaultAsync(x => x.PHLID == pHL.PHLType);
                        pHL.PHLTypeString = type == null ? "Prime" : type.PHLType;
                        pHL.Uid = item.Uid;
                        int ConId = 0;
                        int memId = 0;
                        int.TryParse(item.Contact, out ConId);
                        int.TryParse(item.Company, out memId);
                        if (memId != 0)
                        {
                            TblContractor contractor = await _dbContext.TblContractors.Where(x => x.Id == memId).FirstOrDefaultAsync();
                            if (contractor != null)
                            {
                                pHL.Company = contractor.Name;
                                pHL.memId = contractor.Id;
                            }
                            else
                                pHL.Company = "";
                            if (ConId != 0)
                            {
                                TblContact contact = await _dbContext.TblContacts.Where(x => x.ConId == ConId).FirstOrDefaultAsync();
                                if (contact != null)
                                {
                                    contact.Email = string.IsNullOrEmpty(contact.Email) ? "" : contact.Email;
                                    contact.Phone = string.IsNullOrEmpty(contact.Phone) ? "" : contact.Phone;
                                    if (contact.Email == "")
                                        pHL.Contact = contact.Phone;
                                    else
                                        pHL.Contact = (contact.Email) + ", " + contact.Phone;

                                }
                                else
                                {
                                    pHL.Contact = "";
                                    pHL.Company = "";
                                }
                            }
                        }
                        pHL.bidding = item.Bidding;

                        information.pInfo.Add(pHL);
                    }
                }
                List<tblBidDateTime> tbl = await _dbContext.tblBidDateTime.Where(x => x.ProjId == id && x.IsDeleted == false).ToListAsync();

                if (information.phlInfo != null && information.phlInfo.Count > 0 && information.phlInfo.Any(m => m.BidDate != null))
                {
                    information.MBDCheck = "Y";
                }
                else
                {
                    information.MBDCheck = "N";
                }

            }

            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return information;
        }
        /// <summary>
        /// Get Project Type by Id (No use)
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public string GetProjectName(string searchText)
        {
            string res = "";
            if (!string.IsNullOrEmpty(searchText))
            {

                try
                {
                    int NewInt = Convert.ToInt32(searchText);
                    if (NewInt != 0)
                    {
                        res = _dbContext.TblProjType.SingleOrDefault(x => x.ProjTypeId == NewInt).ProjType;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return res;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="uniqueDate"></param>
        /// <param name="projectNumber"></param>
        /// <returns></returns>
        public async Task<dynamic> UniqueDate(string uniqueDate, string projectNumber)
        {
            List<DateTime?> date2 = new List<DateTime?>();
            string date1 = string.Empty;
            if (!string.IsNullOrEmpty(uniqueDate))
                date1 = Convert.ToDateTime(uniqueDate).ToString("dd/MM/yyyy");
            date2 = (from f in _dbContext.tblProject where f.ProjNumber == projectNumber select f.BidDt).ToList();
            for (int i = 0; i < date2.Count; i++)
            {
                var D1 = date2[i];
                if (D1 != null)
                {
                    var D2 = Convert.ToDateTime((DateTime?)D1).ToString("dd/MM/yyyy");
                    var date3 = await _dbContext.tblProject.SingleOrDefaultAsync(t => D2 == date1);
                }

            }

            return date2;
        }
        /// <summary>
        /// Get Phl type in phl section of editprojectinfo page
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetPhlType(string SelectedValue = "")
        {
            var response = await (from tab in _dbContext.TblPHLType where tab.IsActive == true select tab).ToListAsync();
            var result = response.ToList().OrderBy(m => m.PHLID).Select(x => new SelectListItem
            {
                Text = x.PHLType,
                Value = x.PHLID.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.PHLID.ToString() == SelectedValue ? true : false))
            }).ToList();
            return result;
        }
        /// <summary>
        /// Not in use
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="PHLNote"></param>
        /// <returns></returns>
        public async Task<dynamic> AddPhlNote(int ProjId, string PHLNote)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProject subPlans = await _dbContext.tblProject.FirstOrDefaultAsync(x => x.ProjId == ProjId);
                if (subPlans == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    subPlans.Phlnote = PHLNote;
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
        /// Not in use
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="BrNote"></param>
        /// <returns></returns>
        public async Task<dynamic> AddBrNote(int ProjId, string BrNote)

        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProject subPlans = await _dbContext.tblProject.FirstOrDefaultAsync(x => x.ProjId == ProjId);
                if (subPlans == null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Something went wrong";
                }
                else
                {
                    subPlans.Brnote = BrNote;
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
        /// Get popup of phl in project preview page
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public async Task<dynamic> ShowCard(int Id, int ConId)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                MemberShipRegistration member = new();
                TblContractor tbl = await _dbContext.TblContractors.Where(x => x.Id == Id).FirstOrDefaultAsync();
                TblContact contact = await _dbContext.TblContacts.Where(x => x.ConId == ConId).FirstOrDefaultAsync();
                if (tbl != null)
                {
                    member.Company = string.IsNullOrEmpty(tbl.Name) ? "" : tbl.Name;
                    if (contact != null)
                    {
                        member.ContactName = string.IsNullOrEmpty(contact.Contact) ? "" : contact.Contact;
                        member.Email = string.IsNullOrEmpty(contact.Email) ? "" : contact.Email;
                        member.ContactPhone = string.IsNullOrEmpty(contact.Phone) ? "" : contact.Phone;
                    }
                    else
                    {
                        member.ContactName = "";
                        member.Email = "";
                        member.ContactPhone = "";
                    }
                    response.success = true;
                    response.data = member;
                }
                else
                {
                    member.Company = "";
                    member.CompanyPhone = "";
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
        /// To get value of county on behalf of city and state
        /// </summary>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<dynamic> CheckCountyAsync(string City, string State)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                ZipInfo info = new();
                var TempInfo = (from tab in _dbContext.TblCityCounty
                                join m in _dbContext.TblCounty on tab.CountyId equals m.CountyId
                                where tab.City == City
                                select new
                                {
                                    m.County,
                                    m.CountyId
                                });
                var county = await TempInfo.Select(x => new ZipInfo { County = x.County, CountyId = x.CountyId }).FirstOrDefaultAsync();
                if (county != null)
                {
                    info = county;
                }
                TblCityLatLong latLong = new();
                latLong = await _dbContext.TblCityLatLong.Where(x => x.State == State && x.City == City).FirstOrDefaultAsync();
                if (latLong != null)
                {
                    info.Longitude = latLong.Longitude;
                    info.Latitude = latLong.Latitude;
                    response.data = info;
                }
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
        /// No use 
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetProjectNumberAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblProject> dataList = new();
            try
            {
                dataList = await _dbContext.tblProject.OrderByDescending(m => m.ProjNumber).Where(x => x.IsActive == true)
                    .Select(x => new TblProject
                    {
                        ProjNumber = x.ProjNumber
                    })
                    .ToListAsync();
                response.data = Convert.ToInt32(dataList[0].ProjNumber) + 1;
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
        /// To get list of Counnty id on behaaf of state
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<List<string>> GetExCounties(string State)
        {
            List<string> dataList = new();
            dataList = await _dbContext.TblCounty.Where(x => x.State == State).Select(x => x.CountyId.ToString()).ToListAsync();
            return dataList;
        }
        /// <summary>
        /// Register member as non member from edit project info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> RegNonMember(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            int row = 0;
            try
            {
                TblMember tblMember = new();
                tblMember.Company = model.Company;
                tblMember.MemberType = 13;
                tblMember.MailAddress = model.MailAddress;
                tblMember.MailCity = model.MailCity;
                tblMember.MailState = model.MailState;
                tblMember.Inactive = model.Inactive;
                tblMember.MailZip = model.MailZip;

                var businessEntity = _entityRepository.BusinessEntity_instance(tblMember);
                await _dbContext.BusinessEntities.AddAsync(businessEntity);
                await _dbContext.SaveChangesAsync();

                var address = _entityRepository.Address_instance(tblMember);
                var member = _entityRepository.Member_instance(tblMember);
                await _dbContext.Addresses.AddAsync(address);
                await _dbContext.Members.AddAsync(member);
                //await _dbContext.SaveChangesAsync();

                //await _dbContext.TblMembers.AddAsync(tblMember);
                row = await _dbContext.SaveChangesAsync();
                model.ID = (await _dbContext.BusinessEntities.AnyAsync()) ? await _dbContext.BusinessEntities.MaxAsync(m => m.BusinessEntityId) : 1;
                if (row > 0)
                {
                    if (!string.IsNullOrEmpty(model.ContactName))
                    {
                        TblContact tblContact = new();
                        tblContact.Contact = model.ContactName;
                        tblContact.Email = model.Email;
                        tblContact.Extension = model.Extension;
                        tblContact.Id = model.ID;
                        tblContact.Phone = model.ContactPhone;

                        var user = _userManager.FindByEmailAsync(model.Email);
                        if (user!=null)
                        {
                            tblContact.Uid = user.Id.ToString();
                        }

                        tblContact.FirstName = model.FirstName;
                        tblContact.LastName = model.LastName;
                        //var contact = _entityRepository.Contact_instance(tblContact);
                        //await _dbContext.Contacts.AddAsync(contact);
                        await _dbContext.TblContacts.AddAsync(tblContact);
                        row = await _dbContext.SaveChangesAsync();
                        model.ConID = (await _dbContext.TblContacts.AnyAsync()) ? await _dbContext.TblContacts.MaxAsync(m => m.ConId) : 1; ;
                    }

                }
                httpResponse.data = model;
                httpResponse.success = true;
            }
            catch (Exception Ex)
            {
                httpResponse.success = false;
                _logger.LogWarning(Ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// Register architect 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveNewContact(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            ///  For check unique architect company and email.
            if (model.IsArchitect == true)
            {
                var archmember = _entityRepository.GetEntities().Where(x => x.Company == model.Company).FirstOrDefault();
                var _archmember = await _dbContext.TblArchOwners.Where(x => x.Name == model.Company).FirstOrDefaultAsync();
                if (archmember != null || _archmember != null)
                {
                    httpResponse.success = false;
                    httpResponse.statusMessage = "Company name already exist";
                    return httpResponse;
                }
            }
            /// for check unique contractor company and email.
            if (model.IsContractor == true)
            {
                var Conmember = _entityRepository.GetEntities().Where(x => x.Company == model.Company).FirstOrDefault();
                var _ConMember = await _dbContext.TblContractors.Where(x => x.Name == model.Company).FirstOrDefaultAsync();
                if (_ConMember != null || Conmember != null)
                {
                    httpResponse.success = false;
                    httpResponse.statusMessage = "Company name already exist";
                    return httpResponse;
                }
            }
            int id = _dbContext.TblArchOwners.Max(x => x.Id);
            int row = 0;
            try
            {
                TblMember archOwner = new();
                archOwner.InsertDate = DateTime.Now;
                archOwner.Inactive = false;
                archOwner.CreatedBy = "Staff";
                archOwner.Company = model.Company;
                archOwner.MailAddress = model.MailAddress;
                archOwner.Email = model.Email;
                archOwner.MailCity = model.MailCity;
                archOwner.MailState = model.MailState;
                archOwner.MailZip = model.MailZip;
                archOwner.BillAddress = model.MailAddress;
                archOwner.BillCity = model.MailCity;
                archOwner.BillState = model.MailState;
                archOwner.BillZip = model.MailZip;
                archOwner.IsMember = false;
                archOwner.IsArchitect = model.IsArchitect;
                archOwner.IsContractor = model.IsContractor;

                var businessEntity = _entityRepository.BusinessEntity_instance(archOwner);
                await _dbContext.BusinessEntities.AddAsync(businessEntity);
                await _dbContext.SaveChangesAsync();

                var address = _entityRepository.Address_instance(archOwner);
                var member = _entityRepository.Member_instance(archOwner);
                await _dbContext.Addresses.AddAsync(address);
                await _dbContext.Members.AddAsync(member);
                //await _dbContext.SaveChangesAsync();

                //await _dbContext.TblMembers.AddAsync(archOwner);
                row = await _dbContext.SaveChangesAsync();
                model.ID = (await _dbContext.BusinessEntities.AnyAsync()) ? await _dbContext.BusinessEntities.MaxAsync(m => m.BusinessEntityId) : 1;
                if (row > 0)
                {
                    if (model.IsArchitect == true)
                    {
                        TblContact tblContact = new();
                        tblContact.Contact = model.ContactName;
                        tblContact.Email = model.Email;
                        tblContact.Extension = model.Extension;
                        tblContact.Id = model.ID;
                        tblContact.Phone = model.ContactPhone;
                        tblContact.CompType = 3;
                        tblContact.MainContact = true;
                        tblContact.Active = true;
                        var user = _userManager.FindByEmailAsync(model.Email);
                        if (user != null)
                        {
                            tblContact.Uid = user.Id.ToString();
                        }


                        tblContact.FirstName = model.FirstName;
                        tblContact.LastName = model.LastName;
                        tblContact.Password = model.ContactPassword != null ? model.ContactPassword.ToString() : 0.ToString();
                        //var contact = _entityRepository.Contact_instance(tblContact);
                        //await _dbContext.Contacts.AddAsync(contact);
                        await _dbContext.TblContacts.AddAsync(tblContact);
                        await _dbContext.SaveChangesAsync();
                        model.ConID = tblContact.ConId;
                    }
                    if (model.IsContractor == true)
                    {
                        TblContact tblContact = new();
                        tblContact.Contact = model.ContactName;
                        tblContact.Email = model.Email;
                        tblContact.Extension = model.Extension;
                        tblContact.Id = model.ID;
                        tblContact.Phone = model.ContactPhone;
                        tblContact.CompType = 2;
                        tblContact.MainContact = true;
                        tblContact.Active = true;
                        //tblContact.Uid = UID;
                        var user = _userManager.FindByEmailAsync(model.Email);
                        if (user != null)
                        {
                            tblContact.Uid = user.Id.ToString();
                        }


                        tblContact.FirstName = model.FirstName;
                        tblContact.LastName = model.LastName;
                        tblContact.Password = model.ContactPassword != null ? model.ContactPassword.ToString() : 0.ToString();
                        //var contact = _entityRepository.Contact_instance(tblContact);
                        //await _dbContext.Contacts.AddAsync(contact);
                        await _dbContext.TblContacts.AddAsync(tblContact);
                        await _dbContext.SaveChangesAsync();
                        model.ConID = tblContact.ConId;
                    }

                }
                httpResponse.data = model;
                httpResponse.success = true;
                httpResponse.statusMessage = "Contact data saved successfully";
            }
            catch (Exception Ex)
            {
                httpResponse.success = false;
                _logger.LogWarning(Ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// Save Entity Type from editprojectinfo EntityType input
        /// </summary>
        /// <param name="EntityType"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveEntityTypeAsync(string EntityType)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblEntitytype = await _dbContext.TblEntityType.SingleOrDefaultAsync(m => m.EntityType == EntityType && m.IsActive == true);
                if (_tblEntitytype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Entity Type already exist";
                    return response;
                }
                TblEntityType tblEntityType = new();
                tblEntityType.EntityType = EntityType;
                tblEntityType.IsActive = true;
                await _dbContext.TblEntityType.AddAsync(tblEntityType);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Entity Type created successfully";
                response.data = tblEntityType;
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
        /// Register contact for contractor from editprojectinfo 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> RegPhlCon(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            int id = _dbContext.TblContacts.Max(x => x.Id);
            int row = 0;
            try
            {
                TblContact contact = new();
                contact.MainContact = false;
                contact.Contact = model.ContactName;
                contact.Id = model.CompId;
                contact.Email = model.Email;
                contact.Extension = model.Extension;
                contact.CompType = 2;
                contact.Phone = model.ContactPhone;
                contact.Uid = id.ToString() + "A";
                contact.Password = "";

                contact.FirstName = model.FirstName;
                contact.LastName = model.LastName;
                //var contactobj = _entityRepository.Contact_instance(contact);
                //await _dbContext.Contacts.AddAsync(contactobj);
                await _dbContext.TblContacts.AddAsync(contact);
                row = await _dbContext.SaveChangesAsync();
                contact.ConId = (await _dbContext.TblContacts.AnyAsync()) ? await _dbContext.TblContacts.MaxAsync(m => m.ConId) : 1;
                if (row > 0)
                {
                    httpResponse.data = contact;
                    httpResponse.success = true;
                }
            }
            catch (Exception Ex)
            {
                httpResponse.success = false;
                _logger.LogWarning(Ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// Get bidoption in phl section of editprojectinfo page
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetBidOption(string SelectedValue = "")
        {
            var response = await (from tab in _dbContext.tblBidOption where tab.IsActive == true select tab).ToListAsync();
            var result = response.ToList().OrderBy(m => m.Id).Select(x => new SelectListItem
            {
                Text = x.BidOption,
                Value = x.Id.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.Id.ToString() == SelectedValue ? true : false))
            }).ToList();
            return result;
        }
    }
}