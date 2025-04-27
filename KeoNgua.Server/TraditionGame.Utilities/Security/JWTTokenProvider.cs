using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace TraditionGame.Utilities.Security
{
    public class JWTTokenProvider
    {
        // private const string Secret = "NzM3MzYzamRoZGhkaGRiM09Jc2orQlhFOU5aRHkwdDhXM1RjTmVrckYrMmQvMXNGbldHNEhuVjhUWlkzMGlUT2R0VldKRzhhYld2QjFHbE9nSnVRWmRjRjJMdXFtL2hjY013PT0zNTM=";
        private const string Secret = "1e4f6a3b9e0a2d4cf81d47f6c9cbd780";
        public static string GenerateToken(long userId, string username, int expireMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var secToken = new JwtSecurityToken(
                signingCredentials: credentials,
                issuer: "Sample",
                audience: "Sample",
                claims: new[]
                {

                   new Claim(ClaimTypes.Name, username),
                  new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                },

                expires: DateTime.UtcNow.AddMinutes(expireMinutes));

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        private static bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);

            return true;
        }
        public static long UserID(string authToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                DateTime validateTo = validatedToken.ValidTo;
                var localDate = TimeZone.CurrentTimeZone.ToLocalTime(validateTo);
                if (DateTime.Now > localDate)
                {
                    return 0;
                }
                return ConvertUtil.ToLong(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }

        }
        public static string UserName(string authToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                DateTime validateTo = validatedToken.ValidTo;
                var localDate = TimeZone.CurrentTimeZone.ToLocalTime(validateTo);
                if (DateTime.Now > localDate)
                {
                    return string.Empty;
                }
                return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return string.Empty;
            }

        }
        private static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)) // The same key as the one that generate the token
            };
        }


    }
}
