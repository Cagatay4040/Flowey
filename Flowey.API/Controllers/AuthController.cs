using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.User;
using Flowey.CORE.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserAddDTO dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result.ResultStatus == ResultStatus.Success) return Ok(result);
        return BadRequest(result);
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeDTO dto)
    {
        var result = await _authService.ChangePasswordAsync(dto);
        if (result.ResultStatus == ResultStatus.Success) return Ok(result);
        return BadRequest(result);
    }
}
