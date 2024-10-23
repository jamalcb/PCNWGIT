using Microsoft.AspNetCore.Mvc.Rendering;
using PCNW.Models;
using PCNW.Models.ContractModels;

namespace PCNW.Data.Repository
{
    public interface IProjectRepository
    {
        Task<List<SelectListItem>> GetProjecttype(string SelectedValue = "");
        Task<dynamic> SaveProjectInfoAsync(ProjectInformation model);
        Task<Int64> GetProjectCodeAsync();
        Task<ProjectInformation> GetProjectdetail(int id, IEnumerable<TblCounty> tbls);
        Task<dynamic> UpdateProjectInfoAsync(ProjectInformation project);
        List<AddendaInfo> SetNGetAddendaFolder(List<string> AddendaS3List, int ProjId);
        Task<ProjectInformation> GetProjectPreview(int id);
        Task<IEnumerable<ProjectInformation>> GetSearchFindProjectsAsync(string searchTxt);
        string GetProjectName(string searchText);
        Task<dynamic> UniqueDate(string uniqueDate, string projectNumber);
        Task<List<SelectListItem>> GetPhlType(string SelectedValue = "");
        Task<dynamic> AddPhlNote(int ProjId, string PHLNote);
        Task<dynamic> AddBrNote(int ProjId, string BrNote);
        Task<dynamic> ShowCard(int Id, int ConId);
        Task<dynamic> CheckCountyAsync(string City, string State);
        Task<dynamic> GetProjectNumberAsync();
        Task<List<string>> GetExCounties(string State);
        Task<dynamic> RegNonMember(MemberShipRegistration model);
        Task<dynamic> SaveNewContact(MemberShipRegistration model);
        Task<dynamic> SaveEntityTypeAsync(string EntityType);
        Task<dynamic> RegPhlCon(MemberShipRegistration model);
        Task<List<SelectListItem>> GetBidOption(string SelectedValue = "");
    }
}
