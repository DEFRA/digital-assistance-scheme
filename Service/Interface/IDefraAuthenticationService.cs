using SupplyChain.ClientApplication.Models;
using System;
using System.Threading.Tasks;

namespace SupplyChain.ClientApplication.Service.Interface
{
    public interface IDefraAuthenticationService
    {
        Uri PrepareAuthoriseRequest();
        Task<AccessTokenResponse> RequestAccessToken(string idToken);
        AccessTokenResponse GetFromCache(string key);
    }
}