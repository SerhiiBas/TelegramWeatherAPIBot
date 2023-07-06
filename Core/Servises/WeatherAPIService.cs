using Common.Exceptions;
using Core.WathersAPI.Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using VM.GeocodingApiVM;
using VM.WeatherAPIVM;

namespace Core.WeathersAPI.WeatherApiService
{
    public class WeatherAPIService : IWeatherAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly WatherAPILink _options;

        public WeatherAPIService(HttpClient httpClient, IOptions<WatherAPILink> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<WeatherApiRoot> GetWather(GeocodingLocation location, CancellationToken cancellationToken = default)
        {
            string CultureName = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(CultureName);
            if (ci.NumberFormat.NumberDecimalSeparator != ".")
            {
                // Forcing use of decimal separator for numerical values
                ci.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = ci;
            }

            Contract.NotNull(location, nameof(location));
            string WatherAPIQuery = $"latitude={location.Latitude}&longitude={location.Longitude}" +
                $"&hourly=temperature_2m,rain,snowfall,windspeed_10m" +
                $"&daily=temperature_2m_max,temperature_2m_min,rain_sum,snowfall_sum,windspeed_10m_max" +
                $"&windspeed_unit=ms&forecast_days=1&timezone=auto&forecast_days=14";

            var response = await _httpClient.GetAsync($"{new Uri(_options.WatherLink)}?{WatherAPIQuery}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var responsContent = await response.Content.ReadAsStringAsync(cancellationToken);

            WeatherApiRoot Wather = JsonConvert.DeserializeObject<WeatherApiRoot>(responsContent)?? throw new Exception();

            return Wather;
        }
    }
}