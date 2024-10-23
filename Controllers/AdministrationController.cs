using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;
using System.Security.Claims;

namespace PCNW.Controllers
{
    [Authorize(Roles = "Adminstration,Admin")]
    public class AdministrationController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICommonRepository _commonRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IGlobalMasterRepository _globalMasterRepository;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ICommonRepository commonRepository, IStaffRepository staffRepository, IGlobalMasterRepository globalMasterRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _commonRepository = commonRepository;
            _staffRepository = staffRepository;
            _globalMasterRepository = globalMasterRepository;

        }
        /// <summary>
        /// For going to admin module dashboard screen/page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.ViewRolePolicy)]
        public IActionResult Dashboard()
        {
            return View();
        }
        /// <summary>
        /// For going to admin utility dashboard screen/page, for create/manage roles.
        /// </summary>
        /// <returns></returns>
        public IActionResult UtilityDashboard()
        {
            return View();
        }
        /// <summary>
        /// For going to user list screen/page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.ViewRolePolicy)]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        /// <summary>
        /// For going to edit user list screen/page for particular user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> EditUser(string id)
        {
            ViewBag.Id = id;
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} can't be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Type + ":" + c.Value).ToList()
            };

            return View(model);
        }
        public async Task<JsonResult> SaveEntityType(EntityTypeViewModel model)
        {
            dynamic response;
            if (model.EntityID > 0)
                response = await _globalMasterRepository.UpdateEntityTypeAsync(model);
            else
                response = await _globalMasterRepository.SaveEntityTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// For going to edit user list post method or update user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            ViewBag.Id = model.UserId;
            IdentityUser user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} can't be found";
                return View("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserList", "Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        public async Task<IActionResult> Entities(string ReturnUrl = "")
        {

            MemberManagement model = await _staffRepository.GetEntitiesData();
            if (ReturnUrl != null)
                model.ReturnUrl = ReturnUrl;
            return View(model);
        }
        /// <summary>
        /// For going to create role screen/page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.CreateRolePolicy)]
        public IActionResult CreateRole()
        {
            CreateRoleViewModel model = new CreateRoleViewModel();
            return View(model);
        }
        /// <summary>
        /// For going to create role post method or create role.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.CreateRolePolicy)]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userRole = new IdentityRole { Name = model.RoleName };
                var result = await _roleManager.CreateAsync(userRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        /// <summary>
        /// For going to role list screen/page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        /// <summary>
        /// For going to edit role screen/page, particular id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> EditRole(string id)
        {
            ViewBag.Id = id;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} can't be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id.ToString(),
                RoleName = role.Name
            };

            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        /// <summary>
        /// For going to edit role post method or update role.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} can't be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Role Id = {roleId} can't be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in await _userManager.Users.Where(m => m.UserName.Contains("@")).ToListAsync())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (role != null)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRoleViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRoleViewModel.IsSelected = false;
                    }
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            ViewBag.roleId = roleId;
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Role Id = {roleId} can't be found";
                return View("NotFound");
            }

            if (model != null)
            {
                foreach (var userInfo in model)
                {
                    var user = await _userManager.FindByIdAsync(userInfo.UserId);
                    IdentityResult? result = null;
                    if (role != null)
                    {
                        if (userInfo.IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                        {
                            result = await _userManager.AddToRoleAsync(user, role.Name);
                        }
                        else if (!userInfo.IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                        {
                            result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                        }
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            CreateRoleViewModel model = new CreateRoleViewModel();
            return View(model);
        }
        /// <summary>
        /// For delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.DeleteRolePolicy)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ViewBag.Id = id;
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Role Id = {id} can't be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("UserList", "Administration");
            }
        }
        /// <summary>
        /// For delete role particular id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            ViewBag.Id = id;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Role Id = {id} can't be found";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("RoleList", "Administration");
            }
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} can't be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();
            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    UserId = role.Id,
                    RoleName = role.Name
                };
                if (role != null)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRolesViewModel.IsSelected = false;
                    }
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            ViewBag.userId = userId;
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Role Id = {userId} can't be found";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(m => m.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            ViewBag.userId = userId;
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} can't be found";
                return View("NotFound");
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel();
            var userClaimsViewModel = new UserClaimsViewModel
            {
                UserId = userId
            };
            foreach (Claim claim in ClaimsStore.AllClaims.ToList())
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };

                // If the user has claim , set IsSelected property to true, so the chaeckbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }
            return View(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = AccessClaimPolicy.EditRolePolicy)]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model, string userId)
        {
            ViewBag.userId = userId;
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Role Id = {userId} can't be found";
                return View("NotFound");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            result = await _userManager.AddClaimsAsync(user,
                model.Claims.Select(y => new Claim(y.ClaimType, y.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }
        //if There is parentfolder and addenda id is null/addennda in tblsubaddenda then it means that pdf is located at root folder in aws
        /// <summary>
        /// For going to PopulateS3Data screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PopulateS3Data()
        {
            return View();
        }
        /// <summary>
        /// Fill addenda table from Aws S3 to TblAddenda.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> PopulateAddenda()
        {
            List<string> addendaGet = await GetListOfAddenda("/Addenda/");
            List<string> addenda = new();
            List<string> addendaWPdf = new();
            IDictionary<string, int> projNoId = new Dictionary<string, int>();
            addenda = addendaGet.Where(x => x.Contains("/Addenda/") && x.Contains(".pdf")).Select(x => x.ToString()).ToList();
            addendaWPdf = addendaGet.Where(x => x.Contains("/Addenda/") && !x.Contains(".pdf")).Select(x => x.ToString()).ToList();
            foreach (var AddendaItem in addenda)
            {
                string[] arr = AddendaItem.Split("/");
                arr = arr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                TblProject tbl = await _commonRepository.GetProjByNumber(arr[2]);
                HttpResponseDetail<dynamic> detail = new HttpResponseDetail<dynamic>();
                if (tbl != null)
                {
                    if (arr.Length >= 5)
                    {
                        if (arr.Length == 6)
                        {
                            TblAddenda addenda1 = new TblAddenda();
                            addenda1 = await _commonRepository.CheckAddenda(arr[4], tbl.ProjId, null, 0);
                            if (addenda1 == null)
                            {
                                addenda1 = new();
                                addenda1.AddendaNo = arr[4];
                                addenda1.ProjId = tbl.ProjId;
                                detail = await _commonRepository.PopulateAddenda(addenda1);

                            }
                            else
                            {
                                detail.TempValue = addenda1.AddendaId;
                            }
                        }
                        else if (arr.Length == 5)
                        {
                            tblSubAddenda tblSubParNull = new();
                            tblSubParNull.ProjId = tbl.ProjId;
                            tblSubParNull.Pdfpath = AddendaItem;
                            tblSubParNull.PdfFileName = arr[arr.Length - 1];
                            tblSubParNull.ParentFolder = null;
                            tblSubAddenda tblSubChkParNull = new();
                            tblSubChkParNull = await _commonRepository.CheckSubAddenda(tblSubParNull);
                            if (tblSubChkParNull == null)
                            {
                                tblSubParNull = new();
                                tblSubParNull.ProjId = tbl.ProjId;
                                tblSubParNull.PdfFileName = arr[arr.Length - 1];
                                tblSubParNull.ParentFolder = null;
                                tblSubParNull.Pdfpath = AddendaItem;
                            }
                        }
                        else
                        {
                            TblAddenda addenda1 = new TblAddenda();
                            addenda1 = await _commonRepository.CheckAddenda(arr[4], tbl.ProjId, null, 0);
                            if (addenda1 == null)
                            {
                                addenda1 = new();
                                addenda1.AddendaNo = arr[4];
                                addenda1.ProjId = tbl.ProjId;
                                detail = await _commonRepository.PopulateAddenda(addenda1);

                            }
                            else
                            {
                                detail.TempValue = addenda1.AddendaId;
                            }
                            for (int i = 5; i < arr.Length - 1; i++)
                            {
                                TblAddenda addenda2 = new TblAddenda();
                                addenda2 = await _commonRepository.CheckAddenda(arr[i], tbl.ProjId, arr[i - 1], detail.TempValue);
                                if (addenda2 == null)
                                {
                                    addenda2 = new();
                                    addenda2.AddendaNo = arr[i];
                                    addenda2.ProjId = tbl.ProjId;
                                    addenda2.ParentFolder = arr[i - 1];
                                    addenda2.ParentId = detail.TempValue;
                                    detail = await _commonRepository.PopulateAddenda(addenda2);
                                }
                                else
                                {
                                    detail.TempValue = addenda2.AddendaId;
                                }

                            }

                        }
                    }
                    tblSubAddenda tblSub = new();
                    tblSub.ProjId = tbl.ProjId;
                    tblSub.PdfFileName = arr[arr.Length - 1];
                    tblSub.ParentFolder = arr[arr.Length - 2];
                    tblSub.Pdfpath = AddendaItem;
                    tblSub.AddendaId = detail.TempValue;
                    tblSubAddenda tblSubChk = new();
                    tblSubChk = await _commonRepository.CheckSubAddenda(tblSub);
                    if (tblSubChk == null)
                    {
                        tblSub = new();
                        tblSub.ProjId = tbl.ProjId;
                        tblSub.PdfFileName = arr[arr.Length - 1];
                        tblSub.ParentFolder = arr[arr.Length - 2];
                        tblSub.Pdfpath = AddendaItem;
                        tblSub.AddendaId = detail.TempValue;
                        await _commonRepository.PopulateSubAddenda(tblSub);
                    }
                }

            }
            foreach (var AddendaItem in addendaWPdf)
            {

                { int a = 0; }
                string[] arr = AddendaItem.Split("/");
                arr = arr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                TblProject tbl = await _commonRepository.GetProjByNumber(arr[2]);
                HttpResponseDetail<dynamic> detail = new HttpResponseDetail<dynamic>();
                if (tbl != null)
                {
                    if (arr.Length > 4)
                    {
                        if (arr.Length == 5)
                        {
                            TblAddenda addenda1 = new TblAddenda();
                            addenda1 = await _commonRepository.CheckAddenda(arr[4], tbl.ProjId, null, 0);
                            if (addenda1 == null)
                            {
                                addenda1 = new();
                                addenda1.AddendaNo = arr[4];
                                addenda1.ProjId = tbl.ProjId;
                                detail = await _commonRepository.PopulateAddenda(addenda1);

                            }
                        }
                        else
                        {
                            TblAddenda addenda1 = new TblAddenda();
                            addenda1 = await _commonRepository.CheckAddenda(arr[4], tbl.ProjId, null, 0);
                            if (addenda1 == null)
                            {
                                addenda1 = new();
                                addenda1.AddendaNo = arr[4];
                                addenda1.ProjId = tbl.ProjId;
                                detail = await _commonRepository.PopulateAddenda(addenda1);
                            }
                            else
                            {
                                detail.TempValue = addenda1.AddendaId;
                            }
                            for (int i = 5; i < arr.Length; i++)
                            {
                                TblAddenda addenda2 = new TblAddenda();
                                addenda2 = await _commonRepository.CheckAddenda(arr[i], tbl.ProjId, arr[i - 1], detail.TempValue);
                                if (addenda2 == null)
                                {
                                    addenda2 = new();
                                    addenda2.AddendaNo = arr[i];
                                    addenda2.ProjId = tbl.ProjId;
                                    addenda2.ParentFolder = arr[i - 1];
                                    addenda2.ParentId = detail.TempValue;
                                    detail = await _commonRepository.PopulateAddenda(addenda2);
                                }
                                else
                                {
                                    detail.TempValue = addenda2.AddendaId;
                                }
                            }

                        }
                    }

                }
            }
            return Json(new { Status = "success" });
        }
        /// <summary>
        /// For going to managestaff screen/page.
        /// </summary>
        /// <returns></returns>
        ///
        public async Task<IActionResult> ManageStaff()
        {
            return View();
        }
        /// <summary>
        /// for save new staff member 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveStaffMember(StaffManageViewModel model)
        {
            HttpResponseDetail<dynamic> detail = await _commonRepository.SaveStaffMember(model);
            if (detail.success == true)
            {
                var changedUser = await _userManager.FindByEmailAsync(detail.data.Email);
                if (detail.TempValue == 1)
                {
                    if (changedUser == null)
                    {
                        var user = new IdentityUser { UserName = detail.data.Email, Email = detail.data.Email };
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            #region AssignRole
                            IdentityRole role = await _roleManager.FindByNameAsync("Staff");
                            if (role != null)
                            {
                                IdentityResult? res = null;
                                var AssignUser = await _userManager.FindByEmailAsync(model.Email);
                                res = await _userManager.AddToRoleAsync(AssignUser, role.Name);
                            }
                            #endregion
                        }
                        if (!string.IsNullOrEmpty(detail.Value))
                        {
                            var changedUserEmail = await _userManager.FindByEmailAsync(detail.Value);
                            if (changedUserEmail != null)
                            {
                                _userManager.DeleteAsync(changedUserEmail);
                            }
                        }
                    }
                    else
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(changedUser);

                        var result = await _userManager.ResetPasswordAsync(changedUser, token, detail.data.Password);
                        if (!string.IsNullOrEmpty(detail.Value))
                        {
                            var changedUserEmail = await _userManager.FindByEmailAsync(detail.Value);
                            if (changedUserEmail != null)
                            {
                                _userManager.DeleteAsync(changedUserEmail);
                            }
                        }
                    }
                }
                else
                {
                    if (changedUser != null)
                    {
                        try
                        {
                            _userManager.DeleteAsync(changedUser);
                            if (!string.IsNullOrEmpty(detail.Value))
                            {
                                var changedUserEmail = await _userManager.FindByEmailAsync(detail.Value);
                                if (changedUserEmail != null)
                                {
                                    _userManager.DeleteAsync(changedUserEmail);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var a = ex;
                            throw;
                        }
                    }
                }
            }
            return Json(new { Status = "success" });
        }
        /// <summary>
        /// For populate staff member list on ManageStaff screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetStaffMember()
        {
            HttpResponseDetail<dynamic> detail = await _commonRepository.GetStaffMember();
            return Json(detail);
        }
        /// <summary>
        /// For staffmember's update/edit post method.
        /// </summary>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetEditData(int ConId)
        {
            HttpResponseDetail<dynamic> detail = await _commonRepository.GetEditData(ConId);
            return Json(detail);
        }
    }
}
