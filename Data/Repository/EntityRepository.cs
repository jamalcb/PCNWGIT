using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCNW.Models;

namespace PCNW.Data.Repository
{
    public class EntityRepository : IEntityRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<EntityRepository> _logger;
        private readonly string _connectionString;

        public EntityRepository(ApplicationDbContext dbContext, ILogger<EntityRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
        }
        //for getting the BusinessEntity from tblMember
        public BusinessEntity BusinessEntity_instance(TblMember tblMember)
        {
            if (tblMember != null)
            {
                var businessEntity = new BusinessEntity
                {
                    BusinessEntityName = tblMember.Company,
                    BusinessEntityEmail = tblMember.Email,
                    BusinessEntityPhone = tblMember.CompanyPhone,
                    IsMember = (bool)tblMember.IsMember,
                    IsArchitect = (bool)tblMember.IsArchitect,
                    IsContractor = (bool)tblMember.IsContractor
                };
                return businessEntity;
            }
            return null;
        }

        //async Method
        public async Task<BusinessEntity> BusinessEntity_instanceAsync(TblMember tblMember)
        {
            if (tblMember != null)
            {
                var businessEntity = new BusinessEntity
                {
                    BusinessEntityName = tblMember.Company,
                    BusinessEntityEmail = tblMember.Email,
                    BusinessEntityPhone = tblMember.CompanyPhone,
                    IsMember = (bool)tblMember.IsMember,
                    IsArchitect = (bool)tblMember.IsArchitect,
                    IsContractor = (bool)tblMember.IsContractor
                };
                return businessEntity;
            }
            return null;
        }




        //for getting the Address from tblMember
        public Address Address_instance(TblMember tblMember)
        {
            if (tblMember != null)
            {
                var address = new Address
                {
                    BusinessEntityId = (_dbContext.BusinessEntities.Any()) ? _dbContext.BusinessEntities.Max(m => m.BusinessEntityId) : 1,
                    AddressName = "Main Address",
                    Addr1 = tblMember.BillAddress,
                    City = tblMember.BillCity,
                    State = tblMember.BillState,
                    Zip = tblMember.BillZip
                };
                return address;
            }
            return null;
        }

        //async Method
        public async Task<Address> Address_instanceAsync(TblMember tblMember)
        {
            if (tblMember != null)
            {
                var address = new Address
                {
                    BusinessEntityId = (await _dbContext.BusinessEntities.AnyAsync()) ? await _dbContext.BusinessEntities.MaxAsync(m => m.BusinessEntityId) : 1,
                    AddressName = "Main Address",
                    Addr1 = tblMember.BillAddress,
                    City = tblMember.BillCity,
                    State = tblMember.BillState,
                    Zip = tblMember.BillZip
                };
                return address;
            }
            return null;
        }



