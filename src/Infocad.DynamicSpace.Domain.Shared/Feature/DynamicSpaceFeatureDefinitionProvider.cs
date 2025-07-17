using Infocad.DynamicSpace.Localization;
using System.Linq;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Settings;
using Volo.Abp.Validation.StringValues;

namespace Infocad.DynamicSpace.Feature
{
    public class DynamicSpaceFeatureDefinitionProvider : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            var dynamicSpaceGroup = GetOrCreateGroup(context);

            DefineReportingFeatures(dynamicSpaceGroup);
            DefineProductManagementFeatures(dynamicSpaceGroup);
            DefineUserManagementFeatures(dynamicSpaceGroup);
            DefineIntegrationFeatures(dynamicSpaceGroup);
            DefineAdvancedFeatures(dynamicSpaceGroup);
        }

        private FeatureGroupDefinition GetOrCreateGroup(IFeatureDefinitionContext context)
        {
            var dynamicSpaceGroup = context.GetGroupOrNull(DynamicSpaceFeatures.GroupName);
            if (dynamicSpaceGroup == null)
            {
                dynamicSpaceGroup = context.AddGroup(
                    DynamicSpaceFeatures.GroupName,
                    displayName: L("Feature:DynamicSpace"));
            }
            return dynamicSpaceGroup;
        }

        private void DefineReportingFeatures(FeatureGroupDefinition group)
        {
            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.PdfReporting,
                "false",
                new ToggleStringValueType(),
                displayName: L("Feature:PdfReporting"),
                description: L("Feature:PdfReporting:Description"));

            //AddFeatureIfNotExists(group,
            //    DynamicSpaceFeatures.ExportType,
            //    "Excel",
            //    new SelectionStringValueType
            //    {
            //        ItemSource = new StaticSelectionStringValueItemSource(
            //            new SelectionStringValueItem("Csv", "CSV"),
            //            new SelectionStringValueItem("Pdf", "PDF"),
            //            new SelectionStringValueItem("Json", "JSON")
            //        )
            //    },
            //    displayName: L("Feature:ExportType"),
            //    description: L("Feature:ExportType:Description"));


        }

        private void DefineProductManagementFeatures(FeatureGroupDefinition group)
        {
            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.MaxProductCount,
                "100",
                new FreeTextStringValueType(new NumericValueValidator(1, 10000)),
                displayName: L("Feature:MaxProductCount"),
                description: L("Feature:MaxProductCount:Description"));
        }

        private void DefineUserManagementFeatures(FeatureGroupDefinition group)
        {
            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.MaxUserCount,
                "50",
                new FreeTextStringValueType(new NumericValueValidator(1, 1000)),
                displayName: L("Feature:MaxUserCount"),
                description: L("Feature:MaxUserCount:Description"));
        }

        private void DefineIntegrationFeatures(FeatureGroupDefinition group)
        {
            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.ExternalApiAccess,
                "false",
                new ToggleStringValueType(),
                displayName: L("Feature:ExternalApiAccess"),
                description: L("Feature:ExternalApiAccess:Description"));
        }

        private void DefineAdvancedFeatures(FeatureGroupDefinition group)
        {
            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.AdvancedFeatures,
                "false",
                new ToggleStringValueType(),
                displayName: L("Feature:AdvancedFeatures"),
                description: L("Feature:AdvancedFeatures:Description"));

            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.FileManagement,
                "true",
                new ToggleStringValueType(),
                displayName: L("Feature:FileManagement"),
                description: L("Feature:FileManagement:Description"));

            AddFeatureIfNotExists(group,
                DynamicSpaceFeatures.AdvancedFileStorage,
                "false",
                new ToggleStringValueType(),
                displayName: L("Feature:AdvancedFileStorage"),
                description: L("Feature:AdvancedFileStorage:Description"));
        }

        private void AddFeatureIfNotExists(
            FeatureGroupDefinition group,
            string name,
            string defaultValue,
            IStringValueType valueType,
            ILocalizableString displayName = null,
            ILocalizableString description = null)
        {
            var existingFeature = group.Features.FirstOrDefault(f => f.Name == name);
            if (existingFeature == null)
            {
                group.AddFeature(
                    name,
                    defaultValue: defaultValue,
                    displayName: displayName,
                    description: description,
                    valueType: valueType,
                    isVisibleToClients: true);
            }
        }

        private static ILocalizableString L(string name)
        {
            return LocalizableString.Create<DynamicSpaceResource>(name);
        }

        public class SelectionStringValueItem : ISelectionStringValueItem
        {
            public string Value { get; set; }
            public LocalizableStringInfo DisplayText { get; set; }

            public SelectionStringValueItem()
            {
            }

            public SelectionStringValueItem(string value, string displayText)
            {
                Value = value;
                DisplayText = new LocalizableStringInfo(null, displayText);
            }

            public SelectionStringValueItem(string value, LocalizableStringInfo displayText)
            {
                Value = value;
                DisplayText = displayText;
            }

            public SelectionStringValueItem(string value, string resourceName, string key)
            {
                Value = value;
                DisplayText = new LocalizableStringInfo(resourceName, key);
            }
        }
    }
}