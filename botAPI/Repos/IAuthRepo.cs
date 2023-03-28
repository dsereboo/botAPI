using botAPI.Models;

namespace botAPI.Repos
{
    public interface IAuthRepo
    {
        Task<List<CheckUserResponse>> CheckUserExistence(string telegramUserId);
        Otp GenerateOTP(string telegramUserId);
        string GenerateToken(User user);
        Task<User?> LoginApiUser(string username, string password);
        void persistOtp(Otp otp);
        Task<int> RegisterUser(string telegramUserId, string pin, string phoneNumber);
        Otp SendOtp();
        Task<int> SuspendUser(int userId);
        Task<int> ActivateUser(int userId);
        bool VerifyOtp(Otp otp);
        Task<VerifyPinResponse> VerifyPin(int userId, int pin);
    }
}