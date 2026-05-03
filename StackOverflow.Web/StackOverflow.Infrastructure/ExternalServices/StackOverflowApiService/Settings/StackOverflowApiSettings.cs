namespace StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Settings;

public class StackOverflowApiSettings
{
    public const string SectionName = "StackOverflowApi";

    public string BaseUrl { get; set; } = "https://api.stackexchange.com/2.3";
    public int PageSize { get; set; } = 100;
    public int MinTagCount { get; set; } = 1000;
    public string? ApiKey { get; set; }
}
