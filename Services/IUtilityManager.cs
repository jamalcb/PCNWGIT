namespace PCNW.Services
{
    public interface IUtilityManager
    {
        Task<bool> LogIP(string ipAddress, int? MemberId, string Notes, bool? Show);
        Task<bool> EmailNewIP(string ipAddress, int? MemberId);
        Task<bool> InBlockedList(string ipAddress);
        Task<bool> Holiday(DateTime holidayDt);
    }
}
