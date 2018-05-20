using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nClam;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadFiles()
        {
            var model = new UploadFilesViewModel();
            return View(model);
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            var log = new List<ScanResult>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var clam = new ClamClient("clamav-server", 3310);
                    var ping = await clam.PingAsync();

                    var result = await clam.SendAndScanFileAsync(formFile.OpenReadStream());

                    log.Add(new ScanResult()
                    {
                        FileName = formFile.FileName,
                        Result = result.Result.ToString(),
                        Message = result.InfectedFiles?.FirstOrDefault()?.VirusName,
                        RawResult = result.RawResult
                    });
                }
            }
            
            var model = new UploadFilesViewModel();
            model.Results = log;
                        
            return View(model);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

   
}
