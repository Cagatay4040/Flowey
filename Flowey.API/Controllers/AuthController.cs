using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.User;
using Microsoft.AspNetCore.Mvc;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
    {
        var result = await _authService.LoginAsync(dto);

        return Ok(result);
    }
}
