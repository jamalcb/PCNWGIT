using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace PCNW.Controllers
{
    public class CommonController : Controller
    {
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public int GetMemberId(string MemberName)
        {
            int memberId = 1;
            return memberId;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <returns></returns>
        public string GetIPAddressOfClient()
        {
            string ipAddress = string.Empty;
            IPAddress ip = Request.HttpContext.Connection.RemoteIpAddress;
            if (ip == null)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = Dns.GetHostEntry(ip).AddressList
                        .First(x => x.AddressFamily == AddressFamily.InterNetwork);
                }
                ipAddress = ip.ToString();
            }
            return ipAddress;
        }
        /// <summary>
		/// No USe
		/// </summary>
		/// <param name="nameId"></param>
		/// <returns></returns>
		public async Task<IActionResult> ToMountedDrive(string nameId)
        {
            string url = @"z:\2022\10\" + nameId;
            Process.Start("explorer.exe", url);
            return Redirect("http://54.205.188.229/StaffAccount/Dashboard");

        }
    }
}
