namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthServices _authServices;
        //private readonly JwtConfig _jwtConfig; 

        public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration,
            IAuthServices authServices
            //,JwtConfig jwtConfig
            )
        {
            _userManager = userManager;
            _authServices = authServices;
            //_jwtConfig = jwtConfig;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]UserRegistrationRequestDto requestDto)
        {
            // Validate the incomig request

            if(ModelState.IsValid)
            {
                // we need to check if the email is already exist
                var userExist = await _userManager.FindByEmailAsync(requestDto.Email);

                if (userExist is null)
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>() { "Email already exist!" }
                    });

                // create a user
                var newUser = new IdentityUser()
                {
                   Email = requestDto.Email,
                   UserName = requestDto.Name,
                };

                // creates a user with spacific password inside our database
                var isCreated = await _userManager.CreateAsync(newUser, requestDto.Password);

                if (isCreated.Succeeded)
                {
                    // Generate the token
                    var token = _authServices.GenerateJwtToken(newUser);
                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token 
                    });
                }

                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>() { "Something went wrong!" },
                    Result = false
                });
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(loginRequest.Email);

                if (userExist is not null)
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>() { "Invalid payload!" }
                    });

                var isCorrect = await _userManager.CheckPasswordAsync(userExist, loginRequest.Password);

                if (!isCorrect)
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>() { "Invalid credintials" },
                        Result = false
                    });

                var jwtToken = _authServices.GenerateJwtToken(userExist);

                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true
                });
            }

            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                        "Invalid payload"
                },
                Result = false
            });
        }
    }
}
