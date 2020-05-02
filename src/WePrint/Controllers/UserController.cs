using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using WePrint.Controllers.Base;
using WePrint.Models;
using WePrint.Utilities;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : WePrintController
    {
        private readonly IAvatarProvider _avatar;

        public UserController(IServiceProvider services, IAvatarProvider avatar) : base(services)
        {
            _avatar = avatar;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            var user = await FindUser(id);
            if (id.ToLower() == "current" && user == null)
                return Unauthorized();

            if (user == null || user.Deleted)
                return NotFound();

            var vm = Mapper.Map<UserViewModel>(user);
            return vm;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers()
        {
            return Mapper.Map<List<UserViewModel>>(await Database.Users
                .OrderBy(x => x.UserName)
                .Where(x => !x.Deleted)
                .ToListAsync());
        }


        private async Task<User> FindUser(string query)
        {
            if (query.ToLower() == "current")
                    return await CurrentUser;
            
            if (Guid.TryParse(query, out _))
                return await UserManager.FindByIdAsync(query);
            
            return await UserManager.FindByNameAsync(query);
        }

        [HttpPatch("current")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] JsonPatchDocument<UserViewModelFacade> patchDoc)
        {
            User current = await CurrentUser;

            var facade = new UserViewModelFacade(current);
            patchDoc.ApplyTo(facade);

            await Database.SaveChangesAsync();

            return NoContent();
        }

        #region Avatar Stuff

        [HttpGet("{id}/avatar")]
        public async Task<IActionResult> GetCurrentAvatar()
        {
            return await _avatar.GetAvatarResult(await CurrentUser);
        }

        [Authorize]
        [HttpDelete("current/avatar")]
        public async Task<IActionResult> ClearCurrentAvatar()
        {
            return await _avatar.ClearAvatar(await CurrentUser);
        }

        [Authorize]
        [HttpPost("current/avatar")]
        [RequestSizeLimit(5 * 1_000_000)]
        public async Task<IActionResult> SetCurrentAvatar(IFormFile postedImage)
        {
            var user = await CurrentUser;
            return await _avatar.SetAvatarResult(user, postedImage);
        }

        #endregion
    }
}
