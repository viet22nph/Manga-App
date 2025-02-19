

using Microsoft.AspNetCore.Http;

namespace MangaApp.Application.Abstraction.Services;

public interface IAwsS3Service
{
    Task<string> UploadFileImageAsync(string fileName, IFormFile file);
    string ConvertBucketS3ToCloudFront(string awsS3Url);
}
