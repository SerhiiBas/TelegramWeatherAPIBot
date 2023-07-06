using VM.GeocodingApiVM;

namespace Core.GeocodingApi.GeocodingApiInterfaces
{
    public interface IGeocodingApiService
    {
        Task<List<GeocodingLocation>> GetLocationCity(string locaton, CancellationToken cancellationToken = default);
    }
}
