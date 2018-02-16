using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.PayCallback.Core.Services;
using Lykke.Service.PayCallback.Filter;
using Lykke.Service.PayCallback.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayCallback.Controllers
{
    [Route("api/[controller]")]
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
        /// Adds new payment callback
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerOperation("AddPaymentCallback")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddPaymentCallback([FromBody] CreatePaymentCallbackRequest request)
        {
            try
            {
                await _callbackService.CreatePaymentCallback(request.ToDomain());

                return Ok();
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CallbackController), nameof(AddPaymentCallback), request.ToJson(), ex);
            }

            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }
}
