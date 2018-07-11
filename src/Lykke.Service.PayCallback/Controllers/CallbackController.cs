using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
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

        public CallbackController(ICallbackService callbackService, ILogFactory logFactory)
        {
            _callbackService = callbackService ?? throw new ArgumentNullException(nameof(callbackService));

            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            _log = logFactory.CreateLog(this);
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
                _log.Error(ex, null, request.ToJson());
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
                _log.Error(ex, null, new
                {
                    merchantId,
                    paymentRequestId
                }.ToJson());
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
