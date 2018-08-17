using Lykke.Common.Api.Contract.Responses;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    [Serializable]
    public class InvoiceConfirmationException : Exception
    {
        public IDictionary<string, List<string>> ModelErrors { get; }

        public InvoiceConfirmationException()
        {
        }

        public InvoiceConfirmationException(ErrorResponse errorResponse)
            : base(errorResponse.ErrorMessage)
        {
            ModelErrors = errorResponse.ModelErrors;
        }

        public InvoiceConfirmationException(string message,
            IDictionary<string, List<string>> modelErrors = null)
            : base(message)
        {
            ModelErrors = modelErrors;
        }

        public InvoiceConfirmationException(string message,
            Exception innerException,
            IDictionary<string, List<string>> modelErrors = null)
            : base(message, innerException)
        {
            ModelErrors = modelErrors;
        }

        protected InvoiceConfirmationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
