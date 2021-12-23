using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorWeb.MiddleWare;
using RazorWeb.Models;

namespace RazorWeb.Admin.Role
{
    [Authorize(Policy = "App")]
    [LoggingMiddleWare]
    public class CreateModel : RolePageModel
    {
        public CreateModel(AppDbContext db, RoleManager<IdentityRole> roleManager) : base(db, roleManager)
        {
        }

        [TempData]
        public string? StatusMessage {get; set;}
        public class InputModel
        {
            public string? Name {get; set;}
        }

        [BindProperty]
        public InputModel? input {set; get;}
        public async Task OngetAsync()
        {
            
        }
        public async Task<IActionResult> OnPost()
        {
            if(ModelState.IsValid)
            {
                var role = new IdentityRole(input.Name);
                var res = await _roleManager.CreateAsync(role);
                if(res.Succeeded)
                {
                    StatusMessage = $"Create new role {input.Name} successfull";
                    return RedirectToPage("Index");
                }
                else
                {
                     res.Errors.ToList().ForEach(e => {
                         ModelState.AddModelError(string.Empty, e.Description);
                     });
                }
            }
            return Page();
        }
    }
}