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
    public class UserController : we_print_controller
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
        public async Task<ActionResult<user_view_model>> GetCurrentUser()
        {
            var vm = mapper.Map<user_view_model>(await current_user);
            return Ok(vm);
        }


        [HttpGet("pledges")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<pledge_view_model>>> GetUserPledges()
        {
            var user = await current_user;
            var pledges = await database.Pledges
                .Where(x => x.maker == user)
                .ProjectTo<pledge_view_model>(mapper.ConfigurationProvider)
                .ToListAsync();
            return pledges;
        }

        [HttpGet("pledges/{project}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<pledge_view_model>>> GetUserProjectPledges(Guid project)
        {
            var user = await current_user;
            var pledges = await database.Pledges
                .Where(x => x.maker == user && x.project.Id == project)
                .ProjectTo<pledge_view_model>(mapper.ConfigurationProvider)
                .ToListAsync();
            return pledges;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser([FromBody] user_view_model updated)
        {
            user current = await current_user;
            if (updated.id != current.Id)
                return Unauthorized();

            mapper.Map(updated, current);
            await database.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("avatar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentAvatar()
        {
            return await _avatar.GetAvatarResult(await current_user);
        }

        [HttpDelete("avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearCurrentAvatar()
        {
            return await _avatar.ClearAvatar(await current_user);
        }

        [HttpPost("avatar")]
        [RequestSizeLimit(5 * 1_000_000)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetCurrentAvatar(IFormFile postedImage)
        {
            var user = await current_user;
            return await _avatar.SetAvatarResult(user, postedImage);
        }

        // GET" /api/users/by-id/{id}
        [HttpGet("by-id/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<user_view_model>> GetUserById(Guid id)
        {
            var targetUser = await database.Users.FindAsync(id);
            if (targetUser == null)
                return NotFound();

            var vm = mapper.Map<user_view_model>(targetUser);
            return vm;
        }

        // GET" /api/users/by-name/{id}
        [HttpGet("by-name/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<user_view_model>> GetUserByUsername(string id)
        {
            var targetUser = await user_manager.FindByNameAsync(id);
            if (targetUser == null)
                return NotFound();

            var vm = mapper.Map<user_view_model>(targetUser);
            return vm;
        }

        [HttpGet("by-id/{id}/avatar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvatarById(Guid id)
        {
            var targetUser = await database.Users.FindAsync(id);
            return await _avatar.GetAvatarResult(targetUser);
        }

        [HttpGet("by-name/{id}/avatar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvatarByName(string id)
        {
            var targetUser = await user_manager.FindByNameAsync(id);
            return await _avatar.GetAvatarResult(targetUser);
        }
    }
}
