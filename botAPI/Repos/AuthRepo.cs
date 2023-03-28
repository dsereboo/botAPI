using botAPI.Models;
using botAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace botAPI.Repos
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _memoryCache;
        private readonly ISqlDataAccess _db;

        public AuthRepo(ISqlDataAccess db, IConfiguration config, IMemoryCache memoryCache)
        {
            _config = config;
            _db = db;
            _memoryCache = memoryCache;

        }

        public async Task<User?> LoginApiUser(string username, string password)
        {
            var result = await _db.ReadData<User, LoginRequest>(
                    storedProcedure: "PCES_ApiCustomers_GetByUsernameAndPassword",
                    new LoginRequest(username, password)
                );
            return result.ToList().FirstOrDefault();
        }

        public async Task<int> RegisterUser(string telegramUserId, string pin, string phoneNumber)
        {
            var result = await _db.WriteData<NewUser>(
                    storedProcedure: "PCES_User_AddNewUser",
                    new NewUser(telegramUserId, phoneNumber, pin)
                    );
            return result;
        }

        public async Task<List<CheckUserResponse>> CheckUserExistence(string telegramUserId)
        {
            var result = await _db.ReadData<CheckUserResponse, CheckUser>(
                    storedProcedure: "PCES_User_CheckExistence",
                    new CheckUser(telegramUserId)
                );
            return result.ToList();
        }


        public async Task<VerifyPinResponse> VerifyPin(int userId, int pin)
        {
            var result = await _db.ReadData<VerifyPinResponse, VerifyPinRequest>(
                storedProcedure: "PCES_User_ValidatePin",
                new VerifyPinRequest(userId, pin)
                );
            return result.FirstOrDefault();
        }

        public async Task<int> SuspendUser(int userId)
        {
            var result = await _db.WriteData(
                storedProcedure: "PCES_User_Suspend",
                new StatusRequest(userId)
                );
            return result;
        }

        public async Task<int> ActivateUser(int userId)
        {
            var result = await _db.WriteData(
                storedProcedure: "PCES_User_ChangeActiveStatus",
                new StatusRequest(userId)
                );
            return result;
        }

        public void persistOtp(Otp otp)
        {
            List<Otp> output;
            output = _memoryCache.Get<List<Otp>>("otps");
            if (output is null)
            {
                output = new List<Otp>();
                //add otp value to cache list
                output.Add(otp);
                _memoryCache.Set("otps", output, TimeSpan.FromMinutes(5));
            }
            else
            {
                output.Add(otp);
                _memoryCache.Set("otps", output, TimeSpan.FromMinutes(5));
            }
        }

        private List<Otp> getOtpsFromCache()
        {
            //check for a given otp using a key
            var output = _memoryCache.Get<List<Otp>>("otps");
            return output;
        }

        public Otp SendOtp()
        {
            var otp = GenerateOTP("1234356");
            persistOtp(otp);
            return otp;
        }

        public bool VerifyOtp(Otp otp)
        {
            var otps = getOtpsFromCache();

            if (otps is not null)
            {
                //check in otp value exists in cache
                var index = otps.FindLastIndex(item => string.Equals(item.otpSid, otp.otpSid) && string.Equals(item.pin, otp.pin));
           
                
                if (index > -1)
                {
                    //remove item from cache
                    /*otps.RemoveAt(index);*/
                    //update cache value
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

           
        }

        public Otp GenerateOTP(string telegramUserId)
        {
            string pin = GeneratePin();
            string tag = DateTime.Now.AddMinutes(5).ToShortDateString() + telegramUserId;
            string otpSid = Convert.ToBase64String(Encoding.ASCII.GetBytes(tag));
            return new Otp(pin, otpSid);
        }

        private string GeneratePin()
        {
            Random pin = new Random();
            string result = pin.Next(100000, 999999).ToString();
            return result;
        }


        public string GenerateToken(User user)
        {
            //Read security key in appsettings file
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.Email , user.email),
            };



            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JWT:Audience"],
                claims,
                //Token duration
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            ); ;

            //Generate token
            var result = new JwtSecurityTokenHandler().WriteToken(token);

            return result;
        }
    }
}
