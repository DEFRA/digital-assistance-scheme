namespace SupplyChain.ClientApplication.Models
{
    public class AccessToken
    {
        public string client_id { get; set; }
        public string scope { get; set; }
        public string code_verifier { get; set; }
        public string redirect_uri { get; set; }
        public string grant_type { get; set; } = "authorization_code";
        public string client_secret { get; set; }
        public string code { get; set; }
    }

    public class AccessTokenResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }
}