        //for getting the Member from tblMember
        public Models.Member Member_instance(TblMember tblMember)
        {
            if (tblMember != null)
            {
                var memberobj = new PCNW.Models.Member
                {
                    BusinessEntityId = (_dbContext.BusinessEntities.Any()) ? _dbContext.BusinessEntities.Max(m => m.BusinessEntityId) : 1,
                    Inactive = tblMember.Inactive,
                    InsertDate = (DateTime)tblMember.InsertDate,
                    LastPayDate = DateTime.Now.ToString("MM/dd/yyyy"),
                    RenewalDate = tblMember.RenewalDate,
                    Term = tblMember.Term,
                    Div = tblMember.Div,
                    Discipline = tblMember.Discipline,
                    Note = tblMember.Note,
                    MinorityStatus = tblMember.MinorityStatus,
                    MemberType = tblMember.MemberType,
                    AcceptedTerms = tblMember.AcceptedTerms,
                    AcceptedTermsDt = tblMember.AcceptedTermsDt,
                    DailyEmail = tblMember.DailyEmail,
                    Html = tblMember.Html,
                    Overdue = tblMember.Overdue,
                    Cod = tblMember.Cod,
                    PaperlessBilling = tblMember.PaperlessBilling,
                    MemberCost = tblMember.MemberCost,
                    MagCost = tblMember.MagCost,
                    ArchPkgCost = tblMember.ArchPkgCost,
                    AddPkgCost = tblMember.AddPkgCost,
                    ResourceDate = tblMember.ResourceDate,
                    ResourceCost = tblMember.ResourceCost,
                    WebAdDate = tblMember.WebAdDate,
                    WebAdCost = tblMember.WebAdCost,
                    Phl = tblMember.Phl,
                    Email = tblMember.Email,
                    NameField = tblMember.NameField,
                    FavExp = tblMember.FavExp,
                    Grace = tblMember.Grace,
                    ConId = tblMember.ConId,
                    Gcservices = tblMember.Gcservices,
                    ResourceStandard = tblMember.ResourceStandard,
                    ResourceColor = tblMember.ResourceColor,
                    ResourceLogo = tblMember.ResourceLogo,
                    ResourceAdd = tblMember.ResourceAdd,
                    Dba = tblMember.Dba,
                    Dba2 = tblMember.Dba2,
                    Fka = tblMember.Fka,
                    Suspended = tblMember.Suspended,
                    SuspendedDt = tblMember.SuspendedDt,
                    Fax = tblMember.Fax,
                    MailAddress = tblMember.MailAddress,
                    MailCity = tblMember.MailCity,
                    MailState = tblMember.MailState,
                    MailZip = tblMember.MailZip,
                    OverdueAmt = tblMember.OverdueAmt,
                    OverdueDt = tblMember.OverdueDt,
                    CalSort = tblMember.CalSort,
                    Pdfpkg = tblMember.Pdfpkg,
                    ArchPkg = tblMember.ArchPkg,
                    AddPkg = tblMember.AddPkg,
                    Bend = tblMember.Bend,
                    Credits = tblMember.Credits,
                    FreelanceEstimator = tblMember.FreelanceEstimator,
                    HowdUhearAboutUs = tblMember.HowdUhearAboutUs,
                    IsAutoRenew = tblMember.IsAutoRenew,
                    CompanyPhone = tblMember.CompanyPhone,
                    Logo = tblMember.Logo,
                    CheckDirectory = tblMember.CheckDirectory,
                    MemId = tblMember.MemID,
                    InvoiceId = tblMember.InvoiceId,
                    Discount = tblMember.Discount,
                    PayModeRef = tblMember.PayModeRef,
                    CreatedBy = tblMember.CreatedBy,
                    IsDelete = tblMember.IsDelete,
                    AddGraceDate = tblMember.AddGraceDate,
                    ActualRenewalDate = tblMember.ActualRenewalDate
                };
                return memberobj;
            }
            return null;
        }

