using System.Text.Json.Serialization;

namespace StackOverflow.Infrastructure.ExternalServices.StackOverflowApiService.Dtos;

public class StackOverflowApiResponseDto<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = [];

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonPropertyName("quota_remaining")]
    public int QuotaRemaining { get; set; }

    [JsonPropertyName("quota_max")]
    public int QuotaMax { get; set; }
}

public class StackOverflowTagItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public long Count { get; set; }

    [JsonPropertyName("has_synonyms")]
    public bool HasSynonyms { get; set; }

    [JsonPropertyName("is_moderator_only")]
    public bool IsModeratorOnly { get; set; }

    [JsonPropertyName("is_required")]
    public bool IsRequired { get; set; }
}
