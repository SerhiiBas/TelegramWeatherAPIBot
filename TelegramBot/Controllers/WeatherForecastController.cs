using Common.Exceptions;
using Core.Abstractions;
using Core.GeocodingApi.GeocodingApiInterfaces;
using Core.WathersAPI.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace TelegramBot.Controllers
{
    public class WeatherForecastController : ControllerBase
    {
        private readonly ITelegramBotServise _telegramBotServise;

        public WeatherForecastController(IGeocodingApiService geocoding, IWeatherAPIService wather, ITelegramBotServise telegramBotServise)
        {
            this._telegramBotServise = telegramBotServise;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] object update, CancellationToken cancellationToken = default)
        {
            Contract.NotNull(update, nameof(update));

            var request = JsonConvert.DeserializeObject<Update>(update?.ToString());

            Contract.NotNull(request, nameof(request));

            await _telegramBotServise.HandelMassage(request, cancellationToken);
            
            return Ok();
        }

    }
}