using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SupplyChain.ClientApplication.Models;
using SupplyChain.ClientApplication.Service.Interface;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SupplyChain.ClientApplication.Service
{
    public class DefraAuthenticationService : IDefraAuthenticationService
    {
        private readonly IAntiforgery _antiforgery;
        private readonly HttpContext httpContext;
        private readonly IDistributedCache _cache;
        private static readonly object _lock = new object();
        private readonly string b2cBaseUrl;
        private readonly string SignUpSignInPolicy;
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly string[] Scopes;
        private readonly string RedirectUrl;

        public DefraAuthenticationService(IConfiguration configuration, IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
        {
            httpContext = httpContextAccessor.HttpContext;
            _antiforgery = antiforgery;
            _cache = cache;

            var scopes = configuration.GetValue<string>("Scopes");
            Scopes = scopes.Split(' ');

            SignUpSignInPolicy = configuration.GetSection("B2CSettings:SignUpSignInPolicyId").Value;
            ClientId = configuration.GetSection("B2CSettings:ClientId").Value;
            ClientSecret = configuration.GetSection("B2CSettings:ClientSecret").Value;
            RedirectUrl = configuration.GetSection("B2CSettings:RedirectUri").Value;

            b2cBaseUrl = $"{configuration.GetSection("B2CSettings:Authority").Value}/{configuration.GetSection("B2CSettings:Domain").Value}/{SignUpSignInPolicy}/oauth2/v2.0";
        }

        public Uri PrepareAuthoriseRequest()
        {
            var code_verifier = CodeVerifier();
            var code_challenge = CodeChallenge(code_verifier);
            httpContext.Response.Cookies.Append("c", code_verifier);

            return b2cBaseUrl
                .AppendPathSegment("authorize")
                .SetQueryParam("signupsigninpolicyid", SignUpSignInPolicy)
                .SetQueryParam("client_id", ClientId)
                .SetQueryParam("response_type", "code")
                .SetQueryParam("redirect_uri", RedirectUrl)
                .SetQueryParam("response_mode", "query")
                .SetQueryParam("scope", string.Join(" ", Scopes))
                .SetQueryParam("state", _antiforgery.GetAndStoreTokens(httpContext).RequestToken)
                .SetQueryParam("code_challenge", code_challenge)
                .SetQueryParam("code_challenge_method", "S256")
                .SetQueryParam("prompt", "consent")
                .ToUri();
        }

        public async Task<AccessTokenResponse> RequestAccessToken(string idToken)
        {
            AccessTokenResponse response;
            try
            {
                response = await b2cBaseUrl
                    .AppendPathSegment("token")
                    .PostUrlEncodedAsync(new
                    {
                        client_id = ClientId,
                        code_verifier = httpContext.Request.Cookies["c"],
                        client_secret = ClientSecret,
                        code = idToken,
                        grant_type = "authorization_code",
                        redirect_uri = RedirectUrl,
                        scope = string.Join(" ", Scopes)
                    }, CancellationToken.None).ReceiveJson<AccessTokenResponse>();

                AddToCache("accessToken", response);
            }
            catch (FlurlHttpException e)
            {
                response = new AccessTokenResponse
                {
                    access_token = $"{e.Message} - {e.GetResponseStringAsync().Result}"
                };
            }

            return response;
        }

        public AccessTokenResponse GetFromCache(string key)
        {
            AccessTokenResponse accessTokenResponse = new AccessTokenResponse();
            if (_cache == null) return accessTokenResponse;
            var item = _cache.GetString(key);

            if (item != null)
            {
                accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(item);
            }
            return accessTokenResponse;
        }

        #region Private

        private void AddToCache(string key, AccessTokenResponse accessTokenItem)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));
 
            lock (_lock)
            {
                _cache.SetString(key, JsonConvert.SerializeObject(accessTokenItem), options);
            }
        }

        private static string CodeVerifier()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string CodeChallenge(string verifier)
        {
            var hashProvider = SHA256.Create();
            var challengeBytes = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            var code_challenge = Convert.ToBase64String(challengeBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            return code_challenge;
        }

        #endregion

    }
}