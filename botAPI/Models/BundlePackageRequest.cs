namespace botAPI.Models
{
    public class BundlePackageRequest
    {

        public BundlePackageRequest(int bundleId)
        {
            this.bundleId = bundleId;
        }
        public int bundleId { get; set; }
    }
}
