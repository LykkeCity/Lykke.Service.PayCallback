﻿using System;
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
                case PaymentRequestStatus.Cancelled:

                    response.PaymentStatus = PaymentRequestPublicStatuses.PaymentCancelled;

                    break;
                case PaymentRequestStatus.Error:

                    switch (src.ProcessingError)
                    {
                        case PaymentRequestProcessingError.PaymentAmountAbove:

                            response.PaymentStatus = PaymentRequestPublicStatuses.PaymentError;

                            response.Error = new ErrorModel {Code = PaymentRequestErrorPublicCodes.PaymentAmountAbove};

                            break;
                        case PaymentRequestProcessingError.PaymentAmountBelow:

                            response.PaymentStatus = PaymentRequestPublicStatuses.PaymentError;

                            response.Error = new ErrorModel {Code = PaymentRequestErrorPublicCodes.PaymentAmountBelow};

                            break;
                        case PaymentRequestProcessingError.PaymentExpired:

                            response.PaymentStatus = PaymentRequestPublicStatuses.PaymentError;

                            response.Error = new ErrorModel {Code = PaymentRequestErrorPublicCodes.PaymentExpired};

                            break;
                        case PaymentRequestProcessingError.LatePaid:

                            response.PaymentStatus = PaymentRequestPublicStatuses.PaymentError;

                            response.Error = new ErrorModel {Code = PaymentRequestErrorPublicCodes.LatePaid};

                            break;
                        case PaymentRequestProcessingError.RefundNotConfirmed:

                            response.PaymentStatus = PaymentRequestPublicStatuses.RefundError;

                            response.RefundRequest = Mapper.Map<RefundRequestModel>(src.Refund);

                            response.Error = new ErrorModel
                                {Code = PaymentRequestErrorPublicCodes.TransactionNotConfirmed};

                            break;
                        case PaymentRequestProcessingError.UnknownPayment:

                            response.PaymentStatus = PaymentRequestPublicStatuses.PaymentError;

                            break;
                        case PaymentRequestProcessingError.UnknownRefund:

                            response.PaymentStatus = PaymentRequestPublicStatuses.RefundError;

                            response.RefundRequest = Mapper.Map<RefundRequestModel>(src.Refund);

                            response.Error = new ErrorModel
                            { Code = PaymentRequestErrorPublicCodes.RefundIsNotAvailable };

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
