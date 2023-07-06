using Core.WeathersAPI.WeatherApiService;
using Microsoft.Extensions.Options;
using VM.GeocodingApiVM;
using VM.WeatherAPIVM;

namespace TelegramBotTest.ServiceTests
{
    internal class WeatherApiSeviceTest
    {
        private readonly IOptions<WatherAPILink> options;
        private readonly HttpClient _httpClient = new HttpClient(new MockHttpMessageHandler(string.Empty));

        public WeatherApiSeviceTest(MockHttpMessageHandler mockHttp, IOptions<WatherAPILink> options)
        {
            this.options = options;
        }

        private WeatherAPIService GetWeatherAPIService()
        {
            return new WeatherAPIService(_httpClient, this.options);
        }

        [Test]
        public async Task GetWather_TrueLocations_TrueWeatherApiRoot()
        {
            //Arrange
            var weatherApi = GetWeatherAPIService();
            GeocodingLocation geocodingLocation = new GeocodingLocation() { Latitude = 46.6f, Longitude=46.6f};
            //Act

            var weather = await weatherApi.GetWather(geocodingLocation);
            //Assert

            Assert.IsNotNull(weather);
        }
    }
}
