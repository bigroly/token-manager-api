using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
    Task<bool> DeleteToken(AppToken record);
    Task<bool> ValidateToken(AppToken token);
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

    public async Task<bool> DeleteToken(AppToken record)
    {
      var delReq = new DeleteItemRequest()
      {
        TableName = _config.GetValue<string>("DynamoDb:TokensTable"),
        Key = new Dictionary<string, AttributeValue>() {
            { "AppName", new AttributeValue { S = record.AppName } },
        }
      };

      var deleteComplete = await _dynamoDb.DeleteItemAsync(delReq);
      return true;
    }

    public async Task<bool> ValidateToken(AppToken token)
    {
      var existingToken = await GetToken(token.AppName);

      DateTime localNow = DateTime.Now;
      long unixNow = ((DateTimeOffset)localNow).ToUnixTimeSeconds();

      if(unixNow > token.ExpiryUtc)
      {
        throw new UnauthorizedAccessException("API token Expired");
      }

      string requestSerialized = JsonConvert.SerializeObject(token);
      string tokenSerialized = JsonConvert.SerializeObject(existingToken);

      if(requestSerialized != tokenSerialized)
      {
        throw new Exception("Invalid API token provided");
      }

      return true;
    }

    private async Task<AppToken> GetToken(string appName)
    {

      Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
      {
        { "AppName", new AttributeValue { S = appName } },
      };

      GetItemRequest itemReq = new GetItemRequest()
      {
        TableName = _config.GetValue<string>("DynamoDb:TokensTable"),
        Key = key
      };

      var dbResult = await _dynamoDb.GetItemAsync(itemReq);      

      if(dbResult.Item.Count == 0)
      {
        throw new KeyNotFoundException("API token Not Found");
      }

      return _dbHelper.ToObjectFromDynamoResult<AppToken>(dbResult.Item);
    }
  }
}
