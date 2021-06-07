using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tokenManagerApi.Helpers;
using tokenManagerApi.Models.Tokens;

namespace tokenManagerApi.Services
{
  public interface ITokenService
  {
    Task<List<AppToken>> ListTokens();
    Task<bool> AddEditToken(AppToken record);
  }

  public class TokenService: ITokenService
  {
    private IAmazonDynamoDB _dynamoDb;
    private IConfiguration _config;
    private IDynamoDbHelper _dbHelper;

    public TokenService(IAmazonDynamoDB dynamoDb, IConfiguration config, IDynamoDbHelper dbHelper)
    {
      _dynamoDb = dynamoDb;
      _config = config;
      _dbHelper = dbHelper;
    }

    public async Task<List<AppToken>> ListTokens()
    {
      var tokens = new List<AppToken>();
      ScanRequest scanReq = new ScanRequest()
      {
        TableName = _config.GetValue<string>("DynamoDb:TokensTable")
      };

      var dbResult = await _dynamoDb.ScanAsync(scanReq);
      foreach (var item in dbResult.Items)
      {
        tokens.Add(_dbHelper.ToObjectFromDynamoResult<AppToken>(item));
      }

      return tokens;
    }

    public async Task<bool> AddEditToken(AppToken record)
    {
      var putReq = new PutItemRequest()
      {
        TableName = _config.GetValue<string>("DynamoDb:TokensTable"),
        Item = _dbHelper.ToDynamoAttributeValueDictionary<AppToken>(record)
      };
      var response = await _dynamoDb.PutItemAsync(putReq);

      return true;
    }
  }
}
