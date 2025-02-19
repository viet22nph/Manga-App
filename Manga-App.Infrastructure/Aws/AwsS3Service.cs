

using Amazon.S3;
using Amazon.S3.Model;
using MangaApp.Application.Abstraction.Services;
using MangaApp.Infrastructure.DependencyInjection.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MangaApp.Infrastructure.Aws;

public class AwsS3Service : IAwsS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsS3Options _awsS3Option;
    public AwsS3Service(IAmazonS3 s3Client, IOptions<AwsS3Options> option)
    {
        _s3Client = s3Client;
        _awsS3Option = option.Value ?? throw new ArgumentNullException(nameof(option));
        if (string.IsNullOrEmpty(_awsS3Option.BucketName))
        {
            throw new Exception("AWS S3 bucket name is empty");

        }
    }
    /// <summary>
    /// Upload image file to aws bucket s3
    /// </summary>
    /// <param name="fileName">The name of the file to be stored in S3.</param>
    /// <param name="file">The image file to upload</param>
    /// <returns>Url s3 when upload file success</returns>
    /// <exception cref="Exception"> Thrown when the file extension is not allowed or the upload fails.
    /// </exception>
    public async Task<string> UploadFileImageAsync(string fileName, IFormFile file)
    {
        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        if (!IsFileExtensionAllowed(file, allowedExtensions))
        {
            throw new Exception($"File {file.FileName} extension not allowed.");
        }
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var request = new PutObjectRequest
            {
                BucketName = _awsS3Option.BucketName,
                Key = fileName,
                InputStream = ms,
                ContentType = file.ContentType,
            };
            var response = await _s3Client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return $"https://{_awsS3Option.BucketName}.s3.amazonaws.com/{fileName}";
            }

            throw new Exception("File upload failed");
        }
    }
    /// <summary>
    /// Convert url bucket s3 to cloudfront 
    /// </summary>
    /// <param name="s3Url">Url bucket s3</param>
    /// <returns>Url cloudfront</returns>
    public string ConvertBucketS3ToCloudFront(string s3Url)
    {
        if (string.IsNullOrEmpty(s3Url)) return s3Url;
        return s3Url.Replace($"https://{_awsS3Option.BucketName}.s3.amazonaws.com/", _awsS3Option.CloudFront);
    }
    private bool IsFileExtensionAllowed(IFormFile file, string[] allowedExtensions)
    {
        var extension = Path.GetExtension(file.FileName);
        return allowedExtensions.Contains(extension);
    }
}
