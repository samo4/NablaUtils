using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SummerBack.NablaUtils
{
    public class SendSmtpEmail
    {
        [JsonProperty("sender")]
        public EmailAddress Sender { get; set; }

        [JsonProperty("to")]
        public List<EmailAddress> To { get; set; }

        [JsonProperty("htmlContent")]
        public string HtmlContent { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("attachment")]
        public List<EmailAttachment> Attachment { get; set; }

        //public long? TemplateId { get; set; }
        //public Object Params { get; set; }
    }
}
