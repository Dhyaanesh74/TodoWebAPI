using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using todowebapi.Configurations;
using todowebapi.Exceptions;
using todowebapi.Models;
using todowebapi.Models.DTOs;

namespace todowebapi.Controllers{
  [Route("api/v1/[Controller]")]
  [ApiController]
  public class AuthenticationController : ControllerBase{
     private readonly UserManager<IdentityUser> _userManager;
     private readonly IConfiguration _configuration;
    

     public AuthenticationController(UserManager<IdentityUser> userManager,IConfiguration configuration)
     {
        _userManager = userManager;
        _configuration = configuration;
     }

     [HttpPost("Register")]
     public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto){
        if(ModelState.IsValid){
            var user_exist =  await _userManager.FindByEmailAsync(requestDto.Email);
            if(user_exist !=null){
                return BadRequest(new AuthResults(){
                     Result = false,
                     Errors = new List<string>(){
                        "Email already exist"
                     }
                });
            }

            var new_user = new IdentityUser(){
                Email = requestDto.Email,
                UserName = requestDto.Email
            };

            var is_created = await _userManager.CreateAsync(new_user, requestDto.Password);

            if(is_created.Succeeded){
                    var token  = GenerateJwtToken(new_user);
                    return Ok(new AuthResults(){
                         Result = true,
                         Token = token
                    });
            }
                    return BadRequest(new AuthResults(){
                        Errors = new List<string>(){
                            "Server Error"
                        },
                        Result = false
                    });
        }  

        return BadRequest();
     }
     [HttpPost("Login")]
     public async Task<IActionResult> Login([FromBody] UserLoginRequsetDto loginRequest){
        if (ModelState.IsValid){
           var existing_user =  await _userManager.FindByEmailAsync(loginRequest.Email);
           if(existing_user==null){
              throw new UnAuthorizedException("Unauthorized");
           }
           var isCorrect =  await _userManager.CheckPasswordAsync(existing_user,loginRequest.Password);
            if(!isCorrect){
                  throw new UnAuthorizedException("Password Mismatch");
            }

            var jwtToken = GenerateJwtToken(existing_user);

            return Ok(new AuthResults(){
                Token = jwtToken,
                Result = true
            });

        }

        throw new UnAuthorizedException("Invalid Credintials");
     }


     private string GenerateJwtToken(IdentityUser user){
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key =  Encoding.UTF8.GetBytes(_configuration.GetSection(key:"JwtConfig:Secret").Value);
        var tokenDescriptor = new SecurityTokenDescriptor(){
              Subject =  new ClaimsIdentity(new [] {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
              }),
              Expires = DateTime.Now.AddHours(1),
              SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)
        };

        var token  =  jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken =  jwtTokenHandler.WriteToken(token);
        return jwtToken;
     }

  }
}