using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jdenticon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using WePrint.Data;

namespace WePrint.Utilities
{
    public interface IAvatarProvider
    {
        Task<IActionResult> SetAvatarResult<T>(T entity, IFormFile uploadedFile) where T: IIdentifiable<Guid>;
        Task<IActionResult> GetAvatarResult<T>(T entity, bool useIdenticon = true) where T: IIdentifiable<Guid>;
        Task<IActionResult> ClearAvatar<T>(T entity) where T : IIdentifiable<Guid>;
    }

    public class AvatarProvider : IAvatarProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IBlobContainerProvider _blobContainerProvider;
        private readonly IHostEnvironment _hostEnvironment;

        public AvatarProvider(IConfiguration configuration, IBlobContainerProvider blobContainerProvider, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _blobContainerProvider = blobContainerProvider;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> SetAvatarResult<T>(T entity, IFormFile uploadedFile) where T: IIdentifiable<Guid>
        {
            IImageDecoder decoder;

            switch (uploadedFile.ContentType)
            {
                case "image/png":
                    decoder = new PngDecoder();
                    break;

                case "image/gif":
                    decoder = new GifDecoder();
                    break;

                case "image/jpeg":
                    decoder = new JpegDecoder();
                    break;

                case "image/bmp":
                    decoder = new BmpDecoder();
                    break;

                default:
                    return new BadRequestObjectResult(uploadedFile.ContentType + " is not a supported image type");
            }

            // Load settings from config
            var section = _configuration.GetSection("Avatars");
            bool resize = section.GetValue<bool>("Resize");
            int width = section.GetValue<int>("Width");
            int height = section.GetValue<int>("Height");

            // Memory Buffer to hold resized image
            await using var finalImage = new MemoryStream();

            try
            {
                // Load the image from the incoming stream
                using Image image = Image.Load(uploadedFile.OpenReadStream(), decoder);

                // Resize the image and save as PNG
                if (resize)
                    image.Mutate(x => x.Resize(width, height));

                image.SaveAsPng(finalImage);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Unable to read or manipulate image");
            }

            // Upload the blob to Azure
            var avatar = await GetAvatarBlob(entity);
            finalImage.Position = 0;

            await avatar.DeleteIfExistsAsync();
            await avatar.UploadFromStreamAsync(finalImage);
            return new NoContentResult();
        }

        public async Task<IActionResult> GetAvatarResult<T>(T entity, bool useIdenticon = true) where T: IIdentifiable<Guid> 
        {
            if (entity == null)
                return new NotFoundResult();

            var avatar = await GetAvatarBlob(entity);
            Stream imageStream;
            if (await avatar.ExistsAsync())
            {
                imageStream = await avatar.OpenReadAsync();
            }
            else if (useIdenticon)
            {
                imageStream = new MemoryStream();
                await Identicon
                    .FromValue(entity.Id.ToString(), _configuration.GetSection("Avatars").GetValue<int>("Width"))
                    .SaveAsPngAsync(imageStream);
                imageStream.Position = 0;
            }
            else
            {
                return new NotFoundResult();
            }

            return new FileStreamResult(imageStream, "image/png")
            {
                FileDownloadName = entity.Id.ToString("D") + ".png"
            };
        }

        public async Task<IActionResult> ClearAvatar<T>(T entity) where T : IIdentifiable<Guid>
        {
            if (entity == null)
                return new NotFoundResult();
            var avatar = await GetAvatarBlob(entity);
            await avatar.DeleteIfExistsAsync();
            return new NoContentResult();
        }


        private async Task<CloudBlockBlob> GetAvatarBlob<T>(T entity) where T: IIdentifiable<Guid>
        {
            var fileStorage = _blobContainerProvider.GetContainerReference(typeof(T).Name.ToLower() + "-avatars");
            await fileStorage.CreateIfNotExistsAsync();
            return fileStorage.GetBlockBlobReference(entity.Id.ToString("D"));
        }
    }
}
