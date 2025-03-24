using auth.Data;
using auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using auth.Services;
using MongoDB.Bson;
using auth.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace auth.Controllers;

[Authorize]
public class ChatsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMongoCollection<ChatMessage> _chatCollection;

    private readonly IHubContext<ChatHub> _hubContext;

    public ChatsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        MongoDbService mongoDbService,
        IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _chatCollection = mongoDbService.GetChatCollection();
        _hubContext = hubContext;
    }

    private async Task<bool> AreFriendsAsync(string? userId, string? friendId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(friendId))
        {
            return false; // If either userId or friendId is null, return false
        }

        return await _context.Friendships
            .AnyAsync(f => f.Status == "Accepted" &&
                           ((f.UserId == userId && f.FriendId == friendId) ||
                            (f.UserId == friendId && f.FriendId == userId)));
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(string friendId)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated." });
            }

            if (!await AreFriendsAsync(userId, friendId))
            {
                return Unauthorized("You are not friends with this user.");
            }

            var messages = await _chatCollection
                .Find(m => (m.SenderId == userId && m.ReceiverId == friendId) ||
                           (m.SenderId == friendId && m.ReceiverId == userId))
                .SortByDescending(m => m.Timestamp)
                .Limit(50) // Load the last 50 messages
                .ToListAsync();

            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    public async Task<IActionResult> FriendsHub()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated." });
        }

        var friends = await _context.Friendships
            .Where(f => f.Status == "Accepted" && (f.UserId == userId || f.FriendId == userId))
            .Select(f => f.UserId == userId ? f.Friend : f.User) // Get friend data
            .ToListAsync();

        return View(friends);
    }


    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            message.SenderId = userId;
            message.Timestamp = DateTime.UtcNow;
            message.Id = ObjectId.GenerateNewId().ToString();

            await _chatCollection.InsertOneAsync(message);

            // Send via SignalR to both sender and receiver
            await _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
            await _hubContext.Clients.User(message.ReceiverId).SendAsync("ReceiveMessage", message);

            return Ok(new { success = true, message = message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }



    /*
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            try
            {
                // Your existing validation code...

                var userId = _userManager.GetUserId(User);
                message.SenderId = userId;
                message.Timestamp = DateTime.UtcNow;
                message.Id = ObjectId.GenerateNewId().ToString();

                // Save to MongoDB
                await _chatCollection.InsertOneAsync(message);

                // Send through SignalR
                await _hubContext.Clients.User(message.ReceiverId)
                    .SendAsync("ReceiveMessage", message);

                return Ok(new { success = true, message = "Message sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    */


    /*

    //Working post code please do not delete
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            try
            {
                Console.WriteLine($"Received message: ReceiverId={message?.ReceiverId}, Content={message?.Content}");

                if (message == null || string.IsNullOrEmpty(message.ReceiverId) || string.IsNullOrEmpty(message.Content))
                {
                    return BadRequest(new { error = "Invalid message." });
                }

                var userId = _userManager.GetUserId(User); // Current user is always the sender
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "User not authenticated." });
                }

                message.SenderId = userId; // Set sender ID to current user ID
                message.Timestamp = DateTime.UtcNow;

                // Ensure sender and receiver are friends
                if (!await AreFriendsAsync(userId, message.ReceiverId))
                {
                    return Unauthorized(new { error = "You are not friends with this user." });
                }

                // Ensure _id is not set to null (MongoDB requirement)
                message.Id = ObjectId.GenerateNewId().ToString();

                Console.WriteLine("Inserting message into MongoDB...");
                await _chatCollection.InsertOneAsync(message);
                Console.WriteLine("Message successfully inserted.");

                return Ok(new { success = true, message = "Message sent successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

    */




    /*
        // Send a new chat message
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            try
            {
                Console.WriteLine($"Received message: ReceiverId={message?.ReceiverId}, Content={message?.Content}");

                if (message == null || string.IsNullOrEmpty(message.ReceiverId) || string.IsNullOrEmpty(message.Content))
                {
                    return BadRequest(new { error = "Invalid message." });
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "User not authenticated." });
                }

                message.SenderId = userId;
                message.Timestamp = DateTime.UtcNow;

                if (!await AreFriendsAsync(userId, message.ReceiverId))
                {
                    return Unauthorized(new { error = "You are not friends with this user." });
                }

                // Ensure _id is not set to null
                message.Id = ObjectId.GenerateNewId().ToString();

                Console.WriteLine("Inserting message into MongoDB...");
                await _chatCollection.InsertOneAsync(message);
                Console.WriteLine("Message successfully inserted.");

                return Ok(new { success = true, message = "Message sent successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    */
    public async Task<IActionResult> Chat(string friendId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated." });
        }

        // Check if they are friends
        var isFriend = await AreFriendsAsync(userId, friendId);
        if (!isFriend)
        {
            return Unauthorized("You are not friends with this user.");
        }

        // Retrieve friend's information
        var friend = await _context.Users.FirstOrDefaultAsync(u => u.Id == friendId);
        if (friend == null)
        {
            return NotFound("Friend not found.");
        }

        ViewBag.FriendId = friendId;
        ViewBag.FriendName = friend.UserName;
        ViewBag.UserId = userId; // Add this line!

        return View();
    }


}

// Chat message model
public class ChatMessage
{
    public string Id { get; set; } // MongoDB ObjectId
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}
