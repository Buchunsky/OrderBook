using Newtonsoft.Json;

namespace OrderBook.DTO;

public class GetOrdersProviderBody
{
    public long Timestamp { get; set; }
    
    public long Microtimestamp { get; set; }
    
    [JsonProperty("bids")]
    public List<List<string>> Bids { get; set; }
    [JsonProperty("asks")]
    public List<List<string>> Asks { get; set; }
}