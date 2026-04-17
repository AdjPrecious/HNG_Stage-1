using HNG_Stage_1.Dto;
using System.Text.Json;

namespace HNG_Stage_1.External_API
{
    public interface IExternalApiService
    {
        Task<GenderizeResponse> GetGenderAsync(string name);
        Task<AgifyResponse> GetAgeAsync(string name);
        Task<NationalizeResponse> GetNationalityAsync(string name);

        public class ExternalApiService : IExternalApiService
        {
            private readonly HttpClient _http;
            private static readonly JsonSerializerOptions JsonOpts = new()
            {
                PropertyNameCaseInsensitive = true
            };

            public ExternalApiService(HttpClient http)
            {
                _http = http;
            }

            public async Task<GenderizeResponse> GetGenderAsync(string name)
            {
                var response = await FetchAsync($"https://api.genderize.io?name={Uri.EscapeDataString(name)}");
                return Deserialize<GenderizeResponse>(response, "Genderize");
            }

            public async Task<AgifyResponse> GetAgeAsync(string name)
            {
                var response = await FetchAsync($"https://api.agify.io?name={Uri.EscapeDataString(name)}");

                return Deserialize<AgifyResponse>(response, "Agify");
            }

            public async Task<NationalizeResponse> GetNationalityAsync(string name)
            {
                var response = await FetchAsync($"https://api.nationalize.io?name={Uri.EscapeDataString(name)}");

                return Deserialize<NationalizeResponse>(response, "Nationalize");
            }


            //----- helper --------

            private async Task<string> FetchAsync(string url)
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }

            private static T Deserialize<T>(string json, string apiName)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<T>(json, JsonOpts);
                    if (result == null)
                        throw new Exception(apiName);
                    return result;
                }
                catch(JsonException ex)
                {
                    throw new Exception(apiName, ex);
                }
            }


            public class ExternalApiException : Exception
            {
                public string ApiName { get; }
                public ExternalApiException(string apiName, Exception? inner = null)
                    : base($"{apiName} returned an invalid response", inner)
                {
                    ApiName = apiName;
                }
            } }
    } }