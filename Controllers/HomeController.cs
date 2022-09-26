using DTKWebSample.Interfaces;
using DTKWebSample.Models;
using DTKWebSample.Services;
using DTKWebSample.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DTKWebSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly IBufferedFileUploadService _bufferedFileUploadService;



        public HomeController(ILogger<HomeController> logger, IBufferedFileUploadService bufferedFileUploadService)
        {
            _logger = logger;
            _bufferedFileUploadService = bufferedFileUploadService;

        }

        public IActionResult Index()
        {
            var res = LicenseControl.ShowCurrentLicInfo();
            return View(res);
        }
        public IActionResult Mobile()
        {
            var res = LicenseControl.ShowCurrentLicInfo();
            return View(res);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ActivateLicense(string license_key)
        {
            var res = LicenseControl.ActivateLicense(license_key);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Index(IFormFile file)
        {
            try
            {
                var uploadResponse = await _bufferedFileUploadService.UploadFile(file);
                if (uploadResponse.Status)
                {
                    
                    ViewBag.FileUrl = "/UploadedFiles/"+uploadResponse.UploadedPhoto;
                    

                    var resp = Recognizer.Worker("", 80, 300, uploadResponse.Response.ToString());
                    ViewBag.PlateUrl = resp.PlatePhoto;

                    ViewBag.Plate = resp.Plate.ToString();
                    ViewBag.Message = resp.Error!=null?resp.Error.ToString():null;
                }
                else
                {
                    ViewBag.Message = uploadResponse.Error;
                }
            }
            catch (Exception ex)
            {
                //Log ex
                ViewBag.Message = ex.Message;
            }
            var res = LicenseControl.ShowCurrentLicInfo();
            return View(res);
        }
        [HttpPost]
        public async Task<JsonResult> UploadCam(IFormFile file)
        {
            try
            {
                var uploadResponse = await _bufferedFileUploadService.UploadFile(file);
                if (uploadResponse.Status)
                {

                    var resp = Recognizer.Worker("", 80, 300, uploadResponse.Response.ToString());
                    resp.UploadedPhoto = "/UploadedFiles/" + uploadResponse.UploadedPhoto;

                    return Json(resp);
                }
                else
                {
                    return Json(uploadResponse.Error);
                    
                }
            }
            catch (Exception ex)
            {
                return Json("error");
                //Log ex
                ViewBag.Message = ex.Message;
            }
            
          
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}