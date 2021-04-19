using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SupplyChain.ClientApplication.Helpers;
using SupplyChain.ClientApplication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;

namespace SupplyChain.ClientApplication.Controllers
{
    public class DemoController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly string tokenEndpoint;
        private readonly string authorizeEndpoint;
        private readonly string SignUpSignInPolicy;
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly string[] Scopes;
        private readonly string RedirectUrl;

        public DemoController(IConfiguration configuration, IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
            var scopes = configuration.GetValue<string>("Scopes");
            Scopes = scopes.Split(' ');
            SignUpSignInPolicy = configuration.GetSection("B2CSettings:SignUpSignInPolicyId").Value;
            ClientId = configuration.GetSection("B2CSettings:ClientId").Value;
            ClientSecret = configuration.GetSection("B2CSettings:ClientSecret").Value;
            RedirectUrl = configuration.GetSection("B2CSettings:RedirectUrl").Value;

            var B2CBaseUrl =
                $"{configuration.GetSection("B2CSettings:Authority").Value}/{configuration.GetSection("B2CSettings:Domain").Value}/{SignUpSignInPolicy}/oauth2/v2.0";
            tokenEndpoint = $"{B2CBaseUrl}/token";
            authorizeEndpoint = $"{B2CBaseUrl}/authorize";
        }

        [Route("Begin-Demo")]
        public IActionResult StartDemo()
        {
            CodeGenerationHelper _codeGeneration = new CodeGenerationHelper();
            var code_verifier = _codeGeneration.CodeVerifier();
            var code_challenge = _codeGeneration.CodeChallenge(code_verifier);
            HttpContext.Response.Cookies.Append("c", code_verifier);
            
            var url = authorizeEndpoint
                .SetQueryParam("signupsigninpolicyid", SignUpSignInPolicy)
                .SetQueryParam("client_id", ClientId)
                .SetQueryParam("response_type", "code")
                .SetQueryParam("redirect_uri", RedirectUrl)
                .SetQueryParam("response_mode", "query")
                .SetQueryParam("scope", string.Join(" ", Scopes))
                .SetQueryParam("state", _antiforgery.GetAndStoreTokens(HttpContext).RequestToken)
                .SetQueryParam("code_challenge", code_challenge)
                .SetQueryParam("code_challenge_method", "S256")
                .SetQueryParam("prompt", "consent")
                .ToUri();

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
            AccessTokenResponse response;
            try
            {
                response =
                    await
                        tokenEndpoint
                            .PostUrlEncodedAsync(new
                            {
                                client_id = ClientId,
                                code_verifier = HttpContext.Request.Cookies["c"],
                                client_secret = ClientSecret,
                                code,
                                grant_type = "authorization_code",
                                redirect_uri = RedirectUrl,
                                scope = string.Join(" ", Scopes)
                            }).ReceiveJson<AccessTokenResponse>();
            }
            catch (FlurlHttpException e)
            {
                response = new AccessTokenResponse
                {
                    access_token = $"{e.Message} - {e.GetResponseStringAsync().Result}"
                };
            }

            return View(response);
        }

        [Route("API-Call")]
        public async Task<IActionResult> MakeApiCall(string access)
        {
            EhcMetadata metadata;
            ViewBag.Error = "";
            try
            {
                metadata = await "https://gateway.trade.defra.gov.uk/trade-sci-exports/uat/v1"
                    .AppendPathSegment("ehc-metadata")
                    .WithHeader("x-api-version", 1)
                    .WithOAuthBearerToken(access)
                    .GetJsonAsync<EhcMetadata>();
            }
            catch (FlurlHttpException e)
            {
                metadata = new EhcMetadata();
                ViewBag.Error = $"{e.Message} ({e.GetResponseStringAsync().Result})";
            }

            var jwt = access;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            ViewBag.Token = token.Claims;

            return View(metadata);
        }
    }
}