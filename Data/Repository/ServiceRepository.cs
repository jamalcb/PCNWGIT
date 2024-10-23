using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCNW.Data.ContractResponse;
using PCNW.Models;
using System.Data.Entity.Validation;
using System.Text;

namespace PCNW.Data.Repository
{
    /// <summary>
    /// No Use
    /// </summary>
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ServiceRepository> _logger;
        private readonly IEntityRepository _entityRepository;

        public ServiceRepository(ApplicationDbContext dbContext, ILogger<ServiceRepository> logger, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _entityRepository = entityRepository;
        }
        public async Task<List<AddNotification>> GetNotificationContactsAsync(int MemberID)
        {
            List<AddNotification> response = new();
            try
            {
                response = await _dbContext.TblContacts.Where(m => m.Id == MemberID)
                        .Select(x => new AddNotification
                        {
                            MemberID = x.Id,
                            Contact = x.Contact,
                            Email = x.Email,
                            Daily = x.Daily
                        }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<bool> AddNotificationContactsAsync(int MemberID, string Contact, string Email, bool? Daily)
        {
            bool rowsAffected = false;
            try
            {
                TblContact tblContact = new();
                tblContact.Id = MemberID;
                tblContact.Email = Email;
                tblContact.Daily = Daily;
                tblContact.Contact = Contact;
                tblContact.MainContact = false;

                //var contact = _entityRepository.Contact_instance(tblContact);
                //await _dbContext.Contacts.AddAsync(contact);
                await _dbContext.TblContacts.AddAsync(tblContact);
                await _dbContext.SaveChangesAsync();
                rowsAffected = true;
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder strErr = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    strErr.Append($"Entity of type {eve.Entry.Entity.GetType().Name}" +
                    $"in the state {eve.Entry.State} " +
                    $"has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        strErr.Append($"Property: {ve.PropertyName}," +
                            $" Error: {ve.ErrorMessage}");
                    }
                    foreach (var ve in eve.ValidationErrors)
                    {
                        strErr.Append($"Property: {ve.PropertyName}, " +
                            $"Value: {eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName)}," +
                            $" Error: {ve.ErrorMessage}");
                    }
                }
                string err = strErr.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return rowsAffected;
        }
    }
}
