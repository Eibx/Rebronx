using System;
using System.Security.Cryptography;
using Rebronx.Server.Repositories;

namespace Rebronx.Server.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public string GenerateUniqueToken()
        {
            var token = GenerateToken();

            while (!_tokenRepository.IsTokenAvailable(token))
            {
                token = GenerateToken();
            }

            return token;
        }

        private string GenerateToken()
        {
            var bytes = new byte[32];
            var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}