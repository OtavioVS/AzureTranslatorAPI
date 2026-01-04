using AzureTradutor.Models;
using AzureTradutor.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureTradutor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslationController : ControllerBase
{
    private readonly TranslationService _translationService;

    public TranslationController(TranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpPost("translate")]
    public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
    {
        if (string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.TargetLanguage))
        {
            return BadRequest("Text and TargetLanguage are required.");
        }

        var translatedText = await _translationService.TranslateTextAsync(request.Text, request.TargetLanguage, request.SourceLanguage);
        var response = new TranslationResponse { TranslatedText = translatedText };
        return Ok(response);
    }

    [HttpPost("translate-article")]
    public async Task<IActionResult> TranslateArticle([FromBody] ScrapeRequest request)
    {
        if (string.IsNullOrEmpty(request.Url) || string.IsNullOrEmpty(request.TargetLanguage))
        {
            return BadRequest("Url and TargetLanguage are required.");
        }

        var article = await _translationService.ScrapeDevToArticlesAsync(request.Url, request.TargetLanguage);
        return Ok(article);
    }
}