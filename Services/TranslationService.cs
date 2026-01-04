using Azure.AI.Translation.Text;
using Azure;
using HtmlAgilityPack;
using ReverseMarkdown;

namespace AzureTradutor.Services;

public class TranslationService
{
    private readonly TextTranslationClient? _client;
    private readonly HttpClient _httpClient;
    private readonly Converter _markdownConverter;
    private readonly bool _useMock;

    public TranslationService(IConfiguration configuration)
    {
        _useMock = configuration.GetValue<bool>("AzureTranslator:UseMock");

        if (!_useMock)
        {
            var endpoint = configuration["AzureTranslator:Endpoint"];
            var key = configuration["AzureTranslator:Key"];
            var region = configuration["AzureTranslator:Region"];

            _client = new TextTranslationClient(new AzureKeyCredential(key!), new Uri(endpoint!), region!);
        }

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        _markdownConverter = new Converter();
    }

    public async Task<string> TranslateTextAsync(string text, string targetLanguage, string? sourceLanguage = null)
    {
        if (_useMock)
        {
            // Mock translation: prepend "Translated to [lang]: " 
            return $"Translated to {targetLanguage}: {text}";
        }

        var options = new TextTranslationTranslateOptions(targetLanguage, text);
        if (!string.IsNullOrEmpty(sourceLanguage))
        {
            options.SourceLanguage = sourceLanguage;
        }

        var response = await _client!.TranslateAsync(options);
        var translation = response.Value.FirstOrDefault();
        return translation?.Translations?.FirstOrDefault()?.Text ?? "Translation not available";
    }

    public async Task<ArticleSummary> ScrapeDevToArticlesAsync(string url, string targetLanguage)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // For dev.to articles, extract title and content
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'fs-2xl')]");
            var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'crayons-article__body')]");

            if (titleNode == null || contentNode == null)
            {
                return new ArticleSummary
                {
                    OriginalTitle = "Title not found",
                    TranslatedTitle = await TranslateTextAsync("Title not found", targetLanguage),
                    OriginalSummary = "Content not found",
                    TranslatedSummary = await TranslateTextAsync("Content not found", targetLanguage),
                    Link = url
                };
            }

            var title = titleNode.InnerText.Trim();
            var content = contentNode.InnerText.Trim();

            // Convert content to Markdown
            var markdownContent = _markdownConverter.Convert(content);

            // Translate title and content
            var translatedTitle = await TranslateTextAsync(title, targetLanguage);
            var translatedContent = await TranslateTextAsync(markdownContent, targetLanguage);

            return new ArticleSummary
            {
                OriginalTitle = title,
                TranslatedTitle = translatedTitle,
                OriginalSummary = content,
                TranslatedSummary = translatedContent,
                Link = url
            };
        }
        catch (HttpRequestException)
        {
            // Mock response for any HTTP error (403, 404, etc.)
            return new ArticleSummary
            {
                OriginalTitle = "Sample Article Title",
                TranslatedTitle = await TranslateTextAsync("Sample Article Title", targetLanguage),
                OriginalSummary = "This is a sample article content because the site blocked or the URL was not found.",
                TranslatedSummary = await TranslateTextAsync("This is a sample article content because the site blocked or the URL was not found.", targetLanguage),
                Link = url
            };
        }
    }
}

public class ArticleSummary
{
    public string OriginalTitle { get; set; } = string.Empty;
    public string TranslatedTitle { get; set; } = string.Empty;
    public string OriginalSummary { get; set; } = string.Empty;
    public string TranslatedSummary { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}