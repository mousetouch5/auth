using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using auth.Services;

[ApiController]
[Route("api/chat")] // Consistent API route
public class ChatController : ControllerBase
{
    private readonly GeminiService _geminiService; // Updated service name

    public ChatController(GeminiService geminiService) // Inject GeminiService instead of ChatGPTService
    {
        _geminiService = geminiService;
    }

    [HttpPost("GetResponse")]
    public async Task<IActionResult> GetResponse([FromBody] ChatRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Invalid request. Message is required." });
        }

        try
        {
            var response = await _geminiService.GetChatResponse(request.Message); // Call Gemini API
            return Ok(new { reply = response });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}




/*
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using auth.Services;

[ApiController]
[Route("api/chat")] // Consistent API route
public class ChatController : ControllerBase
{
    private readonly ChatGPTService _chatGPTService;

    public ChatController(ChatGPTService chatGPTService)
    {
        _chatGPTService = chatGPTService;
    }

    [HttpPost("GetResponse")]
    public async Task<IActionResult> GetResponse([FromBody] ChatRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Invalid request. Message is required." });
        }

        try
        {
            var response = await _chatGPTService.GetChatResponse(request.Message);
            return Ok(new { reply = response });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}

*/