using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SupplyChain.ClientApplication.Models;
using SupplyChain.ClientApplication.Service.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SupplyChain.ClientApplication.Controllers
{
    public class DemoController : Controller
    {
        private readonly IDefraAuthenticationService _defraAuthenticationService;
        private readonly IExportHealthCertificatesService _exportHealthCertificate;

        public DemoController(IDefraAuthenticationService defraAuthenticationService, IExportHealthCertificatesService exportHealthCertificatesService)
        {
            _defraAuthenticationService = defraAuthenticationService;
            _exportHealthCertificate = exportHealthCertificatesService;
        }

        [Route("Begin-Demo")]
        public IActionResult StartDemo()
        {
            var url = _defraAuthenticationService.PrepareAuthoriseRequest();
            ViewBag.Url = url;
            return View();
        }

        [HttpGet("auth")]
        public IActionResult IDTokenToAccess(string code)
        {
            ViewBag.code = code;
            return View();
        }

        [Route("Access-Token")]
        public async Task<IActionResult> GetAccessToken(string code)
        {
            AccessTokenResponse response = await _defraAuthenticationService.RequestAccessToken(code);

            return View(response);
        }

        [Route("API-Call")]
        public async Task<IActionResult> MakeApiCall()
        {
            EhcMetadata metadata;
            ViewBag.Error = "";
            try
            {
                metadata = await _exportHealthCertificate.GetEhcMetadata();
            }
            catch (FlurlHttpException e)
            {
                metadata = new EhcMetadata();
                ViewBag.Error = $"{e.Message} ({e.GetResponseStringAsync().Result})";
            }
            return View(metadata);
        }

        [Route("API-Call/EHC-Application")]
        public async Task<IActionResult> MakeApiCallPostEhc()
        {
            string requestContent;
            using (StreamReader r = new StreamReader("DemoFiles/ehc-application.json"))
            {
                requestContent = r.ReadToEnd();
            }
            JObject requestContentParsed = JObject.Parse(requestContent);

            var responseContent = await _exportHealthCertificate.Create(requestContentParsed);

            ViewBag.Response = responseContent;
            ViewBag.Request = requestContent;
            return View();
        }

        [Route("API-Call/EHC-Application/Check/{application}")]
        public async Task<IActionResult> MakeApplicationCheck([FromRoute] Guid application)
        {
            dynamic responseContent = await _exportHealthCertificate.CheckStatus(application);

            ViewBag.Response = responseContent;
            return View();
        }
    }
}