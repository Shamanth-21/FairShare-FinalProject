namespace FairShare.Web.Services
{
    public interface ICurrencyRateService
    {
        Task<decimal?> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken ct = default);
    }
}
