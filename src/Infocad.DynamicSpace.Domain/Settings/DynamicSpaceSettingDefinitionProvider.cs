using Volo.Abp.Settings;

namespace Infocad.DynamicSpace.Settings;

public class DynamicSpaceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DynamicSpaceSettings.MySetting1));
    }
}
