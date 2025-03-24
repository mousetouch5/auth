using auth.Data;
using auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class FriendshipsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FriendshipsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> FriendsList()
    {
        var userId = _userManager.GetUserId(User);

        var friends = await _context.Friendships
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == "Accepted")
            .Include(f => f.User)
            .Include(f => f.Friend)
            .ToListAsync();

        var friendList = friends.Select(f => new
        {
            Id = f.UserId == userId ? f.FriendId : f.UserId,
            Name = f.UserId == userId ? f.Friend.UserName : f.User.UserName  // Fixes name display
        }).ToList();

        return Json(friendList);
    }



    [HttpPost]
    public async Task<IActionResult> RemoveFriend([FromBody] RemoveFriendRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.FriendId))
        {
            return BadRequest("Invalid request.");
        }

        var userId = _userManager.GetUserId(User);
        var friendId = request.FriendId;

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) ||
                                      (f.UserId == friendId && f.FriendId == userId));

        if (friendship == null)
            return NotFound("Friendship not found.");

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Friend removed!" });
    }

    [HttpPost]
    public async Task<IActionResult> AcceptRequest([FromBody] FriendRequestAction request)
    {
        if (request == null || request.FriendshipId <= 0)
        {
            return BadRequest("Invalid request.");
        }

        var currentUserId = _userManager.GetUserId(User);

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.Id == request.FriendshipId && f.FriendId == currentUserId && f.Status == "Pending");

        if (friendship == null)
        {
            return NotFound("Friend request not found.");
        }

        friendship.Status = "Accepted";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Friend request accepted!" });
    }



    [HttpPost]
    public async Task<IActionResult> DeclineRequest([FromBody] FriendRequestAction request)
    {
        if (request == null || request.FriendshipId <= 0)
        {
            return BadRequest("Invalid request.");
        }

        var currentUserId = _userManager.GetUserId(User);
        int friendshipId = request.FriendshipId;

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.Id == friendshipId && f.FriendId == currentUserId && f.Status == "Pending");

        if (friendship == null)
            return NotFound("Friend request not found.");

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Friend request declined." });
    }





    // GET: Search Friends
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query is required.");
        }

        var currentUserId = _userManager.GetUserId(User);

        // Get IDs of users the current user is already friends with
        var friendIds = await _context.Friendships
            .Where(f => f.UserId == currentUserId || f.FriendId == currentUserId)
            .Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId)
            .ToListAsync();

        // Fetch users who match the search query but are NOT already friends
        var potentialUsers = await _context.Users
            .Where(u => (u.UserName.Contains(query) || u.Email.Contains(query))
                        && u.Id != currentUserId
                        && !friendIds.Contains(u.Id)) // Exclude friends
            .ToListAsync(); // Fetch first, then filter roles in memory

        // Filter out users who have the Admin role
        var filteredUsers = new List<object>();
        foreach (var user in potentialUsers)
        {
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                filteredUsers.Add(new { user.Id, user.UserName });
            }
        }

        return Ok(filteredUsers);
    }



    // Model for JSON input (fixing dynamic issue)


    // POST: Add Friend
    [HttpPost]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.FriendId))
        {
            return BadRequest("Invalid request.");
        }

        var currentUserId = _userManager.GetUserId(User);

        // Prevent duplicate requests
        var existingFriendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
                (f.UserId == currentUserId && f.FriendId == request.FriendId) ||
                (f.UserId == request.FriendId && f.FriendId == currentUserId));

        if (existingFriendship != null)
        {
            return BadRequest("Friend request already exists.");
        }

        var friendship = new Friendship
        {
            UserId = currentUserId,
            FriendId = request.FriendId,
            Status = "Pending"  // Waiting for acceptance
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Friend request sent!" });
    }

    [HttpGet]
    public async Task<IActionResult> PendingRequests()
    {
        var currentUserId = _userManager.GetUserId(User);

        var requests = await _context.Friendships
            .Where(f => f.FriendId == currentUserId && f.Status == "Pending")
            .Select(f => new { f.Id, SenderName = f.User.UserName }) // Include sender info
            .ToListAsync();

        return Ok(requests);
    }


}

public class AddFriendRequest
{
    public string FriendId { get; set; }
}

public class FriendRequestAction
{
    public int FriendshipId { get; set; }
}

public class RemoveFriendRequest
{
    public string FriendId { get; set; }
}