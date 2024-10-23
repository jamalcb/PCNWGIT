using Microsoft.AspNetCore.Identity;
using PCNW.Models;

namespace PCNW.Data.Repository
{
    public interface IEntityRepository
    {
        BusinessEntity BusinessEntity_instance(TblMember tblMember);
        Task<BusinessEntity> BusinessEntity_instanceAsync(TblMember tblMember);
        Address Address_instance(TblMember tblMember);
        Task<Address> Address_instanceAsync(TblMember tblMember);
        Member Member_instance(TblMember tblMember);
        Task<Member> Member_instanceAsync(TblMember tblMember);
        //Contact Contact_instance(TblContact tblContact);
        List<TblMember> GetEntities();
        Task<List<TblMember>> GetEntitiesAsync();
        void UpdateEntity(TblMember tblMember);
        Task UpdateEntityAsync(TblMember tblMember);
        void UpdateContactUserId(string email, IdentityUser user);
    }
}
