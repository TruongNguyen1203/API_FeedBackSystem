using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using API.Extensions;
using Core.Entities;
using Core.Entities.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly StoreContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<AppUser> userManager,
                                RoleManager<Role> roleManager,
                                IConfiguration configuration, StoreContext context, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {

            var userExist = await userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            AppUser user = new AppUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string mes = "";
                foreach (var error in result.Errors)
                {
                    mes += error.Description + "\n";
                }
                return Ok(new { status = "error", message = mes });
            }

            // add to admin, trainee, trainer table
            switch (model.Role)
            {
                case Role.Admin:

                    Admin admin = new Admin()
                    {
                        AdminID = Guid.NewGuid().ToString(),
                        AppUser = user
                    };

                    try
                    {
                        _context.Add(admin);
                    }
                    catch (Exception e)
                    {
                        return Ok(new { status = "error", message = e.ToString() });
                    }
                    break;
                case Role.Trainee:
                    Trainee trainee = new Trainee()
                    {
                        TraineeID = Guid.NewGuid().ToString(),
                        AppUser = user

                    };
                    _context.Add(trainee);
                    break;
                case Role.Trainer:
                    Trainer trainer = new Trainer()
                    {
                        TrainerID = Guid.NewGuid().ToString(),
                        AppUser = user
                    };
                    _context.Add(trainer);
                    break;
            }
            // check role
            if (!await roleManager.RoleExistsAsync(Role.Admin))
                await roleManager.CreateAsync(new Role("Admin"));
            if (!await roleManager.RoleExistsAsync(Role.Trainee))
                await roleManager.CreateAsync(new Role("Trainee"));
            if (!await roleManager.RoleExistsAsync(Role.Trainer))
                await roleManager.CreateAsync(new Role("Trainer"));
            if (model.Role != Role.Admin && model.Role != Role.Trainee && model.Role != Role.Trainer)
            {
                return Ok(new { status = "Fail", message = "Role is incorrect" });
            }

            result = await userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                string mes = "";
                foreach (var error in result.Errors)
                {
                    mes += error.Description + "\n";
                }
                return Ok(new { status = "error", message = mes });
            }
            return Ok(new { status = "Success", message = "User created successfully and add to it's table" });
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var obj = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaim.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authIdentity = new ClaimsIdentity(authClaim, "Auth Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { authIdentity });

                await HttpContext.SignInAsync(userPrincipal);

                var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: authClaim,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );
                var userId = user.UserName.ToString();
                // set session
                HttpContext.Session.SetString(SessionKey.AdminName, user.UserName.ToString());
                HttpContext.Session.SetString(SessionKey.Role, model.Role);

                string specifiId = "";
                switch (model.Role)
                {
                    case Role.Admin:
                        specifiId = _context.Admins.Include(x => x.AppUser)
                            .Where(x => x.AppUser.Id == user.Id)
                            .Select(x => x.AdminID).SingleOrDefault().ToString();
                        break;
                    case Role.Trainee:
                        specifiId = _context.Trainees.Include(x => x.AppUser)
                            .Where(x => x.AppUser.Id == user.Id)
                            .Select(x => x.TraineeID).SingleOrDefault().ToString();
                        break;
                    case Role.Trainer:
                        specifiId = _context.Trainers.Include(x => x.AppUser)
                            .Where(x => x.AppUser.Id == user.Id)
                            .Select(x => x.TrainerID).SingleOrDefault().ToString();
                        break;
                }

                HttpContext.Session.SetString(SessionKey.Id, specifiId);

                return Ok(new
                {
                    userId = userId,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    authClaim = authClaim,
                    roleManager = userRoles
                });
            }
            return Ok(new { success = "Failed", message = "Login Failed, Check Your Login Details" });
            // return Ok(user);
        }

    }
}