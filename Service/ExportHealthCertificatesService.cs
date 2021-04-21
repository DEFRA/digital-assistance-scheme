using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using SupplyChain.ClientApplication.Models;
using SupplyChain.ClientApplication.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SupplyChain.ClientApplication.Service
{
    public class ExportHealthCertificatesService : IExportHealthCertificatesService
    {
        private readonly IDefraAuthenticationService _defraAuthenticationService;
        private readonly string defraTradeApiGateway;

        public ExportHealthCertificatesService(IConfiguration configuration, IDefraAuthenticationService defraAuthenticationService)
        {
            _defraAuthenticationService = defraAuthenticationService;
            defraTradeApiGateway = configuration.GetSection("DefraTradeGateway").Value;
        }

        public async Task<dynamic> GetEhcMetadata()
        {
            var accessToken = _defraAuthenticationService.GetFromCache("accessToken");
            EhcMetadata metadata = await defraTradeApiGateway
                .AppendPathSegments("trade-sci-exports","uat","v1","ehc-metadata")
                .WithOAuthBearerToken(accessToken.access_token)
                .GetJsonAsync<EhcMetadata>(CancellationToken.None);

            return metadata;
        }

        public async Task<dynamic> GetEhcExample(string ehc)
        {
            var accessToken = _defraAuthenticationService.GetFromCache("accessToken");
            var responseContent = await defraTradeApiGateway
                .AppendPathSegments("trade-sci-exports", "uat", "v1", "ehc-application-example", ehc)
                .WithOAuthBearerToken(accessToken.access_token)
                .AllowAnyHttpStatus()
                .GetJsonAsync(CancellationToken.None);

            return responseContent;
        }

        public async Task<dynamic> Create(JObject requestContentParsed)
        {
            var accessToken = _defraAuthenticationService.GetFromCache("accessToken");
            var responseContent = await defraTradeApiGateway
                .AppendPathSegments("trade-sci-exports", "uat", "v1", "ehc-application")
                .WithOAuthBearerToken(accessToken.access_token)
                .AllowAnyHttpStatus()
                .PostJsonAsync(requestContentParsed, CancellationToken.None).ReceiveJson();

            return responseContent;
        }

        public async Task<dynamic> CheckRequestStatus(Guid application)
        {
            var accessToken = _defraAuthenticationService.GetFromCache("accessToken");
            var responseContent = await defraTradeApiGateway
                .AppendPathSegments("trade-sci-exports", "uat", "v1", "ehc-application", application, "request-status")
                .WithOAuthBearerToken(accessToken.access_token)
                .AllowAnyHttpStatus()
                .GetJsonAsync(CancellationToken.None);

            return responseContent;
        }

        public async Task<dynamic> CheckApplicationStatus(Guid application)
        {
            var accessToken = _defraAuthenticationService.GetFromCache("accessToken");
            var responseContent = await defraTradeApiGateway
                .AppendPathSegments("trade-sci-exports", "uat", "v1", "ehc-application", application, "status")
                .WithOAuthBearerToken(accessToken.access_token)
                .AllowAnyHttpStatus()
                .GetJsonAsync(CancellationToken.None);

            return responseContent;
        }
    }
}