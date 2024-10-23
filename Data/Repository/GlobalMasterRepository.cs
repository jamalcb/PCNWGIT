using Microsoft.EntityFrameworkCore;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;

namespace PCNW.Data.Repository
{
    public class GlobalMasterRepository : IGlobalMasterRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<GlobalMasterRepository> _logger;
        private readonly string _connectionString;
        public string url;

        public GlobalMasterRepository(ApplicationDbContext dbContext, ILogger<GlobalMasterRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            url = "";
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public async Task<dynamic> DeleteProjectTypeAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProjType tblProjType = await _dbContext.TblProjType.SingleOrDefaultAsync(m => m.ProjTypeId == id);
                if (tblProjType != null)
                {
                    tblProjType.IsActive = false;
                    _dbContext.Entry(tblProjType).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Type deleted successfully";
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
        /// Getting data for GlobalMaster/ProjectType
        /// </summary>
        /// <returns></returns>
		public async Task<dynamic> GetProjectTypeAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<ProjectTypeViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblProjType.OrderByDescending(m => m.IsActive)
                    .Select(x => new ProjectTypeViewModel
                    {
                        ProjTypeId = x.ProjTypeId,
                        ProjType = x.ProjType,
                        IsActive = x.IsActive,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Type data bind successfully";
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
        /// <param name="id"></param>
        /// <returns></returns>
		public async Task<dynamic> HasMaster(int id)
        {

            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProjType tblProjType = await _dbContext.TblProjType.SingleOrDefaultAsync(m => m.ProjTypeId == id);
                if (tblProjType != null)
                {
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Project Type get successfully";
                }
                else
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Project Type invalid id";
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
        /// Getting new project type for GlobalMaster/ProjectType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		public async Task<dynamic> SaveProjectTypeAsync(ProjectTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblProjtype = await _dbContext.TblProjType.SingleOrDefaultAsync(m => m.ProjType == model.ProjType);
                if (_tblProjtype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "ProjectType already exist";
                    return response;
                }
                TblProjType tblProjType = new();
                tblProjType.ProjType = model.ProjType;
                tblProjType.IsActive = model.IsActive;
                await _dbContext.TblProjType.AddAsync(tblProjType);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Type created successfully";
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
        /// Updating new project type for GlobalMaster/ProjectType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		public async Task<dynamic> UpdateProjectTypeAsync(ProjectTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblProjtype = await _dbContext.TblProjType.SingleOrDefaultAsync(m => m.ProjType == model.ProjType && m.ProjTypeId != model.ProjTypeId);
                if (_tblProjtype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "ProjectType already exist";
                    return response;
                }
                TblProjType tblProjType = await _dbContext.TblProjType.SingleOrDefaultAsync(m => m.ProjTypeId == model.ProjTypeId);
                if (tblProjType != null)
                {
                    tblProjType.ProjType = model.ProjType;
                    tblProjType.IsActive = model.IsActive;
                    _dbContext.Entry(tblProjType).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Type updated successfully";
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteProjectSubTypesync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProjSubType tblProjSubType = await _dbContext.TblProjSubType.SingleOrDefaultAsync(m => m.ProjSubTypeID == id);
                if (tblProjSubType != null)
                {
                    tblProjSubType.IsActive = false;
                    _dbContext.Entry(tblProjSubType).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project sub Type deleted successfully";
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
        /// Getting data for GlobalMaster/ProjectSubType
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetProjectSubTypeAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<ProjectSubTypeViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblProjSubType.OrderByDescending(m => m.IsActive == true)
                    .Select(x => new ProjectSubTypeViewModel
                    {
                        ProjSubTypeID = x.ProjSubTypeID,
                        ProjSubType = x.ProjSubType,
                        SortOrder = x.SortOrder,
                        ProjTypeID = x.ProjTypeID,
                        IsActive = x.IsActive,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Sub Type data bind successfully";
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> HasMaster1(int id)
        {

            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProjSubType tblProjSubType = await _dbContext.TblProjSubType.SingleOrDefaultAsync(m => m.ProjSubTypeID == id);
                if (tblProjSubType != null)
                {
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Project Type get successfully";
                }
                else
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Project Type invalid id";
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
        /// Save new data for GlobalMaster/ProjectSubType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveProjectSubTypeAsync(ProjectSubTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblProjSubtype = await _dbContext.TblProjSubType.SingleOrDefaultAsync(m => m.ProjSubType == model.ProjSubType);
                if (_tblProjSubtype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Project Sub Type already exist";
                    return response;
                }
                TblProjSubType tblProjSubType = new();
                tblProjSubType.ProjSubType = model.ProjSubType;
                tblProjSubType.SortOrder = model.SortOrder;
                tblProjSubType.ProjTypeID = model.ProjTypeID;
                tblProjSubType.IsActive = model.IsActive;
                await _dbContext.TblProjSubType.AddAsync(tblProjSubType);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Sub Type created successfully";
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
        /// Update new data for GlobalMaster/ProjectSubType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateProjectSubTypeAsync(ProjectSubTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblProjSubtype = await _dbContext.TblProjSubType.SingleOrDefaultAsync(m => m.ProjSubType == model.ProjSubType && m.ProjSubTypeID != model.ProjSubTypeID);
                if (_tblProjSubtype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Project Sub Type already exist";
                    return response;
                }
                TblProjSubType tblProjSubType = await _dbContext.TblProjSubType.SingleOrDefaultAsync(m => m.ProjSubTypeID == model.ProjSubTypeID);
                if (tblProjSubType != null)
                {
                    tblProjSubType.ProjSubType = model.ProjSubType;
                    tblProjSubType.SortOrder = model.SortOrder;
                    tblProjSubType.ProjTypeID = model.ProjTypeID;
                    tblProjSubType.IsActive = model.IsActive;
                    _dbContext.Entry(tblProjSubType).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project Sub Type updated successfully";
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
        /// Delete EntityType from StaffAccount/MemberMangement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteEntityTypeAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblEntityType tblentity = await _dbContext.TblEntityType.SingleOrDefaultAsync(m => m.EntityID == id);
                if (tblentity != null)
                {
                    tblentity.IsActive = false;
                    _dbContext.Entry(tblentity).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Entity Type deleted successfully";
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
        /// Get Data on GlobalMaster/EntityType
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetEntityTypeAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<EntityTypeViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblEntityType.OrderByDescending(m => m.IsActive == true)
                    .Select(x => new EntityTypeViewModel
                    {
                        EntityID = x.EntityID,
                        EntityType = x.EntityType,
                        IsActive = x.IsActive,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Entity Type data bind successfully";
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> HasMaster2(int id)
        {

            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblEntityType tblentity = await _dbContext.TblEntityType.SingleOrDefaultAsync(m => m.EntityID == id);
                if (tblentity != null)
                {
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Entity Type get successfully";
                }
                else
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Entity Type invalid id";
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
        /// Save ney entitytype from GlobalMaster/EntityType, StaffAccount/MemberManagement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveEntityTypeAsync(EntityTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblEntitytype = await _dbContext.TblEntityType.SingleOrDefaultAsync(m => m.EntityType == model.EntityType && m.IsActive == true);
                if (_tblEntitytype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Entity Type already exist";
                    return response;
                }
                TblEntityType tblEntityType = new();
                tblEntityType.EntityType = model.EntityType;
                tblEntityType.IsActive = model.IsActive;
                await _dbContext.TblEntityType.AddAsync(tblEntityType);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Entity Type created successfully";
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
        /// Edityt/Update ney entitytype from GlobalMaster/EntityType, StaffAccount/MemberManagement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateEntityTypeAsync(EntityTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblEntitytype = await _dbContext.TblEntityType.SingleOrDefaultAsync(m => m.EntityType == model.EntityType && m.EntityID != model.EntityID);
                if (_tblEntitytype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Entity Type already exist";
                    return response;
                }
                TblEntityType tblEntityType = await _dbContext.TblEntityType.SingleOrDefaultAsync(m => m.EntityID == model.EntityID);
                if (tblEntityType != null)
                {
                    tblEntityType.EntityType = model.EntityType;
                    tblEntityType.IsActive = model.IsActive;
                    _dbContext.Entry(tblEntityType).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Entity Type updated successfully";
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
        /// No Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeletePHLTypeAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblPHLType tblPHL = await _dbContext.TblPHLType.SingleOrDefaultAsync(m => m.PHLID == id);
                if (tblPHL != null)
                {
                    tblPHL.IsActive = false;
                    _dbContext.Entry(tblPHL).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "PHL Type deleted successfully";
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
        /// Get Data on GlobalMaster/PHLType
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPHLTypeAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<PHLTypeViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblPHLType.OrderByDescending(m => m.IsActive == true)
                    .Select(x => new PHLTypeViewModel
                    {
                        PHLID = x.PHLID,
                        PHLType = x.PHLType,
                        IsActive = x.IsActive,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "PHL Type data bind successfully";
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> HasMaster3(int id)
        {

            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblPHLType tblphl = await _dbContext.TblPHLType.SingleOrDefaultAsync(m => m.PHLID == id);
                if (tblphl != null)
                {
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "PHL Type get successfully";
                }
                else
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "PHL Type invalid id";
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
        /// Save new phlType Data on GlobalMaster/PHLType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SavePHLTypeAsync(PHLTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblPHLtype = await _dbContext.TblPHLType.SingleOrDefaultAsync(m => m.PHLType == model.PHLType);
                if (_tblPHLtype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "PHL Type already exist";
                    return response;
                }
                TblPHLType tblPHLType = new();
                tblPHLType.PHLType = model.PHLType;
                tblPHLType.IsActive = model.IsActive;
                await _dbContext.TblPHLType.AddAsync(tblPHLType);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "PHL Type created successfully";
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
        /// Update phlType Data on GlobalMaster/PHLType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdatePHLTypeAsync(PHLTypeViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblPHLtype = await _dbContext.TblPHLType.SingleOrDefaultAsync(m => m.PHLType == model.PHLType && m.PHLID != model.PHLID);
                if (_tblPHLtype != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "PHL Type already exist";
                    return response;
                }
                TblPHLType tblPHLType = await _dbContext.TblPHLType.SingleOrDefaultAsync(m => m.PHLID == model.PHLID);
                if (tblPHLType != null)
                {
                    tblPHLType.PHLType = model.PHLType;
                    tblPHLType.IsActive = model.IsActive;
                    _dbContext.Entry(tblPHLType).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "PHL Type updated successfully";
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
        /// Get Data for  GlobalMaster/ProjectNotification (Project Update)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetProjNotificationAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<ProjNotificationViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblProjNotification.Select(x => new ProjNotificationViewModel
                {
                    Id = x.Id,
                    Email = x.Email
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project notification data bind successfully";
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
        /// Save new data from GlobalMaster/ProjectNotification (Project Update)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveProjNotificationAsync(ProjNotificationViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblProjNotify = await _dbContext.TblProjNotification.SingleOrDefaultAsync(m => m.Email == model.Email);
                if (_tblProjNotify != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Email already exist";
                    return response;
                }
                TblProjNotification tblProjNotification = new();
                tblProjNotification.Email = model.Email;
                await _dbContext.TblProjNotification.AddAsync(tblProjNotification);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Add email successfully";
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
        /// Delete data from GlobalMaster/ProjectNotification (Project Update)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteProjNotificationAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblProjNotification tblProjNotification = await _dbContext.TblProjNotification.SingleOrDefaultAsync(m => m.Id == id);
                if (tblProjNotification != null)
                {

                    _dbContext.Entry(tblProjNotification).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Email deleted successfully";
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
        /// Get data for GlobalMaster/MemberSignUp (Sign Up)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetMemberSignUpAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<MemberSignUpViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblMemberSignUp.Select(x => new MemberSignUpViewModel
                {
                    Id = x.Id,
                    Email = x.Email
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Member signUp data bind successfully";
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
        /// Save data from GlobalMaster/MemberSignUp (Sign Up)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveMemberSignUpAsync(MemberSignUpViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblMembersignup = await _dbContext.TblMemberSignUp.SingleOrDefaultAsync(m => m.Email == model.Email);
                if (_tblMembersignup != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Email already exist";
                    return response;
                }
                TblMemberSignUp tblMemberSignUp = new();
                tblMemberSignUp.Email = model.Email;
                await _dbContext.TblMemberSignUp.AddAsync(tblMemberSignUp);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Add email successfully";
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
        ///Delete data from GlobalMaster/MemberSignUp (Sign Up) 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteMemberSignUpAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblMemberSignUp tblMemberSignUp = await _dbContext.TblMemberSignUp.SingleOrDefaultAsync(m => m.Id == id);
                if (tblMemberSignUp != null)
                {

                    _dbContext.Entry(tblMemberSignUp).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Email deleted successfully";
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
        /// Get data for GlobalMaster/PrintOrder (Print Order)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPrintOrderAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<PrintOrderViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblPrintOrder.Select(x => new PrintOrderViewModel
                {
                    Id = x.Id,
                    Email = x.Email
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Print Order data bind successfully";
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
        /// Save data from GlobalMaster/PrintOrder (Print Order)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SavePrintOrderAsync(PrintOrderViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblPrintOrder = await _dbContext.TblPrintOrder.SingleOrDefaultAsync(m => m.Email == model.Email);
                if (_tblPrintOrder != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Email already exist";
                    return response;
                }
                TblPrintOrder tblPrintOrder = new();
                tblPrintOrder.Email = model.Email;
                await _dbContext.TblPrintOrder.AddAsync(tblPrintOrder);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Add email successfully";
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
        /// delete data from GlobalMaster/PrintOrder (Print Order)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeletePrintOrderAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblPrintOrder tblPrintOrder = await _dbContext.TblPrintOrder.SingleOrDefaultAsync(m => m.Id == id);
                if (tblPrintOrder != null)
                {

                    _dbContext.Entry(tblPrintOrder).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Email deleted successfully";
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
        /// Get data for GlobalMaster/PHLUpdate (PHL Update)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPHLUpdateAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<PHLUpdateViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblPHLUpdate.Select(x => new PHLUpdateViewModel
                {
                    Id = x.Id,
                    Email = x.Email
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "PHL update data bind successfully";
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
        /// Save data from GlobalMaster/PHLUpdate (PHL Update)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SavePHLUpdateAsync(PHLUpdateViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblPhlUpdate = await _dbContext.TblPHLUpdate.SingleOrDefaultAsync(m => m.Email == model.Email);
                if (_tblPhlUpdate != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Email already exist";
                    return response;
                }
                TblPHLUpdate tblPHLUpdate = new();
                tblPHLUpdate.Email = model.Email;
                await _dbContext.TblPHLUpdate.AddAsync(tblPHLUpdate);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Add email successfully";
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
        /// delete data from GlobalMaster/PHLUpdate (PHL Update)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeletePHLUpdateAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblPHLUpdate tblPHLUpdate = await _dbContext.TblPHLUpdate.SingleOrDefaultAsync(m => m.Id == id);
                if (tblPHLUpdate != null)
                {

                    _dbContext.Entry(tblPHLUpdate).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Email deleted successfully";
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
        #region Membership Expire
        /// <summary>
        /// Get data for GlobalMaster/MembershipExpire (Membership Expire)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetMembershipExpireAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<MembershipExpireViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblMembershipExpire.Select(x => new MembershipExpireViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    Timer = x.Timer,
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Membership Expire data bind successfully";
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
        /// Save data from GlobalMaster/MembershipExpire (Membership Expire)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveMembershipExpireAsync(MembershipExpireViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblMembershipExpire = await _dbContext.TblMembershipExpire.SingleOrDefaultAsync(m => m.Email == model.Email);
                if (_tblMembershipExpire != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Email already exist";
                    return response;
                }
                TblMembershipExpire tblMembershipExpire = new();
                tblMembershipExpire.Email = model.Email;
                tblMembershipExpire.Timer = model.Timer;
                await _dbContext.TblMembershipExpire.AddAsync(tblMembershipExpire);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Add email successfully";
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
        /// Delete data from GlobalMaster/MembershipExpire (Membership Expire)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteMembershipExpireAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblMembershipExpire tblMembershipExpire = await _dbContext.TblMembershipExpire.SingleOrDefaultAsync(m => m.Id == id);
                if (tblMembershipExpire != null)
                {

                    _dbContext.Entry(tblMembershipExpire).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Email deleted successfully";
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
        #endregion
        #region Auto Logoff 
        /// <summary>
        /// Save auto log off time from GlobalMaster/LogOff(Auto logoff)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveLogOffAsync(LogOffViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {

                TblLogoff tblLogoff = new();
                tblLogoff.LogOff = model.LogOff;
                await _dbContext.TblLogoff.AddAsync(tblLogoff);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Set LogOff times successfully";
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
        /// Get log off data for GlobalMaster/LogOff(Auto logoff)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetLogOffAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<LogOffViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblLogoff.Select(x => new LogOffViewModel
                {
                    Id = x.Id,
                    LogOff = x.LogOff
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Log off data bind successfully";
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
        /// Save auto log off time from GlobalMaster/LogOff(Auto logoff)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateLogOffAsync(LogOffViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblLogoff tblLogoff = await _dbContext.TblLogoff.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (tblLogoff != null)
                {
                    tblLogoff.LogOff = model.LogOff;
                    _dbContext.Entry(tblLogoff).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.data = model.LogOff;
                }

                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Log off time  updated successfully";
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
        #endregion
        #region Faq
        /// <summary>
        /// Get data for GlobalMaster/Faq
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetFaqAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<FAQViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblFAQ.Select(x => new FAQViewModel
                {
                    Id = x.Id,
                    Question = x.Question,
                    Answer = x.Answer,
                    IsActive = x.IsActive
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "faq data bind successfully";
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
        /// Save data from GlobalMaster/Faq
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveFaqAsync(FAQViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {

                TblFAQ tblFAQ = new();
                tblFAQ.Question = model.Question;
                tblFAQ.Answer = model.Answer;
                tblFAQ.IsActive = model.IsActive;
                await _dbContext.TblFAQ.AddAsync(tblFAQ);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Create faq successfully";
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
        /// Update data from GlobalMaster/Faq
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateFaqAsync(FAQViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {

                TblFAQ tblFAQ = await _dbContext.TblFAQ.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (tblFAQ != null)
                {
                    tblFAQ.Question = model.Question;
                    tblFAQ.Answer = model.Answer;
                    tblFAQ.IsActive = model.IsActive;
                    _dbContext.Entry(tblFAQ).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }

                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Faq updated successfully";
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
        #endregion
        #region File Storage Control GlobalMasterController
        #region Past Project
        /// <summary>
        /// Get data for GlobalMaster/PastProject (Past Project)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetFileAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<FileStorageViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblFileStorage.Select(x => new FileStorageViewModel
                {
                    Id = x.Id,
                    FileStorage = x.FileStorage
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "File storage data bind successfully";
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
        /// Save data from GlobalMaster/PastProject (Past Project)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveFileAsync(FileStorageViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {

                TblFileStorage tblFileStorage = new();
                tblFileStorage.FileStorage = model.FileStorage;
                await _dbContext.TblFileStorage.AddAsync(tblFileStorage);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "File storage added successfully";
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
        /// Update data from GlobalMaster/PastProject (Past Project)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateFileAsync(FileStorageViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {

                TblFileStorage tblFileStorage = await _dbContext.TblFileStorage.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (tblFileStorage != null)
                {
                    tblFileStorage.FileStorage = model.FileStorage;
                    _dbContext.Entry(tblFileStorage).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.data = model.FileStorage;
                }

                response.success = true;
                response.statusCode = "200";
                response.statusMessage = " File Storage updated successfully";
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

        #endregion
        #region Copy Center
        /// <summary>
        /// Get Data for GlobalMaster/CopyCenter (Copy Center)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCopyCenterAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<CopyCenterViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.tblCopyCenterFile.Select(x => new CopyCenterViewModel
                {
                    Id = x.Id,
                    CopyCenter = x.CopyCenter
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Copy Center data bind successfully";
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
        /// Save Data from GlobalMaster/CopyCenter (Copy Center)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveCopyCenterAsync(CopyCenterViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblCopyCenterFile tblCopy = new();
                tblCopy.CopyCenter = model.CopyCenter;
                await _dbContext.tblCopyCenterFile.AddAsync(tblCopy);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Copy center file storage added successfully";
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
        /// Update Data from GlobalMaster/CopyCenter (Copy Center)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateCopyCenterAsync(CopyCenterViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {

                tblCopyCenterFile tblCopyCenter = await _dbContext.tblCopyCenterFile.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (tblCopyCenter != null)
                {
                    tblCopyCenter.CopyCenter = model.CopyCenter;
                    _dbContext.Entry(tblCopyCenter).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.data = model.CopyCenter;
                }

                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Copy center file Storage updated successfully";
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
        #endregion
        #endregion

        #region Career Posting GlobalMasterController
        /// <summary>
        /// Delete data from GlobalMaster/CareerPosting (Career Posting) 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteCareerPostingAsync(int Id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblCareerPostings tblCareerPostings = await _dbContext.TblCareerPostings.SingleOrDefaultAsync(m => m.Id == Id);
                if (tblCareerPostings != null)
                {
                    tblCareerPostings.IsActive = false;
                    _dbContext.Entry(tblCareerPostings).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Career Posting deleted successfully";
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
        /// Get data for GlobalMaster/CareerPosting (Career Posting)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCareerPostingAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<CareerPostingViewModel> dataList = new();
            try
            {
                dataList = await _dbContext.TblCareerPostings.Where(m => m.IsActive == true)
                    .Select(x => new CareerPostingViewModel
                    {
                        Id = x.Id,
                        PositionName = x.PositionName,
                        OpaningNo = x.OpaningNo,
                        Experience = x.Experience,
                        Qualification = x.Qualification,
                        ContactPerson = x.ContactPerson,
                        ContactNumber = x.ContactNumber,
                        JobDescription = x.JobDescription,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Career Posting data bind successfully";
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
        /// Save data from GlobalMaster/CareerPosting (Career Posting)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveCareerPostingAsync(CareerPostingViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblCareerPostings tblCareerPostings = new();
                tblCareerPostings.Id = model.Id;
                tblCareerPostings.PositionName = model.PositionName;
                tblCareerPostings.OpaningNo = model.OpaningNo;
                tblCareerPostings.Experience = model.Experience;
                tblCareerPostings.Qualification = model.Qualification;
                tblCareerPostings.ContactPerson = model.ContactPerson;
                tblCareerPostings.ContactNumber = model.ContactNumber;
                tblCareerPostings.JobDescription = model.JobDescription;
                tblCareerPostings.IsActive = true;
                await _dbContext.TblCareerPostings.AddAsync(tblCareerPostings);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Career Posting created successfully";
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
        /// Update data from GlobalMaster/CareerPosting (Career Posting)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateCareerPostingAsync(CareerPostingViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblCareerPostings tblCareerPostings = await _dbContext.TblCareerPostings.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (tblCareerPostings != null)
                {
                    tblCareerPostings.PositionName = model.PositionName;
                    tblCareerPostings.OpaningNo = model.OpaningNo;
                    tblCareerPostings.Experience = model.Experience;
                    tblCareerPostings.Qualification = model.Qualification;
                    tblCareerPostings.ContactPerson = model.ContactPerson;
                    tblCareerPostings.ContactNumber = model.ContactNumber;
                    tblCareerPostings.JobDescription = model.JobDescription;
                    tblCareerPostings.IsActive = true;
                    _dbContext.Entry(tblCareerPostings).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Career Posting updated successfully";
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
        #endregion

        #region Holiday Setting (GlobalMasterController)
        /// <summary>
        /// Delete Data from GlobalMaster/HolidaySetting(Holiday Setting)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteHolidaySettingAsync(int Id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblDailyInfoHoliday tblDailyInfoHoliday = await _dbContext.TblDailyInfoHoliday.SingleOrDefaultAsync(m => m.DiholidayId == Id);
                if (tblDailyInfoHoliday != null)
                {
                    tblDailyInfoHoliday.IsActive = false;
                    _dbContext.Entry(tblDailyInfoHoliday).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Holiday Settings deleted successfully";
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
        /// Get Data for GlobalMaster/HolidaySetting(Holiday Setting)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetHolidaySettingAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblDailyInfoHoliday> dataList = new();
            try
            {
                dataList = await _dbContext.TblDailyInfoHoliday.Where(m => m.IsActive == true)
                    .Select(x => new TblDailyInfoHoliday
                    {
                        DiholidayId = x.DiholidayId,
                        Holiday = x.Holiday,
                        HolidayDt = x.HolidayDt,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Holiday Setting data bind successfully";
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
        /// Save Data from GlobalMaster/HolidaySetting(Holiday Setting)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveHolidaySettingAsync(TblDailyInfoHoliday model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblDailyInfoHoliday tblDailyInfoHoliday = new();
                tblDailyInfoHoliday.DiholidayId = model.DiholidayId;
                tblDailyInfoHoliday.Holiday = model.Holiday;
                tblDailyInfoHoliday.HolidayDt = model.HolidayDt;
                tblDailyInfoHoliday.IsActive = true;
                await _dbContext.TblDailyInfoHoliday.AddAsync(tblDailyInfoHoliday);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Holiday Settings created successfully";
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
        /// Update Data from GlobalMaster/HolidaySetting(Holiday Setting)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateHolidaySettingAsync(TblDailyInfoHoliday model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblDailyInfoHoliday tblDailyInfoHoliday = await _dbContext.TblDailyInfoHoliday.SingleOrDefaultAsync(m => m.DiholidayId == model.DiholidayId);
                if (tblDailyInfoHoliday != null)
                {
                    tblDailyInfoHoliday.Holiday = model.Holiday;
                    tblDailyInfoHoliday.HolidayDt = model.HolidayDt;
                    _dbContext.Entry(tblDailyInfoHoliday).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Holiday Settings updated successfully";
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
        #endregion

        /// <summary>
        /// Get Data for GlobalMaster/LoginReport
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetLoginReportAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblLogActivity> dataList = new();
            try
            {
                dataList = await _dbContext.TblLogActivity.Select(x => new TblLogActivity
                {
                    UserName = x.UserName,
                    LoginTime = x.LoginTime,
                    LastActivity = x.LastActivity,
                    IsAutoLogout = x.IsAutoLogout,
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Login Report data bind successfully";
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

        #region GlobalMaster/SpecialMsg
        /// <summary>
        /// Delete special mesaage from GlobalMaster/SpecialMsg (Special Messge)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteSpecialMsgAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblSpecialMsg tblSpecialMsg = await _dbContext.TblSpecialMsg.SingleOrDefaultAsync(m => m.Id == id);
                if (tblSpecialMsg != null)
                {
                    _dbContext.Entry(tblSpecialMsg).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Special Message deleted successfully";
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
        /// Get Data for GlobalMaster/SpecialMsg (Special Message)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetSpecialMsgAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<TblSpecialMsg> dataList = new();
            try
            {
                dataList = await _dbContext.TblSpecialMsg.Select(x => new TblSpecialMsg
                {
                    Id = x.Id,
                    Type = x.Type,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    SpMessage = x.SpMessage,
                    IsActive = x.IsActive,
                })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Special Message data bind successfully";
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
        /// Save special mesaage from GlobalMaster/SpecialMsg (Special Messge)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveSpecialMsgAsync(TblSpecialMsg model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                if (model.Type.ToLower() == "maintenance")
                {
                    List<TblSpecialMsg> list = await _dbContext.TblSpecialMsg.Where(msg => msg.Type == "Maintenance").ToListAsync();
                    List<TblSpecialMsg> overlappingRanges = list.Where(dr => dr.IsActive == true && ((model.StartDate <= dr.EndDate && model.StartDate >= dr.StartDate) || (model.EndDate <= dr.EndDate && model.EndDate >= dr.StartDate) || (model.StartDate <= dr.StartDate && model.EndDate >= dr.EndDate)) && model.Id != dr.Id)
                        .ToList();

                    if (overlappingRanges != null && overlappingRanges.Count > 0 && (bool)model.IsActive)
                    {
                        response.success = false;
                        response.statusCode = "404";
                        response.statusMessage = "There is already a message set at given time range.";
                        return response;
                    }
                    TblSpecialMsg tblSpecialMsg = new();
                    tblSpecialMsg.Type = model.Type;
                    tblSpecialMsg.StartDate = model.StartDate;
                    tblSpecialMsg.EndDate = model.EndDate;
                    tblSpecialMsg.SpMessage = model.SpMessage;
                    tblSpecialMsg.IsActive = true;
                    await _dbContext.TblSpecialMsg.AddAsync(tblSpecialMsg);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Special Message created successfully";
                }
                else
                {
                    List<TblSpecialMsg> list = await _dbContext.TblSpecialMsg.Where(msg => msg.Type == "Marketing").ToListAsync();
                    List<TblSpecialMsg> overlappingRanges = list.Where(dr => dr.IsActive == true && ((model.StartDate <= dr.EndDate && model.StartDate >= dr.StartDate) || (model.EndDate <= dr.EndDate && model.EndDate >= dr.StartDate) || (model.StartDate <= dr.StartDate && model.EndDate >= dr.EndDate)) && model.Id != dr.Id)
                        .ToList();

                    if (overlappingRanges != null && overlappingRanges.Count > 0 && (bool)model.IsActive)
                    {
                        response.success = false;
                        response.statusCode = "404";
                        response.statusMessage = "There is already a message set at given time range.";
                        return response;
                    }
                    TblSpecialMsg tblSpecialMsg = new();
                    tblSpecialMsg.Type = model.Type;
                    tblSpecialMsg.StartDate = model.StartDate;
                    tblSpecialMsg.EndDate = model.EndDate;
                    tblSpecialMsg.SpMessage = model.SpMessage;
                    tblSpecialMsg.IsActive = true;
                    await _dbContext.TblSpecialMsg.AddAsync(tblSpecialMsg);
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Special Message created successfully";
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
        /// Update special mesaage from GlobalMaster/SpecialMsg (Special Messge)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns> 

        public async Task<dynamic> UpdateSpecialMsgAsync(TblSpecialMsg model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                if (model.Type == "Maintenance")
                {
                    List<TblSpecialMsg> list = await _dbContext.TblSpecialMsg.Where(msg => msg.Type == "Maintenance").ToListAsync();
                    TblSpecialMsg overlappingRanges =
                    list.Where(dr => dr.IsActive == true && ((model.StartDate <= dr.EndDate && model.StartDate >= dr.StartDate) || (model.EndDate <= dr.EndDate && model.EndDate >= dr.StartDate) || (model.StartDate <= dr.StartDate && model.EndDate >= dr.EndDate)) && model.Id != dr.Id)
                        .FirstOrDefault();
                    if (overlappingRanges != null && (bool)model.IsActive)
                    {
                        if (model.Id != overlappingRanges.Id)
                        {
                            response.success = false;
                            response.statusCode = "404";
                            response.statusMessage = "There is already a message set at given time range.";
                            return response;
                        }
                    }
                    TblSpecialMsg tblSpecialMsg = await _dbContext.TblSpecialMsg.SingleOrDefaultAsync(m => m.Id == model.Id);
                    if (tblSpecialMsg != null)
                    {
                        tblSpecialMsg.Type = model.Type;
                        tblSpecialMsg.StartDate = model.StartDate;
                        tblSpecialMsg.EndDate = model.EndDate;
                        tblSpecialMsg.SpMessage = model.SpMessage;
                        tblSpecialMsg.IsActive = model.IsActive;
                        _dbContext.Entry(tblSpecialMsg).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Special Message updated successfully";
                }
                else
                {
                    List<TblSpecialMsg> list = await _dbContext.TblSpecialMsg.Where(msg => msg.Type == "Marketing").ToListAsync();
                    TblSpecialMsg overlappingRanges =
                    list.Where(dr => dr.IsActive == true && ((model.StartDate <= dr.EndDate && model.StartDate >= dr.StartDate) || (model.EndDate <= dr.EndDate && model.EndDate >= dr.StartDate) || (model.StartDate <= dr.StartDate && model.EndDate >= dr.EndDate)) && model.Id != dr.Id)
                        .FirstOrDefault();
                    if (overlappingRanges != null && (bool)model.IsActive)
                    {
                        if (model.Id != overlappingRanges.Id)
                        {
                            response.success = false;
                            response.statusCode = "404";
                            response.statusMessage = "There is already a message set at given time range.";
                            return response;
                        }
                    }
                    TblSpecialMsg tblSpecialMsg = await _dbContext.TblSpecialMsg.SingleOrDefaultAsync(m => m.Id == model.Id);
                    if (tblSpecialMsg != null)
                    {
                        tblSpecialMsg.Type = model.Type;
                        tblSpecialMsg.StartDate = model.StartDate;
                        tblSpecialMsg.EndDate = model.EndDate;
                        tblSpecialMsg.SpMessage = model.SpMessage;
                        tblSpecialMsg.IsActive = model.IsActive;
                        _dbContext.Entry(tblSpecialMsg).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Special Message updated successfully";
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
        #endregion

        #region GlobalMaster/ManageFreeTrialTab (Show/Hide Free Trial)
        /// <summary>
        /// Get/set Data for GlobalMaster/ManageFreeTrialTab 
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetTabData()
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblFreeTab freeTab = await _dbContext.tblFreeTab.FirstOrDefaultAsync(x => x.Id == 1);
                if (freeTab != null)
                {
                    response.data = freeTab;
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Special Message updated successfully";
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
        /// Set Data from GlobalMaster/ManageFreeTrialTab (Show/Hide Free Trial)
        /// </summary>
        /// <param name="SetTab"></param>
        /// <returns></returns>
        public async Task<dynamic> SetTabData(bool SetTab)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblFreeTab freeTab = await _dbContext.tblFreeTab.FirstOrDefaultAsync(x => x.Id == 1);
                if (freeTab != null)
                {
                    freeTab.SetTab = SetTab;
                    _dbContext.Entry(freeTab).State = EntityState.Modified;
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
        #endregion

        #region GlobalMaster/AdditionalPricing (Additional Pricing)
        /// <summary>
        /// Get Data for GlobalMaster/AdditionalPricing (Additional Pricing)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PaymentInfo>> GetAdditionalPackage()
        {
            IEnumerable<PaymentInfo> response = new List<PaymentInfo>();
            try
            {
                var result = await (from tab in _dbContext.tblMemberShipSubPlans
                                    join mplan in _dbContext.tblMemberShipPlans
                                    on tab.MemberShipPlanId equals mplan.MemberShipPlanId
                                    select new
                                    {
                                        PlanName = mplan.MemberShipPlanName,
                                        PlanId = tab.MemberShipPlanId,
                                        SubPlanActive = tab.Active,
                                        SubPlanId = tab.SubMemberShipPlanId,
                                        MPrice = tab.AddMonthlyPrice,
                                        YPrice = tab.AddYearlyPrice,
                                        QPrice = tab.AddQuarterlyPrice,
                                        SubPlanName = tab.SubMemberShipPlanName
                                    }).ToListAsync();
                response = result.Select(x => new PaymentInfo
                {
                    MemberShipPlanName = x.PlanName,
                    SubMemberShipPlanName = x.SubPlanName,
                    MemberShipPlanId = x.PlanId,
                    AddMonthlyPrice = decimal.Round((decimal)x.MPrice, 2, MidpointRounding.AwayFromZero),
                    AddYearlyPrice = decimal.Round((decimal)x.YPrice, 2, MidpointRounding.AwayFromZero),
                    AddQuarterlyPrice = decimal.Round((decimal)x.QPrice, 2, MidpointRounding.AwayFromZero),
                    SubMemberShipPlanId = x.SubPlanId,
                    Active = x.SubPlanActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Save/Edit Data from GlobalMaster/AdditionalPricing (Additional Pricing)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveAddPackageData(PaymentInfo model)
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
                    subPlans.AddMonthlyPrice = model.AddMonthlyPrice;
                    subPlans.AddQuarterlyPrice = model.AddQuarterlyPrice;
                    subPlans.AddYearlyPrice = model.AddYearlyPrice;
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
        #endregion

        #region Copy Center Data
        #region GlobalMaster/DeliveryType (Delivery Type)
        /// <summary>
        /// Get data for GlobalMaster/DeliveryType (Delivery Type)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetDeliveryTypeAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblDeliveryMaster> dataList = new();
            try
            {
                dataList = await _dbContext.tblDeliveryMaster.OrderByDescending(m => m.IsActive)
                    .Select(x => new tblDeliveryMaster
                    {
                        DelivId = x.DelivId,
                        DelivName = x.DelivName,
                        IsActive = x.IsActive,
                    })
                    .ToListAsync();
                response.data = dataList;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery Type data bind successfully";
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
        /// Save data from GlobalMaster/DeliveryType (Delivery Type)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveDeliveryTypeAsync(tblDeliveryMaster model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblDeliveryMaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivName == model.DelivName);
                if (_tblDeliveryMaster != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "DeliveryType already exist";
                    return response;
                }
                tblDeliveryMaster tblDeliveryMaster = new();
                tblDeliveryMaster.DelivName = model.DelivName;
                tblDeliveryMaster.IsActive = model.IsActive;
                await _dbContext.tblDeliveryMaster.AddAsync(tblDeliveryMaster);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery Type created successfully";
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
        /// Update data from GlobalMaster/DeliveryType (Delivery Type)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDeliveryTypeAsync(tblDeliveryMaster model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblDeliveryMaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivName == model.DelivName && m.DelivId != model.DelivId);
                if (_tblDeliveryMaster != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "DeliveryType already exist";
                    return response;
                }
                tblDeliveryMaster tblDeliveryMaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivId == model.DelivId);
                if (tblDeliveryMaster != null)
                {
                    tblDeliveryMaster.DelivId = model.DelivId;
                    tblDeliveryMaster.DelivName = model.DelivName;
                    tblDeliveryMaster.IsActive = model.IsActive;
                    _dbContext.Entry(tblDeliveryMaster).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery Type updated successfully";
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
        /// Delete data from GlobalMaster/DeliveryType (Delivery Type)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteDeliveryTypeAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblDeliveryMaster tblDeliveryMaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivId == id);
                if (tblDeliveryMaster != null)
                {
                    _dbContext.Entry(tblDeliveryMaster).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery type deleted successfully";
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
        #endregion

        #region GlobalMaster/DeliverySubType (Delivery Sub Type)
        /// <summary>
        /// Get data for GlobalMaster/DeliverySubType (Delivery Sub Type)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetDeliverySubTypeAsync()
        {
            IEnumerable<DeliveryViewModel> response = new List<DeliveryViewModel>();
            try
            {
                response = (from m in _dbContext.tblDeliveryMaster
                            join c in _dbContext.tblDeliveryOption on m.DelivId equals c.DelivId
                            select new
                            {
                                m.DelivId,
                                m.DelivName,
                                c.DelivOptId,
                                c.DelivOptName,
                                c.IsActive
                            }).ToList().Select(x => new DeliveryViewModel
                            {
                                DelivId = x.DelivId,
                                DelivOptId = x.DelivOptId,
                                DelivName = x.DelivName,
                                DelivOptName = x.DelivOptName,
                                IsActive = x.IsActive
                            }).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// Save data from GlobalMaster/DeliverySubType (Delivery Sub Type)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveDeliverySubTypeAsync(tblDeliveryOption model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblDeliveryOption = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptName == model.DelivOptName && m.DelivId == model.DelivId);
                if (_tblDeliveryOption != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Delivery subtype already exist";
                    return response;
                }
                tblDeliveryOption tblDeliveryOption = new();
                tblDeliveryOption.DelivId = model.DelivId;
                tblDeliveryOption.DelivOptName = model.DelivOptName;
                tblDeliveryOption.IsActive = model.IsActive;
                await _dbContext.tblDeliveryOption.AddAsync(tblDeliveryOption);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery subtype created successfully";
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
        /// Update data from GlobalMaster/DeliverySubType (Delivery Sub Type)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDeliverySubTypeAsync(tblDeliveryOption model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var _tblDeliveryOption = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptName == model.DelivOptName && m.DelivId != model.DelivId && m.DelivOptId == model.DelivOptId);
                if (_tblDeliveryOption != null)
                {
                    response.success = false;
                    response.statusCode = "404";
                    response.statusMessage = "Delivery subtype already exist";
                    return response;
                }
                tblDeliveryOption tblDeliveryOption = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptId == model.DelivOptId);
                if (tblDeliveryOption != null)
                {
                    tblDeliveryOption.DelivId = model.DelivId;
                    tblDeliveryOption.DelivOptId = model.DelivOptId;
                    tblDeliveryOption.DelivOptName = model.DelivOptName;
                    tblDeliveryOption.IsActive = model.IsActive;
                    _dbContext.Entry(tblDeliveryOption).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery subtype updated successfully";
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
        /// Delete data from GlobalMaster/DeliverySubType (Delivery Sub Type)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteDeliverySubTypeAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                tblDeliveryOption tblDeliveryOption = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptId == id);
                if (tblDeliveryOption != null)
                {
                    _dbContext.Entry(tblDeliveryOption).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Delivery subtype deleted successfully";
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
        #endregion 
        #endregion
    }
}
