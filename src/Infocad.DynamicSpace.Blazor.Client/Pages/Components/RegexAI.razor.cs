using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazorise;
using Infocad.DynamicSpace.RegexAI;
using Microsoft.AspNetCore.Components;

namespace Infocad.DynamicSpace.Blazor.Client.Pages.Components;

public partial class RegexAI : DynamicSpaceComponentBase
{
    private Offcanvas offcanvasRef;
    private bool cancelClose;

    private string Description;
    private string GeneratedRegex;
    private string Explanation;
    private string TestText;
    private List<string> Matches = new();

    [Parameter] public EventCallback<string> SetEventCallback { get; set; }

    private async Task GenerateRegex()
    {
        var request = new { prompt = Description };
        var response = await RegexAIService.GetRegexPath(Description);

        GeneratedRegex = response.Pattern;
        Explanation = response.Explanation;
    }

    private void TestRegex()
    {
        try
        {
            var matches = Regex.Matches(TestText ?? "", GeneratedRegex ?? "");
            Matches = matches.Select(m => m.Value).ToList();
            if(Matches.Count == 0)
                Matches.Add("Nessun match trovato");
        }
        catch (Exception ex)
        {
            Matches = new List<string> { "Errore nella regex: " + ex.Message };
        }
    }

    public Task ShowOffcanvas()
    {
        return offcanvasRef.Show();
    }

    public Task HideOffcanvas()
    {
        return offcanvasRef.Hide();
    }

    private async Task SetRegex()
    {
        if (!string.IsNullOrEmpty(GeneratedRegex))
        {
            if (SetEventCallback.HasDelegate)
                await SetEventCallback.InvokeAsync(GeneratedRegex);
        }

        HideOffcanvas();
    }
}