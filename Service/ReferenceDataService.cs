using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using SupplyChain.ClientApplication.Models;
using SupplyChain.ClientApplication.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SupplyChain.ClientApplication.Service
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly IDefraAuthenticationService _defraAuthenticationService;
        private readonly string defraTradeApiGateway;

        public ReferenceDataService(IConfiguration configuration, IDefraAuthenticationService defraAuthenticationService)
        {
            _defraAuthenticationService = defraAuthenticationService;
            defraTradeApiGateway = configuration.GetSection("DefraTradeGateway").Value;
        }

        public async Task<List<ReferenceDataMeta>> GetEhcMetadata()
        {
            List<ReferenceDataMeta> metadata;
            try
            {
                var accessToken = _defraAuthenticationService.GetFromCache("accessToken");

                metadata = await defraTradeApiGateway
                    .AppendPathSegments("trade-sci-reference-data", "uat", "v1", "metadata")
                    .WithOAuthBearerToken(accessToken.access_token)
                    .GetJsonAsync<List<ReferenceDataMeta>>(CancellationToken.None);
            }
            catch (Exception)
            {
                metadata = new List<ReferenceDataMeta>(0);
            }

            return metadata;
        }

        public async Task<dynamic> GetRefDataWithDynamicEndpoint(string endpoint)
        {
            dynamic metadata;
            try
            {
                var accessToken = _defraAuthenticationService.GetFromCache("accessToken");

                metadata = await defraTradeApiGateway
                    .AppendPathSegments("trade-sci-reference-data", "uat", "v1", endpoint)
                    .WithOAuthBearerToken(accessToken.access_token)
                    .GetJsonAsync(CancellationToken.None);
            }
            catch (FlurlHttpException e)
            {
                metadata = await e.GetResponseJsonAsync();
            }

            return metadata;
        }
    }
}