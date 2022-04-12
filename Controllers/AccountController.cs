using System.Text;
using System.Net;
using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Api.Controller
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(DataContext context, ITokenService tokenService,UserManager<AppUser> userManager,SignInManager<AppUser> _signInManager)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
            this._signInManager = _signInManager;

        }


        [HttpPost]
        [Route("register")]
        // Controller automatic determine from where data is coming like body or query or routing
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDto)
        {
            if (this.UserExists(registerDto.Username))
            {
                return BadRequest("user is already registered");
            }
            //using var hmac = new HMACSHA512();
            var user = new AppUser
            {

                UserName = registerDto.Username.ToLower(),
                //PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                //PasswordSalt = hmac.Key,
                knownAs=registerDto.knownAs,
                Gender=registerDto.Gender,
                DateOfBirth=registerDto.DateOfBirth.ToUniversalTime()
               
            };
            //add user to database 
            await _userManager.CreateAsync(user, registerDto.password);
            await _userManager.AddToRoleAsync(user, "Member");

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                UserPic = (user.Photos?.FirstOrDefault(x => x.IsMain))?.Url,
                Gender=user.Gender
                
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDTO loginDto)
        {

            //find username  data
            var user = await _userManager.Users.
                Include(p=>p.Photos).
                SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null)
                return Unauthorized("Invalid username");
            var result = await this._signInManager.CheckPasswordSignInAsync(user, loginDto.password,true);         //insert salt
            //using var hmac = new HMACSHA512(user.PasswordSalt);
            ////computed hash with salt
            //var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
            ////check hash   
            //for (int i = 0; i < computedHash.Length; i++)
            //{
            //    if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            //}
            //Console.WriteLine(_tokenService.CreateToken(user));

            if (!result.Succeeded)
                return BadRequest("Wrong Password");
            UserDto res;
            if (user.Photos.Count!= 0)
            {
                 res = new UserDto
                {
                    UserName = user.UserName,
                    Token =await _tokenService.CreateToken(user),
                     UserPic = (user.Photos?.FirstOrDefault(x => x.IsMain))?.Url,
                     Gender = user.Gender
                 };
            }
            else
            {
                 res = new UserDto
                 {
                    UserName = user.UserName,
                    Token =await _tokenService.CreateToken(user),
                    UserPic =null,
                    Gender = user.Gender
                 };
            }
            return res;
        }



        private bool UserExists(string username)
        {
            //checkusername is already exists or not
            return _userManager.Users.Any(x => x.UserName == username.ToLower());
        }




    }


}