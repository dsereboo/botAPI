using botAPI.Models;
using botAPI.Services;

namespace botAPI.Repos
{
    public class BundleRepo : IBundleRepo
    {
        private readonly ISqlDataAccess _db;

        public BundleRepo(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<IEnumerable<BundlePackages>> GetBundlePackages(int bundleId)
        {

            var result = await _db.ReadData<BundlePackages, BundlePackageRequest>(
                   storedProcedure: "PCES_BundlePackages_Get",
                   new BundlePackageRequest(bundleId)
                  );
            return result;
        }

        public async Task<int> BuyDataPackage(int id, int packageId, string mode)
        {
            var result = await _db.WriteData<PurchaseRequest>(
                storedProcedure: "PCES_DataPurchases_InsTransaction",
                new PurchaseRequest(id, packageId, mode)
                );
            return result;
        }

        public async Task<IEnumerable<PurchaseResponse>> GetPurchaseDetails(int id)
        {
            var result = await _db.ReadData<PurchaseResponse, PurchaseDetailsRequest>(
                storedProcedure: "PCES_DataPurchases_GetLatest",
                new PurchaseDetailsRequest(id)
                );

            return result;
        }
    }
}
