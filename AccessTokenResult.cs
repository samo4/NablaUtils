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

        public ObjectResult HttpResult
        {
            get
            {
                ObjectResult result;
                if (IsValid) { result = new OkObjectResult(new { status = "ok", message = "Authenticated." }); }
                else
                {
                    result = new ObjectResult(new { status = "error", message = Status.ToString() });
                    switch (Status)
                    {
                        case AccessTokenStatus.Error:
                            result = new ObjectResult(new { status = "error", message = Exception.Message, stackTrace = Exception.StackTrace });
                            result.StatusCode = 500;
                            break;
                        case AccessTokenStatus.NoToken:
                            result.StatusCode = 403;
                            break;
                        case AccessTokenStatus.Expired:
                            result.StatusCode = 401;
                            break;
                        default:
                            result.StatusCode = 200;
                            break;
                    }
                }
                return result;
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