using Newtonsoft.Json;

namespace OrderBook.DTO;

public class GetOrdersProviderResponse
{
    [JsonProperty("data")]
    public GetOrdersProviderBody Body { get; set; }
    public string Channel { get; set; }
}