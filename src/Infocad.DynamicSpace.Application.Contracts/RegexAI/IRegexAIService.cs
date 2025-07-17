using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.RegexAI;

public interface IRegexAIService : IApplicationService
{
    Task<RegexResponseDto> GetRegexPath(string prompt);
}