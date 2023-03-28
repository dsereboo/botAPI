namespace botAPI.Models
{
    public class StatusRequest
    {

        public StatusRequest(int userId)
        {
            this.userId = userId;
        }
        public int userId { get; set; }
    }
}
