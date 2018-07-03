using Common;
using Common.Log;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using System.Net.Http;
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
            ILog log, string url)
        {
            _repository = repository;
            _xmlSerializer = xmlSerializer;
            _url = url;
            _log = log;
        }

        public async Task ProcessAsync(InvoiceConfirmation invoiceConfirmation)
        {
            string data = _xmlSerializer.Serialize(invoiceConfirmation);
            var response = await HttpClient.PostAsync(_url, new StringContent(data, 
                Encoding.UTF8, "application/xml"));

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
