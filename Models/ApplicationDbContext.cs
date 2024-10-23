using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCNW.Data;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using PCNW.ViewModel;

namespace PCNW.Models
{
    public partial class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(120);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=192.168.1.99;Database=PCNWProjectDB;User ID=sa;Password=sa123;Integrated Security=False;MultipleActiveResultSets=True;")
        //                  .EnableSensitiveDataLogging()
        //                  .LogTo(Console.WriteLine, LogLevel.Information);
        //}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TblProjectCode>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<CopyCenterMemberInfo>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<MemberDashboardViewModel>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<MemberSummaryViewModel>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<MemberTypeViewModel>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<MemberProjects>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<TrackedProjects>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<FindProjectView>(builder =>
            {
                builder.HasNoKey();
            });
            builder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");
                entity.Property(e => e.Addr1)
                    .HasMaxLength(255)
                    .HasDefaultValue("");
                entity.Property(e => e.AddressName)
                    .HasMaxLength(20)
                    .HasDefaultValue("");
                entity.Property(e => e.BusinessEntityId).HasColumnName("BusinessEntityID");
                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasDefaultValue("");
                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .HasDefaultValue("");
                entity.Property(e => e.Zip)
                    .HasMaxLength(50)
                    .HasDefaultValue("");

                entity.HasOne(d => d.BusinessEntity).WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.BusinessEntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_BusinessEntity");
            });

            builder.Entity<BusinessEntity>(entity =>
            {
                entity.ToTable("BusinessEntity");

                entity.Property(e => e.BusinessEntityId).HasColumnName("BusinessEntityID");
                entity.Property(e => e.BusinessEntityEmail)
                    .HasMaxLength(50);
                entity.Property(e => e.BusinessEntityName)
                    .HasMaxLength(50);
                entity.Property(e => e.BusinessEntityPhone)
                    .HasMaxLength(50);
            });



            builder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");
                entity.Property(e => e.AcceptedTermsDt).HasColumnType("datetime");
                entity.Property(e => e.ActualRenewalDate).HasColumnType("datetime");
                entity.Property(e => e.AddGraceDate).HasColumnType("datetime");
                entity.Property(e => e.AddPkgCost)
                    .HasColumnType("money")
                    .HasColumnName("AddPkg_Cost");
                entity.Property(e => e.ArchPkgCost)
                    .HasColumnType("money")
                    .HasColumnName("ArchPkg_Cost");
                entity.Property(e => e.BusinessEntityId).HasColumnName("BusinessEntityID");
                entity.Property(e => e.CalSort).HasMaxLength(50);
                entity.Property(e => e.Cod).HasColumnName("COD");
                entity.Property(e => e.CompanyPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ConId).HasColumnName("Con_ID");
                entity.Property(e => e.CreatedBy).HasMaxLength(200);
                entity.Property(e => e.Credits).HasColumnName("credits");
                entity.Property(e => e.DailyEmail)
                    .HasMaxLength(255)
                    .HasColumnName("Daily_Email");
                entity.Property(e => e.Dba)
                    .HasMaxLength(100)
                    .HasColumnName("DBA");
                entity.Property(e => e.Dba2)
                    .HasMaxLength(100)
                    .HasColumnName("DBA2");
                entity.Property(e => e.Discipline).HasMaxLength(255);
                entity.Property(e => e.Discount).HasMaxLength(500);
                entity.Property(e => e.Div).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.FavExp).HasColumnType("datetime");
                entity.Property(e => e.Fax).HasMaxLength(50);
                entity.Property(e => e.Fka)
                    .HasMaxLength(100)
                    .HasColumnName("FKA");
                entity.Property(e => e.Gcservices).HasColumnName("GCservices");
                entity.Property(e => e.HowdUhearAboutUs)
                    .HasMaxLength(255)
                    .HasColumnName("HowdUHearAboutUs");
                entity.Property(e => e.Html).HasColumnName("HTML");
                entity.Property(e => e.InsertDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.LastPayDate);
                entity.Property(e => e.MagCost)
                    .HasColumnType("money")
                    .HasColumnName("Mag_Cost");
                entity.Property(e => e.MailAddress).HasMaxLength(50);
                entity.Property(e => e.MailCity).HasMaxLength(50);
                entity.Property(e => e.MailState).HasMaxLength(2);
                entity.Property(e => e.MailZip).HasMaxLength(50);
                entity.Property(e => e.MemId).HasColumnName("MemID");
                entity.Property(e => e.MemberCost)
                    .HasColumnType("money")
                    .HasColumnName("Member_Cost");
                entity.Property(e => e.MinorityStatus).HasMaxLength(50);
                entity.Property(e => e.NameField).HasColumnType("decimal(18, 0)");
                entity.Property(e => e.Note).HasColumnType("ntext");
                entity.Property(e => e.OverdueAmt).HasColumnType("money");
                entity.Property(e => e.OverdueDt).HasColumnType("datetime");
                entity.Property(e => e.PaperlessBilling)
                    .HasMaxLength(50)
                    .HasColumnName("Paperless_billing");
                entity.Property(e => e.PayModeRef)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Pdfpkg).HasColumnName("PDFPkg");
                entity.Property(e => e.Phl).HasColumnName("PHL");
                entity.Property(e => e.RenewalDate).HasColumnType("datetime");
                entity.Property(e => e.ResourceAdd)
                    .HasMaxLength(50)
                    .HasColumnName("Resource_Add");
                entity.Property(e => e.ResourceColor)
                    .HasMaxLength(50)
                    .HasColumnName("Resource_Color");
                entity.Property(e => e.ResourceCost)
                    .HasColumnType("money")
                    .HasColumnName("Resource_cost");
                entity.Property(e => e.ResourceDate)
                    .HasMaxLength(50)
                    .HasColumnName("Resource_date");
                entity.Property(e => e.ResourceLogo)
                    .HasMaxLength(50)
                    .HasColumnName("Resource_Logo");
                entity.Property(e => e.ResourceStandard)
                    .HasMaxLength(50)
                    .HasColumnName("Resource_Standard");
                entity.Property(e => e.SuspendedDt).HasMaxLength(50);
                entity.Property(e => e.Term).HasMaxLength(50);
                entity.Property(e => e.TmStamp)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("tmStamp");
                entity.Property(e => e.WebAdCost)
                    .HasColumnType("money")
                    .HasColumnName("WebAd_cost");
                entity.Property(e => e.WebAdDate)
                    .HasMaxLength(50)
                    .HasColumnName("WebAd_date");

                entity.HasOne(d => d.BusinessEntity).WithMany(p => p.Members)
                    .HasForeignKey(d => d.BusinessEntityId)
                    .HasConstraintName("FK_Member_BusinessEntity");
            });
            base.OnModelCreating(builder);
        }
        public DbSet<CopyCenterMemberInfo> CopyCenterMemberInfo { get; set; }
        public DbSet<TblProjectCode> TblProjectCode { get; set; }
        // public DbSet<MemberDivision> MemberDivision { get; set; }
        public DbSet<TblProject> tblProject { get; set; }
        public DbSet<TblEntity> TblEntity { get; set; }
        public virtual DbSet<tblTracking> TblTracking { get; set; }
        public DbSet<tblMemberShipSubPlans> tblMemberShipSubPlans { get; set; }
        public DbSet<tblMemberShipPlans> tblMemberShipPlans { get; set; }
        public DbSet<MemberProjects> MemberProjectsView { get; set; }
        public DbSet<TrackedProjects> TrackedProjectsView { get; set; }
        public DbSet<Members> MembersView { get; set; }
        public DbSet<Contractors> ContractorsView { get; set; }
        public DbSet<Entities> EntitiesView { get; set; }
        public DbSet<FindProjectView> FindProjectView { get; set; }
        public virtual DbSet<TblAddenda> TblAddenda { get; set; }
        public virtual DbSet<TblArchOwner> TblArchOwners { get; set; }
        public virtual DbSet<BusinessEntity> BusinessEntities { get; set; }
        public virtual DbSet<TblContact> TblContacts { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<TblAutoSearch> TblAutoSearch { get; set; }
        public virtual DbSet<TblBidStatus> TblBidStatus { get; set; }
        public virtual DbSet<TblBlockedIp> TblBlockedIps { get; set; }
        public virtual DbSet<TblCalendarNotice> TblCalendarNotice { get; set; }
        public virtual DbSet<TblCityCounty> TblCityCounty { get; set; }
        public virtual DbSet<TblCityLatLong> TblCityLatLong { get; set; }
        public virtual DbSet<TblContractor> TblContractors { get; set; }
        public virtual DbSet<TblCounty> TblCounty { get; set; }
        public virtual DbSet<TblDailyInfoHoliday> TblDailyInfoHoliday { get; set; }
        public virtual DbSet<TblIprelation> TblIprelations { get; set; }
        public virtual DbSet<TblMemNote> TblMemNotes { get; set; }
        public DbSet<TblMemberDiv> TblMemberDivs { get; set; }
        public virtual DbSet<TblMemberTypeCounty> TblMemberTypeCounty { get; set; }
        public DbSet<TblProjCounty> TblProjCounty { get; set; }
        public virtual DbSet<TblProjOrder> TblProjOrder { get; set; }
        public virtual DbSet<TblProjOrderDetail> TblProjOrderDetail { get; set; }
        public virtual DbSet<TblProjType> TblProjType { get; set; }
        public virtual DbSet<TblProjSubType> TblProjSubType { get; set; }
        public virtual DbSet<TblState> TblState { get; set; }
        public DbSet<tblSubAddenda> TblSubAddenda { get; set; }
        public virtual DbSet<TblWebDiv> TblWebDivs { get; set; }
        public DbSet<PCNW.Models.ResponseContracts.TblProjectPreview> TblProjectPreview { get; set; }
        public DbSet<StaffDashboardViewModel> StaffDashboardViewModel { get; set; }
        public DbSet<MemberDashboardViewModel> MemberDashboardViewModel { get; set; }
        public DbSet<MemberSummaryViewModel> MemberSummaryViewModel { get; set; }
        public DbSet<MemberTypeViewModel> MemberTypeViewModel { get; set; }
        public DbSet<TblEntityType> TblEntityType { get; set; }
        public DbSet<TblPHLType> TblPHLType { get; set; }
        public DbSet<TblProjNotification> TblProjNotification { get; set; }
        public DbSet<TblMemberSignUp> TblMemberSignUp { get; set; }
        public DbSet<TblPrintOrder> TblPrintOrder { get; set; }
        public DbSet<TblPHLUpdate> TblPHLUpdate { get; set; }
        public DbSet<TblMembershipExpire> TblMembershipExpire { get; set; }
        public DbSet<TblLogoff> TblLogoff { get; set; }
        public DbSet<TblFAQ> TblFAQ { get; set; }
        public DbSet<TblFileStorage> TblFileStorage { get; set; }
        public DbSet<tblCopyCenterFile> tblCopyCenterFile { get; set; }
        public DbSet<tblPaymentCardDetail> tblPaymentCardDetail { get; set; }
        public DbSet<tblProjOrdChrgsDetails> tblProjOrdChrgsDetails { get; set; }
        public DbSet<tblDiscount> tblDiscount { get; set; }
        public DbSet<tblIncompleteSignUp> tblIncompleteSignUp { get; set; }
        public DbSet<TblLogActivity> TblLogActivity { get; set; }
        public DbSet<TblCareerPostings> TblCareerPostings { get; set; }
        public DbSet<TblMemberSubscribe> TblMemberSubscribe { get; set; }
        public DbSet<TblSpecialMsg> TblSpecialMsg { get; set; }
        public DbSet<TblDailyMailer> TblDailyMailer { get; set; }
        public DbSet<TblLocList> TblLocList { get; set; }
        public DbSet<TblDirectoryCheck> TblDirectoryCheck { get; set; }
        public DbSet<tblLicense> tblLicense { get; set; }
        public DbSet<tblBidDateTime> tblBidDateTime { get; set; }
        public DbSet<tblFreeTab> tblFreeTab { get; set; }
        public DbSet<tblUsedProjNuber> tblUsedProjNuber { get; set; }
        public DbSet<tblPreBidInfo> tblPreBidInfo { get; set; }
        public DbSet<tblEstCostDetails> tblEstCostDetails { get; set; }
        public DbSet<tblDeliveryMaster> tblDeliveryMaster { get; set; }
        public DbSet<tblDeliveryOption> tblDeliveryOption { get; set; }
        public DbSet<tblBidOption> tblBidOption { get; set; }
        public DbSet<tblPhlInfo> tblPhlInfo { get; set; }
    }
}
