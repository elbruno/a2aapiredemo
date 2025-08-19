using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace AnalyzePhotoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotoAnalysisController : ControllerBase
{
    private readonly ILogger<PhotoAnalysisController> _logger;

    public PhotoAnalysisController(ILogger<PhotoAnalysisController> logger)
    {
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<PhotoAnalysisResult>> AnalyzeAsync([FromForm] IFormFile image, [FromForm] string prompt)
    {
        try
        {
            _logger.LogInformation("Analyzing photo with prompt: {Prompt}", prompt);

            // Simulate AI photo analysis
            await Task.Delay(1000); // Simulate processing time

            var result = new PhotoAnalysisResult
            {
                Description = $"Photo analysis for prompt: '{prompt}'. Detected a room that needs renovation work. The image shows surfaces that require preparation and finishing.",
                DetectedMaterials = DetermineDetectedMaterials(prompt, image.FileName)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing photo");
            return StatusCode(500, "An error occurred while analyzing the photo");
        }
    }

    private string[] DetermineDetectedMaterials(string prompt, string? fileName)
    {
        var materials = new List<string>();

        // Simple keyword-based material detection
        var promptLower = prompt.ToLower();
        var fileNameLower = fileName?.ToLower() ?? "";

        if (promptLower.Contains("paint") || promptLower.Contains("wall"))
            materials.AddRange(new[] { "paint", "wall", "surface preparation" });

        if (promptLower.Contains("wood") || promptLower.Contains("deck"))
            materials.AddRange(new[] { "wood", "stain", "sanding" });

        if (promptLower.Contains("tile") || promptLower.Contains("bathroom"))
            materials.AddRange(new[] { "tile", "grout", "adhesive" });

        if (promptLower.Contains("garden") || promptLower.Contains("landscape"))
            materials.AddRange(new[] { "soil", "plants", "tools" });

        // Default materials if none detected
        if (materials.Count == 0)
            materials.AddRange(new[] { "general tools", "measuring", "safety equipment" });

        return materials.ToArray();
    }
}
