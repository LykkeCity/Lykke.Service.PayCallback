using Common;
using Common.Log;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static readonly HttpClient HttpClient = new HttpClient();

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

            var content = new StringContent(data, Encoding.UTF8, "text/xml");
            var response = await HttpClient.PostAsync(_url, content);

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

            await _repository.AddAsync(invoiceConfirmation);
        }
    }
}
