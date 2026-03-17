using Flowey.BUSINESS.Features.Auth.Commands;
using Flowey.BUSINESS.Features.Auth.Queries;
using Flowey.CORE.DTO.User;
using Flowey.CORE.Result.Concrete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
    {
        var result = await _sender.Send(new LoginQuery(dto.Email, dto.Password));
        if (result.ResultStatus == ResultStatus.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserAddDTO dto)
    {
        var result = await _sender.Send(new RegisterCommand(dto.Email, dto.Password, dto.Name, dto.Surname));
        if (result.ResultStatus == ResultStatus.Success) return Ok(result);
        return BadRequest(result);
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeDTO dto)
    {
        var result = await _sender.Send(new ChangePasswordCommand(dto.UserId, dto.OldPassword, dto.NewPassword, dto.NewPasswordConfirm));
        if (result.ResultStatus == ResultStatus.Success) return Ok(result);
        return BadRequest(result);
    }
}
