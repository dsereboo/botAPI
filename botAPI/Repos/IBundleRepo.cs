using botAPI.Models;

namespace botAPI.Repos
{
    public interface IBundleRepo
    {
        Task<int> BuyDataPackage(int id, int packageId, string mode);
        Task<IEnumerable<BundlePackages>> GetBundlePackages(int bundleId);
        Task<IEnumerable<PurchaseResponse>> GetPurchaseDetails(int id);
    }
}