using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Infocad.DynamicSpace.RegexAI
{
    public class RegexAIService : DynamicSpaceAppService, IRegexAIService
    {
        private readonly IConfiguration _configuration;
        private readonly IChatClient _chatClient;

        public RegexAIService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Inizializzazione del client Ollama con l'URL e il modello configurati
            var baseUrl = configuration["Ollama:Url"] ?? "http://localhost:11434";
            var model = configuration["Ollama:Model"] ?? "mistral";

            _chatClient = new OllamaChatClient(new Uri(baseUrl), model);
        }

        public async Task<RegexResponseDto> GetRegexPath(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
            }

            // Istruzioni chiare e precise per il modello
            string systemPrompt = @"
Sei un esperto di espressioni regolari per .NET. Genera SOLO regex valide e ben formate.

ERRORI DA EVITARE:
1. MAI posizionare il carattere '-' in mezzo a una classe di caratteri senza escape: SBAGLIATO: [\w-\.] CORRETTO: [\w\.-]
2. MAI inserire '.' senza escape fuori dalle classi di caratteri: SBAGLIATO: email@domain.com CORRETTO: email@domain\.com
3. SEMPRE usare escape per caratteri speciali come '.': SBAGLIATO: .com CORRETTO: \.com

ESEMPI CORRETTI:
- Email generico: ^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$
- Email specifico dominio: ^[a-zA-Z0-9._%+-]+@example\.com$
- Numeri di telefono: ^[0-9]{3}-[0-9]{3}-[0-9]{4}$

RITORNA SOLO questo formato JSON esatto, nient'altro:
{""valid"": true, ""pattern"": ""REGEX_QUI"", ""explanation"": ""breve spiegazione""}
";

            try
            {
                var messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System,systemPrompt),

                    new ChatMessage(ChatRole.User, prompt)
                };
                var chatOptions = new ChatOptions();
                var chatResponse = await _chatClient.GetResponseAsync(messages, chatOptions);


                // Estrazione del testo dalla risposta
                string response = chatResponse.Text;

                if (!string.IsNullOrEmpty(response))
                {
                    try
                    {
                        // Tentativo di deserializzazione diretta
                        var result = JsonSerializer.Deserialize<RegexResponseDto>(
                            response,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                        if (result != null)
                            return result;
                    }
                    catch (JsonException)
                    {
                        // Fallback: estrazione pattern tramite regex
                        var patternMatch = System.Text.RegularExpressions.Regex.Match(
                            response,
                            "\"pattern\"\\s*:\\s*\"([^\"]+)\"");

                        if (patternMatch.Success)
                        {
                            return new RegexResponseDto
                            {
                                Valid = true,
                                Pattern = patternMatch.Groups[1].Value,
                                Explanation = "Pattern estratto dalla risposta del modello"
                            };
                        }

                        // Ulteriore tentativo: ricerca diretta di regex
                        var regexMatch = System.Text.RegularExpressions.Regex.Match(
                            response,
                            @"(\^[a-zA-Z0-9\[\]\(\)\{\}\.\+\*\?\|\^\$\\-]+\$)");

                        if (regexMatch.Success)
                        {
                            return new RegexResponseDto
                            {
                                Valid = true,
                                Pattern = regexMatch.Groups[1].Value,
                                Explanation = "Pattern regex estratto dalla risposta del modello"
                            };
                        }
                    }
                }

                // Risposta fallback in caso di problemi
                return new RegexResponseDto
                {
                    Valid = false,
                    Pattern = string.Empty,
                    Explanation = "Impossibile estrarre un pattern regex valido dalla risposta del modello"
                };
            }
            catch (Exception ex)
            {
                return new RegexResponseDto
                {
                    Valid = false,
                    Pattern = string.Empty,
                    Explanation = $"Errore durante la chiamata al modello: {ex.Message}"
                };
            }
        }
    }
}