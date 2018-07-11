using System;
using Common;
using Common.Log;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.PayCallback.Core.Services;

namespace Lykke.Service.PayCallback.Services
{
    public class InvoiceConfirmationService : IInvoiceConfirmationService
    {
        private readonly IInvoiceConfirmationRepository _repository;
        private readonly IInvoiceConfirmationXmlSerializer _xmlSerializer;
        private readonly string _url;
        private readonly ILog _log;
        private readonly HttpClient HttpClient;

        private bool ServerCertificateCustomValidationCallback(
            HttpRequestMessage httpRequestMessage,
            X509Certificate2 x509Certificate2, X509Chain arg3, SslPolicyErrors arg4)
        {
            _log.WriteInfo(nameof(ServerCertificateCustomValidationCallback),
                ExportToPEM(x509Certificate2),
                "Certificate.");

            return true;
        }

        /// <summary>
        /// Export a certificate to a PEM format string
        /// </summary>
        /// <param name="cert">The certificate to export</param>
        /// <returns>A PEM encoded string</returns>
        public static string ExportToPEM(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        public InvoiceConfirmationService(IInvoiceConfirmationRepository repository,
            IInvoiceConfirmationXmlSerializer xmlSerializer,
            ILog log, string url, string authorization)
        {
            _repository = repository;
            _xmlSerializer = xmlSerializer;
            _url = url;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += ServerCertificateCustomValidationCallback;
            HttpClient = new HttpClient(handler);

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authorization);
            _log = log;
        }

        public async Task ProcessAsync(InvoiceConfirmation invoiceConfirmation)
        {
            string data = _xmlSerializer.Serialize(invoiceConfirmation);

            _log.WriteInfo(nameof(ProcessAsync), new { data }, "Sending data starting");

            HttpResponseMessage response;
            try
            {
                var content = new StringContent(data, Encoding.UTF8, "text/xml");
                response = await HttpClient.PostAsync(_url, content);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(InvoiceConfirmationService), nameof(ProcessAsync), new
                {
                    InvoiceConfirmation = data
                }.ToJson(), ex);

                throw;
            }            

            if (!response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var exception = new InvoiceConfirmationException("Invoice confirmation post failed.")
                {
                    StatusCode = response.StatusCode,
                    Content = responseContent
                };

                await _log.WriteErrorAsync(nameof(InvoiceConfirmationService), nameof(ProcessAsync), new
                {
                    StatusCode = response.StatusCode,
                    Content = responseContent,
                    InvoiceConfirmation = data
                }.ToJson(), exception);

                throw exception;
            }

            _log.WriteInfo(nameof(ProcessAsync), new { data }, "Data successfully sent");

            await _repository.AddAsync(invoiceConfirmation);
        }
    }
}
