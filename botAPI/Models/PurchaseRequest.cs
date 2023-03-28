namespace botAPI.Models
{
    public class PurchaseRequest
    {
        public PurchaseRequest (int userId, int bundlePackageId, string purchaseMode)
        {
           this.userId = userId;
           this.bundlePackageId = bundlePackageId;
           this.purchaseMode = purchaseMode;
        }

        public int userId { get; set; } 

        public int bundlePackageId { get; set; }    

        public string purchaseMode { get; set; }
    }
}
