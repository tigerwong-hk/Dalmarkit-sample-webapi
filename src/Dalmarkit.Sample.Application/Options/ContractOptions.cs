namespace Dalmarkit.Sample.Application.Options;

public class ContractOptions
{
    public IDictionary<string, ContractInfo>? ContractInfo { get; set; }
}

public class ContractInfo
{
    public IDictionary<string, string>? ContractAddresses { get; set; }
    public string? ContractJsonAbiFile { get; set; }
}
