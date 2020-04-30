using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace NablaUtils
{
    public enum AccessTokenStatus
    {
        Valid,
        Expired,
        Error,
        NoToken
    }

    /// <summary>
    /// Contains the result of an access token check.
    /// </summary>
    public sealed class AccessTokenResult
    {
        private AccessTokenResult() { }

        public ClaimsPrincipal Principal { get; private set; }

        public string Sub
        {
            get
            {
                if (Principal == null || Principal.Claims == null)
                {
                    return null;
                }
                var nameIdentifier = Principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (nameIdentifier == null || nameIdentifier.Count() < 1)
                {
                    return null;
                }
                return nameIdentifier.Select(c => c.Value).FirstOrDefault();
            }
        }

        public AccessTokenStatus Status { get; private set; }
        public Exception Exception { get; private set; }

        public StatusCodeResult HttpResult
        {
            get
            {
                switch (Status)
                {
                    case AccessTokenStatus.Error:
                        return new StatusCodeResult(500);
                    case AccessTokenStatus.NoToken:
                        return new StatusCodeResult(403); // Forbidden
                    case AccessTokenStatus.Expired:
                        return new StatusCodeResult(401); // Unauthorized
                    default:
                        return null;
                }
            }
        }

        public bool IsValid { get { return Status == AccessTokenStatus.Valid; } }

        public static AccessTokenResult Success(ClaimsPrincipal principal)
        {
            return new AccessTokenResult
            {
                Principal = principal,
                Status = AccessTokenStatus.Valid
            };
        }

        public static AccessTokenResult Expired()
        {
            return new AccessTokenResult
            {
                Status = AccessTokenStatus.Expired
            };
        }

        public static AccessTokenResult Error(Exception ex)
        {
            return new AccessTokenResult
            {
                Status = AccessTokenStatus.Error,
                Exception = ex
            };
        }

        public static AccessTokenResult NoToken()
        {
            return new AccessTokenResult
            {
                Status = AccessTokenStatus.NoToken
            };
        }
    }
}