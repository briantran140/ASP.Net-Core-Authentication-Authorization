using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class UserProfileModel : PageModel
    {
        private UserManager<User> UserManager { get; }

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }

        [BindProperty]
        public string? SuccessMessage { get; set; }

        public UserProfileModel(UserManager<User> userManager)
        {
            this.UserManager = userManager;
            this.UserProfile = new UserProfileViewModel();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            this.SuccessMessage = string.Empty;

            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
            if (user != null)
            {
                this.UserProfile.Email = User.Identity?.Name ?? string.Empty;
                this.UserProfile.Department = departmentClaim?.Value ?? string.Empty;
                this.UserProfile.Position = positionClaim?.Value ?? string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
                if (user != null && departmentClaim != null)
                {
                    await UserManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));
                }

                if (user != null && positionClaim != null)
                {
                    await UserManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
                }
            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error occured during updating user profile.");
            }

            this.SuccessMessage = "The user profile is updated successfully.";

            return Page();
        }

        private async Task<(User? user, Claim? departmentClaim, Claim? positionClaim)> GetUserInfoAsync()
        {
            var user = await UserManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            if (user != null)
            {
                var claims = await UserManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

                return (user, departmentClaim, positionClaim);
            }
            else
            {
                return (null, null, null);
            }
        }

        public class UserProfileViewModel
        {
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Department { get; set; } = string.Empty;
            [Required]
            public string Position { get; set; } = string.Empty;
        }
    }
}
