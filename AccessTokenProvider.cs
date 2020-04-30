using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace NablaUtils
{
    /// <summary>
    /// Validates a incoming request and extracts any <see cref="ClaimsPrincipal"/> contained within the bearer token.
    /// </summary>
    public static class AccessTokenProvider /* : IAccessTokenProvider */
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";

        private static readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        static AccessTokenProvider()
        {
            /*_issuerToken = issuerToken;
            _audience = audience;
            _issuer = issuer;*/
            var domain = Environment.GetEnvironmentVariable("DOMAIN");
            var issuer = $"https://{domain}/";

            var documentRetriever = new HttpDocumentRetriever();
            documentRetriever.RequireHttps = issuer.StartsWith("https://");

            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuer}.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        public static bool HasTokenInRequest(HttpRequest request)
        {
            return (request != null && request.Headers.ContainsKey(AUTH_HEADER_NAME) && request.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX));
        }

        public static string ExtractTokenFromRequest(HttpRequest request)
        {
            if (!HasTokenInRequest(request))
            {
                return "";
            }
            var token = request.Headers[AUTH_HEADER_NAME].ToString().Substring(BEARER_PREFIX.Length);
            return token;
        }

        public static async Task<AccessTokenResult> ValidateTokenAsync(HttpRequest request)
        {
            var config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
            var domain = Environment.GetEnvironmentVariable("DOMAIN");
            var issuer = $"https://{domain}/";
            var audience = Environment.GetEnvironmentVariable("AUDIENCE");

            try
            {
                if (HasTokenInRequest(request))
                {
                    var token = ExtractTokenFromRequest(request);

                    // Create the parameters
                    var tokenParams = new TokenValidationParameters()
                    {
                        RequireSignedTokens = true,
                        ValidAudience = audience,
                        ValidateAudience = true,
                        ValidIssuer = issuer,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKeys = config.SigningKeys
                    };

                    var handler = new JwtSecurityTokenHandler();
                    var result = handler.ValidateToken(token, tokenParams, out var securityToken);
                    return AccessTokenResult.Success(result);
                }
                else
                {
                    return AccessTokenResult.NoToken();
                }
            }
            catch (SecurityTokenExpiredException)
            {
                return AccessTokenResult.Expired();
            }
            catch (Exception ex)
            {
                return AccessTokenResult.Error(ex);
            }
        }

        public static async Task<UserInfo> GetUserInfoAsync(HttpRequest req)
        {
            var domain = Environment.GetEnvironmentVariable("DOMAIN");
            var apiClient = new AuthenticationApiClient(domain);
            var token = ExtractTokenFromRequest(req);
            var userInfo = await apiClient.GetUserInfoAsync(token);
            return userInfo;
        }
    }
}
