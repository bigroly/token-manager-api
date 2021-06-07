using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tokenManagerApi.Contracts.Tokens;
using tokenManagerApi.Services;

namespace tokenManagerApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TokensController : ControllerBase
  {
    private ITokenService _tokenService;
    public TokensController(ITokenService tokenService)
    {
      _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<IActionResult> ListTokens()
    {
      try
      {
        return Ok(
          new GetListTokensResponse()
          {
            Success = true,
            Tokens = await _tokenService.ListTokens()
          }
        );
      }
      catch (Exception e)
      {
        return BadRequest(new GetListTokensResponse()
        {
          Success = false,
          ErrorMessage = e.Message
        });
      }
    }

    [HttpPost]
    public async Task<IActionResult> AddUpdateToken(PostAddUpdateTokenRequest req)
    {
      try
      {
        return Ok(new PostAddUpdateTokenResponse()
        {
          Success = await _tokenService.AddEditToken(req.Token)
        });
      }
      catch (Exception e)
      {
        return BadRequest(new PostAddUpdateTokenResponse()
        {
          Success = false,
          ErrorMessage = e.Message
        });
      }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteToken(DeleteTokenRequest req)
    {
      try
      {
        return Ok(new DeleteTokenResponse()
        {
          Success = await _tokenService.DeleteToken(req.Token)
        });
      }
      catch (Exception e)
      {
        return BadRequest(new DeleteTokenResponse()
        {
          Success = false,
          ErrorMessage = e.Message
        });
      }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken(PostValidateTokenRequest request)
    {
      //conforming to the responses dictated in spec .yml file
      try
      {
        bool tokenOK = await _tokenService.ValidateToken(request.ApiToken);
        return Ok("API Token is valid");
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
      catch(UnauthorizedAccessException e)
      {
        return Unauthorized(e.Message);
      }
      catch(Exception e)
      {
        if (e.Message == "Invalid API token provided")
        {
          return BadRequest(e.Message);
        }
        return StatusCode(500);
      }      
    }

  }
}
