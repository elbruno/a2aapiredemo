using SingleAgentDemo.Models;

namespace SingleAgentDemo.Services;

public interface IAnalyzePhotoService
{
    Task<PhotoAnalysisResult> AnalyzePhotoAsync(IFormFile image, string prompt);
}