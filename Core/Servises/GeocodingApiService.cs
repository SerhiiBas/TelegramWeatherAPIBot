using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Core.GeocodingApi.GeocodingApiInterfaces;
using VM.GeocodingApiVM;

namespace Core.GeocodingApi.GeocodingApiServices
{
    public class GeocodingApiService: IGeocodingApiService
    {
        private readonly HttpClient _httpClient;

        private readonly GeocodingApiLink _options;


        public GeocodingApiService(HttpClient httpClient, IOptions<GeocodingApiLink> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<List<GeocodingLocation>> GetLocationCity(string city, CancellationToken cancellationToken = default)
        {
            string geocodingAPIQuery = $"city={city}";

            var response = await _httpClient.GetAsync($"{_options.GeocodingLink}?{geocodingAPIQuery}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var responsContent = await response.Content.ReadAsStringAsync(cancellationToken);

            List<GeocodingLocation> geocodingLocation = JsonConvert.DeserializeObject<List<GeocodingLocation>>(responsContent) ??throw new Exception();

            return geocodingLocation;
        }
    }
}
