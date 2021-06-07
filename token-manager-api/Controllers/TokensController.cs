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

    //[HttpPost]
    //public IActionResult AddUpdateToken()
    //{

    //}
  }
}
