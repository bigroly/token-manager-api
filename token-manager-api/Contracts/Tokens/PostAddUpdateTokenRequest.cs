using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tokenManagerApi.Models.Tokens;

namespace tokenManagerApi.Contracts.Tokens
{
  public class PostAddUpdateTokenRequest
  {
    public AppToken Token { get; set; }
  }
}
