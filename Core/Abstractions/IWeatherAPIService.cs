using VM.GeocodingApiVM;
using VM.WeatherAPIVM;

namespace Core.WathersAPI.Interface
{
    public interface IWeatherAPIService
    {
        Task<WeatherApiRoot> GetWather(GeocodingLocation location, CancellationToken cancellationToken = default);
    }
}
