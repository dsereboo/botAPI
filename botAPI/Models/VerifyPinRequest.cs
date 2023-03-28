namespace botAPI.Models
{
    public class VerifyPinRequest
    {
        public VerifyPinRequest(int userId, int pin)
        {
            this.pin = pin;
            this.userId = userId;
        }
        public int userId { get; set; }

        public int pin { get; set; }
    }
}
