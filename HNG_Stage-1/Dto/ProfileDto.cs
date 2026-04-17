using System.Text.Json.Serialization;

namespace HNG_Stage_1.Dto
{
    public class GenderizeResponse 
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("probability")]
        public double Probability { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

    }


    public class AgifyResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }


    public class NationalizeResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public List<CountryEntry>? Country { get; set; }
    }

    public class CountryEntry
    {
        [JsonPropertyName("country_id")]
        public string CountryId { get; set; } = string.Empty;

        [JsonPropertyName("probability")]
        public double Probability { get; set; }
    }

    public class CreateProfileRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class ProfileResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("gender_probability")]
        public double GenderProbability { get; set; }

        [JsonPropertyName("sample_size")]
        public int SampleSize { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("age_group")]
        public string AgeGroup { get; set; } = string.Empty;

        [JsonPropertyName("country_id")]
        public string CountryId { get; set; } = string.Empty;

        [JsonPropertyName("country_probability")]
        public double CountryProbability { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class ProfileListItemResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("age_group")]
        public string AgeGroup { get; set; } = string.Empty;

        [JsonPropertyName("country_id")]
        public string CountryId { get; set; } = string.Empty;
    }
}
