// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppMvc.Areas.Identity.Models.RoleViewModels;
using AppMvc.Common;
using AppMvc.ExtensionMethods;
using AppMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppMvc.Areas.Identity.Controllers
{

    //[Authorize(Roles = "Administrator")]
    [Authorize(Policy = "App")]
    [Area("Identity")]
    [Route("/Role/[action]")]
    public class RoleController : Controller
    {

        private readonly ILogger<RoleController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public RoleController(ILogger<RoleController> logger, RoleManager<IdentityRole> roleManager, AppDbContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        //
        // GET: /Role/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //    var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            //    var roles = new List<RoleModel>();
            //    foreach (var _r in r)
            //    {
            //        var claims = await _roleManager.GetClaimsAsync(_r);
            //        var claimsString = claims.Select(c => c.Type  + "=" + c.Value);

            //        var rm = new RoleModel()
            //        {
            //            Name = _r.Name,
            //            Id = _r.Id,
            //            Claims = claimsString.ToArray()
            //        };
            //        roles.Add(rm);
            //    }

            //     return View(roles);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getRoles()
        {
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            var roles = new List<RoleModel>();
            foreach (var _r in r)
            {

                var rm = await GetRoleModel(_r);
                roles.Add(rm);
            }
            ApiResponseVm res = new ApiResponseVm();
            res.Ret = ResultType.OK;
            res.Msg = roles;
            return Json(res);
        }

        private async Task<RoleModel> GetRoleModel(IdentityRole role)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var claimsString = claims.Select(c => c.Type + "=" + c.Value);

            return new RoleModel()
            {
                Name = role.Name,
                Id = role.Id,
                Claims = claimsString.ToArray()
            };
        }

        [HttpGet]
        public async Task<IActionResult> getRole(string id)
        {
            var r = await _roleManager.FindByIdAsync(id);
            ApiResponseVm res = new ApiResponseVm();
            if (r != null)
            {
                res.Msg = await GetRoleModel(r);
                res.Ret = ResultType.OK;
            }
            else
            {
                res.UserMsg = "NotFound";
            }
            return Json(res);
        }
        // GET: /Role/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //POST: /Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateRoleModel model)
        {
            ApiResponseVm res = new ApiResponseVm();
            if (!ModelState.IsValid)
            {
                res.UserMsg = "Model is not valid";
                res.Msg = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                return Json(res);
            }

            var newRole = new IdentityRole(model.Name);
            var result = await _roleManager.CreateAsync(newRole);
            if (result.Succeeded)
            {
                // StatusMessage = $"Bạn vừa tạo role mới: {model.Name}";
                // return RedirectToAction(nameof(Index));
                res.Ret = ResultType.OK;
                res.UserMsg = $"Bạn vừa tạo role mới: {model.Name}";
                res.Msg = new
                {
                    Id = newRole.Id,
                    Name = newRole.Name
                };
            }
            else
            {
                ModelState.AddModelError(result);
                res.UserMsg = result.ToString();
            }
            return Json(res);
        }


        //POST: /Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync2(CreateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var newRole = new IdentityRole(model.Name);
            var result = await _roleManager.CreateAsync(newRole);
            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa tạo role mới: {model.Name}";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(result);
            }
            return View();
        }

        // GET: /Role/Delete/roleid
        [HttpGet("{roleid}")]
        public async Task<IActionResult> DeleteAsync(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            return View(role);
        }

        // POST: /Role/Edit/1
        [HttpPost("{roleid}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmAsync(string roleid)
        {
            ApiResponseVm res = new ApiResponseVm();
            if (roleid == null) return NotFound("role id không được null");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound("Không tìm thấy role");

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa xóa: {role.Name}";
                //return RedirectToAction(nameof(Index));
                res.Ret = ResultType.OK;
                res.UserMsg = StatusMessage;
            }
            else
            {
                ModelState.AddModelError(result);
            }

            res.Msg = result;
            return Json(res);
        }

        // GET: /Role/Edit/roleid
        [HttpGet("{roleid}")]
        public async Task<IActionResult> EditAsync(string roleid, [Bind("Name")] EditRoleModel model)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            model.Name = role.Name;
            model.Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
            model.role = role;
            ModelState.Clear();
            return View(model);

        }

        // POST: /Role/Edit/1
        [HttpPost("{roleid}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmAsync(string roleid, [Bind("Name")] EditRoleModel model)
        {
            ApiResponseVm res = new ApiResponseVm();
            if (roleid == null) return NotFound("Không tìm thấy roleid null");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            model.Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
            model.role = role;
            if (!ModelState.IsValid)
            {
                res.UserMsg = "model invalid";
                res.Msg = new{
                    model = role,
                    validation = ModelState
                };
                return Json(res);
            }

            role.Name = model.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa đổi tên: {model.Name}";
                res.Ret = ResultType.OK;
                res.UserMsg = StatusMessage;
                //return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(result);
            }
            res.Msg = result;

            return Json(res);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmAsync(string roleid, string rolename)
        {
            ApiResponseVm res = new ApiResponseVm();

            if (roleid == null) return NotFound("Không tìm thấy role");
            IdentityRole role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            string oldname = role.Name;
            role.Name = rolename;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                res.Ret = ResultType.OK;
                StatusMessage = $"Bạn vừa đổi tên: {oldname} -> {role.Name}";
                res.UserMsg = StatusMessage;
                res.Msg = role;
            }
            else
            {
                ModelState.AddModelError(result);
                res.UserMsg = "Update failed";
                res.Msg = result;
            }

            return Json(res);
        }
        // GET: /Role/AddRoleClaim/roleid
        [HttpGet("{roleid}")]
        public async Task<IActionResult> AddRoleClaimAsync(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }

            var model = new EditClaimModel()
            {
                role = role
            };
            return View(model);
        }

        // POST: /Role/AddRoleClaim/roleid
        [HttpPost("{roleid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleClaimAsync(string roleid, [Bind("ClaimType", "ClaimValue")] EditClaimModel model)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            model.role = role;
            if (!ModelState.IsValid) return View(model);


            if ((await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == model.ClaimType && c.Value == model.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return View(model);
            }

            var newClaim = new Claim(model.ClaimType, model.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(result);
                return View(model);
            }

            StatusMessage = "Vừa thêm đặc tính (claim) mới";

            return RedirectToAction("Edit", new { roleid = role.Id });

        }



        // GET: /Role/EditRoleClaim/claimid
        [HttpGet("{claimid:int}")]
        public async Task<IActionResult> EditRoleClaim(int claimid)
        {
            var claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            var role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy role");
            ViewBag.claimid = claimid;

            var Input = new EditClaimModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
                role = role
            };


            return View(Input);
        }

        // GET: /Role/EditRoleClaim/claimid
        [HttpPost("{claimid:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoleClaim(int claimid, [Bind("ClaimType", "ClaimValue")] EditClaimModel Input)
        {
            var claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            ViewBag.claimid = claimid;

            var role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy role");
            Input.role = role;
            if (!ModelState.IsValid)
            {
                return View(Input);
            }
            if (_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return View(Input);
            }


            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Vừa cập nhật claim";

            return RedirectToAction("Edit", new { roleid = role.Id });
        }
        // POST: /Role/EditRoleClaim/claimid
        [HttpPost("{claimid:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClaim(int claimid, [Bind("ClaimType", "ClaimValue")] EditClaimModel Input)
        {
            var claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            var role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy role");
            Input.role = role;
            if (!ModelState.IsValid)
            {
                return View(Input);
            }
            if (_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return View(Input);
            }


            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            StatusMessage = "Vừa xóa claim";


            return RedirectToAction("Edit", new { roleid = role.Id });
        }


    }
}
