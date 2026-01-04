# Azure Translator API

A .NET 9 Web API that provides text translation and article scraping with Markdown conversion, powered by Azure AI Translator.

## Features

- **Text Translation**: Translate plain text to any supported language.
- **Article Translation**: Scrape articles from URLs, convert HTML to Markdown, and translate the content.
- **Mock Mode**: Results are mocked by default for development. Switch to real Azure Translator by updating `appsettings.json`.

## Prerequisites

- .NET 9 SDK
- Azure subscription (for real translations)

## Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/OtavioVS/AzureTranslatorAPI.git
   cd AzureTranslatorAPI
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Update `appsettings.json` with your Azure Translator settings (optional for mock mode):
   ```json
   {
     "AzureTranslator": {
       "UseMock": false,
       "Endpoint": "https://your-resource.cognitiveservices.azure.com/",
       "Key": "your-api-key",
       "Region": "your-region"
     }
   }
   ```

4. Run the application:
   ```bash
   dotnet run
   ```
   The API will be available at `http://localhost:8080`.

## API Endpoints

### 1. Translate Text
**POST** `/api/translation/translate`

Translates plain text.

**Request Body:**
```json
{
  "text": "Hello, world!",
  "targetLanguage": "es",
  "sourceLanguage": "en"
}
```

**Response:**
```json
{
  "translatedText": "Translated to es: Hello, world!"
}
```

**Example (PowerShell):**
```powershell
Invoke-WebRequest -Uri "http://localhost:8080/api/translation/translate" -Method POST -Headers @{ "Content-Type" = "application/json" } -Body '{"text":"Hello","targetLanguage":"es"}'
```

### 2. Translate Article
**POST** `/api/translation/translate-article`

Scrapes an article from a URL, converts to Markdown, and translates.

**Request Body:**
```json
{
  "url": "https://example.com/article",
  "targetLanguage": "es"
}
```

**Response:**
```json
{
  "originalTitle": "Sample Article Title",
  "translatedTitle": "Translated to es: Sample Article Title",
  "originalSummary": "This is a sample article content...",
  "translatedSummary": "Translated to es: This is a sample article content...",
  "link": "https://example.com/article"
}
```

**Example (PowerShell):**
```powershell
Invoke-WebRequest -Uri "http://localhost:8080/api/translation/translate-article" -Method POST -Headers @{ "Content-Type" = "application/json" } -Body '{"url":"https://example.com","targetLanguage":"es"}'
```

## Mock Mode

By default, `"UseMock": true` in `appsettings.json`. This simulates translations (e.g., "Translated to [lang]: [text]") without requiring Azure credentials. For production, set to `false` and provide real Azure settings.

## OpenAPI Spec

Access the API documentation at `http://localhost:8080/swagger/v1/swagger.json`.

## Technologies

- .NET 9
- ASP.NET Core Web API
- Azure AI Translation Text
- HtmlAgilityPack (for scraping)
- ReverseMarkdown (for HTML to Markdown conversion)

## License

MIT