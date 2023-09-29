using NablaUtils.SimpleInBlue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public interface ITransactionalEmailApi
    {
        Task<CreateSmtpEmail> SendTransactionalEmailAsync(SendSmtpEmail sendSmtpEmail);
    }
}