        public async Task<Models.Member> Member_instanceAsync(TblMember tblMember)
        {
            if (tblMember != null)
            {
                var memberobj = new PCNW.Models.Member
                {
                    BusinessEntityId = (await _dbContext.BusinessEntities.AnyAsync()) ? await _dbContext.BusinessEntities.MaxAsync(m => m.BusinessEntityId) : 1,
                    Inactive = tblMember.Inactive,
                    InsertDate = (DateTime)tblMember.InsertDate,
                    LastPayDate = DateTime.Now.ToString("MM/dd/yyyy"),
                    RenewalDate = tblMember.RenewalDate,
                    Term = tblMember.Term,
                    Div = tblMember.Div,
                    Discipline = tblMember.Discipline,
                    Note = tblMember.Note,
                    MinorityStatus = tblMember.MinorityStatus,
                    MemberType = tblMember.MemberType,
                    AcceptedTerms = tblMember.AcceptedTerms,
                    AcceptedTermsDt = tblMember.AcceptedTermsDt,
                    DailyEmail = tblMember.DailyEmail,
                    Html = tblMember.Html,
                    Overdue = tblMember.Overdue,
                    Cod = tblMember.Cod,
                    PaperlessBilling = tblMember.PaperlessBilling,
                    MemberCost = tblMember.MemberCost,
                    MagCost = tblMember.MagCost,
                    ArchPkgCost = tblMember.ArchPkgCost,
                    AddPkgCost = tblMember.AddPkgCost,
                    ResourceDate = tblMember.ResourceDate,
                    ResourceCost = tblMember.ResourceCost,
                    WebAdDate = tblMember.WebAdDate,
                    WebAdCost = tblMember.WebAdCost,
                    Phl = tblMember.Phl,
                    Email = tblMember.Email,
                    NameField = tblMember.NameField,
                    FavExp = tblMember.FavExp,
                    Grace = tblMember.Grace,
                    ConId = tblMember.ConId,
                    Gcservices = tblMember.Gcservices,
                    ResourceStandard = tblMember.ResourceStandard,
                    ResourceColor = tblMember.ResourceColor,
                    ResourceLogo = tblMember.ResourceLogo,
                    ResourceAdd = tblMember.ResourceAdd,
                    Dba = tblMember.Dba,
                    Dba2 = tblMember.Dba2,
                    Fka = tblMember.Fka,
                    Suspended = tblMember.Suspended,
                    SuspendedDt = tblMember.SuspendedDt,
                    Fax = tblMember.Fax,
                    MailAddress = tblMember.MailAddress,
                    MailCity = tblMember.MailCity,
                    MailState = tblMember.MailState,
                    MailZip = tblMember.MailZip,
                    OverdueAmt = tblMember.OverdueAmt,
                    OverdueDt = tblMember.OverdueDt,
                    CalSort = tblMember.CalSort,
                    Pdfpkg = tblMember.Pdfpkg,
                    ArchPkg = tblMember.ArchPkg,
                    AddPkg = tblMember.AddPkg,
                    Bend = tblMember.Bend,
                    Credits = tblMember.Credits,
                    FreelanceEstimator = tblMember.FreelanceEstimator,
                    HowdUhearAboutUs = tblMember.HowdUhearAboutUs,
                    IsAutoRenew = tblMember.IsAutoRenew,
                    CompanyPhone = tblMember.CompanyPhone,
                    Logo = tblMember.Logo,
                    CheckDirectory = tblMember.CheckDirectory,
                    MemId = tblMember.MemID,
                    InvoiceId = tblMember.InvoiceId,
                    Discount = tblMember.Discount,
                    PayModeRef = tblMember.PayModeRef,
                    CreatedBy = tblMember.CreatedBy,
                    IsDelete = tblMember.IsDelete,
                    AddGraceDate = tblMember.AddGraceDate,
                    ActualRenewalDate = tblMember.ActualRenewalDate
                };
                return memberobj;
            }
            return null;
        }

        //for converting the tblcontact instance to Contact..
        //public Contact Contact_instance(TblContact tblContact)
        //{
        //    if (tblContact != null)
        //    {
        //        int max_ConID = (_dbContext.Contacts.Any()) ? _dbContext.Contacts.Max(m => m.ContactId) : 1;
        //        string U_ID = (max_ConID).ToString() + "A";
        //        var contact = new Contact()
        //        {
        //            BusinessEntityId = (tblContact.Id != 0) ? tblContact.Id : ((_dbContext.BusinessEntities.Any()) ? _dbContext.BusinessEntities.Max(m => m.BusinessEntityId) : 1),
        //            ContactName = tblContact.Contact,
        //            MainContact = tblContact.MainContact,
        //            ContactPhone = tblContact.Phone,
        //            ContactEmail = tblContact.Email,
        //            UserId = Guid.NewGuid(),
        //            Uid = U_ID,
        //            Password = tblContact.Password,
        //            ContactTitle = tblContact.Title,
        //            Daily = tblContact.Daily,
        //            TextMsg = tblContact.TextMsg,
        //            Message = tblContact.Message,
        //            MessageDt = tblContact.MessageDt,
        //            AutoSearch = tblContact.AutoSearch,
        //            ContactState = tblContact.ContactState,
        //            ContactCity = tblContact.ContactCity,
        //            ContactAddress = tblContact.ContactAddress,
        //            ContactZip = tblContact.ContactZip,
        //            ContactCounty = tblContact.ContactCounty,
        //            LocId = tblContact.LocId,
        //            BillEmail = tblContact.BillEmail,
        //            Extension = tblContact.Extension,
        //            CompType = tblContact.CompType,
        //            Active = tblContact.Active
        //        };
        //        return contact;

