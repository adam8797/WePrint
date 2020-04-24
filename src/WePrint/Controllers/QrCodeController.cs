using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WePrint.Controllers.Base;
using WePrint.Models;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WePrint.Permissions;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/qrcodes")]
    public class QrCodeController : WePrintController
    {
        private readonly IPermissionProvider<Project, ProjectCreateModel> _projectPermissionProvider;

        public QrCodeController(IServiceProvider services, IPermissionProvider<Project, ProjectCreateModel> projectPermissionProvider) : base(services)
        {
            _projectPermissionProvider = projectPermissionProvider;
        }

        [Authorize]
        [HttpGet("generate/{pledgeId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQrCode(Guid pledgeId)
        {
            var currentUser = await CurrentUser;
            if (currentUser == null)
                return Unauthorized();

            var pledge = await Database.Pledges.FindAsync(pledgeId);
            if (pledge == null)
                return NotFound();
            if (pledge.Maker != currentUser)
                return Forbid();

            QRCodeGenerator gen = new QRCodeGenerator();
            Bitmap qrCodeImg = (new QRCode(gen.CreateQrCode($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{Url.Action("ScanQrCode", new { pledgeId })}", QRCodeGenerator.ECCLevel.Q))).GetGraphic(20);

            await using var ms = new MemoryStream();
            qrCodeImg.Save(ms, ImageFormat.Png);
            return File(ms.ToArray(), "image/png", "qrcode.png");
        }

        [HttpGet("scan/{pledgeId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ScanQrCode(Guid pledgeId)
        {
            var pledge = await Database.Pledges.FindAsync(pledgeId);
            if (pledge == null)
                return NotFound();

            if (!await _projectPermissionProvider.AllowWrite(await CurrentUser, pledge.Project))
                return Forbid();

            pledge.Status = PledgeStatus.Finished;
            await Database.SaveChangesAsync();
            return Ok();
        }

    }
}
