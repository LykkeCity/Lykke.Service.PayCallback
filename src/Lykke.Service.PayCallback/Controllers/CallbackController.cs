using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayCallback.Core.Services;
using Lykke.Service.PayCallback.Filter;
using Lykke.Service.PayCallback.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RabbitMQ.Client.Apigen.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayCallback.Controllers
{
    [Route("api/callback")]
    public class CallbackController : Controller
    {
        private readonly ILog _log;
        private readonly ICallbackService _callbackService;

        public CallbackController(ICallbackService callbackService, ILog log)
        {
            _callbackService = callbackService ?? throw new ArgumentNullException(nameof(callbackService));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>
        /// Adds or updates existing payment request callback url
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("SetPaymentCallback")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SetPaymentCallback([FromBody] SetPaymentCallbackRequest request)
        {
            try
            {
                await _callbackService.SetPaymentRequestCallback(Mapper.Map<SetPaymentRequestCallbackCommand>(request));

                return Ok();
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CallbackController), nameof(SetPaymentCallback), request.ToJson(), ex);
            }

            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [Route("{merchantId}/{paymentRequestId}")]
        [SwaggerOperation("GetPaymentCallback")]
        [ProducesResponseType(typeof(PaymentCallbackResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPaymentCallback(string merchantId, string paymentRequestId)
        {
            try
            {
                IPaymentCallback callback = await _callbackService.GetPaymentRequestCallback(merchantId, paymentRequestId);

                if (callback == null)
                    return NotFound(ErrorResponse.Create("Callback information not found"));

                return Ok(Mapper.Map<PaymentCallbackResponse>(callback));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CallbackController), nameof(GetPaymentCallback), new
                {
                    merchantId,
                    paymentRequestId
                }.ToJson(), ex);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
