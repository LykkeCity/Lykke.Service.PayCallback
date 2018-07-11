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
        private static readonly HttpClient HttpClient;

        static InvoiceConfirmationService()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            HttpClient = new HttpClient(handler);
        }

        public InvoiceConfirmationService(IInvoiceConfirmationRepository repository,
            IInvoiceConfirmationXmlSerializer xmlSerializer,
            ILog log, string url, string authorization)
        {
            _repository = repository;
            _xmlSerializer = xmlSerializer;
            _url = url;
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
