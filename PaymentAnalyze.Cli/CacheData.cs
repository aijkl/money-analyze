using Newtonsoft.Json;

namespace PaymentAnalyze.Cli;

public class CacheData
{
    [JsonIgnore]
    private string _filepath = "";
    public CacheData(string token)
    {
        Token = token;
    }
    
    [JsonProperty("token")]
    public string Token { set; get; }
    
    [JsonProperty("lastLogin")]
    public DateTime LastLogin { set; get; }
    
    public void SaveToFile()
    {
        File.WriteAllText(_filepath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static CacheData LoadFromFile(string filePath)
    {
        if(!File.Exists(filePath)) File.WriteAllText(filePath, JsonConvert.SerializeObject(new CacheData(string.Empty)));
        
        var cacheData = JsonConvert.DeserializeObject<CacheData>(File.ReadAllText(filePath)) ?? throw new Exception();
        cacheData._filepath = filePath;
        return cacheData;
    }
}