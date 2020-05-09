using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : WePrintController
    {
        private readonly IAvatarProvider _avatar;

        public UserController(IServiceProvider services, IAvatarProvider avatar) : base(services)
        {
            _avatar = avatar;
        }

        // GET: /api/users/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserViewModel>> GetCurrentUser()
        {
            var vm = Mapper.Map<UserViewModel>(await CurrentUser);
            return Ok(vm);
        }


        [HttpGet("pledges")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PledgeViewModelNoUser>>> GetUserPledges()
        {
            var user = await CurrentUser;
            var pledges = await Database.Pledges
                .Where(x => x.Maker == user)
                .ProjectTo<PledgeViewModelNoUser>(Mapper.ConfigurationProvider)
                .ToListAsync();
            return pledges;
        }

        [HttpGet("pledges/{project}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PledgeViewModelNoUser>>> GetUserProjectPledges(Guid project)
        {
            var user = await CurrentUser;
            var pledges = await Database.Pledges
                .Where(x => x.Maker == user && x.Project.Id == project)
                .ProjectTo<PledgeViewModelNoUser>(Mapper.ConfigurationProvider)
                .ToListAsync();
            return pledges;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel updated)
        {
            User current = await CurrentUser;
            if (updated.Id != current.Id)
                return Unauthorized();

            Mapper.Map(updated, current);
            await Database.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("avatar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentAvatar()
        {
            return await _avatar.GetAvatarResult(await CurrentUser);
        }

        [HttpDelete("avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearCurrentAvatar()
        {
            return await _avatar.ClearAvatar(await CurrentUser);
        }

        [HttpPost("avatar")]
        [RequestSizeLimit(5 * 1_000_000)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetCurrentAvatar(IFormFile postedImage)
        {
            var user = await CurrentUser;
            return await _avatar.SetAvatarResult(user, postedImage);
        }

        // GET" /api/users/by-id/{id}
        [HttpGet("by-id/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> GetUserById(Guid id)
        {
            var targetUser = await Database.Users.FindAsync(id);
            if (targetUser == null)
                return NotFound();

            var vm = Mapper.Map<UserViewModel>(targetUser);
            return vm;
        }

        // GET" /api/users/by-name/{id}
        [HttpGet("by-name/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> GetUserByUsername(string id)
        {
            var targetUser = await UserManager.FindByNameAsync(id);
            if (targetUser == null)
                return NotFound();

            var vm = Mapper.Map<UserViewModel>(targetUser);
            return vm;
        }

        [HttpGet("by-id/{id}/avatar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvatarById(Guid id)
        {
            var targetUser = await Database.Users.FindAsync(id);
            return await _avatar.GetAvatarResult(targetUser);
        }

        [HttpGet("by-name/{id}/avatar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvatarByName(string id)
        {
            var targetUser = await UserManager.FindByNameAsync(id);
            return await _avatar.GetAvatarResult(targetUser);
        }
    }
}
