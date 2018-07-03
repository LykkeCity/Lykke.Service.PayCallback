using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class DisputeOperation
    {        
        [JsonConverter(typeof(StringEnumConverter))]
        public DisputeStatus Status { get; set; }
        public string Reason { get; set; }
        public DateTime DateTime { get; set; }
    }
}
