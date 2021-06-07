using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tokenManagerApi.Models.Tokens
{
  public class AppToken
  {
    public string AppName { get; set; }
    public string Token { get; set; }
    public long ExpiryUtc { get; set; }
  }
}