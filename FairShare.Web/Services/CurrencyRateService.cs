using System.Net.Http.Json;

namespace FairShare.Web.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly HttpClient _client;
        public CurrencyRateService(HttpClient client) => _client = client;

        public async Task<decimal?> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken ct = default)
        {
            // Free, no key: https://api.exchangerate.host/convert?from=USD&to=EUR
            var url = $"convert?from={fromCurrency}&to={toCurrency}";
            try
            {
                var json = await _client.GetFromJsonAsync<ConvertResponse>(url, ct);
                return json?.info?.rate;
            }
            catch
            {
                return null;
            }
        }

        private class ConvertResponse
        {
            public Info? info { get; set; }
            public class Info { public decimal rate { get; set; } }
        }
    }
}