        //    }
        //    return null;
        //}
        public List<TblMember> GetEntities()
        {
            try
            {
                var businessEntitiesQuery = _dbContext.BusinessEntities
                    .Include(be => be.Members)
                    .Include(be => be.Addresses);

                // Execute the query to retrieve BusinessEntities
                var businessEntities = businessEntitiesQuery.ToList();
                var result = businessEntities
                    .SelectMany(be => be.Members.DefaultIfEmpty(), (be, m) => new TblMember
                    {
                        Id = be.BusinessEntityId,
                        InsertDate = m != null ? m.InsertDate : default(DateTime?),
                        Company = be.BusinessEntityName,
                        Inactive = m != null ? m.Inactive : false,
                        CompanyPhone = be.BusinessEntityPhone,
                        Email = be.BusinessEntityEmail,
                        BillAddress = be.Addresses.FirstOrDefault() != null ? be.Addresses.FirstOrDefault().Addr1 : null,
                        BillCity = be.Addresses.FirstOrDefault() != null ? be.Addresses.FirstOrDefault().City : null,
                        BillState = be.Addresses.FirstOrDefault() != null ? be.Addresses.FirstOrDefault().State : null,
                        BillZip = be.Addresses.FirstOrDefault() != null ? be.Addresses.FirstOrDefault().Zip : null,
                        LastPayDate = m != null ? m.LastPayDate : null,
                        RenewalDate = m != null ? m.RenewalDate : null,
                        Term = m != null ? m.Term : null,
                        Div = m != null ? m.Div : null,
                        Discipline = m != null ? m.Discipline : null,
                        Note = m != null ? m.Note : null,
                        MinorityStatus = m != null ? m.MinorityStatus : null,
                        MemberType = m != null ? m.MemberType : null,
                        AcceptedTerms = m != null ? m.AcceptedTerms : false,
                        AcceptedTermsDt = m != null ? m.AcceptedTermsDt : null,
                        DailyEmail = m != null ? m.DailyEmail : null,
                        Html = m != null ? m.Html : null,
                        Overdue = m != null ? m.Overdue : null,
                        Cod = m != null ? m.Cod : null,
                        PaperlessBilling = m != null ? m.PaperlessBilling : null,
                        MemberCost = m != null ? m.MemberCost : null,
                        MagCost = m != null ? m.MagCost : null,
                        ArchPkgCost = m != null ? m.ArchPkgCost : null,
                        AddPkgCost = m != null ? m.AddPkgCost : null,
                        ResourceDate = m != null ? m.ResourceDate : null,
                        ResourceCost = m != null ? m.ResourceCost : null,
                        WebAdDate = m != null ? m.WebAdDate : null,
                        WebAdCost = m != null ? m.WebAdCost : null,
                        Phl = m != null ? m.Phl : null,
                        NameField = m != null ? m.NameField : null,
                        FavExp = m != null ? m.FavExp : null,
                        Grace = m != null ? m.Grace : null,
                        ConId = m != null ? m.ConId : null,
                        Gcservices = m != null ? m.Gcservices : null,
                        ResourceStandard = m != null ? m.ResourceStandard : null,
                        ResourceColor = m != null ? m.ResourceColor : null,
                        ResourceLogo = m != null ? m.ResourceLogo : null,
                        ResourceAdd = m != null ? m.ResourceAdd : null,
                        Dba = m != null ? m.Dba : null,
                        Dba2 = m != null ? m.Dba2 : null,
                        Fka = m != null ? m.Fka : null,
                        Suspended = m != null ? m.Suspended : null,
                        SuspendedDt = m != null ? m.SuspendedDt : null,
                        Fax = m != null ? m.Fax : null,
                        MailAddress = m != null ? m.MailAddress : null,
                        MailCity = m != null ? m.MailCity : null,
                        MailState = m != null ? m.MailState : null,
                        MailZip = m != null ? m.MailZip : null,
                        OverdueAmt = m != null ? m.OverdueAmt : null,
                        OverdueDt = m != null ? m.OverdueDt : null,
                        CalSort = m != null ? m.CalSort : null,
                        Pdfpkg = m != null ? m.Pdfpkg : null,
                        ArchPkg = m != null ? m.ArchPkg : null,
                        AddPkg = m != null ? m.AddPkg : null,
                        Bend = m != null ? m.Bend : null,
                        Credits = m != null ? m.Credits : null,
                        FreelanceEstimator = m != null ? m.FreelanceEstimator : null,
                        HowdUhearAboutUs = m != null ? m.HowdUhearAboutUs : null,
                        IsAutoRenew = m != null ? m.IsAutoRenew : null,
                        Logo = m != null ? m.Logo : null,
                        CheckDirectory = m != null ? m.CheckDirectory : false,
                        MemID = m != null ? (m.MemId != null ? (int)m.MemId : 0) : 0,
                        InvoiceId = m != null ? (m.InvoiceId != null ? (int)m.InvoiceId : 0) : 0,
                        Discount = m != null ? m.Discount : null,
                        PayModeRef = m != null ? m.PayModeRef : null,
                        CreatedBy = m != null ? m.CreatedBy : null,
                        IsDelete = m != null ? m.IsDelete : false,
                        AddGraceDate = m != null ? m.AddGraceDate : null,
                        IsMember = be.IsMember,
                        IsArchitect = be.IsArchitect,
                        IsContractor = be.IsContractor,
                        ActualRenewalDate = m != null ? m.ActualRenewalDate : null
                    })
                .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw; // Re-throw the exception to maintain the original behavior
            }

        }

