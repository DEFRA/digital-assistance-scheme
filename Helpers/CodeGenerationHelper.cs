using System;
using System.Security.Cryptography;
using System.Text;

namespace SupplyChain.ClientApplication.Helpers
{
    public class CodeGenerationHelper
    {
        public string CodeVerifier()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public string CodeChallenge(string verifier)
        {
            var hashProvider = SHA256.Create();
            var challengeBytes = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            var code_challenge = Convert.ToBase64String(challengeBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            return code_challenge;
        }
    }
}
