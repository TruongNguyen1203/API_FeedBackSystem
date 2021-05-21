using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Entities.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController:ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly StoreContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<AppUser> userManager,
                                RoleManager<Role> roleManager,
                                IConfiguration configuration, StoreContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExist= await userManager.FindByNameAsync(model.UserName);
            if(userExist!=null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            AppUser user = new AppUser()
            {
                Email=model.Email,
                SecurityStamp =Guid.NewGuid().ToString(),
                UserName=model.UserName
            };
            var result= await userManager.CreateAsync(user,model.Password);
            if(!result.Succeeded)
            {
                string mes="";
                foreach(var error in  result.Errors)
                {
                    mes+=error.Description+"\n";
                }
                return Ok( new {status="error", message=mes});
            }
            if(!await roleManager.RoleExistsAsync(Role.Admin))
                await roleManager.CreateAsync( new Role("Admin"));
            if(!await roleManager.RoleExistsAsync(Role.Trainee))
                await roleManager.CreateAsync( new Role("Trainee"));
            if(!await roleManager.RoleExistsAsync(Role.Trainer))
                await roleManager.CreateAsync( new Role("Trainer"));
            if(model.Role!= Role.Admin && model.Role!=Role.Trainee&& model.Role!=Role.Trainer)
            {
                return Ok(new {status="Fail",message="Role is incorrect"});
            }
            // add to admin, trainee, trainer table
            switch(model.Role)
            {
                case Role.Admin:
                
                    Admin admin= new Admin()
                    {
                        AppUser=user
                    };

                    _context.Add(admin);
                    break;
                case Role.Trainee:
                    Trainee trainee = new Trainee()
                    {
                        AppUser = user
                    };     
                    _context.Add(trainee);
                    break;
                case Role.Trainer:
                    Trainer trainer= new Trainer()
                    {
                        AppUser=user
                    };
                    _context.Add(trainer);
                    break;
            }
            result=await userManager.AddToRoleAsync(user,model.Role);
            if(!result.Succeeded)
            {
                string mes="";
                foreach(var error in  result.Errors)
                {
                    mes+=error.Description+"\n";
                }
                return Ok( new {status="error", message=mes});
            }
            return Ok(new {status="Success",message="User created successfully and add to it's table"});
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user= await userManager.FindByNameAsync(model.UserName);
            if(user!=null && await userManager.CheckPasswordAsync(user,model.Password))
            {
                var userRoles=await userManager.GetRolesAsync(user);
                var authClaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                foreach(var userRole in userRoles)
                {
                    authClaim.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigninKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token= new JwtSecurityToken(
                    issuer:_configuration["JWT:ValidIssuer"],
                    audience:_configuration["JWT:ValidAudience"],
                    expires:DateTime.Now.AddHours(5),
                    claims:authClaim,
                    signingCredentials: new SigningCredentials(authSigninKey,SecurityAlgorithms.HmacSha256)
                );
                return Ok(new
                {
                    token= new JwtSecurityTokenHandler().WriteToken(token),
                    UserLoginInfo=user,
                    roleManager=userRoles
                });
            }
            return Unauthorized();
            // return Ok(user);
        }
        
    }
}