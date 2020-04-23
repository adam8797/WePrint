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
using System.IO;
namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/qrcodes")]
    public class QrCodeController : WePrintController
    {
        public QrCodeController(IServiceProvider services) : base(services)
        {
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
            Bitmap qrCodeImg = (new QRCode(gen.CreateQrCode($"{HttpContext.Request.Host}/api/qrcodes/scan/{pledgeId}", QRCodeGenerator.ECCLevel.Q))).GetGraphic(20);
            ImageConverter converter = new ImageConverter();
            return File((byte[])converter.ConvertTo(qrCodeImg, typeof(byte[])), "image/jpeg");
        }

        [HttpGet("scan/{pledgeId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ScanQrCode(Guid pledgeId)
        {
            var pledge = await Database.Pledges.FindAsync(pledgeId);
            if (pledge == null)
                return NotFound();

            pledge.Status = PledgeStatus.Finished;
            await Database.SaveChangesAsync();
            return Ok();
        }

    }
}
