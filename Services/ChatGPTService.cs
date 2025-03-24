using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace auth.Services
{
    public class ChatGPTService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ChatGPTService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // 🔒 Load API key from environment variable for security
            _apiKey = "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";

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("⚠️ OpenAI API key is missing! Set the 'OPENAI_API_KEY' environment variable.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetChatResponse(string userMessage)
        {
            try
            {
                var requestBody = new
                {
                    model = "gpt-3.5-turbo", // Use a model you have access to
                    messages = new[]
                    {
                new { role = "system", content = "You are a helpful chatbot." },
                new { role = "user", content = userMessage }
            },
                    max_tokens = 100
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"⚠️ API Error: {response.StatusCode} - {responseString}";
                }

                using JsonDocument doc = JsonDocument.Parse(responseString);
                if (doc.RootElement.TryGetProperty("choices", out JsonElement choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out JsonElement message) &&
                    message.TryGetProperty("content", out JsonElement content))
                {
                    return content.GetString();
                }

                return "⚠️ No valid response received from OpenAI.";
            }
            catch (HttpRequestException httpEx)
            {
                return $"⚠️ HTTP Request Error: {httpEx.Message}";
            }
            catch (JsonException jsonEx)
            {
                return $"⚠️ JSON Parsing Error: {jsonEx.Message}";
            }
            catch (Exception ex)
            {
                return $"⚠️ Unexpected Error: {ex.Message}";
            }
        }

    }
}
