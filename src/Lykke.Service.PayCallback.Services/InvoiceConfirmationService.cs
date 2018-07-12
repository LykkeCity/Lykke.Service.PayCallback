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
using Lykke.Common.Log;
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
            ILogFactory logFactory, string url, string authorization)
        {
            _repository = repository;
            _xmlSerializer = xmlSerializer;
            _url = url;
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authorization);
            _log = logFactory.CreateLog(this);
        }

        public async Task ProcessAsync(InvoiceConfirmation invoiceConfirmation)
        {
            string data = _xmlSerializer.Serialize(invoiceConfirmation);

            _log.Info("Sending data starting", new { data });

            HttpResponseMessage response;
            try
            {
                var content = new StringContent(data, Encoding.UTF8, "text/xml");
                response = await HttpClient.PostAsync(_url, content);
            }
            catch (Exception ex)
            {
                _log.Error(ex, null,new
                {
                    InvoiceConfirmation = data
                }.ToJson());

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

                _log.Error(exception, null, new
                {
                    StatusCode = response.StatusCode,
                    Content = responseContent,
                    InvoiceConfirmation = data
                }.ToJson());

                throw exception;
            }

            _log.Info("Data successfully sent", new { data });

            await _repository.AddAsync(invoiceConfirmation);
        }
    }
}
