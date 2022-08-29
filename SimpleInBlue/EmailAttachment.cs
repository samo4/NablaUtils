using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SummerBack.NablaUtils
{
    public record EmailAttachment
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonIgnore]
        public byte[] ContentBytes 
        { 
            set 
            { 
                Content = Convert.ToBase64String(value); 
            } 
            get
            {
                return Convert.FromBase64String(Content);
            }
        }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
