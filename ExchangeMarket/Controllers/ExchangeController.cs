using ExchangeMarket.Model;
using ExchangeMarket.Model.Exceptions;
using ExchangeMarket.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExchangeMarket.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/exchange")]
    public class ExchangeController : Controller
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<ExchangeController> _logger;

        public ExchangeController(IExchangeService exchangeService, ILogger<ExchangeController> logger)
        {
            _exchangeService = exchangeService;
            _logger = logger;
        }

        [HttpGet("{Currency}")]
        [SwaggerOperation(
            Summary = "Get the current rate of the provided Currency", 
            Description = "Provided a supported ISO Currency (BRL or USD at the moment) you get its current rate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetExchangeAsync(string Currency)
        {
            if (string.IsNullOrEmpty(Currency) || !Regex.IsMatch(Currency, @"[a-zA-Z]+"))
                throw new InvalidCurrencyException(Currency);

            return Json(await _exchangeService.GetExchangeRateByISOCodeAsync(Currency));
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Purchase of currency",
            Description = "Provided an Amount (expressed in argentian pesos), Currency (BRL or USD) and User Id, you can try to buy currency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> BuyCurrencyAsync([FromBody]BuyCurrencyOrder model)
        {
            await _exchangeService.BuyCurrency(model);
            _logger.LogInformation($"User: {model.UserId} Bought {model.Currency}");
            return Ok();
        }
    }
}
