using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Models.Users;
using DiplomService.Services;
using DiplomService.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
        public async Task<IActionResult> CheckPhone([FromBody] ApiAuth apiAuth)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == PhoneWorker.NormalizePhone(apiAuth.Phone));
            if (user != null)
            {
                GenerateCode(user);
                return Ok(new { timeToNextRequest });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("LoginByCode")]
        public async Task<IActionResult> LoginByCode([FromBody] ApiAuth apiAuth)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == PhoneWorker.NormalizePhone(apiAuth.Phone));

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
        [Authorize]
        public async Task<IActionResult> AddDeviceKey([FromBody] string token)
        {
            var user = await _userManager.GetUserAsync(User);
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
                else
                {
                    user.DeviceTokens.RemoveAll(x => x.DeviceToken != token);
                    _context.SaveChanges();
                    return Ok();
                }
            return BadRequest();
        }

        [HttpPost("ResendCode")]
        public async Task<IActionResult> ResendCode([FromBody] ApiAuth apiAuth)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == PhoneWorker.NormalizePhone(apiAuth.Phone));

            if (user != null)
            {
                GenerateCode(user);
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
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
                return Ok(new { stamp = user.SecurityStamp, role = await _userManager.GetRolesAsync(user) });
            return BadRequest();
        }

        [HttpPost("GetUser")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var viewModel = new MobileUserViewModel
            {
                Name = user.Name ?? "",
                SecondName = user.SecondName ?? "",
                LastName = user.LastName ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            };
            if (user is MobileUser)
            {
                viewModel.Course = (user as MobileUser).Course;
                viewModel.Birthday = (user as MobileUser).Birthday;
            }
            if (user.Image != null)
            {
                viewModel.SetImageBytes(user.Image);
            }
            return Ok(viewModel);
        }
        [HttpPost("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] MobileUserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return NotFound();

            if (user.PhoneNumber != model.PhoneNumber)
            {
                GenerateCode(user);
            }

            user.Image = model.GetDecodedImage();
            user.Name = model.Name;
            user.SecondName = model.SecondName;
            user.LastName = model.LastName;
            user.Course = model.Course;
            user.Birthday = model.Birthday;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("UpdatePhone")]
        [Authorize]
        public async Task<IActionResult> UpdatePhone([FromBody] ApiAuth code)
        {
            var user = await _userManager.GetUserAsync(User) as MobileUser;
            if (user is null)
                return NotFound();

            string original = codes[user.Id];
            if (original == code.Code)
            {
                user.PhoneNumber = code.Phone;
                user.UserName = code.Phone;
                user.NormalizedUserName = code.Phone;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("UserSuggestion")]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> UserSuggestion(string suggestion)
        {
            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return Unauthorized();

            var users = new List<User>();
            suggestion = suggestion.Split('(')[0];
            if (suggestion != string.Empty)
            {
                var organizationUsers = user.Organization.OrganizationUsers.Where(x => x.GetFullName().ToLower().Contains(suggestion.ToLower()));
                if (organizationUsers is not null && organizationUsers.Any())
                {
                    users.AddRange(organizationUsers);
                }
                var webUsers = _context.WebUsers.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(suggestion.ToLower()));
                if (webUsers is not null && webUsers.Any())
                {
                    users.AddRange(webUsers);
                }
                var result = new List<UserSuggestionViewModel>();
                foreach (var item in users)
                {
                    if (item is OrganizationUsers)
                    {
                        result.Add(new()
                        {
                            Id = item.Id,
                            UserName = item.GetFullName() + $"(Организация: {(item as OrganizationUsers)?.Organization.Name})"
                        });
                    }
                    else if (item is WebUser)
                    {
                        result.Add(new()
                        {
                            Id = item.Id,
                            UserName = item.GetFullName() + $"(Образовательное учреждение: {(item as WebUser)?.WorkingPlace})"
                        }); ;
                    }

                }
                return Ok(result);
            }
            return NotFound();
        }
        [HttpGet("DivisionMemberSuggestion")]
        [Authorize(Roles = "OrganizationUser")]
        public async Task<IActionResult> DivisionMemberSuggestion(string suggestion, bool divsionLeader = false)
        {
            var user = await _userManager.GetUserAsync(User) as OrganizationUsers;
            if (user is null)
                return Unauthorized();

            var model = new List<MobileUserViewModel>();
            if (divsionLeader)
            {
                var webUsers = _context.WebUsers.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(suggestion.ToLower())).ToList();
                var organizationUsers = user.Organization.OrganizationUsers.Where(x => x.GetFullName().ToLower().Contains(suggestion.ToLower())).ToList();
                foreach (var item in webUsers)
                {
                    model.Add(new()
                    {
                        Id = item.Id,
                        Name = item.Name ?? string.Empty,
                        SecondName = item.SecondName ?? string.Empty,
                        LastName = item.LastName ?? string.Empty,
                        WorkingPlace = item.WorkingPlace,
                    });
                }
                foreach (var item in organizationUsers)
                {
                    model.Add(new()
                    {
                        Id = item.Id,
                        Name = item.Name ?? string.Empty,
                        SecondName = item.SecondName ?? string.Empty,
                        LastName = item.LastName ?? string.Empty,
                        WorkingPlace = user.Organization.Name,
                    });
                }
            }
            else
            {
                var list = _context.MobileUsers.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(suggestion.ToLower()));
                foreach (var item in list)
                {
                    model.Add(new()
                    {
                        Id = item.Id,
                        Name = item.Name ?? string.Empty,
                        SecondName = item.SecondName ?? string.Empty,
                        LastName = item.LastName ?? string.Empty,
                        WorkingPlace = item.WorkingPlace,
                    });
                }
            }


            return Ok(model);
        }

        [HttpGet("ApplicationMemberSuggestion")]
        [Authorize(Roles = "WebUser")]
        public async Task<IActionResult> ApplicationMemberSuggestion(string suggestion)
        {
            var user = await _userManager.GetUserAsync(User) as WebUser;
            if (user is null)
                return Unauthorized();

            var model = new List<MobileUserViewModel>();

            var list = _context.MobileUsers.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(suggestion.ToLower()));
            foreach (var item in list)
            {
                model.Add(new()
                {
                    Id = item.Id,
                    Name = item.Name ?? string.Empty,
                    SecondName = item.SecondName ?? string.Empty,
                    LastName = item.LastName ?? string.Empty,
                    WorkingPlace = item.WorkingPlace,
                });
            }

            return Ok(model);
        }
        private async void GenerateCode(User user)
        {
            codes.Remove(user.Id);

            string code = "";
            var random = new Random();

            for (int i = 0; i < 5; i++)
                code += random.Next(10);
            Console.WriteLine(code);
            codes.Add(user.Id, code);
            //SmsService.SendSms(user.PhoneNumber,$"PrograMath. Ваш код авторизации - {code}");

            await Task.Delay(timeToNextRequest);
            codes.Remove(user.Id);
        }
    }
}
