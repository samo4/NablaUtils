using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SummerBack.NablaUtils.SimpleInBlue
{
    public class EmailsApi : ITransactionalEmailApi
    {
        private readonly HttpClient _client;

        public EmailsApi(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        public static bool HasConfiguration()
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SENDINBLUE_APIKEY", EnvironmentVariableTarget.Process));
        }

        public async Task<CreateSmtpEmail> SendTransactionalEmailAsync(SendSmtpEmail message)
        {
            if(message == null) { throw new ArgumentNullException(nameof(message)); }

            var sendInBlueApiKey = Environment.GetEnvironmentVariable("SENDINBLUE_APIKEY", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(sendInBlueApiKey))
            {
                throw new Exception("sending not configured SENDINBLUE_APIKEY");
            }

            var allowedRecipientsRaw = Environment.GetEnvironmentVariable("ALLOWED_RECIPIENTS", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(allowedRecipientsRaw))
            {
                throw new Exception("ALLOWED_RECIPIENTS not configured");
            }

            var allowedRecipients = allowedRecipientsRaw.Split(",").ToList();

            if (!allowedRecipients.Contains("*"))
            {
                message.To = message.To.Where(to => allowedRecipients.Where(ar => to.Email.EndsWith(ar.Trim())).Any()).ToList();
            }

            if (message.To.Count == 0)
            {
                throw new EmailsApiException("nothing_to_send", "No recipients. Were they all removed by ALLOWED_RECIPIENTS");
            }

            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.sendinblue.com/v3/smtp/email"),
                Content = content,
            };

            request.Headers.Add("api-key", sendInBlueApiKey);

            var response = await _client.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            var successResult = JsonConvert.DeserializeObject<CreateSmtpEmail>(raw);
            if (successResult?.MessageId != null)
            {
                return successResult;
            }
            var errorResult = JsonConvert.DeserializeObject<EmailsApiError>(raw);
            throw new EmailsApiException(errorResult.Code, errorResult.Message);
        }
    }
}
