using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using System.Data;
using System.Dynamic;

namespace PCNW.Data.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<StaffRepository> _logger;
        private readonly string _connectionString;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEntityRepository _entityRepository;

        public StaffRepository(ApplicationDbContext dbContext, ILogger<StaffRepository> logger, UserManager<IdentityUser> userManager, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            _userManager = userManager;
            _entityRepository = entityRepository;
        }
        /// <summary>
        /// Get Data at staff Copy center
        /// </summary>
        /// <param name="returnurl"></param>
        /// <returns></returns>
        public async Task<List<OrderTables>> GetDashboardProjectsAsync(string returnurl)
        {
            List<OrderTables> response = new List<OrderTables>();
            List<TblProjOrder> result = await _dbContext.TblProjOrder.OrderByDescending(t => t.OrderId).Take(100).ToListAsync();
            List<TblProjOrderDetail> details = new();
            foreach (var x in result)
            {
                OrderTables order = new();
                order.OrderId = x.OrderId;
                order.Viewed = x.Viewed;
                order.ProjId = x.ProjId;
                order.PO = x.Po;
                order.Sz1Qty = x.Sz1Qty;
                order.Sz2Qty = x.Sz2Qty;
                order.Sz3Qty = x.Sz3Qty;
                order.Sz4Qty = x.Sz4Qty;
                order.Sz5Qty = x.Sz5Qty;
                order.Sz6Qty = x.Sz6Qty;
                order.Prints = x.Prints;
                order.Instructions = x.Instructions;
                order.Company = x.Company;
                order.Name = x.Name;
                order.Addr = x.Addr;
                order.CSZ = x.CSZ;
                order.Phone = x.Phone;
                order.Email = x.Email;
                order.DeliveryDt = x.DeliveryDt;
                //order.HowShip = x.HowShip;
                if (int.TryParse(x.HowShip, out int howShipNumeric))
                {
                    // x.HowShip is a numeric value, you can use howShipNumeric as an int
                    //order.HowShip = howShipNumeric.ToString();
                    var list = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptId == howShipNumeric);
                    if (list != null)
                    {
                        int id = list.DelivId;
                        var tbldeliverymaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivId == id);
                        order.HowShip = tbldeliverymaster.DelivName;
                    }
                }
                else
                {
                    // x.HowShip is a string value, you can use x.HowShip as a string
                    order.HowShip = x.HowShip;
                }
                order.OrderDt = x.OrderDt;
                order.DoneDt = x.DoneDt;
                order.ShipDt = x.ShipDt;
                order.EmailContent = returnurl + x.OrderId;
                response.Add(order);
            }
            return response;
        }
        /// <summary>
        /// To update donedt (Complete/Send Notice or mail) from staff copy center
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateDoneDt(string Id, OrderTables data)
        {
            int OrderId = Convert.ToInt32(Id);
            TblProjOrder projOrder = await _dbContext.TblProjOrder.SingleAsync(s => s.OrderId == OrderId);
            projOrder.DoneDt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            data.Email = projOrder.Email;
            data.OrderId = projOrder.OrderId;
            return projOrder;
        }
        /// <summary>
        /// For update copycenter order status(Ready for Pickup/Delivery) in staffAccount/CopyCenter
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateShipDt(string Id, OrderTables model)
        {
            int OrderId = Convert.ToInt32(Id);
            TblProjOrder projOrder = await _dbContext.TblProjOrder.SingleAsync(s => s.OrderId == OrderId);
            projOrder.ShipDt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            model.Email = projOrder.Email;
            return projOrder;
        }
        /// <summary>
        /// To update view status in staffAccount/CopyCenter
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateViewed(string Id)
        {
            TblProjOrder projOrder = new();
            int OrderId = Convert.ToInt32(Id);
            try
            {
                projOrder = await _dbContext.TblProjOrder.SingleAsync(s => s.OrderId == OrderId);
                projOrder.Viewed = true;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception Ex)
            {
                projOrder = null;
            }
            return projOrder;
        }
        /// <summary>
        /// To view copy center docs from staffAccount/CopyCenter and member/CopyCenter
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<string>> ViewDoc(string Id)
        {
            List<string> lst = new List<string>();
            int OrderId = Convert.ToInt32(Id);
            try
            {
                lst = (from f in _dbContext.TblProjOrderDetail
                       where f.OrderId == OrderId
                       select f.FileName).ToList();
                lst.Add(Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return lst;
        }
        /// <summary>
        /// Check/Uncheck plan box in project dashboard(StaffAccount/Dashboard) pending projects tab
        /// </summary>
        /// <param name="SpcChange"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> ChangeSpecsOnPlans(bool SpcChange, string Id)
        {
            int ProjId = Convert.ToInt32(Id);
            TblProject proj = await _dbContext.tblProject.SingleAsync(s => s.ProjId == ProjId);
            proj.SpecsOnPlans = SpcChange;
            await _dbContext.SaveChangesAsync();
            return ProjId;
        }
        /// <summary>
        /// Check/Uncheck Spec box in project dashboard(StaffAccount/Dashboard) pending projects tab
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> ChangeSpc(bool Change, string Id)
        {
            int ProjId = Convert.ToInt32(Id);
            TblProject proj = await _dbContext.tblProject.SingleAsync(s => s.ProjId == ProjId);
            proj.Publish = Change;
            await _dbContext.SaveChangesAsync();
            return ProjId;
        }
        /// <summary>
        /// To publish project from project dashboard(StaffAccount/Dashboard) pending/active projects tab
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<dynamic> ChangePublish(bool Change, string Id)
        {
            int ProjId = Convert.ToInt32(Id);
            TblProject proj = await _dbContext.tblProject.SingleAsync(s => s.ProjId == ProjId);
            proj.Publish = Change;
            proj.ImportDt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return ProjId;
        }
        /// <summary>
        /// To save priint order form details from StaffAccount/CopyCenter, Home/CopyCenter, Member/CopyCenter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveCopyCenterInfoAsync(OrderTables model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            TblProjOrder tblProjectOrder = new();

            int OrderId;
            try
            {
                tblProjectOrder.Company = model.Company;
                tblProjectOrder.Name = model.Name;
                tblProjectOrder.Addr = model.Addr;
                tblProjectOrder.CSZ = model.City + " " + model.State + " " + model.Zip;
                tblProjectOrder.Phone = model.Phone;
                tblProjectOrder.Email = model.Email;
                tblProjectOrder.ShipAmt = model.ShipAmt;
                tblProjectOrder.ProjId = model.ProjId;
                tblProjectOrder.Po = model.PO;
                tblProjectOrder.Prints = model.Prints;
                tblProjectOrder.DeliveryDt = model.DeliveryDt;
                tblProjectOrder.HowShip = model.HowShip;
                tblProjectOrder.NotifyAddress = model.NotifyAddress;
                tblProjectOrder.Instructions = model.Instructions;
                tblProjectOrder.Done = false;
                tblProjectOrder.Canceled = false;
                tblProjectOrder.Billed = false;
                tblProjectOrder.Shipped = false;
                tblProjectOrder.Viewed = false;
                tblProjectOrder.OrderDt = DateTime.Now;
                tblProjectOrder.Uid = model.UID;
                tblProjectOrder.NonMember = model.NonMember;
                tblProjectOrder.PaymentMode = model.PaymentMode;
                _dbContext.TblProjOrder.Add(tblProjectOrder);
                _dbContext.SaveChanges();
                model.OrderId = tblProjectOrder.OrderId;
                httpResponse.data = model;
                foreach (OrderDetails dt in model.GetTblProjs)
                {
                    TblProjOrderDetail detail = new();
                    detail.OrderId = model.OrderId;
                    detail.FileName = dt.FileName;
                    detail.Price = dt.Price;
                    detail.Pages = dt.Pages;
                    detail.Size = dt.Size;
                    detail.Copies = dt.Copies;
                    detail.PrintName = dt.PrintName;
                    _dbContext.TblProjOrderDetail.Add(detail);
                    _dbContext.SaveChanges();
                    //int id = model.OrderId;
                }
                tblProjectOrder = new();
                tblProjectOrder = await _dbContext.TblProjOrder.SingleOrDefaultAsync(x => x.OrderId == model.OrderId);

                if (tblProjectOrder.Email != null)
                {
                    List<int> Cons = await _dbContext.TblPrintOrder.Select(x => x.Id).ToListAsync();
                    List<string> EmailsTo = new();
                    foreach (int i in Cons)
                    {
                        TblPrintOrder tblPrintOrder = await _dbContext.TblPrintOrder.SingleOrDefaultAsync(x => x.Id == i);
                        string temp = tblPrintOrder.Email;
                        if (!string.IsNullOrEmpty(temp))
                        {
                            EmailsTo.Add(temp);
                        }
                    }
                    model.EmailsTo = EmailsTo;

                    httpResponse.data = model;
                    httpResponse.success = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TblProjectPreview>> GetProjectdetail(int id)
        {

            Dictionary<string, object> Params = new();
            Params.Add("@ProjId", id);
            string ProcName = "GetProjectDetail";
            List<TblProjectPreview> response = new();
            try
            {
                /*dynamic response =*/
                //var results = await GeneralMethods.DynamicListFromSqlAsync(_dbContext, ProcName, Params);
                //response = results.Tol.Select(x => new TblProjectPreview
                //{
                //	ProjId = x.ProjId,
                //	ProjNote = x.ProjNote,
                //}).ToList();
                var results = await _dbContext.tblProject.SingleOrDefaultAsync(m => m.ProjId == id);

                TblProjectPreview item = new();
                if (results != null)
                {
                    item.ProjId = results.ProjId;
                    item.ProjNote = results.ProjNote;
                    item.ProjTypeId = results.ProjTypeId;
                    item.LocAddr1 = results.LocAddr1;
                    item.BidDt = results.BidDt;
                    item.PreBidDt = results.PreBidDt;

                }
                response.Add(item);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// For delete project on staff's dashboard screen/page.(StaffAccount/Dashboard)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteProject(int Id)
        {
            int ProjIdDel = Id;
            TblProject Proj = await _dbContext.tblProject.Where(M => M.ProjId == ProjIdDel).SingleOrDefaultAsync();
            if (Proj != null)
            {
                List<tblUsedProjNuber> lstUsedProjNuber = await _dbContext.tblUsedProjNuber.Where(x => (x.ProjNumber == Proj.ProjNumber) && (x.IsUsed == false)).ToListAsync();
                if (lstUsedProjNuber == null || lstUsedProjNuber.Count == 0)
                {
                    tblUsedProjNuber tblUsedProjNuber = new tblUsedProjNuber();
                    tblUsedProjNuber.ProjNumber = Proj.ProjNumber;
                    tblUsedProjNuber.IsUsed = false;
                    await _dbContext.tblUsedProjNuber.AddAsync(tblUsedProjNuber);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    for (int i = 0; i < lstUsedProjNuber.Count; i++)
                    {
                        if (i == 0)
                        {
                            lstUsedProjNuber[i].IsUsed = false;
                            _dbContext.Entry(lstUsedProjNuber[i]).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            lstUsedProjNuber[i].IsUsed = true;
                            _dbContext.Entry(lstUsedProjNuber[i]).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            Proj.IsActive = false;
            Proj.ProjNumber = 0.ToString();
            _dbContext.tblProject.Update(Proj);
            await _dbContext.SaveChangesAsync();
            return ProjIdDel;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MemberShipRegistration> GetMembers()
        {
            List<MemberShipRegistration> response = new List<MemberShipRegistration>();
            //return await _dbContext.TblMembers.OrderByDescending(t => t. Id).ToListAsync();
            try
            {
                response = (from m in _entityRepository.GetEntities()
                            join c in _dbContext.TblContacts on m.Id equals c.Id
                            where (m.Inactive == false && c.MainContact == true)
                            //join t in _dbContext.TblMemberTypeCounty on m.MemberType equals t.MemberType
                            // where any acondition apply
                            select new
                            {
                                m.Company,
                                c.Contact,
                                m.BillCity,
                                m.BillState,
                                c.Phone,
                                c.Email,
                                m.MemberCost,
                                //  Package=    (from x in _dbContext.TblMemberTypeCounty where x.MemberType == m.MemberType select new {x.Package}).Distinct().OrderBy(m=>m.Package),
                                m.MemberType,
                                m.RenewalDate,
                                m.Discipline,
                                m.Id,
                                m.Term,
                                //t.Package
                            }).Take(500).ToList()
                                   .Select(x => new MemberShipRegistration
                                   {
                                       Company = x.Company,
                                       ContactName = x.Contact,
                                       BillCity = x.BillCity,
                                       BillState = x.BillState,
                                       ContactPhone = x.Phone,
                                       Email = x.Email,
                                       MemberType = x.MemberCost.ToString(),
                                       MemberCost = x.MemberType.ToString(),
                                       RenewalDate = x.RenewalDate,
                                       Discipline = x.Discipline,
                                       CompId = x.Id,
                                       Term = x.Term,
                                       //Package = "Not Known"
                                       Package = string.IsNullOrEmpty(x.MemberType.ToString()) ? "Not Known" : GetPackage(Convert.ToInt32(x.MemberType))
                                   }).ToList();
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;

        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MemberShipRegistration> GetContractorArchitect()
        {
            List<MemberShipRegistration> response = new List<MemberShipRegistration>();
            try
            {
                var contractors = (from m in _dbContext.TblContractors
                                   join c in _dbContext.TblContacts on m.Id equals c.Id
                                   select new
                                   {
                                       m.Name,
                                       c.Contact,
                                       m.City,
                                       m.State,
                                       c.Phone,
                                       c.Email,
                                       //m.MemberCost,
                                       //m.MemberType,
                                       //m.RenewalDate,
                                       //m.Discipline,
                                       m.Id,
                                       c.CompType,
                                       //m.Term,
                                   }).OrderByDescending(x => x.Id).Take(500).ToList()
                                  .Select(x => new MemberShipRegistration
                                  {
                                      Company = x.Name,
                                      ContactName = x.Contact,
                                      BillCity = x.City,
                                      BillState = x.State,
                                      ContactPhone = x.Phone,
                                      Email = x.Email,
                                      //MemberType = x.MemberCost.ToString(),
                                      // MemberCost = x.MemberType.ToString(),
                                      // RenewalDate = x.RenewalDate,
                                      //Discipline = x.Discipline,
                                      ID = x.Id,
                                      CompType = x.CompType,
                                      //Term = x.Term,
                                      //Package = "Not Known"
                                      // Package = string.IsNullOrEmpty(x.MemberType.ToString()) ? "Not Known" : GetPackage(Convert.ToInt32(x.MemberType))
                                  }).ToList();
                var architects = (from m in _dbContext.TblArchOwners
                                  join c in _dbContext.TblContacts on m.Id equals c.Id
                                  select new
                                  {
                                      m.Name,
                                      c.Contact,
                                      m.City,
                                      m.State,
                                      c.Phone,
                                      c.Email,
                                      //m.MemberCost,
                                      //m.MemberType,
                                      //m.RenewalDate,
                                      //m.Discipline,
                                      m.Id,
                                      c.CompType
                                      //m.Term,
                                  }).OrderByDescending(x => x.Id).Take(500).ToList()
                                  .Select(x => new MemberShipRegistration
                                  {
                                      Company = x.Name,
                                      ContactName = x.Contact,
                                      BillCity = x.City,
                                      BillState = x.State,
                                      ContactPhone = x.Phone,
                                      Email = x.Email,
                                      //MemberType = x.MemberCost.ToString(),
                                      // MemberCost = x.MemberType.ToString(),
                                      // RenewalDate = x.RenewalDate,
                                      //Discipline = x.Discipline,
                                      ID = x.Id,
                                      CompType = x.CompType,
                                      //Term = x.Term,
                                      //Package = "Not Known"
                                      // Package = string.IsNullOrEmpty(x.MemberType.ToString()) ? "Not Known" : GetPackage(Convert.ToInt32(x.MemberType))
                                  }).ToList();
                var members = (from m in _entityRepository.GetEntities()
                               join c in _dbContext.TblContacts on m.Id equals c.Id
                               where m.Inactive == true
                               select new
                               {
                                   m.Company,
                                   c.Contact,
                                   m.BillCity,
                                   m.BillState,
                                   c.Phone,
                                   c.Email,
                                   m.MemberCost,
                                   m.MemberType,
                                   m.RenewalDate,
                                   m.Discipline,
                                   m.Id,
                                   m.Term,
                               }).OrderByDescending(x => x.Id).Take(500).ToList()
                                   .Select(x => new MemberShipRegistration
                                   {
                                       Company = x.Company,
                                       ContactName = x.Contact,
                                       BillCity = x.BillCity,
                                       BillState = x.BillState,
                                       ContactPhone = x.Phone,
                                       Email = x.Email,
                                       MemberType = x.MemberCost.ToString(),
                                       MemberCost = x.MemberType.ToString(),
                                       RenewalDate = x.RenewalDate,
                                       Discipline = x.Discipline,
                                       ID = x.Id,
                                       Term = x.Term,
                                       Package = string.IsNullOrEmpty(x.MemberType.ToString()) ? "Not Known" : GetPackage(Convert.ToInt32(x.MemberType))
                                   }).ToList();
                response.AddRange(contractors);
                response.AddRange(architects);
                response.AddRange(members);
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;

        }
        /// <summary>
        /// Get Packege text from from membertype
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetPackage(int id)
        {
            string package = (from x in _dbContext.TblMemberTypeCounty where x.MemberType == id select x.Package).FirstOrDefault();
            if (package != null)
                return package;
            else
                return package;
        }
        //public IEnumerable<MemberShipRegistration> GetArchitect()
        //{
        //    List<MemberShipRegistration> response = new List<MemberShipRegistration>();
        //    try
        //    {
        //        response = _dbContext.TblArchOwners.OrderByDescending(x => x.Id).Select(x => new MemberShipRegistration
        //        {
        //            ID = x.Id,
        //            ArchitectName = x.Name,
        //            BillCity = x.City,
        //            BillState = x.State,
        //            ContactPhone = x.Phone,
        //            Email = x.Email,
        //            Type = x.Type1
        //        }).Take(200).ToList();
        //    }
        //    catch (Exception Ex)
        //    {
        //        _logger.LogWarning(Ex.Message);
        //    }
        //    return response;

        //}
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntityTypeViewModel> GetEntity()
        {
            List<EntityTypeViewModel> response = new List<EntityTypeViewModel>();
            try
            {
                response = _dbContext.TblEntityType.Where(x => x.IsActive == true).OrderByDescending(x => x.EntityID).Select(x => new EntityTypeViewModel
                {
                    EntityID = x.EntityID,
                    EntityType = x.EntityType,
                    IsActive = x.IsActive
                }).Take(200).ToList();
            }
            catch (Exception Ex)
            {
                _logger.LogWarning(Ex.Message);
            }
            return response;
        }
        /// <summary>
        /// To get contractor profile detail on StaffAccount/ContractorProfile from id on StaffAccount/MemberManagement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MemberShipRegistration> GetContractorProfileAsync(int id)
        {
            MemberShipRegistration response = new MemberShipRegistration();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(t => t.Id == id && t.IsContractor == true);
                if (tblMember != null)
                {
                    response.ID = tblMember.Id;
                    response.ContractorName = tblMember.Company;
                    response.BillAddress = tblMember.BillAddress;
                    response.BillCity = tblMember.BillCity;
                    response.BillState = tblMember.BillState;
                    response.BillStateId = (from x in _dbContext.TblState where x.State == tblMember.BillState select x.StateId.ToString()).FirstOrDefault();
                    response.BillZip = tblMember.BillZip;
                    TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(a => a.Id == id && a.MainContact == true && a.CompType == 2);
                    if (tblContact != null)
                    {
                        response.ContactPhone = tblContact.Phone;
                        response.Email = tblContact.Email;
                    }
                    else
                    {
                        response.ContactPhone = tblMember.CompanyPhone;
                        response.Email = tblMember.Email;
                    }

                    //response.ContactPhone = tblContractor.Phone;
                }
                else
                {
                    TblContractor tblContractor = _dbContext.TblContractors.SingleOrDefault(t => t.Id == id);

                    if (tblContractor != null)
                    {
                        response.ID = tblContractor.Id;
                        response.ContractorName = tblContractor.Name;
                        response.BillAddress = tblContractor.Addr1;
                        response.BillCity = tblContractor.City;
                        response.BillState = tblContractor.State;
                        response.BillStateId = (from x in _dbContext.TblState where x.State == tblContractor.State select x.StateId.ToString()).FirstOrDefault();

                        response.BillZip = tblContractor.Zip;
                        response.ContactPhone = tblContractor.Phone;
                        response.Email = tblContractor.Email;
                    }
                }


                List<MemberContactInfo> contactList = (from c in _dbContext.TblContacts where c.Id == id select c).ToList().Where(x => x.CompType == 2 && x.MainContact == false).Select(c => new MemberContactInfo
                {
                    Contact = c.Contact,
                    Phone = c.Phone,
                    Email = c.Email,
                    ConID = c.ConId,
                }).ToList();
                response.ContactList = contactList;
                List<NoteInfo> NoteList = (from c in _dbContext.TblMemNotes where c.MemId == id && c.Flag == false && c.CompType == 2.ToString() select c).ToList()
                                  .Select(c => new NoteInfo
                                  {
                                      Id = c.Id,
                                      Note = c.Note,
                                      LogDate = c.LogDate
                                  }).ToList();
                response.NoteList = NoteList;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// To Save/update contractor profile detail StaffAccount/ContractorProfile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> EditContractorProfileAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                model.BillState = GetSelectedStateText(model.BillState);
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(x => x.Id == model.ID && x.IsContractor == true);
                if (tblMember != null)
                {
                    tblMember.Company = model.ContractorName;
                    tblMember.BillAddress = model.BillAddress;
                    tblMember.MailAddress = model.BillAddress;
                    tblMember.BillCity = model.BillCity;
                    tblMember.MailCity = model.BillCity;
                    tblMember.BillState = model.BillState;
                    tblMember.MailState = model.BillState;
                    tblMember.BillZip = model.BillZip;
                    tblMember.MailZip = model.BillZip;
                    tblMember.Email = model.Email;
                    tblMember.CompanyPhone = model.ContactPhone;
                    //_dbContext.Entry(tblMember).State = EntityState.Modified;
                    //await _dbContext.SaveChangesAsync();
                    await _entityRepository.UpdateEntityAsync(tblMember);
                    response.success = true;
                    if (response.success == true)
                    {
                        TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(x =>
                            x.Id == model.ID && x.MainContact == true && x.CompType == 2);
                        if (tblContact != null)
                        {
                            tblContact.Phone = model.ContactPhone;
                            tblContact.Email = model.Email;
                            _dbContext.Entry(tblContact).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                        response.success = true;
                        response.statusCode = "200";
                        response.statusMessage = "Profile updated";

                    }
                }
                else
                {
                    TblContractor tblContractor = await _dbContext.TblContractors.SingleOrDefaultAsync(m => m.Id == model.ID);
                    if (tblContractor != null)
                    {
                        tblContractor.Name = model.ContractorName;
                        tblContractor.Addr1 = model.BillAddress;
                        tblContractor.City = model.BillCity;
                        tblContractor.State = model.BillState;
                        tblContractor.Zip = model.BillZip;
                        tblContractor.Phone = model.ContactPhone;
                        tblContractor.Email = model.Email;
                        _dbContext.Entry(tblContractor).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                        response.success = true;
                        response.statusCode = "200";
                        response.statusMessage = "Profile updated";
                    }
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
        /// To get State text from state id
        /// </summary>
        /// <param name="SelectedValue"></param>
        /// <returns></returns>
        public string GetSelectedStateText(string SelectedValue)
        {
            var response = (from tab in _dbContext.TblState.Where(m => m.StateId.ToString() == SelectedValue) select new { tab.State }).SingleOrDefault();
            string result = response != null ? response.State : "NA";
            return result;
        }
        /// <summary>
        /// To save new contact for contractor from StaffAccount/ContractorProfile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> AddNewUserAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblContact = new();
                tblContact.Id = model.ID;
                tblContact.Contact = model.FirstName + model.LastName;
                tblContact.Email = model.ContactEmail;
                tblContact.Phone = model.ContactPhone;
                tblContact.CompType = model.CompType;
                tblContact.MainContact = false;
                var user = _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    tblContact.Uid = user.Id.ToString();
                }


                tblContact.FirstName = model.FirstName;
                tblContact.LastName = model.LastName;
                //var contact = _entityRepository.Contact_instance(tblContact);
                //await _dbContext.Contacts.AddAsync(contact);

                await _dbContext.TblContacts.AddAsync(tblContact);
                await _dbContext.SaveChangesAsync();
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "User data saved";
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
        /// To get contractor profile detail on StaffAccount/ContractorProfile from id on StaffAccount/MemberManagemen
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MemberShipRegistration> GetArchitectProfileAsync(int id)
        {
            MemberShipRegistration response = new MemberShipRegistration();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(t => t.Id == id && t.IsArchitect == true);
                if (tblMember != null)
                {
                    response.ID = tblMember.Id;
                    response.ArchitectName = tblMember.Company;
                    response.BillAddress = tblMember.BillAddress;
                    response.BillCity = tblMember.BillCity;
                    response.BillState = tblMember.BillState;
                    response.BillStateId =
                        (from x in _dbContext.TblState where x.State == tblMember.BillState select x.StateId.ToString())
                        .FirstOrDefault();
                    response.BillZip = tblMember.BillZip;
                    TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(a => a.Id == id && a.MainContact == true && a.CompType == 3);
                    if (tblContact != null)
                    {
                        response.ContactPhone = tblContact.Phone;
                        response.Email = tblContact.Email;
                    }
                    else
                    {
                        response.ContactPhone = tblMember.CompanyPhone;
                        response.Email = tblMember.Email;
                    }
                }
                else
                {
                    TblArchOwner tblArchOwner = _dbContext.TblArchOwners.SingleOrDefault(t => t.Id == id);

                    if (tblArchOwner != null)
                    {
                        response.ID = tblArchOwner.Id;
                        response.ArchitectName = tblArchOwner.Name;
                        response.BillAddress = tblArchOwner.Addr1;
                        response.BillCity = tblArchOwner.City;
                        response.BillState = tblArchOwner.State;
                        response.BillStateId = (from x in _dbContext.TblState where x.State == tblArchOwner.State select x.StateId.ToString()).FirstOrDefault();
                        response.BillZip = tblArchOwner.Zip;
                        response.ContactPhone = tblArchOwner.Phone;
                        response.Email = tblArchOwner.Email;
                        response.Type = tblArchOwner.Type1;
                    }
                }


                List<MemberContactInfo> contactList = (from c in _dbContext.TblContacts where c.Id == id select c).ToList().Where(x => x.CompType == 3 && x.MainContact == false).Select(c => new MemberContactInfo
                {
                    Contact = c.Contact,
                    Phone = c.Phone,
                    Email = c.Email,
                    ConID = c.ConId,
                }).ToList();
                response.ContactList = contactList;
                List<NoteInfo> NoteList = (from c in _dbContext.TblMemNotes where c.MemId == id && c.Flag == false && c.CompType == 3.ToString() select c).ToList()
                                 .Select(c => new NoteInfo
                                 {
                                     Id = c.Id,
                                     Note = c.Note,
                                     LogDate = c.LogDate
                                 }).ToList();
                response.NoteList = NoteList;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        /// <summary>
        /// To Save/update contractor profile detail StaffAccount/ContractorProfile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> UpdateArchitectProfileAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                model.MailState = GetSelectedStateText(model.MailState);
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(x => x.Id == model.ID && x.IsArchitect == true);
                if (tblMember != null)
                {
                    tblMember.Company = model.ArchitectName;
                    tblMember.BillAddress = model.MailAddress;
                    tblMember.MailAddress = model.MailAddress;
                    tblMember.BillCity = model.MailCity;
                    tblMember.MailCity = model.MailCity;
                    tblMember.BillState = model.MailState;
                    tblMember.MailState = model.MailState;
                    tblMember.BillZip = model.MailZip;
                    tblMember.MailZip = model.MailZip;
                    tblMember.Email = model.Email;
                    tblMember.CompanyPhone = model.ContactPhone;
                    _entityRepository.UpdateEntity(tblMember);
                    //_dbContext.Entry(tblMember).State = EntityState.Modified;
                    //await _dbContext.SaveChangesAsync();
                    response.success = true;
                    if (response.success == true)
                    {
                        TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(x =>
                            x.Id == model.ID && x.MainContact == true && x.CompType == 3);
                        if (tblContact != null)
                        {
                            tblContact.Phone = model.ContactPhone;
                            tblContact.Email = model.Email;
                            _dbContext.Entry(tblContact).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                            response.success = true;
                            response.statusCode = "200";
                            response.statusMessage = "Profile updated";
                        }
                    }
                }
                else
                {
                    TblArchOwner tblArchOwner = await _dbContext.TblArchOwners.SingleOrDefaultAsync(m => m.Id == model.ID);
                    if (tblArchOwner != null)
                    {
                        tblArchOwner.Name = model.ArchitectName;
                        tblArchOwner.Addr1 = model.MailAddress;
                        tblArchOwner.City = model.MailCity;
                        tblArchOwner.State = model.MailState;
                        tblArchOwner.Zip = model.MailZip;
                        tblArchOwner.Phone = model.ContactPhone;
                        tblArchOwner.Email = model.Email;
                        tblArchOwner.Type1 = model.Type;
                        _dbContext.Entry(tblArchOwner).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Profile updated";
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
        /// Show modal popup of View Order field of staffAccount/copyCenter and Member/CopyCenter
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<dynamic> GetViewOrderDocAsync(int OrderId)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            OrderTables response = new OrderTables();
            try
            {

                TblProjOrder x = await _dbContext.TblProjOrder.SingleOrDefaultAsync(t => t.OrderId == OrderId);
                OrderTables order = new();
                order.OrderId = x.OrderId;
                order.ProjId = x.ProjId;
                order.PO = x.Po;
                order.Instructions = x.Instructions;
                order.Company = x.Company;
                order.Name = x.Name;
                order.Addr = x.Addr;
                order.Phone = x.Phone;
                order.Email = x.Email;
                order.DeliveryDt = x.DeliveryDt;
                order.ShipAmt = x.ShipAmt;
                if (int.TryParse(x.HowShip, out int howShipNumeric))
                {
                    var list = await _dbContext.tblDeliveryOption.SingleOrDefaultAsync(m => m.DelivOptId == howShipNumeric);
                    if (list != null)
                    {
                        int id = list.DelivId;
                        var tbldeliverymaster = await _dbContext.tblDeliveryMaster.SingleOrDefaultAsync(m => m.DelivId == id);
                        order.HowShip = tbldeliverymaster.DelivName + " (" + list.DelivOptName + ")";
                    }
                }
                else
                {
                    order.HowShip = x.HowShip;
                }
                List<TblProjOrderDetail> details = await _dbContext.TblProjOrderDetail.Where(x => x.OrderId == OrderId && x.Price != null).ToListAsync();
                order.GetTblProjs = new();
                if (details != null)
                {
                    foreach (var detail in details)
                    {
                        OrderDetails _details = new();
                        _details.FileName = detail.FileName;
                        _details.Pages = detail.Pages;
                        _details.Copies = detail.Copies;
                        _details.PrintName = detail.PrintName;
                        _details.Size = detail.Size;
                        _details.Price = detail.Price;
                        //order.GetTblProjs = _details;
                        order.GetTblProjs.Add(_details);
                    }
                }

                // httpResponse.data = details;
                httpResponse.data = order;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// For updating bid result from StaffAccount/Dashboard (Past Project Tab)
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<dynamic> BidResultAsync(int ProjId, string name)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                var currentDate = DateTime.Now.Date;
                currentDate = Convert.ToDateTime(currentDate.ToString("yyyy-MM-dd"));
                TblProject tblProject = await _dbContext.tblProject.SingleOrDefaultAsync(m => m.ProjId == ProjId);
                if (tblProject != null)
                {
                    tblProject.Brnote = "Bid Result Posted on " + currentDate + " by" + name;
                    _dbContext.Entry(tblProject).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Bid result";
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
        /// For delete user member from staffaccount/memberprofile and member/memberprofile(User Management section).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteUserManagementAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContact tblcontact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.ConId == id);
                if (tblcontact != null)
                {
                    tblcontact.Active = false;
                    await _dbContext.SaveChangesAsync();
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "User deleted successfully";
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
        /// To save data after coming from renewalpayment interface of staffaccount/memberprofile and member/memberprofile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveRenewalPaymentAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            try
            {
                tblDiscount tblDiscount = new();
                var discount = "";
                if (model.DiscountId != null && model.DiscountId > 0)
                {
                    tblDiscount = await _dbContext.tblDiscount.SingleOrDefaultAsync(m => m.DiscountId == model.DiscountId);
                    discount = "Discounted @" + tblDiscount.DiscountRate + "% under offer " + tblDiscount.Description + " applicable between " + Convert.ToDateTime(tblDiscount.StartDate).ToString("MM/dd/yyyy") + " to " + Convert.ToDateTime(tblDiscount.EndDate).ToString("MM/dd/yyyy");
                }

                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.ID);
                model.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                if (tblMember != null && model.Term != null)
                {
                    if (model.Term == "Yearly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddYears(1).AddDays(-1) : null;
                    if (model.Term == "Quarterly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(4).AddDays(-1) : null;
                    if (model.Term == "Monthly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(1).AddDays(-1) : null;
                    if (model.hdnTerm == "Free Trial")
                        model.RenewalDate = DateTime.Now.AddDays(364);
                    tblMember.LastPayDate = model.LastPayDate;
                    tblMember.RenewalDate = model.RenewalDate;
                    tblMember.MemID = model.MemID;
                    tblMember.InvoiceId = model.InvoiceId;
                    tblMember.Term = model.Term;
                    tblMember.Inactive = false;
                    tblMember.MemberType = Convert.ToInt32(model.MemberType);
                    tblMember.MemberCost = Convert.ToInt32(model.MemberCost);
                    tblMember.Discount = discount;
                    _dbContext.Entry(tblMember).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
                TblContact contact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.Id == model.ID && x.MainContact == true);
                model.Email = contact.Email;
                httpResponse.data = model;
                httpResponse.success = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// To save data after coming from renewalpayment interface of staffaccount/memberprofile and member/memberprofile in case of error
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveErrorRenewalPaymentAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == model.ID);

                if (tblMember != null)
                {
                    tblMember.MemID = model.MemID;
                    tblMember.InvoiceId = model.InvoiceId;
                    _dbContext.Entry(tblMember).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// Get Data on StaffAccount/MemberMangement
        /// </summary>
        /// <returns></returns>
        public async Task<MemberManagement> GetMemberManagementData()
        {
            MemberManagement memberManagement = new();
            memberManagement.Members = new();
            memberManagement.Contractor = new();
            try
            {
                memberManagement.Members = await _dbContext.MembersView.FromSqlRaw("EXEC GetMembersData").ToListAsync();
                //memberManagement.NonMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetNonMemberData").ToListAsync();
                //memberManagement.FreeTrialMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetFreeTrialMemberData").ToListAsync();
                //memberManagement.InactiveMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetInactiveMemberData").ToListAsync();
                // memberManagement.Contractor = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetContractorData").ToListAsync();
                // memberManagement.Architect = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetArchitectData").ToListAsync();
                //memberManagement.Entities = await _dbContext.EntitiesView.FromSqlRaw("EXEC GetEntityData").ToListAsync();
                //memberManagement.ArchitectFromMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetArchitectDataFromMember").AsNoTracking().ToListAsync();
                //memberManagement.ContractorFromMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetContractorDataFromMember").AsNoTracking().ToListAsync();
                //memberManagement.OtherTabData = new List<Contractors>();
                //AddItemsToList(memberManagement.NonMember, memberManagement.OtherTabData);
                //AddItemsToList(memberManagement.FreeTrialMember, memberManagement.OtherTabData);
                //AddItemsToList(memberManagement.InactiveMember, memberManagement.OtherTabData);
                //AddItemsToList(memberManagement.Contractor, memberManagement.OtherTabData);
                //AddItemsToList(memberManagement.Architect, memberManagement.OtherTabData);
                //AddItemsToList(memberManagement.ContractorFromMember.OrderByDescending(m => m.Id).ToList(), memberManagement.OtherTabData);
                //AddItemsToList(memberManagement.ArchitectFromMember.OrderByDescending(m => m.Id).ToList(), memberManagement.OtherTabData);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return memberManagement;
        }
        public async Task<MemberManagement> GetOtherTabsData(int page, int pageSize, string searchTerm)
        {
            MemberManagement memberManagement = new MemberManagement();

            try
            {
                memberManagement.NonMember = await ExecuteStoredProcAsync("GetNonMemberData");
                memberManagement.FreeTrialMember = await ExecuteStoredProcAsync("GetFreeTrialMemberData");
                memberManagement.InactiveMember = await ExecuteStoredProcAsync("GetInactiveMemberData");
                memberManagement.ArchitectFromMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetArchitectDataFromMember").AsNoTracking().ToListAsync();
                memberManagement.ContractorFromMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetContractorDataFromMember").AsNoTracking().ToListAsync();

                memberManagement.OtherTabData = new List<Contractors>();
                memberManagement.OtherTabData.AddRange(memberManagement.NonMember);
                memberManagement.OtherTabData.AddRange(memberManagement.FreeTrialMember);
                memberManagement.OtherTabData.AddRange(memberManagement.InactiveMember);
                memberManagement.OtherTabData.AddRange(memberManagement.ContractorFromMember);
                memberManagement.OtherTabData.AddRange(memberManagement.ArchitectFromMember);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var filteredData = memberManagement.OtherTabData.Where(c => ContainsSearchText(c, searchTerm)).ToList();
                    memberManagement.OtherTabData = filteredData;
                }

                // Get total count of records (for pagination)
                int totalRecords = memberManagement.OtherTabData.Count;

                // Apply pagination
                List<Contractors> paginatedData = memberManagement.OtherTabData.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // Set data in the model
                memberManagement.OtherTabData = paginatedData;
                memberManagement.TotalRecords = totalRecords;
                memberManagement.CurrentPage = page;
                memberManagement.PageSize = pageSize;
                memberManagement.Searchtext = searchTerm;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while fetching other tabs data.");
            }

            return memberManagement;
        }

        private async Task<List<Contractors>> ExecuteStoredProcAsync(string procName)
        {
            return await _dbContext.ContractorsView.FromSqlRaw($"EXEC {procName}").ToListAsync();
        }

        public async Task<MemberManagement> GetEntitiesData()
        {

            MemberManagement memberManagement = new();
            memberManagement.Members = new();
            memberManagement.Contractor = new();
            try
            {
                memberManagement.Entities = await _dbContext.EntitiesView.FromSqlRaw("EXEC GetEntityData").ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return memberManagement;
        }

        public async Task<MemberManagement> GetOtherTabsSearchData(string searchText)
        {
            MemberManagement memberManagement = new();
            memberManagement.Members = new();
            memberManagement.Contractor = new();
            memberManagement.NonMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetNonMemberData").ToListAsync();
            memberManagement.FreeTrialMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetFreeTrialMemberData").ToListAsync();
            memberManagement.InactiveMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetInactiveMemberData").ToListAsync();
            memberManagement.ArchitectFromMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetArchitectDataFromMember").AsNoTracking().ToListAsync();
            memberManagement.ContractorFromMember = await _dbContext.ContractorsView.FromSqlRaw("EXEC GetContractorDataFromMember").AsNoTracking().ToListAsync();
            memberManagement.OtherTabData = new List<Contractors>();
            if (memberManagement.NonMember != null)
            {
                memberManagement.OtherTabData.AddRange(memberManagement.NonMember);
            }
            if (memberManagement.FreeTrialMember != null)
            {
                memberManagement.OtherTabData.AddRange(memberManagement.FreeTrialMember);
            }
            if (memberManagement.InactiveMember != null)
            {
                memberManagement.OtherTabData.AddRange(memberManagement.InactiveMember);
            }
            if (memberManagement.ContractorFromMember != null)
            {
                memberManagement.OtherTabData.AddRange(memberManagement.ContractorFromMember);
            }
            if (memberManagement.ArchitectFromMember != null)
            {
                memberManagement.OtherTabData.AddRange(memberManagement.ArchitectFromMember);
            }
            if (memberManagement.OtherTabData != null && !string.IsNullOrEmpty(searchText))
            {
                var filteredData = memberManagement.OtherTabData
                    .Where(c => ContainsSearchText(c, searchText))
                    .ToList();
                if (filteredData.Count() > 500)
                    filteredData = filteredData.Take(500).ToList();
                memberManagement.OtherTabData = filteredData;
            }
            memberManagement.Searchtext = searchText;
            return memberManagement;
        }

        private bool ContainsSearchText(Contractors contractor, string searchText)
        {
            return (contractor.Id?.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Company?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Contact?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.BillCity?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.BillState?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Phone?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Email?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.MemberCost?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.MemberType?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.RenewalDate?.ToString("yyyy-MM-dd").Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Discipline?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Term?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (contractor.Package.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                   contractor.CompType.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }
        void AddItemsToList(List<Contractors> list, List<Contractors> otherTabData, int count = 400)
        {
            // Check if the list is not null and has items
            if (list != null && list.Count > 0)
            {
                // Order the list by ID
                //list.Sort((x, y) => Comparer<int>.Default.Compare((int)x.Id, (int)y.Id));

                // Add the first 100 items from the list to OtherTabData
                for (int i = 0; i < Math.Min(count, list.Count); i++)
                //for (int i = 0; i < list.Count; i++)
                {
                    otherTabData.Add(list[i]);
                }
            }
        }
        /// <summary>
        /// For delete member from staffaccount/membermanagement screen/page(Member tab).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteMemberAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == id);
                if (tblMember != null)
                {
                    tblMember.Inactive = true;
                    tblMember.IsDelete = true;
                    _entityRepository.UpdateEntity(tblMember);
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Member deleted successfully";
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
        /// To get data from tblcontact while verifying from StaffAccount/Login
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<bool> GetContactData(string Email)
        {
            TblContact contact = new();
            bool status = false;
            try
            {
                contact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.Email == Email && x.CompType == 4);
                if (contact != null)
                {
                    if (contact.Active == true)
                    {
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return status;
        }
        /// <summary>
        /// To get packege list data for registering member from staff siTe (StaffAccount/NewRegMember)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPKGListAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblMemberShipPlans> tblMemberShipPlans = new();
            List<tblMemberShipSubPlans> tblMemberShipSubPlans = new();
            try
            {
                tblMemberShipPlans = await _dbContext.tblMemberShipPlans.Where(m => m.Active == true)
                    .Select(x => new tblMemberShipPlans
                    {
                        MemberShipPlanId = x.MemberShipPlanId,
                        MemberShipPlanName = x.MemberShipPlanName
                    }).ToListAsync();
                tblMemberShipSubPlans = await _dbContext.tblMemberShipSubPlans.Where(m => m.Active == true)
                    .Select(x => new tblMemberShipSubPlans
                    {
                        SubMemberShipPlanId = x.SubMemberShipPlanId,
                        MemberShipPlanId = x.MemberShipPlanId,
                        SubMemberShipPlanName = x.SubMemberShipPlanName,
                        YearlyPrice = x.YearlyPrice,
                        MonthlyPrice = x.MonthlyPrice,
                        QuarterlyPrice = x.QuarterlyPrice,
                    }).ToListAsync();
                Dictionary<string, List<tblMemberShipSubPlans>> keyValuePairs = new Dictionary<string, List<tblMemberShipSubPlans>>();
                foreach (var item in tblMemberShipPlans)
                {
                    keyValuePairs.Add(item.MemberShipPlanName, await _dbContext.tblMemberShipSubPlans.Where(m => m.MemberShipPlanId == item.MemberShipPlanId).ToListAsync());

                }
                var datalist = keyValuePairs;
                for (int i = 0; i < tblMemberShipPlans.Count; i++)
                {
                    var data = keyValuePairs[tblMemberShipPlans[i]?.MemberShipPlanName];
                }
                response.data = datalist;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Package bind";
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
        /// To get all the details according term and package name
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<dynamic> GetPkgDetailsAsync(string pkg, string term)
        {
            dynamic response = new ExpandoObject();
            TblMemberTypeCounty tblMemberTypeCounty = new TblMemberTypeCounty();
            tblMemberShipSubPlans tblMemberShipSubPlans = new tblMemberShipSubPlans();

            var MemberTypeCounty = await _dbContext.TblMemberTypeCounty.FirstOrDefaultAsync(x => x.isActive == true && x.Package == pkg);
            if (MemberTypeCounty != null)
            {
                tblMemberTypeCounty.MemberType = MemberTypeCounty.MemberType;
                response.MemberTypeCounty = tblMemberTypeCounty; // Set the property in the response
            }

            var MemberShipSubPlans = await _dbContext.tblMemberShipSubPlans.FirstOrDefaultAsync(x => x.SubMemberShipPlanName == pkg);
            if (MemberShipSubPlans != null)
            {
                tblMemberShipSubPlans.SubMemberShipPlanName = MemberShipSubPlans.SubMemberShipPlanName;
                tblMemberShipSubPlans.YearlyPrice = MemberShipSubPlans.YearlyPrice;
                tblMemberShipSubPlans.QuarterlyPrice = MemberShipSubPlans.QuarterlyPrice;
                tblMemberShipSubPlans.MonthlyPrice = MemberShipSubPlans.MonthlyPrice;
                response.MemberShipSubPlans = tblMemberShipSubPlans;
            }
            response.success = true;
            response.statusCode = "200";
            return response;
        }
        /// <summary>
        /// Saving new member from StaffAccount/NewRegMember
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> SaveNewRegMemberAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            TblMember tblMember = new();
            TblContact tblContact = new();
            tblDiscount tblDiscount = new();
            try
            {
                var discount = "";
                if (model.DiscountId != null && model.DiscountId > 0)
                {
                    tblDiscount = await _dbContext.tblDiscount.SingleOrDefaultAsync(m => m.DiscountId == model.DiscountId);
                    discount = "Discounted @" + tblDiscount.DiscountRate + "% under offer " + tblDiscount.Description + " applicable between " + Convert.ToDateTime(tblDiscount.StartDate).ToString("MM/dd/yyyy") + " to " + Convert.ToDateTime(tblDiscount.EndDate).ToString("MM/dd/yyyy");
                }
                if (model.BillState != null)
                {
                    model.BillState = GetSelectedStateText(model.BillState);
                }
                model.MailState = GetSelectedStateText(model.MailState);
                model.ContactName = model.FirstName + " " + model.LastName;
                model.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                var rowsAffected = 0;
                if (model.Inactive == false)
                {
                    if (model.hdnTerm == "Yearly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddYears(1).AddDays(-1) : null;
                    if (model.hdnTerm == "Quarterly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(4).AddDays(-1) : null;
                    if (model.hdnTerm == "Monthly")
                        model.RenewalDate = model.LastPayDate != null ? Convert.ToDateTime(model.LastPayDate).AddMonths(1).AddDays(-1) : null;
                    if (model.hdnTerm == "Free Trial")
                        model.RenewalDate = DateTime.Now.AddDays(364);
                }

                if (model.ArchitectName != null)
                {
                    TblMember _tblMember = _entityRepository.GetEntities().SingleOrDefault(x => x.Id == model.ID && x.IsArchitect == true);
                    if (_tblMember != null)
                    {
                        _tblMember.InsertDate = DateTime.Now;
                        _tblMember.Company = model.Company;
                        _tblMember.Inactive = false;
                        _tblMember.BillAddress = model.BillAddress;
                        _tblMember.BillCity = model.BillCity;
                        _tblMember.BillState = model.BillState;
                        _tblMember.BillZip = model.BillZip;
                        _tblMember.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                        _tblMember.RenewalDate = model.RenewalDate;
                        _tblMember.Term = model.hdnTerm;
                        _tblMember.MemberType = Convert.ToInt32(model.MemberType);
                        _tblMember.AcceptedTerms = true;
                        _tblMember.AcceptedTermsDt = DateTime.Now;
                        _tblMember.MemberCost = Convert.ToDecimal(model.MemberCost);
                        _tblMember.Dba = model.Dba;
                        _tblMember.MailAddress = model.MailAddress;
                        _tblMember.MailCity = model.MailCity;
                        _tblMember.MailState = model.MailState;
                        _tblMember.MailZip = model.MailZip;
                        _tblMember.CompanyPhone = model.CompanyPhone;
                        _tblMember.CreatedBy = "Staff";
                        _tblMember.IsArchitect = false;
                        _tblMember.IsMember = true;
                        _entityRepository.UpdateEntity(_tblMember);
                        TblContact _tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(x =>
                            x.Id == model.ID && x.MainContact == true && x.CompType == 3);
                        if (_tblContact != null)
                        {
                            _tblContact.UserId = model.ASPUserId;
                            _tblContact.Contact = model.ContactName;
                            _tblContact.MainContact = true;
                            _tblContact.Phone = model.ContactPhone;
                            _tblContact.Email = model.ContactEmail;
                            _tblContact.Password = model.ContactPassword;
                            _tblContact.Extension = model.Extension;
                            _tblContact.CompType = 1;
                            _tblContact.Active = true;
                            _dbContext.Entry(_tblContact).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                            response.success = true;
                            response.statusMessage = "Member created successfully.";
                            response.statusCode = "200";
                            return response;
                        }
                    }
                    else
                    {
                        TblArchOwner tblArchOwner = await _dbContext.TblArchOwners.SingleOrDefaultAsync(m => m.Id == model.ID);
                        if (tblArchOwner != null)
                        {
                            tblArchOwner.Active = false;
                            _dbContext.Entry(tblArchOwner).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
                if (model.ContractorName != null)
                {
                    TblMember _tblMember = _entityRepository.GetEntities().SingleOrDefault(x => x.Id == model.ID && x.IsContractor == true);
                    if (_tblMember != null)
                    {
                        _tblMember.InsertDate = DateTime.Now;
                        _tblMember.Company = model.Company;
                        _tblMember.Inactive = false;
                        _tblMember.BillAddress = model.BillAddress;
                        _tblMember.BillCity = model.BillCity;
                        _tblMember.BillState = model.BillState;
                        _tblMember.BillZip = model.BillZip;
                        _tblMember.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                        _tblMember.RenewalDate = model.RenewalDate;
                        _tblMember.Term = model.hdnTerm;
                        _tblMember.MemberType = Convert.ToInt32(model.MemberType);
                        _tblMember.AcceptedTerms = true;
                        _tblMember.AcceptedTermsDt = DateTime.Now;
                        _tblMember.MemberCost = Convert.ToDecimal(model.MemberCost);
                        _tblMember.Dba = model.Dba;
                        _tblMember.MailAddress = model.MailAddress;
                        _tblMember.MailCity = model.MailCity;
                        _tblMember.MailState = model.MailState;
                        _tblMember.MailZip = model.MailZip;
                        _tblMember.CompanyPhone = model.CompanyPhone;
                        _tblMember.CreatedBy = "Staff";
                        _tblMember.IsContractor = false;
                        _tblMember.IsMember = true;
                        _entityRepository.UpdateEntity(_tblMember);
                        //_dbContext.Entry(_tblMember).State = EntityState.Modified;
                        TblContact _tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(x =>
                            x.Id == model.ID && x.MainContact == true && x.CompType == 2);
                        if (_tblContact != null)
                        {
                            _tblContact.UserId = model.ASPUserId;
                            _tblContact.Contact = model.ContactName;
                            _tblContact.MainContact = true;
                            _tblContact.Phone = model.ContactPhone;
                            _tblContact.Email = model.ContactEmail;
                            _tblContact.Password = model.ContactPassword;
                            _tblContact.Extension = model.Extension;
                            _tblContact.CompType = 1;
                            _tblContact.Active = true;
                            _dbContext.Entry(_tblContact).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                            response.success = true;
                            response.statusMessage = "Member created successfully.";
                            response.statusCode = "200";
                            return response;
                        }
                    }
                    else
                    {
                        TblContractor tblContractor = await _dbContext.TblContractors.SingleOrDefaultAsync(m => m.Id == model.ID);
                        if (tblContractor != null)
                        {
                            tblContractor.Active = false;
                            _dbContext.Entry(tblContractor).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }

                tblMember.InsertDate = DateTime.Now;
                tblMember.Company = model.Company;
                tblMember.Inactive = false;
                tblMember.BillAddress = model.BillAddress;
                tblMember.BillCity = model.BillCity;
                tblMember.BillState = model.BillState;
                tblMember.BillZip = model.BillZip;
                tblMember.LastPayDate = DateTime.Now.ToString("MM/dd/yyyy");
                tblMember.RenewalDate = model.RenewalDate;
                tblMember.Term = model.hdnTerm;
                tblMember.MemberType = Convert.ToInt32(model.MemberType);
                tblMember.AcceptedTerms = true;
                tblMember.AcceptedTermsDt = DateTime.Now;
                tblMember.MemberCost = Convert.ToDecimal(model.MemberCost);
                tblMember.Dba = model.Dba;
                tblMember.MailAddress = model.MailAddress;
                tblMember.MailCity = model.MailCity;
                tblMember.MailState = model.MailState;
                tblMember.MailZip = model.MailZip;
                tblMember.CompanyPhone = model.CompanyPhone;
                tblMember.CreatedBy = "Staff";
                tblMember.IsMember = true;
                tblMember.IsContractor = false;
                tblMember.IsArchitect = false;

                var businessEntity = _entityRepository.BusinessEntity_instance(tblMember);
                await _dbContext.BusinessEntities.AddAsync(businessEntity);
                await _dbContext.SaveChangesAsync();

                var address = _entityRepository.Address_instance(tblMember);
                var member = _entityRepository.Member_instance(tblMember);
                await _dbContext.Addresses.AddAsync(address);
                await _dbContext.Members.AddAsync(member);
                // Save changes to the database
                await _dbContext.SaveChangesAsync();
                //await _dbContext.TblMembers.AddAsync(tblMember);
                //_dbContext.SaveChanges();
                response.success = true;
                if (response.success)
                {
                    tblContact.Id = (_dbContext.BusinessEntities.Any()) ? _dbContext.BusinessEntities.Max(m => m.BusinessEntityId) : 1;
                    tblContact.Contact = model.ContactName;
                    tblContact.MainContact = true;
                    tblContact.Phone = model.ContactPhone;
                    tblContact.Email = model.ContactEmail;
                    tblContact.UserId = model.ASPUserId;
                    tblContact.Password = model.ContactPassword;
                    tblContact.Extension = model.Extension;
                    tblContact.CompType = 1;
                    tblContact.Active = true;
                    var user = _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        tblContact.Uid = user.Id.ToString();
                    }
                    tblContact.FirstName = model.FirstName;
                    tblContact.LastName = model.LastName;
                    //var contact = _entityRepository.Contact_instance(tblContact);

                    //await _dbContext.Contacts.AddAsync(contact);
                    await _dbContext.TblContacts.AddAsync(tblContact);
                    _dbContext.SaveChanges();
                    response.success = true;
                    response.statusMessage = "Member created successfully.";
                    response.statusCode = "200";
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
        /// Register contractor
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<dynamic> RegContractor(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            int id = _dbContext.TblContractors.Max(x => x.Id);
            int row = 0;
            try
            {
                TblContractor contractor = new();
                contractor.Name = model.Company;
                contractor.Uid = id.ToString() + "A";
                contractor.Addr1 = model.MailAddress;
                contractor.City = model.MailCity;
                contractor.State = model.MailState;
                contractor.Zip = model.MailZip;
                contractor.Email = model.Email;
                contractor.Phone = model.ContactPhone;
                contractor.Active = true;
                await _dbContext.TblContractors.AddAsync(contractor);
                row = await _dbContext.SaveChangesAsync();
                model.ID = (await _dbContext.BusinessEntities.AnyAsync()) ? await _dbContext.BusinessEntities.MaxAsync(m => m.BusinessEntityId) : 1;
                if (row > 0)
                {
                    TblContact tblContact = new();
                    tblContact.Contact = model.ContactName;
                    tblContact.Email = model.Email;
                    tblContact.Extension = model.Extension;
                    tblContact.Id = model.ID;
                    tblContact.Phone = model.ContactPhone;
                    tblContact.CompType = 2;
                    tblContact.MainContact = true;

                    var user = _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        tblContact.Uid = user.Id.ToString();
                    }
                    tblContact.FirstName = model.FirstName;
                    tblContact.LastName = model.LastName;
                    //var contact = _entityRepository.Contact_instance(tblContact);
                    //await _dbContext.Contacts.AddAsync(contact);
                    await _dbContext.TblContacts.AddAsync(tblContact);
                    row = await _dbContext.SaveChangesAsync();
                    model.ConID = (await _dbContext.TblContacts.AnyAsync()) ? await _dbContext.TblContacts.MaxAsync(m => m.ConId) : 1;
                }
                httpResponse.data = model;
                httpResponse.success = true;
            }
            catch (Exception Ex)
            {
                httpResponse.success = false;
                _logger.LogWarning(Ex.Message);
            }
            return httpResponse;
        }
        /// <summary>
        /// Delete inactive member permanentaly from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteInactiveMemberAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            var EmailTo = "";
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().SingleOrDefault(m => m.Id == id);
                if (tblMember != null)
                {
                    _dbContext.Entry(tblMember).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                }
                if (response.success == true)
                {
                    TblContact tblContact = await _dbContext.TblContacts.SingleOrDefaultAsync(m => m.Id == id && m.MainContact == true && m.CompType == 1);
                    if (tblContact != null)
                    {
                        response.success = false;
                        EmailTo = tblContact.Email;
                        _dbContext.Entry(tblContact).State = EntityState.Deleted;
                        await _dbContext.SaveChangesAsync();
                        response.success = true;
                        if (response.success == true)
                        {
                            var user = await _userManager.FindByEmailAsync(EmailTo);
                            if (user != null)
                            {
                                var result = await _userManager.DeleteAsync(user);
                                if (result.Succeeded)
                                {
                                    response.statusCode = "200";
                                    response.statusMessage = "Deleted successfully";
                                }
                            }
                        }
                    }
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
        /// Delete/Inactive contractor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteContractorAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblContractor tblContractor = await _dbContext.TblContractors.SingleOrDefaultAsync(m => m.Id == id);
                if (tblContractor != null)
                {
                    tblContractor.Active = false;
                    _dbContext.Entry(tblContractor).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Contractor deleted successfully";
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
        /// Delete/Inactive architect
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> DeleteArchitectAsync(int id)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblArchOwner tblArchOwner = await _dbContext.TblArchOwners.SingleOrDefaultAsync(m => m.Id == id);
                if (tblArchOwner != null)
                {
                    tblArchOwner.Active = false;
                    _dbContext.Entry(tblArchOwner).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    response.success = true;
                    response.statusCode = "200";
                    response.statusMessage = "Architect deleted successfully";
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

        public async Task<dynamic> UpdateGracePeriodAsync(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                TblMember tblMember = _entityRepository.GetEntities().FirstOrDefault(x => x.Id == model.ID);
                if (tblMember != null)
                {
                    if (tblMember.ActualRenewalDate == null)
                    {
                        tblMember.ActualRenewalDate = tblMember.RenewalDate;
                        //_dbContext.Entry(tblMember).State = EntityState.Modified;
                        await _entityRepository.UpdateEntityAsync(tblMember);
                        await _dbContext.SaveChangesAsync();
                        if (model.Grace != null)
                        {
                            tblMember.Grace = model.Grace;
                            tblMember.AddGraceDate = DateTime.Now;
                            //tblMember.RenewalDate = tblMember.RenewalDate.Value.AddDays((double)model.Grace);
                            tblMember.RenewalDate = model.RenewalDate;
                            //_dbContext.Entry(tblMember).State = EntityState.Modified;
                            await _entityRepository.UpdateEntityAsync(tblMember);
                            await _dbContext.SaveChangesAsync();
                            response.success = true;
                            response.statusCode = "200";
                            response.statusMessage = "Grace period updated successfully";
                        }
                    }
                    else
                    {
                        if (model.Grace != null)
                        {
                            tblMember.Grace = model.Grace;
                            tblMember.AddGraceDate = DateTime.Now;
                            tblMember.RenewalDate = model.RenewalDate;
                            //_dbContext.Entry(tblMember).State = EntityState.Modified;
                            await _entityRepository.UpdateEntityAsync(tblMember);
                            await _dbContext.SaveChangesAsync();
                            response.success = true;
                            response.statusCode = "200";
                            response.statusMessage = "Grace period updated successfully";
                        }
                    }

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
    }
}
