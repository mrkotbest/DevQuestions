namespace DevQuestions.Application.FilesStorage;

public interface IFileProvider
{
    public Task<string> UploadAsync(Stream stream, string key, string bucket);
}