        //async method
        public async Task<List<TblMember>> GetEntitiesAsync()
        {
            var result = _dbContext.BusinessEntities
                .Include(be => be.Members)
                .Include(be => be.Addresses)
                .SelectMany(be => be.Members.Select(m => new TblMember
                {
                    Id = be.BusinessEntityId,
                    InsertDate = m.InsertDate,
                    Company = be.BusinessEntityName,
                    Inactive = m.Inactive,
                    CompanyPhone = be.BusinessEntityPhone,
                    BillAddress = be.Addresses.FirstOrDefault().Addr1,
                    BillCity = be.Addresses.FirstOrDefault().City,
                    BillState = be.Addresses.FirstOrDefault().State,
                    BillZip = be.Addresses.FirstOrDefault().Zip,
                    LastPayDate = m.LastPayDate.ToString(),
                    RenewalDate = m.RenewalDate,
                    Term = m.Term,
                    Div = m.Div,
                    Discipline = m.Discipline,
                    Note = m.Note,
                    MinorityStatus = m.MinorityStatus,
                    MemberType = m.MemberType,
                    AcceptedTerms = m.AcceptedTerms,
                    AcceptedTermsDt = m.AcceptedTermsDt,
                    DailyEmail = m.DailyEmail,
                    Html = m.Html,
                    Overdue = m.Overdue,
                    Cod = m.Cod,
                    PaperlessBilling = m.PaperlessBilling,
                    MemberCost = m.MemberCost,
                    MagCost = m.MagCost,
                    ArchPkgCost = m.ArchPkgCost,
                    AddPkgCost = m.AddPkgCost,
                    ResourceDate = m.ResourceDate,
                    ResourceCost = m.ResourceCost,
                    WebAdDate = m.WebAdDate,
                    WebAdCost = m.WebAdCost,
                    Phl = m.Phl,
                    Email = m.Email,
                    NameField = m.NameField,
                    FavExp = m.FavExp,
                    Grace = m.Grace,
                    ConId = m.ConId,
                    Gcservices = m.Gcservices,
                    ResourceStandard = m.ResourceStandard,
                    ResourceColor = m.ResourceColor,
                    ResourceLogo = m.ResourceLogo,
                    ResourceAdd = m.ResourceAdd,
                    Dba = m.Dba,
                    Dba2 = m.Dba2,
                    Fka = m.Fka,
                    Suspended = m.Suspended,
                    SuspendedDt = m.SuspendedDt,
                    Fax = m.Fax,
                    MailAddress = m.MailAddress,
                    MailCity = m.MailCity,
                    MailState = m.MailState,
                    MailZip = m.MailZip,
                    OverdueAmt = m.OverdueAmt,
                    OverdueDt = m.OverdueDt,
                    CalSort = m.CalSort,
                    Pdfpkg = m.Pdfpkg,
                    ArchPkg = m.ArchPkg,
                    AddPkg = m.AddPkg,
                    Bend = m.Bend,
                    Credits = m.Credits,
                    FreelanceEstimator = m.FreelanceEstimator,
                    HowdUhearAboutUs = m.HowdUhearAboutUs,
                    IsAutoRenew = m.IsAutoRenew,
                    Logo = m.Logo,
                    CheckDirectory = m.CheckDirectory,
                    MemID = (int)m.MemId,
                    InvoiceId = (int)m.InvoiceId,
                    Discount = m.Discount,
                    PayModeRef = m.PayModeRef,
                    CreatedBy = m.CreatedBy,
                    IsDelete = m.IsDelete,
                    AddGraceDate = m.AddGraceDate,
                    IsMember = be.IsMember,
                    IsArchitect = be.IsArchitect,
                    IsContractor = be.IsContractor,
                    ActualRenewalDate = m.ActualRenewalDate,
                }))
                .ToList();

            return result;
        }
        public void UpdateEntity(TblMember tblMember)
        {
            try
            {
                if (tblMember != null)
                {
                    var businessEntityobj = _dbContext.BusinessEntities.FirstOrDefault(m => m.BusinessEntityId == tblMember.Id);
                    if (businessEntityobj != null)
                    {
                        businessEntityobj.BusinessEntityName = tblMember.Company;
                        businessEntityobj.BusinessEntityEmail = tblMember.Email;
                        businessEntityobj.BusinessEntityPhone = tblMember.CompanyPhone;
                        businessEntityobj.IsMember = tblMember.IsMember ?? false;
                        businessEntityobj.IsArchitect = tblMember.IsArchitect ?? false;
                        businessEntityobj.IsContractor = tblMember.IsContractor ?? false;
                    }

                    var memberobj = _dbContext.Members.SingleOrDefault(m => m.BusinessEntityId == tblMember.Id);
                    if (memberobj != null)
                    {
                        memberobj.Inactive = tblMember.Inactive;
                        memberobj.InsertDate = (DateTime)tblMember.InsertDate;
                        memberobj.LastPayDate = tblMember.LastPayDate;
                        memberobj.RenewalDate = tblMember.RenewalDate;
                        memberobj.Term = tblMember.Term;
                        memberobj.Div = tblMember.Div;
                        memberobj.Discipline = tblMember.Discipline;
                        memberobj.Note = tblMember.Note;
                        memberobj.MinorityStatus = tblMember.MinorityStatus;
                        memberobj.MemberType = tblMember.MemberType;
                        memberobj.AcceptedTerms = tblMember.AcceptedTerms;
                        memberobj.AcceptedTermsDt = tblMember.AcceptedTermsDt;
                        memberobj.DailyEmail = tblMember.DailyEmail;
                        memberobj.Html = tblMember.Html;
                        memberobj.Overdue = tblMember.Overdue;
                        memberobj.Cod = tblMember.Cod;
                        memberobj.PaperlessBilling = tblMember.PaperlessBilling;
                        memberobj.MemberCost = tblMember.MemberCost;
                        memberobj.MagCost = tblMember.MagCost;
                        memberobj.ArchPkgCost = tblMember.ArchPkgCost;
                        memberobj.AddPkgCost = tblMember.AddPkgCost;
                        memberobj.ResourceDate = tblMember.ResourceDate;
                        memberobj.ResourceCost = tblMember.ResourceCost;
                        memberobj.WebAdDate = tblMember.WebAdDate;
                        memberobj.WebAdCost = tblMember.WebAdCost;
                        memberobj.Phl = tblMember.Phl;
                        memberobj.Email = tblMember.Email;
                        memberobj.NameField = tblMember.NameField;
                        memberobj.FavExp = tblMember.FavExp;
                        memberobj.Grace = tblMember.Grace;
                        memberobj.ConId = tblMember.ConId;
                        memberobj.Gcservices = tblMember.Gcservices;
                        memberobj.ResourceStandard = tblMember.ResourceStandard;
                        memberobj.ResourceColor = tblMember.ResourceColor;
                        memberobj.ResourceLogo = tblMember.ResourceLogo;
                        memberobj.ResourceAdd = tblMember.ResourceAdd;
                        memberobj.Dba = tblMember.Dba;
                        memberobj.Dba2 = tblMember.Dba2;
                        memberobj.Fka = tblMember.Fka;
                        memberobj.Suspended = tblMember.Suspended;
                        memberobj.SuspendedDt = tblMember.SuspendedDt;
                        memberobj.Fax = tblMember.Fax;
                        memberobj.MailAddress = tblMember.MailAddress;
                        memberobj.MailCity = tblMember.MailCity;
                        memberobj.MailState = tblMember.MailState;
                        memberobj.MailZip = tblMember.MailZip;
                        memberobj.OverdueAmt = tblMember.OverdueAmt;
                        memberobj.OverdueDt = tblMember.OverdueDt;
                        memberobj.CalSort = tblMember.CalSort;
                        memberobj.Pdfpkg = tblMember.Pdfpkg;
                        memberobj.ArchPkg = tblMember.ArchPkg;
                        memberobj.AddPkg = tblMember.AddPkg;
                        memberobj.Bend = tblMember.Bend;
                        memberobj.Credits = tblMember.Credits;
                        memberobj.FreelanceEstimator = tblMember.FreelanceEstimator;
                        memberobj.HowdUhearAboutUs = tblMember.HowdUhearAboutUs;
                        memberobj.IsAutoRenew = tblMember.IsAutoRenew;
                        memberobj.CompanyPhone = tblMember.CompanyPhone;
                        memberobj.Logo = tblMember.Logo;
                        memberobj.CheckDirectory = tblMember.CheckDirectory;
                        memberobj.MemId = tblMember.MemID;
                        memberobj.InvoiceId = tblMember.InvoiceId;
                        memberobj.Discount = tblMember.Discount;
                        memberobj.PayModeRef = tblMember.PayModeRef;
                        memberobj.CreatedBy = tblMember.CreatedBy;
                        memberobj.IsDelete = tblMember.IsDelete;
                        memberobj.AddGraceDate = tblMember.AddGraceDate;
                        memberobj.ActualRenewalDate = tblMember.ActualRenewalDate;

                    }

                    var addressobj = _dbContext.Addresses.SingleOrDefault(m => m.BusinessEntityId == tblMember.Id);
                    if (addressobj != null)
                    {
                        addressobj.Addr1 = tblMember.BillAddress;
                        addressobj.City = tblMember.BillCity;
                        addressobj.State = tblMember.BillState;
                        addressobj.Zip = tblMember.BillZip;

                    };

                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //async Method
        public async Task UpdateEntityAsync(TblMember tblMember)
        {
            try
            {
                if (tblMember != null)
                {
                    var businessEntityobj = await _dbContext.BusinessEntities.FirstOrDefaultAsync(m => m.BusinessEntityId == tblMember.Id);
                    if (businessEntityobj != null)
                    {
                        businessEntityobj.BusinessEntityName = tblMember.Company;
                        businessEntityobj.BusinessEntityEmail = tblMember.Email;
                        businessEntityobj.BusinessEntityPhone = tblMember.CompanyPhone;
                        businessEntityobj.IsMember = tblMember.IsMember ?? false;
                        businessEntityobj.IsArchitect = tblMember.IsArchitect ?? false;
                        businessEntityobj.IsContractor = tblMember.IsContractor ?? false;
                    }

                    var memberobj = await _dbContext.Members.SingleOrDefaultAsync(m => m.BusinessEntityId == tblMember.Id);
                    if (memberobj != null)
                    {
                        memberobj.Inactive = tblMember.Inactive;
                        memberobj.InsertDate = (DateTime)tblMember.InsertDate;
                        memberobj.LastPayDate = tblMember.LastPayDate;
                        memberobj.RenewalDate = tblMember.RenewalDate;
                        memberobj.Term = tblMember.Term;
                        memberobj.Div = tblMember.Div;
                        memberobj.Discipline = tblMember.Discipline;
                        memberobj.Note = tblMember.Note;
                        memberobj.MinorityStatus = tblMember.MinorityStatus;
                        memberobj.MemberType = tblMember.MemberType;
                        memberobj.AcceptedTerms = tblMember.AcceptedTerms;
                        memberobj.AcceptedTermsDt = tblMember.AcceptedTermsDt;
                        memberobj.DailyEmail = tblMember.DailyEmail;
                        memberobj.Html = tblMember.Html;
                        memberobj.Overdue = tblMember.Overdue;
                        memberobj.Cod = tblMember.Cod;
                        memberobj.PaperlessBilling = tblMember.PaperlessBilling;
                        memberobj.MemberCost = tblMember.MemberCost;
                        memberobj.MagCost = tblMember.MagCost;
                        memberobj.ArchPkgCost = tblMember.ArchPkgCost;
                        memberobj.AddPkgCost = tblMember.AddPkgCost;
                        memberobj.ResourceDate = tblMember.ResourceDate;
                        memberobj.ResourceCost = tblMember.ResourceCost;
                        memberobj.WebAdDate = tblMember.WebAdDate;
                        memberobj.WebAdCost = tblMember.WebAdCost;
                        memberobj.Phl = tblMember.Phl;
                        memberobj.Email = tblMember.Email;
                        memberobj.NameField = tblMember.NameField;
                        memberobj.FavExp = tblMember.FavExp;
                        memberobj.Grace = tblMember.Grace;
                        memberobj.ConId = tblMember.ConId;
                        memberobj.Gcservices = tblMember.Gcservices;
                        memberobj.ResourceStandard = tblMember.ResourceStandard;
                        memberobj.ResourceColor = tblMember.ResourceColor;
                        memberobj.ResourceLogo = tblMember.ResourceLogo;
                        memberobj.ResourceAdd = tblMember.ResourceAdd;
                        memberobj.Dba = tblMember.Dba;
                        memberobj.Dba2 = tblMember.Dba2;
                        memberobj.Fka = tblMember.Fka;
                        memberobj.Suspended = tblMember.Suspended;
                        memberobj.SuspendedDt = tblMember.SuspendedDt;
                        memberobj.Fax = tblMember.Fax;
                        memberobj.MailAddress = tblMember.MailAddress;
                        memberobj.MailCity = tblMember.MailCity;
                        memberobj.MailState = tblMember.MailState;
                        memberobj.MailZip = tblMember.MailZip;
                        memberobj.OverdueAmt = tblMember.OverdueAmt;
                        memberobj.OverdueDt = tblMember.OverdueDt;
                        memberobj.CalSort = tblMember.CalSort;
                        memberobj.Pdfpkg = tblMember.Pdfpkg;
                        memberobj.ArchPkg = tblMember.ArchPkg;
                        memberobj.AddPkg = tblMember.AddPkg;
                        memberobj.Bend = tblMember.Bend;
                        memberobj.Credits = tblMember.Credits;
                        memberobj.FreelanceEstimator = tblMember.FreelanceEstimator;
                        memberobj.HowdUhearAboutUs = tblMember.HowdUhearAboutUs;
                        memberobj.IsAutoRenew = tblMember.IsAutoRenew;
                        memberobj.CompanyPhone = tblMember.CompanyPhone;
                        memberobj.Logo = tblMember.Logo;
                        memberobj.CheckDirectory = tblMember.CheckDirectory;
                        memberobj.MemId = tblMember.MemID;
                        memberobj.InvoiceId = tblMember.InvoiceId;
                        memberobj.Discount = tblMember.Discount;
                        memberobj.PayModeRef = tblMember.PayModeRef;
                        memberobj.CreatedBy = tblMember.CreatedBy;
                        memberobj.IsDelete = tblMember.IsDelete;
                        memberobj.AddGraceDate = tblMember.AddGraceDate;
                        memberobj.ActualRenewalDate = tblMember.ActualRenewalDate;

                    }

                    var addressobj = await _dbContext.Addresses.SingleOrDefaultAsync(m => m.BusinessEntityId == tblMember.Id);
                    if (addressobj != null)
                    {
                        addressobj.Addr1 = tblMember.BillAddress;
                        addressobj.City = tblMember.BillCity;
                        addressobj.State = tblMember.BillState;
                        addressobj.Zip = tblMember.BillZip;
                    };

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void UpdateContactUserId(string email, IdentityUser user)
        {
            if (user.Id != null)
            {
                var contact = _dbContext.TblContacts.FirstOrDefault(m => m.Email == email);
                if (Guid.TryParse(user.Id, out Guid userIdGuid))
                {
                    contact.UserId = userIdGuid;
                    _dbContext.Entry(contact).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                }
            }
        }
    }
}
