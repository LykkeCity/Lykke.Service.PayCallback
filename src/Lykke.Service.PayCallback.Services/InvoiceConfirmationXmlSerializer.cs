using System.Linq;
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

            CreateIfNotNull(xmlInvoiceConfirmation, "UserEmail",
                invoiceConfirmation.UserEmail);

            AddIfNotNull(xmlInvoiceConfirmation, CreateInvoiceList(invoiceConfirmation.InvoiceList));
            AddIfNotNull(xmlInvoiceConfirmation, CreateCashOut(invoiceConfirmation.CashOut));

            CreateIfNotNull(xmlInvoiceConfirmation, "SettledInBlockchainDateTime",
                invoiceConfirmation.SettledInBlockchainDateTime?.ToString("s"));
            CreateIfNotNull(xmlInvoiceConfirmation, "BlockchainHash",
                invoiceConfirmation.BlockchainHash);

            return xmlInvoiceConfirmation.ToString().Replace(" xmlns=\"\"",string.Empty);
        }

        private XElement CreateInvoiceList(InvoiceOperation[] invoiceList)
        {
            var xmlInvoiceList = new XElement("InvoiceList");
            if (invoiceList != null)
            {
                foreach (var invoiceOperation in invoiceList)
                {
                    var xmlInvoice = new XElement("Invoice");
                    xmlInvoiceList.Add(xmlInvoice);

                    var xmlInvoiceNumber = new XElement("InvoiceNumber", invoiceOperation.InvoiceNumber);
                    xmlInvoice.Add(xmlInvoiceNumber);

                    CreateIfNotNull(xmlInvoice, "AmountPaid", invoiceOperation.AmountPaid);
                    CreateIfNotNull(xmlInvoice, "AmountLeftPaid", invoiceOperation.AmountLeftPaid);

                    if (invoiceOperation.Dispute != null)
                    {
                        var xmlDispute = new XElement("Dispute");
                        xmlInvoice.Add(xmlDispute);

                        CreateIfNotNull(xmlDispute, "Status", invoiceOperation.Dispute.Status.ToString());
                        CreateIfNotNull(xmlDispute, "Reason", invoiceOperation.Dispute.Reason);
                        CreateIfNotNull(xmlDispute, "DateTime", invoiceOperation.Dispute.DateTime.ToString("s"));
                    }
                }
            }

            return xmlInvoiceList;
        }

        private XElement CreateCashOut(CashOut cashOut)
        {
            if (cashOut == null)
            {
                return null;
            }

            var xmlCashOut = new XElement("CashOut");

            var xmlAmount = new XElement("Amount", cashOut.Amount);
            xmlCashOut.Add(xmlAmount);

            var xmlCurrency = new XElement("Currency", cashOut.Currency);
            xmlCashOut.Add(xmlCurrency);

            return xmlCashOut;
        }

        private XElement CreateIfNotNull(XElement root, string name, object value)
        {
            if (value == null)
            {
                return null;
            }

            var element = new XElement(name, value);
            return AddIfNotNull(root, element);
        }

        private XElement AddIfNotNull(XElement root, XElement child)
        {
            if (child != null)
            {
                root.Add(child);
            }
            
            return child;
        }
    }
}
