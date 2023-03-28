namespace botAPI.Models
{
  
    public class LoginRequest
    {
       public LoginRequest(string username,string password)
        {
            this.username = username;
            this.password = password;
        }
        public string? username { get; set; }

        public string? password { get; set; }
    }

    public class User 
    {
        public string username { get; set; }

        public string email { get; set; }
    }

    public class CheckUser 
    { 
        public CheckUser(string telegramUserId) 
        {
            this.telegramUserId = telegramUserId;
        }
        public string telegramUserId { get; set; }
    }

    public class CheckUserResponse
    {
       public bool existence { get; set; }
    }


    public class NewUser
    {

        public NewUser(string telegramUserId, string phoneNumber, string pin)
        {
            this.telegramUserId = telegramUserId;
            this.phoneNumber = phoneNumber;
            this.pin = pin;
        }
    
        public string telegramUserId { get; set; }

        public string phoneNumber { get; set; }

        public string pin { get; set; }
    }

    public class Otp
    {

        public Otp(string pin, string otpSid)
        {
            this.pin = pin;
            this.otpSid = otpSid;
        }

        public string pin {get; set;}

        public string otpSid { get; set; }
        
    }
}
