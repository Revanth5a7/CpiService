using System.Net.Http.Json;
using CpiService.Models;

namespace CpiService.Services
{
    public class BlsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.bls.gov/publicAPI/v1/timeseries/data/CUUR0000SA0";

        public BlsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CpiResponse?> GetCpiDataAsync(int year, string month)
        {
            var url = $"{BaseUrl}?startyear={year}&endyear={year}";
            var response = await _httpClient.GetFromJsonAsync<dynamic>(url);

            var series = response["Results"]["series"][0];
            var data = ((IEnumerable<dynamic>)series["data"])
                        .FirstOrDefault(d => d["year"].ToString() == year.ToString() &&
                                             d["periodName"].ToString().Equals(month, StringComparison.OrdinalIgnoreCase));

            if (data == null) return null;

            return new CpiResponse
            {
                CpiValue = (int)Math.Round(Convert.ToDouble(data["value"].ToString())),
                Notes = string.Join("; ", data["footnotes"]
                    .Select(fn => fn["text"]?.ToString())
                    .Where(text => !string.IsNullOrEmpty(text)))
            };
        }
    }
}
