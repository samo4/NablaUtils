using SummerBack.NablaUtils.SimpleInBlue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerBack.NablaUtils
{
    public static class EmailSettings
    {
        public static EmailAddress GetDefaultEmailSender()
        {
            var email = Environment.GetEnvironmentVariable("DEFAULT_SENDER_EMAIL", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("DEFAULT_SENDER_EMAILnot configured");
            }
            var name = Environment.GetEnvironmentVariable("DEFAULT_SENDER_NAME", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("DEFAULT_SENDER_NAME not configured");
            }
            return new EmailAddress { Email = email, Name = name };
        }
    }
}
