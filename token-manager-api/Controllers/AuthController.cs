using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tokenManagerApi.Contracts.Auth;
using tokenManagerApi.Services;

namespace tokenManagerApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private IAuthService _authService;

    public AuthController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(PostLoginRequest request)
    {

      try
      {
        var authResult = await _authService.Login(request.Username, request.Password, request.ClientId);
        return Ok(new PostLoginResponse()
        {
          Success = true,
          IdToken = authResult.IdToken,
          TokenExpiresIn = authResult.ExpiresIn
        });
      }
      catch (Exception e)
      {
        return BadRequest(new PostLoginResponse()
        {
          Success = false,
          ErrorMessage = e.Message
        });
      }

    }
  }
}
