using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation
{
    [Serializable]
    public class InvoiceConfirmationException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }

        public InvoiceConfirmationException()
        {
        }

        public InvoiceConfirmationException(string message):base(message)
        {
        }

        protected InvoiceConfirmationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
