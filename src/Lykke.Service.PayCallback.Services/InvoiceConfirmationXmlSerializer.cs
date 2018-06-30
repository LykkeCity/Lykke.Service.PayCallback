using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using Lykke.Service.PayCallback.Core.Services;
using System.Xml.Linq;

namespace Lykke.Service.PayCallback.Services
{
    public class InvoiceConfirmationXmlSerializer : IInvoiceConfirmationXmlSerializer
    {
        public string Serialize(IInvoiceConfirmation invoiceConfirmation)
        {
            XNamespace ns = "http://schemas.datacontract.org/2004/07/LykkeRESTService";
            XNamespace nsi = "http://www.w3.org/2001/XMLSchema-instance";
            var xmlInvoiceConfirmation = new XElement(ns + "InvoiceConfirmation",
                new XAttribute(XNamespace.Xmlns + "i", nsi));

            var xmlUserEmail = new XElement("UserEmail", invoiceConfirmation.UserEmail);
            xmlInvoiceConfirmation.Add(xmlUserEmail);

            var xmlInvoiceList = new XElement("InvoiceList");
            xmlInvoiceConfirmation.Add(xmlInvoiceList);

            foreach (var invoiceOperation in invoiceConfirmation.InvoiceList)
            {
                var xmlInvoice = new XElement("Invoice");
                xmlInvoiceList.Add(xmlInvoice);

                var xmlInvoiceNumber = new XElement("InvoiceNumber", invoiceOperation.InvoiceNumber);
                xmlInvoice.Add(xmlInvoiceNumber);

                AddIfNotNull(xmlInvoice, "AmountPaid", invoiceOperation.AmountPaid);
                AddIfNotNull(xmlInvoice, "AmountLeftPaid", invoiceOperation.AmountLeftPaid);

                if (invoiceOperation.Dispute != null)
                {
                    var xmlDispute = new XElement("Dispute");
                    xmlInvoice.Add(xmlDispute);

                    AddIfNotNull(xmlDispute, "Status", invoiceOperation.Dispute.Status);
                    AddIfNotNull(xmlDispute, "Reason", invoiceOperation.Dispute.Reason);
                    AddIfNotNull(xmlDispute, "DateTime", invoiceOperation.Dispute.DateTime.ToString("s"));
                }
            }

            var xmlSettledInBlockchainDateTime = new XElement("SettledInBlockchainDateTime", invoiceConfirmation.SettledInBlockchainDateTime.ToString("s"));
            xmlInvoiceConfirmation.Add(xmlSettledInBlockchainDateTime);

            var xmlBlockchainHash = new XElement("BlockchainHash", invoiceConfirmation.BlockchainHash);
            xmlInvoiceConfirmation.Add(xmlBlockchainHash);

            return xmlInvoiceConfirmation.ToString().Replace(" xmlns=\"\"",string.Empty);
        }

        private XElement AddIfNotNull(XElement root, string name, object value)
        {
            if (value == null)
            {
                return null;
            }

            var element = new XElement(name, value);
            root.Add(element);

            return element;
        }
    }
}
