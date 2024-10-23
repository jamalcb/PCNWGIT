using PCNW.Data.ContractResponse;

namespace PCNW.Data.Repository
{
    public interface IServiceRepository
    {
        Task<List<AddNotification>> GetNotificationContactsAsync(int MemberID);
        Task<bool> AddNotificationContactsAsync(int MemberID, string Contact, string Email, bool? Daily);
    }
}
