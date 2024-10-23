using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using PCNW.Helpers;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Controllers
{
    public partial class HomeController : BaseController
    {
        /// <summary>
        /// Get Count of available bidding project to be shown on Home page
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCurrentBiddingProject()
        {
            HttpResponseDetail<dynamic> response = new();
            response.data = _commonRepository.GetCurrentBiddingProject();
            response.success = true;
            return Json(response);
        }
        #region Common Functions
        /// <summary>
        /// To check valid email on Home/FreeTrailMember
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public JsonResult ValidateEmailAddress(string email)
        {
            return Json(_commonRepository.CheckUniqueEmail(email));
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
		public JsonResult CheckUniqueEmail(string email)
        {
            return Json(_commonRepository.CheckUniqueEmail(email));
        }
        #endregion
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetDashboardInfo()
        {
            HttpResponseDetail<dynamic> response = new();
            response.data = await _commonRepository.GetDashboardInfo(User.Identity.Name);
            response.success = true;
            return Json(response);
        }
        /// <summary>
        /// To get user info to set information on Layout-home and layout-member
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public JsonResult GetUserInfo(string Email)
        {
            DisplayLoginInfo model = _commonRepository.GetUserInfo(Email);

            return Json(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveMemberProject(MemberProjectInfo model)//, IFormFile[] pdfFile
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            httpResponse = await _commonRepository.SaveProjectInfoAsync(model);
            //model.REMOTE_ADDR = GetIPAddressOfClient();
            //model.AuthorizedBy = user;
            //EmailViewModel emailObj = new();
            //_emailServiceManager.UploadPostProject(emailObj, model);
            //emailObj.EmailTos = model.EmailsTo;
            //var httpresponse = _emailServiceManager.SendEmail(emailObj);

            if (httpResponse != null)
            {
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                //bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/");
                //if(!test)
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Uploads");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
                string pathPhoto = Path.Combine(this.Environment.WebRootPath, model.ProjNumber);
                pathPhoto = Path.Combine(pathPhoto, "Uploads");
                string[] fileArray = Directory.GetFiles(pathPhoto);
                foreach (string s in fileArray)
                {
                    UploadToS3(s, rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Uploads");
                }
                Directory.Delete(pathPhoto, true);
                return Json(new { status = "OK" });
            }
            return Json(new { status = "OK" });
        }
        /// <summary>
        /// Upload file to AWS S3 folder when posting project from Home/SentProjectFiles
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="subFolder"></param>
        public void UploadToS3(string filePath, string subFolder)
        {
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1));

                string bucketName = "contractor-aws";


                //if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                //{
                //    bucketName = _bucketName; //no subdirectory just bucket name  
                //}
                //else
                //{   // subdirectory and bucket name  
                bucketName = bucketName + @"/" + subFolder;
                //}


                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);
                //Console.WriteLine("Upload 1 completed");


            }
            catch (AmazonS3Exception s3Exception)
            {
                //Console.WriteLine(s3Exception.Message,
                //s3Exception.InnerException);
            }
        }
    }
}
