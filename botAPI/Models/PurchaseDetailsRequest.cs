namespace botAPI.Models
{
    public class PurchaseDetailsRequest
    {
        public PurchaseDetailsRequest(int userId)
        {
            this.userId = userId;
        }

        public int userId { get; set; }
    }
}
