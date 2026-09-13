using DevQuestions.Application.FilesStorage;

namespace DevQuestions.Infrastructure.S3;

public class S3Provider : IFileProvider
{
    public Task<string> UploadAsync(Stream stream, string key, string bucket)
    {
        throw new NotImplementedException();
    }
}
