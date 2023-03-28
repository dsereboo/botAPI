using botAPI.Models;
using botAPI.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace botAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;

        public AuthController(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }


        [AllowAnonymous]
        [Route("apiuserlogin")]
        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody]LoginRequest req) 
        {
            var user = await _authRepo.LoginApiUser(req.username, req.password);
            if(user != null)
            {
                var result = _authRepo.GenerateToken(user);
                return new OkObjectResult(result);
            }
            return NotFound();
        }

        [Authorize]
        [Route("validatepin")]
        [HttpPost]
        public async Task<IActionResult> ValidatePin([FromBody]VerifyPinRequest req)
        {
            var result = await _authRepo.VerifyPin(req.userId, req.pin);
            if (result is not null)
            {
                return new OkObjectResult(true);
            }
            else
            {
                return new OkObjectResult(false);
            }
        }

        [Authorize]
        [Route("suspenduser")]
        [HttpPost]
        public async Task<IActionResult> SuspendUser([FromBody]StatusRequest req)
        {
            var result =await _authRepo.SuspendUser(req.userId);
            if(result < 1)
            {
                return new BadRequestObjectResult(result);
            }
            return new OkObjectResult(result);
        }


        [Authorize]
        [Route("checkuser")]
        [HttpPost]
        
        public async Task <IActionResult> CheckUserExistence([FromBody]CheckUser user)
        {
            var result = await _authRepo.CheckUserExistence(user.telegramUserId);
            var item = result.FirstOrDefault();
            if (item is not null )
            {
                return new OkObjectResult(item);  
            }
            return new NotFoundObjectResult("User does not exist");
        }


        [Authorize]
        [HttpPost]
        [Route("registeruser")]
        public async Task<IActionResult> RegisterUser([FromBody]NewUser user) 
        {
            var result = await _authRepo.RegisterUser(user.telegramUserId, user.pin, user.phoneNumber);
            if (result >= 1)
            {
                //What do i do if a new user has been created successfully??
                return new OkObjectResult("Success");
            }
            return new BadRequestObjectResult("Registration Failed.");
        }

        [Authorize]
        [HttpPost]
        [Route("getotp")]
        public IActionResult SendOtp()
        {
            var result = _authRepo.SendOtp();
            if (result is not null)
            {
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult(result);
        
        }

        [Authorize]
        [HttpPost]
        [Route("verifyotp")]
        public async Task<IActionResult> VerifyOtp([FromBody]Otp otp)
        {
            //call verify otp method in authRepo
            //return sucess message
            var result = _authRepo.VerifyOtp(otp);
            return new OkObjectResult(result);
        }


        [Authorize]
        [HttpPost]
        [Route("activateuser")]

        public async Task<IActionResult> ActivateUser([FromBody]CheckUser user)
        {

            var result =await _authRepo.ActivateUser(Convert.ToInt32 (user.telegramUserId));
            if (result > 0)
            {
                return new OkResult();
            }
            else
            {
                return new BadRequestObjectResult("User could not be activated");
            }
        }


      /*  [Authorize]
        [HttpPost]
        [Route("checkuserstatus")]
        public async Task<IActionResult> CheckSuspendedStatus()
        {

        }*/

    }
}
