namespace AzureTradutor.Models;

public class TranslationRequest
{
    public string Text { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
    public string? SourceLanguage { get; set; }
}

public class TranslationResponse
{
    public string TranslatedText { get; set; } = string.Empty;
}

public class ScrapeRequest
{
    public string Url { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
}