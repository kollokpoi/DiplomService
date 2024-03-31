using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiplomService.Controllers.ApiContollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private static Dictionary<string, string> codes = new Dictionary<string, string>();
        private static readonly TimeSpan timeToNextRequest = new TimeSpan(0, 3, 0);

        public UserController(ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager) 
        {
            this._context = context;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        [HttpPost("CheckPhone")]
        public async Task<IActionResult> CheckPhone([FromBody]ApiAuth apiAuth)
        {
            var user =await _context.MobileUsers.FirstOrDefaultAsync(x=>x.PhoneNumber == PhoneWorker.NormalizePhone(apiAuth.Phone));
            if (user != null)
            {
                GenerateCode(user.Id);
                return Ok(new {timeToNextRequest});
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("LoginByCode")]
        public async Task<IActionResult> LoginByCode([FromBody]ApiAuth apiAuth)
        {
            var user = await _context.MobileUsers.FirstOrDefaultAsync(x => x.PhoneNumber == PhoneWorker.NormalizePhone(apiAuth.Phone));

            if (user != null)
            {
                string original = codes[user.Id];
                if (original == apiAuth.Code)
                {
                    
                    await _signInManager.SignInAsync(user, true);
                    
                    return Ok(user.SecurityStamp);
                }
                return BadRequest();
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost("AddDeviceKey")]
        public async Task<IActionResult> AddDeviceKey([FromBody] string token)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user != null)
                if (!user.DeviceTokens.Any(x => x.DeviceToken == token))
                {
                    _context.UserDeviceTokens.Add(new()
                    {
                        User = user,
                        DeviceToken = token,
                    });
                    _context.SaveChanges();
                    return Ok();
                }
            return BadRequest();
        }

        [HttpPost("ResendCode")]
        public async Task<IActionResult> ResendCode([FromBody] ApiAuth apiAuth)
        {
            var user = await _context.MobileUsers.FirstOrDefaultAsync(x => x.PhoneNumber == PhoneWorker.NormalizePhone(apiAuth.Phone));

            if (user != null)
            {
                GenerateCode(user.Id);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("CheckAuthorize")]
        public async Task<IActionResult> CheckAuthorize()
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is not null)
                return Ok(new { stamp = user.SecurityStamp });
            return BadRequest();
        }

        [HttpPost("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user == null)
                return NotFound();

            var viewModel = new MobileUserViewModel
            {
                Name = user.Name??"",
                SecondName = user.SecondName ?? "",
                LastName = user.LastName ?? "",
                Course= user.Course,
                Birthday = user.Birthday,
                PhoneNumber = user.PhoneNumber ?? ""
            };
            if (user.Image!=null)
            {
                viewModel.SetImageBytes(user.Image);
            }
            return Ok(viewModel);
        }
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody]MobileUserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return NotFound();

            if (user.PhoneNumber!=model.PhoneNumber)
            {
                GenerateCode(user.Id);
            }

            user.Image = model.GetDecodedImage();
            user.Name = model.Name;
            user.SecondName = model.SecondName;
            user.LastName = model.LastName;
            user.Course = model.Course;
            user.Birthday= model.Birthday;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("UpdatePhone")]
        public async Task<IActionResult> UpdatePhone([FromBody] ApiAuth code)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return NotFound();

            string original = codes[user.Id];
            if (original == code.Code)
            {
                user.PhoneNumber = code.Phone;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
        private async void GenerateCode(string userId)
        {
            codes.Remove(userId);

            string code = "";
            var random = new Random();
            
            for (int i = 0; i < 5; i++)
                code += random.Next(10);
            Console.WriteLine(code);
            codes.Add(userId, code);

            await Task.Delay(timeToNextRequest);
            codes.Remove(userId);
        }
    }
}
