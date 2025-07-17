using System;

namespace Infocad.DynamicSpace.DynamicEntityAttributes
{
    public class DynamicEntityAttributeDto
    {
        public Guid DynamicAttributeId { get; set; }
        public Guid DynamicEntityId { get; set; }
        public int Order { get; set; }
        public string Label { get; set; }
        public Guid? DynamicFormatId { get; set; }
        public Guid? DynamicRuleId { get; set; }
        public Guid? DynamicControlId { get; set; }

        public string? UIControl { get; set; }
        public bool Required { get; set; }


        public string GetCombinedPattern(string? rawRule)
        {
            var rule = string.IsNullOrWhiteSpace(rawRule)
                ? string.Empty
                : rawRule.Trim('^', '$');

            if (!Required)
                return ".*";
            else
            {
                if (string.IsNullOrEmpty(rule))
                    return @"^.+$";
                else
                    return $@"^(?=.+){rule}$";
            }
        }



        public string GetValidationMessage(string? rawRule, string? description)
        {
            var rule = string.IsNullOrWhiteSpace(rawRule)
                ? string.Empty
                : rawRule.Trim('^', '$');

            if (!Required)
                return !string.IsNullOrWhiteSpace(description)
                    ? description!  :             
                    "FailedPattern"; 

            if (string.IsNullOrEmpty(rule))
                return "ThisFieldIsRequired";  

            return !string.IsNullOrWhiteSpace(description)
                ? description!               
                : "FailedPattern";          
        }

    }
}