using System.Collections.Generic;
using System.Security.Cryptography;
using api_vendace.Entities;
using api_vendace.Interfaces;
using api_vendace.Models.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace api_vendace.Utility;

public class ByteFileUtility
{
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;

    public ByteFileUtility(IWebHostEnvironment environment, IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor)
    {
        this.environment = environment;
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetFileFullPath(string fileName, string enityName)
    {
        var appRootPath = environment.WebRootPath;
        var mediaRootPath = configuration.GetValue<string>("MediaPath");

        return Path.Combine(appRootPath, mediaRootPath!, enityName, fileName);
    }

    public List<TImage> SaveFileInFolder<TImage>(List<IFormFile> files, string entityName, bool isEncrypt = false, bool shouldResize = true)
        where TImage : IThumbnail, new()
    {
        List<TImage> newFileNames = new List<TImage>();
        var appRootPath = environment.WebRootPath;
        var mediaRootPath = configuration.GetValue<string>("MediaPath");

        CheckAndCreatePathDirectory(appRootPath, mediaRootPath!, entityName);
        foreach (var file in files)
        {
            var newFileName = $"{DateTime.Now.Ticks.ToString()}{GetFileExtension(file.FileName)}";
            var newFilePath = Path.Combine(appRootPath, mediaRootPath!, entityName, newFileName);
            var byteArray = ConvertToByteArray(file, false);

            //placeholder
            var placeholderFileName = $"placeholder_{newFileName}";
            var placeholderFilePath = Path.Combine(appRootPath, mediaRootPath!, entityName, placeholderFileName);
            var placeholderBytes = ConvertToByteArray(file, shouldResize);

            if (isEncrypt)
            {
                byteArray = EncryptFile(byteArray);
                placeholderBytes = EncryptFile(placeholderBytes);
            }
            using (var writer = new BinaryWriter(File.OpenWrite(placeholderFilePath)))
            {
                writer.Write(placeholderBytes);
            }
            using (var writer = new BinaryWriter(File.OpenWrite(newFilePath)))
            {
                writer.Write(byteArray);
            }


            var image = new TImage
            {
                ImageUrl = newFileName,
                Placeholder = placeholderFileName
            };
            newFileNames.Add(image);
        }
        return newFileNames;
    }

    public void DeleteFiles<TImage>(ICollection<TImage> images, string entityName)
    where TImage : IThumbnail
    {
        var appRootPath = environment.WebRootPath;
        var mediaRootPath = configuration.GetValue<string>("MediaPath") ?? throw new InvalidOperationException("MediaPath is not configured.");

        foreach (var image in images)
        {
            if (image.ImageUrl != null)
            {
                var imageFilePath = Path.Combine(appRootPath, mediaRootPath, entityName, image.ImageUrl);
                if (File.Exists(imageFilePath))
                {
                    File.Delete(imageFilePath);
                }
            }

            if (image.Placeholder != null)
            {
                var placeholderFilePath = Path.Combine(appRootPath, mediaRootPath, entityName, image.Placeholder);
                if (File.Exists(placeholderFilePath))
                {
                    File.Delete(placeholderFilePath);
                }
            }
        }
    }

    private string GetEntityFolderUrl(string host, string entityName, bool isHttps)
    {
        var mediaRootPath = configuration.GetValue<string>("MediaPath")!.Replace("\\", "/");
        var httpMode = isHttps ? "https" : "http";
        return $"{httpMode}://{host}/{mediaRootPath}/{entityName}";
    }

    private void CheckAndCreatePathDirectory(string appRootPath, string mediaRootPath, string entityFolderName)
    {
        var mediaFullPath = Path.Combine(appRootPath, mediaRootPath);
        if (!Directory.Exists(mediaFullPath))
        {
            Directory.CreateDirectory(mediaFullPath);
        }

        var entityFolderFullPath = Path.Combine(mediaFullPath, entityFolderName);
        if (!Directory.Exists(entityFolderFullPath))
        {
            Directory.CreateDirectory(entityFolderFullPath);
        }
    }

    public List<EntityImageDto> GetEncryptedFileActionUrl(List<EntityImageDto> thumbnailFiles, string entityName)
    {
        List<EntityImageDto> imagesSrc = new List<EntityImageDto>();
        var hostUrl = httpContextAccessor.HttpContext!.Request.Host.Value;
        var isHttps = httpContextAccessor.HttpContext.Request.IsHttps;
        var httpMode = isHttps ? "https" : "https";
        foreach (var thumbnailFile in thumbnailFiles)
        {
            var srcImageUrl = $"{httpMode}://{hostUrl}/api/base/images/{entityName}/{thumbnailFile.ImageUrl}";
            var srcPlaceholder = $"{httpMode}://{hostUrl}/api/base/images/{entityName}/{thumbnailFile.Placeholder}";
            var imageSrc = new EntityImageDto
            {
                Id = thumbnailFile.Id,
                ImageUrl = srcImageUrl,
                Placeholder = srcPlaceholder
            };
            imagesSrc.Add(imageSrc);
        }
        return imagesSrc;
    }

    public byte[] ConvertToByteArray(IFormFile file, bool shouldResize)
    {
        using var stream = new MemoryStream();

        if (shouldResize)
        {
            using (var image = Image.Load(file.OpenReadStream()))
            {
                // Resize the image as needed
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(50, 50),
                    Mode = ResizeMode.Max
                }));

                // Save the resized image to the stream
                image.Save(stream, new JpegEncoder()); // You can choose a different encoder based on your needs
            }
        }
        else
        {
            file.CopyTo(stream);
        }

        return stream.ToArray();
    }
    public string GetFileUrl(string thumbnailFileName, string entityName)
    {
        var hostUrl = httpContextAccessor.HttpContext!.Request.Host.Value;
        var isHttps = httpContextAccessor.HttpContext.Request.IsHttps;
        var folderPath = GetEntityFolderUrl(hostUrl, entityName, isHttps);
        return $"{folderPath}/{thumbnailFileName}";
    }

    public string GetFileExtension(string fileName)
    {
        var fileInfo = new FileInfo(fileName);
        return fileInfo.Extension;
    }

    public string ConvertToBase64(byte[] data)
    {
        return Convert.ToBase64String(data);
    }

    public byte[] EncryptFile(byte[] fileContent)
    {
        string EncryptionKey = configuration.GetValue<string>("EncryptionKey")!;
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }, 1000, HashAlgorithmName.SHA256);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(fileContent, 0, fileContent.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }

    public byte[] DecryptFile(byte[] fileContent)
    {
        string EncryptionKey = configuration.GetValue<string>("EncryptionKey")!;
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }, 1000, HashAlgorithmName.SHA256);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(fileContent, 0, fileContent.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }
    public class GuidArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                var guids = value.Split(',')
                                 .Select(Guid.Parse)
                                 .ToArray();

                bindingContext.Result = ModelBindingResult.Success(guids);
            }
            catch
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid GUID format");
            }

            return Task.CompletedTask;
        }
    }

}

