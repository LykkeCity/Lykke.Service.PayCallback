using System;
using System.Linq;
using AutoMapper;
using Common;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayCallback.Services
{
    public static class ConvertExtensions
    {
        public static PaymentStatusModel ToStatusApiModel(this PaymentRequestDetailsMessage src)
        {
            PaymentStatusModel response = new PaymentStatusModel
            {
                Id = src.Id,
                PaymentAsset = src.PaymentAssetId,
                SettlementAsset = src.SettlementAssetId,
                OrderId = src.OrderId,
                PaymentRequest = new PaymentRequestModel
                {
                    Amount = src.Order?.PaymentAmount,
                    Address = src.WalletAddress,
                    CreatedAt = src.Timestamp.ToIsoDateTime(),
                    ExchangeRate = src.Order?.ExchangeRate,
                    ExpirationDt = src.DueDate.ToIsoDateTime(),
                    Transactions = src.Transactions.Any() ? src.Transactions.Select(Mapper.Map<PaymentTransactionModel>).ToList() : null
                }
            };

            switch (src.Status)
            {
                case PaymentRequestStatus.New:

                    response.PaymentStatus = PaymentRequestPublicStatuses.PaymentRequestCreated;

                    break;
                case PaymentRequestStatus.Confirmed:

                    response.PaymentStatus = PaymentRequestPublicStatuses.PaymentConfirmed;

                    break;
                case PaymentRequestStatus.InProcess:

                    response.PaymentStatus = PaymentRequestPublicStatuses.PaymentInProgress;

                    break;
                case PaymentRequestStatus.Error:

                    response.PaymentStatus = PaymentRequestPublicStatuses.PaymentError;

                    switch (src.Error)
                    {
                        case PaymentRequestErrorType.PaymentAmountAbove:

                            response.PaymentRequest.Error = PaymentRequestErrorPublicCodes.PaymentAmountAbove;

                            break;
                        case PaymentRequestErrorType.PaymentAmountBelow:

                            response.PaymentRequest.Error = PaymentRequestErrorPublicCodes.PaymentAmountBelow;

                            break;
                        case PaymentRequestErrorType.PaymentExpired:

                            response.PaymentRequest.Error = PaymentRequestErrorPublicCodes.PaymentExpired;

                            break;
                        case PaymentRequestErrorType.RefundNotConfirmed:

                            response.RefundRequest = Mapper.Map<RefundRequestModel>(src.Refund);

                            if (response.RefundRequest != null)
                                response.RefundRequest.Error = PaymentRequestErrorPublicCodes.TransactionNotConfirmed;

                            break;
                        default:
                            throw new Exception("Unknown payment request error type");
                    }

                    break;
                case PaymentRequestStatus.RefundInProgress:

                    response.PaymentStatus = PaymentRequestPublicStatuses.RefundInProgress;

                    response.RefundRequest = Mapper.Map<RefundRequestModel>(src.Refund);

                    break;
                case PaymentRequestStatus.Refunded:

                    response.PaymentStatus = PaymentRequestPublicStatuses.RefundConfirmed;

                    response.RefundRequest = Mapper.Map<RefundRequestModel>(src.Refund);

                    break;
                default:
                    throw new Exception("Unknown payment request status");
            }

            return response;
        }
    }
}
