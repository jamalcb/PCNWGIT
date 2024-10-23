using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using PCNW.ViewModel;
using System.Data.Entity;

namespace PCNW.Data.Repository
{
    public class ImplementRepository : IImplementRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<ImplementRepository> _logger;
        private readonly IEntityRepository _entityRepository;

        public ImplementRepository(ApplicationDbContext applicationDbContext, ILogger<ImplementRepository> logger, IEntityRepository entityRepository)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _entityRepository = entityRepository;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public List<AutoCompleteViewModel> AutoCompleteAsync(string prefix)
        {
            List<AutoCompleteViewModel> response = new();
            if (!string.IsNullOrEmpty(prefix))
            {
                response = (from member in _entityRepository.GetEntities()
                            where member.Company.StartsWith(prefix)
                            select new AutoCompleteViewModel
                            {
                                label = member.Company,
                                val = member.Id
                            }).ToList();
            }
            else
            {

                response = (from member in _entityRepository.GetEntities()
                            select new AutoCompleteViewModel
                            {
                                label = member.Company,
                                val = member.Id
                            }).ToList();
            }
            return response;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public Int64 GetProjectCodeAsync()
        {
            Int64 response = 0;
            try
            {
                IEnumerable<TblProjectCode> result = _applicationDbContext.TblProjectCode.FromSqlRaw("sp_GetProjectNumber").ToList();
                foreach (var item in result)
                {
                    response = item.ProjNumber;
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetStates(string SelectedValue = "")
        {
            var response = await (from tab in _applicationDbContext.TblState select tab).ToListAsync();
            var result = response.ToList().OrderBy(m => m.StateId).Select(x => new SelectListItem
            {
                Text = x.State,
                Value = x.StateId.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.StateId.ToString() == SelectedValue ? true : false))
            }).ToList();
            return result;
        }
        /// <summary>
		/// Get company address on preview(staffaccount page) print order form, staff copy center
		/// </summary>
		/// <param name="Id"></param>
		/// <returns></returns>
		public CopyCenterMemberInfo GetCompanyAddress(int Id)
        {
            CopyCenterMemberInfo copyCenterMemberInfo = new();
            try
            {
                //	TblMember? tblMember1 = _applicationDbContext.TblMembers.SingleOrDefault(m => m.Id == Id);
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == Id); ;
                if (tblMember != null)
                {
                    copyCenterMemberInfo.Id = tblMember.Id;
                    copyCenterMemberInfo.Company = tblMember.Company;
                    copyCenterMemberInfo.BillAddress = tblMember.BillAddress;
                    copyCenterMemberInfo.BillCity = tblMember.BillCity;
                    copyCenterMemberInfo.BillState = tblMember.BillState;
                    copyCenterMemberInfo.BillZip = tblMember.BillZip;
                    copyCenterMemberInfo.Email = tblMember.Email;
                    copyCenterMemberInfo.DailyEmail = tblMember.DailyEmail;
                    copyCenterMemberInfo.PaperlessBilling = tblMember.PaperlessBilling;
                    copyCenterMemberInfo.ContactList = _applicationDbContext.TblContacts.Where(m => m.Id == tblMember.Id).ToList()
                        .Select(x => new MemberContactInfo
                        {
                            Contact = x.Contact,
                            Email = x.Email,
                            Phone = x.Phone,
                            ConID = x.ConId,
                            UID = x.Uid
                        }).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return copyCenterMemberInfo;
        }
        /// <summary>
		/// Get the list of state which is present in active project list on find project here state list and also in geographical search
		/// </summary>
		/// <param name="SelectedValue"></param>
		/// <returns></returns>
		public async Task<List<SelectListItem>> GetDistinctState(string SelectedValue = "")
        {
            var response = (from tab in _applicationDbContext.tblProject
                            join m in _applicationDbContext.TblState on tab.LocState equals m.State into stateGroup
                            from state in stateGroup.DefaultIfEmpty()
                            where (tab.IsActive == true || tab.IsActive == null) && (tab.LocState != null) && tab.ProjNumber != null && tab.Publish == true
                            orderby tab.ProjId descending
                            select new
                            {
                                State = tab.LocState,
                                StateId = state != null ? state.StateId : (int?)null
                            }).Distinct();


            var result = response.ToList().OrderBy(m => m.StateId).Select(x => new SelectListItem
            {
                Text = x.State,
                Value = x.StateId.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.StateId.ToString() == SelectedValue ? true : false))
            }).ToList();
            return result;



        }
        public async Task<List<SelectListItem>> GetDistinctStateOfTab(int SelectedTab = 0, DisplayLoginInfo info = null, string SelectedValue = "")
        {
            FindProjectModel list = new();
            list.ActiveProjs = new();
            try
            {
                if (SelectedTab == 1)
                {
                    list.ActiveProjs = _applicationDbContext.FindProjectView
                .FromSqlRaw("EXEC SP_FindProjectHere @MemberId, @ConId", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId))
                .AsQueryable()
                .ToList();
                }
                else if (SelectedTab == 2)
                {
                    list.ActiveProjs = _applicationDbContext.FindProjectView
                .FromSqlRaw("EXEC SP_FindProjectHereFuture @MemberId, @ConId", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId))
                .AsQueryable()
                .ToList();

                }
                else
                {
                    list.ActiveProjs = _applicationDbContext.FindProjectView
               .FromSqlRaw("EXEC SP_FindProjectHerePrev @MemberId, @ConId", new SqlParameter("@MemberId", info.Id), new SqlParameter("@ConId", info.ConId))
               .AsQueryable()
               .ToList();
                }


            }
            catch (Exception Ex)
            {

                throw;
            }
            var response = (from tab in list.ActiveProjs
                            join m in _applicationDbContext.TblState on tab.LocState equals m.State into stateGroup
                            from state in stateGroup.DefaultIfEmpty()
                            where (tab.LocState != null) && tab.ProjNumber != null && tab.Publish == true
                            orderby tab.ProjId descending
                            select new
                            {
                                State = tab.LocState,
                                StateId = state != null ? state.StateId : (int?)null
                            }).Distinct();

            var result = response.ToList().OrderBy(m => m.StateId).Select(x => new SelectListItem
            {
                Text = x.State,
                Value = x.StateId.ToString(),
                Selected = (string.IsNullOrEmpty(SelectedValue) ? false : (x.StateId.ToString() == SelectedValue ? true : false))
            }).ToList();

            return result;



        }
    }
}
