using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SupplyChain.ClientApplication.Models;
using SupplyChain.ClientApplication.Service.Interface;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SupplyChain.ClientApplication.Controllers
{
    public class DemoController : Controller
    {
        private readonly IDefraAuthenticationService _defraAuthenticationService;
        private readonly IExportHealthCertificatesService _exportHealthCertificate;
        private readonly IReferenceDataService _referenceDataService;

        public DemoController(IDefraAuthenticationService defraAuthenticationService, IExportHealthCertificatesService exportHealthCertificatesService, IReferenceDataService referenceDataService)
        {
            _defraAuthenticationService = defraAuthenticationService;
            _exportHealthCertificate = exportHealthCertificatesService;
            _referenceDataService = referenceDataService;
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

        [Route("API-Call/EHC-Application/Prepare")]
        public async Task<IActionResult> CreateEhcApplication(bool errorWithPayload = false)
        {
            var responseContent = await _exportHealthCertificate.GetEhcExample("8293EHC");

            ViewBag.Response = responseContent;
            ViewBag.ErrorWithPayload = errorWithPayload;
            return View();
        }

        [Route("API-Call/EHC-Application/Submit")]
        public async Task<IActionResult> MakeApiCallPostEhc(string json)
        {
            JObject jObjectEhc;
            try
            {
                jObjectEhc = JObject.Parse(json);
            }
            catch (Exception)
            {
                return RedirectToAction("CreateEhcApplication", new {errorWithPayload = true});
            }

            var responseContent = await _exportHealthCertificate.Create(jObjectEhc);

            ViewBag.Response = responseContent;
            ViewBag.Request = json;
            return View();
        }

        [Route("API-Call/EHC-Application/Check/{application}/RequestStatus")]
        public async Task<IActionResult> MakeApplicationCheck([FromRoute] Guid application)
        {
            dynamic responseContent = await _exportHealthCertificate.CheckRequestStatus(application);

            ViewBag.Response = responseContent;
            return View();
        }

        [Route("API-Call/EHC-Application/Check/{application}/ApplicationStatus")]
        public async Task<IActionResult> ApplicationStatus([FromRoute] Guid application)
        {
            dynamic responseContent = await _exportHealthCertificate.CheckApplicationStatus(application);

            ViewBag.Response = responseContent;
            return View();
        }

        [Route("API-Call/Reference-Data")]
        public async Task<IActionResult> ReferenceData()
        {
            var refDataMeta = await _referenceDataService.GetEhcMetadata();
            return View(refDataMeta);
        }

        [Route("API-Call/Reference-Data/{endpoint}")]
        public async Task<IActionResult> ReferenceDataWithEndpoint(string endpoint)
        {
            var refDataMeta = await _referenceDataService.GetEhcMetadata();
            ViewBag.RefData = await _referenceDataService.GetRefDataWithDynamicEndpoint(endpoint);
            ViewBag.Endpoint = endpoint;
            return View(refDataMeta);
        }

    }
}