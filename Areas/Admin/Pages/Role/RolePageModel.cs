using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorWeb.Models;

namespace RazorWeb.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected AppDbContext? _db;
        protected RoleManager<IdentityRole>? _roleManager;

        public RolePageModel(AppDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }
    }
}