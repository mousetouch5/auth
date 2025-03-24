using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace auth.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = "AIzaSyChkfr6mEZFzW8tZGlIm2qKxBEwWPc-jEw"; // 🔒 Secure API Key

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("⚠️ Google Gemini API key is missing! Set the 'GEMINI_API_KEY' environment variable.");
            }
        }

        public async Task<string> GetChatResponse(string userMessage)
        {
            try
            {
                const string modelName = "gemini-1.5-flash"; // Choose the model you want to use

                var requestBody = new
                {
                    contents = new[]
                    {
                new { role = "user", parts = new[] { new { text = userMessage } } }
            }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={_apiKey}";

                var response = await _httpClient.PostAsync(apiUrl, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"⚠️ API Error: {response.StatusCode} - {responseString}";
                }

                using JsonDocument doc = JsonDocument.Parse(responseString);
                if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out JsonElement content) &&
                    content.TryGetProperty("parts", out JsonElement parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out JsonElement text))
                {
                    return text.GetString();
                }

                return "⚠️ No valid response received from Gemini API.";
            }
            catch (Exception ex)
            {
                return $"⚠️ Error: {ex.Message}";
            }
        }

    }
}
