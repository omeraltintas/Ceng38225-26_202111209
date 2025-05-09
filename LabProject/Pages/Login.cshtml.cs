using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace LabProject.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");

            if (!System.IO.File.Exists(filePath))
            {
                ErrorMessage = "User data file not found.";
                return Page();
            }

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var user = users?.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            var token = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

            return RedirectToPage("/Index"); 
        }
    }
}
