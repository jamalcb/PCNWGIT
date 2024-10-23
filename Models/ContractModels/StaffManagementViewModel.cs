namespace PCNW.Models.ContractModels
{
    public class StaffManagementViewModel
    {
        public IEnumerable<MemberShipRegistration> Members { get; set; }
        public IEnumerable<MemberShipRegistration> Contractors { get; set; }
        //public IEnumerable<MemberShipRegistration> Architects { get; set; }
        public IEnumerable<EntityTypeViewModel> Entities { get; set; }
        public string ReturnUrl { get; set; }
    }
}

