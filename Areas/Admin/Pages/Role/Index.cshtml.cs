using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorWeb.Models;

namespace RazorWeb.Admin.Role
{
    public class IndexModel : RolePageModel
    {
        public IndexModel(AppDbContext db, RoleManager<IdentityRole> roleManager) : base(db, roleManager)
        {
        }

        public List<IdentityRole>? roles {set; get;}
        [TempData]
        public string StatusMessage {get; set;}
        public async Task OngetAsync()
        {
            roles = await _roleManager.Roles.ToListAsync();
        }
        public void OnPost() => RedirectToPage();
    }
}