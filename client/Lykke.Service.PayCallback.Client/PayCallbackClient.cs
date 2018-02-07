using System;
using Common.Log;

namespace Lykke.Service.PayCallback.Client
{
    public class PayCallbackClient : IPayCallbackClient, IDisposable
    {
        private readonly ILog _log;

        public PayCallbackClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
