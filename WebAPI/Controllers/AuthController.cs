using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthController> logger) : ControllerBase
{
    IUserRepository _userRepository = userRepository;
    IJwtService _jwtService = jwtService;
    ILogger<AuthController> _logger = logger;


    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Register a {Role} with a {Phone}", request.Role, request.Phone);
        if (await _userRepository.GetByPhoneAsync(request.Phone) != null) return Conflict("User already exists");
        var user = new User(request.Phone, request.Password, request.Role);
        await _userRepository.AddAsync(user);
        return Ok("User registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
    {
        _logger.LogInformation("User authorization by {Phone}", request.Phone);
        var user = await _userRepository.GetByPhoneAsync(request.Phone);
        if (user == null) return NotFound("Invalid credentials");
        if (!user.VerifyPassword(request.Password)) return Unauthorized("Invalid credentials");
        string token = _jwtService.GenerateAccessToken(user.Id, user.Phone, user.Role);
        string refreshToken = _jwtService.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);
        return Ok(new { AccessToken = token, RefreshToken = refreshToken});
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request) 
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null) return Unauthorized("Invalid refresh token");
        if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Refresh token expired");
        string newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.Phone, user.Role);
        string newRefreshToken = _jwtService.GenerateRefreshToken();
        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);
        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken});
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutnUser([FromBody] RefreshRequest request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user != null)
        {
            user.ClearRefreshToken(); 
            await _userRepository.UpdateAsync(user);
        }
        return NoContent();
    }
}