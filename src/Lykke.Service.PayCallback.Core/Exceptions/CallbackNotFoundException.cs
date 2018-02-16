using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayCallback.Core.Exceptions
{
    public class CallbackNotFoundException : Exception
    {
        public CallbackNotFoundException()
        {
        }

        public CallbackNotFoundException(string paymentRequestId) : base("Callback not found")
        {
            PaymentRequestId = paymentRequestId;
        }

        public CallbackNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CallbackNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string PaymentRequestId { get; set; }
    }
}
