using System.Net.Http.Json;
using System.Text.Json;
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

            // Fetch response as JsonElement
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);

            // Optional: print full response for debugging
            Console.WriteLine("BLS API Response:\n" + JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            // Navigate safely through JSON
            if (!response.TryGetProperty("Results", out var results) ||
                !results.TryGetProperty("series", out var seriesArray) ||
                seriesArray.GetArrayLength() == 0)
                return null;

            var series = seriesArray[0];

            if (!series.TryGetProperty("data", out var dataArray))
                return null;

            // Find matching month/year
            var matching = dataArray
                .EnumerateArray()
                .FirstOrDefault(d =>
                    d.GetProperty("year").GetString() == year.ToString() &&
                    string.Equals(d.GetProperty("periodName").GetString()?.Trim(), month.Trim(), StringComparison.OrdinalIgnoreCase));

            if (matching.ValueKind == JsonValueKind.Undefined)
                return null;

            // Parse CPI value
            var valueStr = matching.GetProperty("value").GetString() ?? "0";
            int cpiValue = (int)Math.Round(Convert.ToDouble(valueStr));

            // Parse notes
            string notes = "";
            if (matching.TryGetProperty("footnotes", out var footnotes))
            {
                var noteTexts = footnotes
                    .EnumerateArray()
                    .Select(fn => fn.TryGetProperty("text", out var txt) ? txt.GetString() : null)
                    .Where(text => !string.IsNullOrWhiteSpace(text));

                notes = string.Join("; ", noteTexts);
            }

            return new CpiResponse
            {
                CpiValue = cpiValue,
                Notes = notes
            };
        }
    }
}
