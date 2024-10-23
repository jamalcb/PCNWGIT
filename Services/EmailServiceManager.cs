using Microsoft.EntityFrameworkCore;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ProcessContracts;
using System.Net;
using System.Net.Mail;

namespace PCNW.Services
{
    public class EmailServiceManager : IEmailServiceManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<MembershipRepository> _logger;
        private readonly IEntityRepository _entityRepository;

        public EmailServiceManager(ApplicationDbContext dbContext, ILogger<MembershipRepository> logger, IEntityRepository entityRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _entityRepository = entityRepository;
        }
        public async Task<string> GetEmailForRegistration(EmailViewModel emailObj, MemberShipRegistration model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var memberInfo = _entityRepository.GetEntities().FirstOrDefault(m => m.Id == model.lngMemID);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (memberInfo != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>The following changes have been requested for " + memberInfo.Company + "''s membership account with Contractor Plan Center. ";
                }
                strEmailHeader += @"This request was submitted by " + model.AuthorizedBy + " at IP Address: " + model.REMOTE_ADDR + " on " + DateTime.Now + "";
                strEmailHeader += @"These changes will be processed by our staff within 1 business day. Please reply to this email if these changes are not authorized.<hr/><br/><br/>";
                strEmailHeader += @"Your User Name: " + model.Email + " Password is : " + model.hdnPass;
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only. Distribution of this report or your user ID and password to those outside of your company is prohibited.</div></div>";


                emailObj.Subject = $"Membership Profile Update {model.Company}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }
        public async Task<string> ChangeBidDateToTracking(EmailViewModel emailObj, ProjectInformation model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.tblProject.FirstOrDefaultAsync(m => m.ProjId == model.ProjId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>The bid date change to " + Convert.ToDateTime(projectInfo.BidDt).ToString("MM/dd/yyyy") + "for " + projectInfo.ProjId.ToString() + "( " + projectInfo.Title + ") ";
                }
                strEmailHeader += @"This request was submitted by " + model.AuthorizedBy + " at IP Address: " + model.REMOTE_ADDR + " on " + DateTime.Now.ToString() + "";
                strEmailHeader += @"These changes will be processed by our staff within 1 business day. Please reply to this email if these changes are not authorized.<hr/><br/><br/>";
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only. Distribution of this report or your user ID and password to those outside of your company is prohibited.</div></div>";


                emailObj.Subject = $"Bid date Update {model.Title}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }

        public async Task<EmailViewModel> Updateproject(EmailViewModel emailObj, ProjectInformation model)
        {
            string strMessage = "";
            try
            {
                string baseUrl = "http://beta.plancenternw.com/";
                string dashboardUrl = baseUrl + "/Member/Dashboard";
                string anchorTag = $"<a href=\"{dashboardUrl}\">Dashboard</a>";

                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.tblProject.FirstOrDefaultAsync(m => m.ProjId == model.ProjId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null && emailObj.strMessage != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>" + emailObj.strMessage + ".";
                }
                strEmailHeader += @"";
                strEmailHeader += " Preview all Projects on the ";
                strEmailHeader += anchorTag;
                strEmailHeader += ".<hr/><br/><br/>"; strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only. Distribution of this report or your user ID and password to those outside of your company is prohibited.</div></div>";


                emailObj.Subject = $"Project Update {model.Title}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
                var members = await _dbContext.TblTracking
                    .Where(m => m.ProjId == model.ProjId)
                    .ToListAsync();

                emailObj.EmailTos = (from tracking in members
                                     join contact in _dbContext.TblContacts
                                     on tracking.ConId equals contact.ConId
                                     select contact.Email)
                            .Distinct()
                            .ToList();

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return emailObj;

        }

        public async Task<EmailViewModel> SiteMaintenance(EmailViewModel emailObj, TblSpecialMsg model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";
                var startdate = model.StartDate?.ToString("dddd, MMMM dd");
                var starttime = model.StartDate?.ToString("hh:mm tt");
                var enddate = model.EndDate?.ToString("dddd, MMMM dd");
                var endtime = model.EndDate?.ToString("hh:mm tt");

                if (model.StartDate >= DateTime.Now || model.EndDate >= DateTime.Now)
                {

                    strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>";
                    strEmailHeader += @"<hr/><br/><br/>";
                    strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>Copyright 2024 Contractor Plan Center, Inc.</div></div>";
                    strMessage = "<div style=\"font-size:16px\">Hi PCNW Member,<br /><br /> We want to make you aware that " + startdate + ", beginning at " + starttime + " PT, the PCNW app will be temporarily down for maintenance. We will be using this time to improve our overall infrastructure and provide a better user experience.<br /><br /> We expect this scheduled maintenance to take several hours and plan to have PCNW available again before " + endtime + " PT, " + enddate + ".<br/><br />" + model.SpMessage + "<br/><br />We appreciate your patience and understanding and look forward to an even better PCNW!<br/><br/>Thanks,<br/>The PCNW Team<br/></div>";

                    emailObj.Subject = $"PCNW Site Maintenance Mail ";
                    strMessage = strEmailTemplate + strEmailHeader + strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                    emailObj.strMessage = strMessage;
                    var members = _entityRepository.GetEntities()
                        .Where(m => (m.IsMember == null || m.IsMember == true) && m.Inactive == false)
                        .ToList();

                    //emailObj.EmailTos = (from tracking in members
                    //                     join contact in _dbContext.TblContacts
                    //                     on tracking.ConId equals contact.ConId
                    //                     select contact.Email)
                    //            .Distinct()
                    //            .ToList();
                    emailObj.EmailTos = (from tracking in members
                                         where tracking.Email != null && tracking.Email != ""
                                         select tracking.Email)
                                .Distinct()
                                .ToList();
                    //emailObj.EmailTos = new List<string>();
                    //emailObj.EmailTos.Add("codingbrains56@gmail.com"); ;
                    //emailObj.EmailTos.Add("codingbrains16@gmail.com"); ;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return emailObj;

        }
        public async Task<dynamic> SendEmail(EmailViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                // 1- we need to define our server connection configuration.
                //var host = "192.168.0.19";
                var host = "smtp.gmail.com";
                var port = 587;
                var credentials = new NetworkCredential("cbdotnetteam@gmail.com", "ntrgnzrwcqshuybp");

                // 2- create the SmtpClient instance, and set the server connection configuration.
                using var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    Credentials = credentials,
                };
                // 3- create the email message
                var emailMessage = new MailMessage
                {
                    //From = new MailAddress("info@contractorplancenter.com"),
                    From = new MailAddress("cbdotnetteam@gmail.com"),
                    Subject = model.Subject,
                    Body = model.strMessage,
                    IsBodyHtml = true,
                };

                if (model.EmailTos != null)
                {
                    foreach (var emailTo in model.EmailTos)
                    {
                        emailMessage.To.Add(new MailAddress(emailTo));
                    }
                }
                //emailMessage.Bcc.Add(new MailAddress("billing@contractorplancenter.com"));
                emailMessage.Bcc.Add(new MailAddress("codingbrains36@gmail.com"));

                // 4- send the email message using the smtp client
                await smtpClient.SendMailAsync(emailMessage);
                response.statusMessage = "<header><div>Thank you, " + model.AuthorizedBy + ", for updating your profile.<br />";
                if (model.EmailTos != null)
                {
                    if (model.EmailTos.Count > 0)
                    {
                        foreach (var emailTo in model.EmailTos)
                        {
                            response.statusMessage += $"An email confirmation has been sent to {emailTo} <br/>";
                        }
                    }
                }
                response.statusMessage += $"Please Note: Membership updates take up to one business day to process.</div></header>";
                response.success = true;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<dynamic> SendEmailTrialMember(EmailViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                // 1- we need to define our server connection configuration.
                //var host = "192.168.0.19";
                var host = "smtp.gmail.com";
                var port = 587;
                var credentials = new NetworkCredential("cbdotnetteam@gmail.com", "ntrgnzrwcqshuybp");

                // 2- create the SmtpClient instance, and set the server connection configuration.
                using var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    Credentials = credentials,
                };
                // 3- create the email message
                var emailMessage = new MailMessage
                {
                    //From = new MailAddress("info@contractorplancenter.com"),
                    From = new MailAddress("cbdotnetteam@gmail.com"),
                    Subject = model.Subject,
                    Body = model.strMessage,
                    IsBodyHtml = true,
                };

                foreach (var emailTo in model.EmailTos)
                {
                    emailMessage.To.Add(new MailAddress(emailTo));
                }
                //emailMessage.Bcc.Add(new MailAddress("billing@contractorplancenter.com"));
                emailMessage.Bcc.Add(new MailAddress("codingbrains36@gmail.com"));

                // 4- send the email message using the smtp client
                await smtpClient.SendMailAsync(emailMessage);
                response.statusMessage = "<header><div>Thank you, " + model.AuthorizedBy + ", for updating your profile.<br />";
                if (model.EmailTos.Count > 0)
                {
                    foreach (var emailTo in model.EmailTos)
                    {
                        response.statusMessage += $"An email confirmation has been sent to {emailTo} <br/>";
                    }
                }
                response.statusMessage += $"Please Note: Membership updates take up to one business day to process.</div></header>";
                response.success = true;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<string> SavePrintOrder(EmailViewModel emailObj, OrderTables model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.TblProjOrder.FirstOrDefaultAsync(m => m.OrderId == model.OrderId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>Print Order received for " + projectInfo.Name + "for " + projectInfo.Company + ". Pleae log on to PCNW for more details. Thank You ! <br/><br/>";
                }
                strEmailHeader += @" This request was submitted by " + model.AuthorizedBy + " at IP Address: " + model.REMOTE_ADDR + " on " + DateTime.Now.ToString() + "";
                strEmailHeader += @"<hr/><br/><br/>";
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only.</div></div>";


                emailObj.Subject = $"Print Order Summary {model.Company}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }
        public async Task<string> GetEmailForCreateUser(EmailViewModel useremailObj, OrderTables model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.TblProjOrder.FirstOrDefaultAsync(m => m.OrderId == model.OrderId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>Thank you for using our Copy Center " + projectInfo.Name + "for " + projectInfo.Company + ".  You will receive a message when your order is complete and ready for pickup or delivery. <br/><br/>";
                }
                strEmailHeader += @" This request was submitted by " + model.AuthorizedBy + " at IP Address: " + model.REMOTE_ADDR + " on " + DateTime.Now.ToString() + "";
                strEmailHeader += @"<hr/><br/><br/>";
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only.</div></div>";


                useremailObj.Subject = $"Print Order Summary {model.Company}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                useremailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }
        public async Task<dynamic> SendEmailSavePrintOrder(EmailViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                // 1- we need to define our server connection configuration.
                //var host = "192.168.0.19";
                var host = "smtp.gmail.com";
                var port = 587;
                var credentials = new NetworkCredential("cbdotnetteam@gmail.com", "ntrgnzrwcqshuybp");

                // 2- create the SmtpClient instance, and set the server connection configuration.
                using var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    Credentials = credentials,
                };
                // 3- create the email message
                var emailMessage = new MailMessage
                {
                    //From = new MailAddress("info@contractorplancenter.com"),
                    From = new MailAddress("cbdotnetteam@gmail.com"),
                    Subject = model.Subject,
                    Body = model.strMessage,
                    IsBodyHtml = true,
                };

                foreach (var emailTo in model.EmailTos)
                {
                    emailMessage.To.Add(new MailAddress(emailTo));
                }
                //emailMessage.Bcc.Add(new MailAddress("billing@contractorplancenter.com"));
                emailMessage.Bcc.Add(new MailAddress("codingbrains36@gmail.com"));

                // 4- send the email message using the smtp client
                await smtpClient.SendMailAsync(emailMessage);
                response.statusMessage = "<header><div>Thank you, " + model.AuthorizedBy + ", for updating your profile.<br />";
                if (model.EmailTos.Count > 0)
                {
                    foreach (var emailTo in model.EmailTos)
                    {
                        response.statusMessage += $"An email confirmation has been sent to {emailTo} <br/>";
                    }
                }
                response.statusMessage += $"Please Note: Membership updates take up to one business day to process.</div></header>";
                response.success = true;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<string> ReadyForPickup(EmailViewModel emailObj, OrderTables model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.TblProjOrder.FirstOrDefaultAsync(m => m.OrderId == model.OrderId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>Your Order completed and Ready For Pickup/Delivery. " + projectInfo.Name + "for " + projectInfo.Company + ". Thank You !";
                }
                strEmailHeader += @" This request was submitted by " + model.AuthorizedBy + " at IP Address: " + model.REMOTE_ADDR + " on " + DateTime.Now.ToString() + "";
                strEmailHeader += @"<hr/><br/><br/>";
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only.</div></div>";


                emailObj.Subject = $"Ready For Pickup/Delivery {model.Company}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }
        public async Task<dynamic> SendEmailForPickup(EmailViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                // 1- we need to define our server connection configuration.
                //var host = "192.168.0.19";
                var host = "smtp.gmail.com";
                var port = 587;
                var credentials = new NetworkCredential("cbdotnetteam@gmail.com", "ntrgnzrwcqshuybp");

                // 2- create the SmtpClient instance, and set the server connection configuration.
                using var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    Credentials = credentials,
                };
                // 3- create the email message
                var emailMessage = new MailMessage
                {
                    //From = new MailAddress("info@contractorplancenter.com"),
                    From = new MailAddress("cbdotnetteam@gmail.com"),
                    Subject = model.Subject,
                    Body = model.strMessage,
                    IsBodyHtml = true,
                };

                foreach (var emailTo in model.EmailTos)
                {
                    emailMessage.To.Add(new MailAddress(emailTo));
                }
                //emailMessage.Bcc.Add(new MailAddress("billing@contractorplancenter.com"));
                emailMessage.Bcc.Add(new MailAddress("codingbrains36@gmail.com"));

                // 4- send the email message using the smtp client
                await smtpClient.SendMailAsync(emailMessage);
                response.statusMessage = "<header><div>Thank you, " + model.AuthorizedBy + ", for updating your profile.<br />";
                if (model.EmailTos.Count > 0)
                {
                    foreach (var emailTo in model.EmailTos)
                    {
                        response.statusMessage += $"An email confirmation has been sent to {emailTo} <br/>";
                    }
                }
                response.statusMessage += $"Please Note: Membership updates take up to one business day to process.</div></header>";
                response.success = true;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
        public async Task<string> UploadPostProject(EmailViewModel emailObj, MemberProjectInfo model)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.tblProject.FirstOrDefaultAsync(m => m.ProjId == model.ProjId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null)
                {
                    string bidDate = projectInfo.BidDt != null ? Convert.ToDateTime(projectInfo.BidDt).ToString("MM/dd/yyyy") : "TBD";
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
                    </div><br/><br/>Upload Project <span style='font - weight: bold'>" + projectInfo.Title + "</span> for bidding date " + bidDate + ". Please login to PCNW for more details. Thank You !";
                }

                strEmailHeader += @" This request was submitted by " + model.AuthorizedBy + " at IP Address: " + model.REMOTE_ADDR + " on " + DateTime.Now.ToString() + "";
                strEmailHeader += @"<hr/><br/><br/>";
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members and staff.</div></div>";


                emailObj.Subject = $"Post Project  {model.Title}";
                strMessage = strEmailTemplate + strEmailHeader + model.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }
        public async Task<string> CompleteSendNotice(EmailViewModel emailObj, OrderTables data)
        {
            string strMessage = "";
            try
            {
                string strEmailTemplate = "";
                string strEmailHeader = "";
                string strEmailDisclosure = "";

                var projectInfo = await _dbContext.TblProjOrder.FirstOrDefaultAsync(m => m.OrderId == data.OrderId);
                strEmailTemplate = "<html><body><div align='center'><table style='background-color:white; font-family:Arial, Helvetica, sans-serif; width:775px; padding:5px' border='0'><tr><td>";
                if (projectInfo != null)
                {
                    strEmailHeader = @"<div style='font-size:18;font-weight:bold; text-align:center'><img alt='Contractor Plan Center' src='http://www.contractorplancenter.com/images/logo/00Name2.png'> 
							</div><br/><br/>Your Order completed and send For Pickup/Delivery. " + projectInfo.Name + "for " + projectInfo.Company + ". Thank You !";
                }
                strEmailHeader += @" This request was submitted by " + data.AuthorizedBy + " at IP Address: " + data.REMOTE_ADDR + " on " + DateTime.Now.ToString() + "";
                strEmailHeader += @"<hr/><br/><br/>";
                strEmailDisclosure = @"<br/><br/><hr/><div style='font-size:10px'>The information contained in this email is confidential and designed for our members 
								only.</div></div>";


                emailObj.Subject = $"Order Complete/Send Notice {data.Company}";
                strMessage = strEmailTemplate + strEmailHeader + data.strMessage + strEmailDisclosure + "</td></tr></table></div></body></html>";
                emailObj.strMessage = strMessage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return strMessage;

        }
        public async Task<dynamic> SendForwardMailAsync(EmailViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            try
            {
                // 1- we need to define our server connection configuration.
                //var host = "192.168.0.19";
                var host = "smtp.gmail.com";
                var port = 587;
                var credentials = new NetworkCredential("cbdotnetteam@gmail.com", "ntrgnzrwcqshuybp");

                // 2- create the SmtpClient instance, and set the server connection configuration.
                using var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    Credentials = credentials,
                };
                // 3- create the email message
                var emailMessage = new MailMessage
                {
                    //From = new MailAddress("info@contractorplancenter.com"),
                    From = new MailAddress("cbdotnetteam@gmail.com"),
                    Subject = model.Subject,
                    Body = @"<div style='font-size:18;font-weight:bold; text-align:center;'><img height='75' width='50%' alt='Contractor Plan Center' src='http://18.236.90.172/assets/images/Logo%20Master%20Horiz%20Black.jpg'> 
							</div><br/><br/> <b>Hi,</b><br/><br/>" + model.strMessage + " <br/><br/> This message send by " + model.AuthorizedBy + " Thank You. ",
                    IsBodyHtml = true,
                };

                if (model.EmailTos != null)
                {
                    foreach (var emailTo in model.EmailTos)
                    {
                        emailMessage.To.Add(new MailAddress(emailTo));
                    }
                }
                //emailMessage.Bcc.Add(new MailAddress("billing@contractorplancenter.com"));
                if (!string.IsNullOrEmpty(model.BccEmail))
                {
                    var BcRecipients = model.BccEmail.Split(';');
                    foreach (var BcRecipient in BcRecipients)
                    {
                        emailMessage.Bcc.Add(new MailAddress(BcRecipient.Trim()));
                        //message.Cc.Add(new MailboxAddress(ccRecipient.Trim()));
                    }
                }
                if (!string.IsNullOrEmpty(model.CcEmail))
                {
                    var ccRecipients = model.CcEmail.Split(';');
                    foreach (var ccRecipient in ccRecipients)
                    {
                        emailMessage.CC.Add(new MailAddress(ccRecipient.Trim()));
                    }
                }
                //emailMessage.Bcc.Add(new MailAddress(model.BccEmail));
                //emailMessage.CC.Add(new MailAddress(model.CcEmail));
                // 4- send the email message using the smtp client
                await smtpClient.SendMailAsync(emailMessage);
                response.statusMessage = "Order forwarded successfully ";
                response.success = true;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusMessage = ex.Message;
                _logger.LogWarning(ex.Message);
            }
            return response;
        }
    }
}
