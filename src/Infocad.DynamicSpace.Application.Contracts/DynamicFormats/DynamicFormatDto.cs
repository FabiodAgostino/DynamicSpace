using Infocad.DynamicSpace.DynamicAttributes;
using System;
using System.Globalization;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicFormats
{
    public class DynamicFormatDto : EntityDto<Guid> 
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        // Tipo di attributo supportato
        public DynamicAttributeType AttributeType { get; set; }

        // Pattern di formattazione (es. per DateTime: "dd/MM/yyyy")
        public string? FormatPattern { get; set; }

        public DynamicFormatDto()
        {
            Id = Guid.NewGuid();
        }

        public DynamicFormatDto(Guid id, string name, DynamicAttributeType attributeType, string? description = null, string? formatPattern = null)
        {
            Id = id;
            Name = name;
            AttributeType = attributeType;
            Description = description;
            FormatPattern = formatPattern;
        }

        public string ApplyFormat(object value)
        {
            if (value == null || string.IsNullOrEmpty(FormatPattern))
            {
                return value?.ToString() ?? string.Empty;
            }

            string valueAsString = value?.ToString() ?? string.Empty;

            try
            {
                // Verifica formati speciali per stringhe
                if (AttributeType == DynamicAttributeType.Text)
                {
                    if (FormatPattern.StartsWith("upper:", StringComparison.OrdinalIgnoreCase))
                    {
                        string formatWithoutPrefix = FormatPattern.Substring(6);
                        if (string.IsNullOrEmpty(formatWithoutPrefix) || formatWithoutPrefix == "{0}")
                            return valueAsString.ToUpper();
                        else
                            return string.Format(formatWithoutPrefix, valueAsString).ToUpper();
                    }

                    if (FormatPattern.StartsWith("lower:", StringComparison.OrdinalIgnoreCase))
                    {
                        string formatWithoutPrefix = FormatPattern.Substring(6);
                        if (string.IsNullOrEmpty(formatWithoutPrefix) || formatWithoutPrefix == "{0}")
                            return valueAsString.ToLower();
                        else
                            return string.Format(formatWithoutPrefix, valueAsString).ToLower();
                    }

                    if (FormatPattern.StartsWith("title:", StringComparison.OrdinalIgnoreCase))
                    {
                        string formatWithoutPrefix = FormatPattern.Substring(6);
                        string titleCaseValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(valueAsString.ToLower());
                        if (string.IsNullOrEmpty(formatWithoutPrefix) || formatWithoutPrefix == "{0}")
                            return titleCaseValue;
                        else
                            return string.Format(formatWithoutPrefix, titleCaseValue);
                    }

                    return string.Format(FormatPattern, valueAsString);
                }
                var currentCulture = CultureInfo.CurrentCulture;
                var specificCulture = currentCulture.IsNeutralCulture
                    ? CultureInfo.CreateSpecificCulture(currentCulture.Name)
                    : currentCulture;
                switch (AttributeType)
                {
                    case DynamicAttributeType.DateTime:
                        if (value is DateTime dateTime)
                        {
                            return dateTime.ToString(FormatPattern, specificCulture);
                        }
                        break;

                    case DynamicAttributeType.Number:
                        if (int.TryParse(valueAsString, out int intValue))
                        {
                            return intValue.ToString(FormatPattern, specificCulture);
                        }
                        break;

                    case DynamicAttributeType.Float:
                        if (decimal.TryParse(valueAsString, out decimal decimalValue))
                        {
                            return decimalValue.ToString(FormatPattern, specificCulture);
                        }
                        break;

                    case DynamicAttributeType.Boolean:
                        if (bool.TryParse(valueAsString, out bool boolValue))
                        {
                            // Per i booleani, gestisce format del tipo "Sì;No"
                            if (FormatPattern.Contains(";"))
                            {
                                string[] parts = FormatPattern.Split(';');
                                return boolValue ? parts[0] : (parts.Length > 1 ? parts[1] : "");
                            }

                            return boolValue.ToString();
                        }
                        break;

                    default:
                        return string.Format(CultureInfo.CurrentCulture, FormatPattern, value);
                }
            }
            catch (Exception ex)
            {
                return valueAsString;
            }

            return valueAsString;
        }
    }

    /*
Esempi di stringhe FormatPattern per la formattazione dinamica:

### Per stringhe (DynamicAttributeType.String)

1. "upper:{0}" - Converte tutto il testo in maiuscolo
2. "lower:{0}" - Converte tutto il testo in minuscolo
3. "title:{0}" - Prima lettera di ogni parola maiuscola
4. "Prefisso: {0}" - Aggiunge un prefisso al testo
5. "{0} Suffisso" - Aggiunge un suffisso al testo
6. "upper:Cod. {0}" - Prefisso e testo tutto maiuscolo
7. "{0,-20}" - Allinea il testo a sinistra con una larghezza fissa di 20 caratteri
8. "{0,15}" - Allinea il testo a destra con una larghezza fissa di 15 caratteri

### Per numeri interi (DynamicAttributeType.Number)

1. "D" - Formato decimale standard
2. "D6" - Numero a 6 cifre (con zeri iniziali se necessario)
3. "N0" - Numero senza decimali con separatore delle migliaia
4. "00000" - Numero a 5 cifre con zeri iniziali
5. "X" - Formato esadecimale (maiuscolo)
6. "x" - Formato esadecimale (minuscolo)
7. "+#;-#;zero" - Mostra segno positivo, negativo o "zero"

### Per numeri decimali (DynamicAttributeType.Float)

1. "F2" - Numero fisso con 2 decimali
2. "N2" - Numero con 2 decimali e separatore delle migliaia
3. "P1" - Formato percentuale con 1 decimale
4. "C" - Formato valuta (euro per cultura italiana)
5. "0.00 €" - Formato personalizzato con simbolo valuta
6. "#,##0.00" - Formato personalizzato con migliaia e decimali
7. "0.###E+0" - Formato scientifico

### Per date e orari (DynamicAttributeType.DateTime)

1. "d" - Data breve (esempio: 22/04/2025)
2. "D" - Data lunga (esempio: martedì 22 aprile 2025)
3. "t" - Ora breve (esempio: 14:30)
4. "T" - Ora lunga (esempio: 14:30:45)
5. "g" - Data e ora brevi (esempio: 22/04/2025 14:30)
6. "G" - Data e ora lunghi (esempio: 22/04/2025 14:30:45)
7. "dd/MM/yyyy" - Formato data personalizzato
8. "yyyy-MM-dd" - Formato ISO data
9. "HH:mm" - Solo ora in formato 24 ore
10. "MMMM yyyy" - Solo mese e anno (esempio: aprile 2025)
11. "dddd" - Solo giorno della settimana (esempio: martedì)

### Per booleani (DynamicAttributeType.Boolean)

1. "Sì;No" - Valori personalizzati per true/false
2. "Attivo;Inattivo" - Altra coppia di valori
3. "✓;✗" - Simboli di spunta
4. "1;0" - Valori numerici
5. "Aperto;Chiuso" - Stati
6. "True;False" - Valori booleani standard

### Esempi combinati e casi speciali

1. "upper:ID-{0:D4}" - Prefisso seguito da numero con zeri iniziali, tutto maiuscolo
2. "Creato il {0:dd/MM/yyyy} alle {0:HH:mm}" - Formattazione complessa con data e ora
3. "Valore: {0:C2} ({0:P1})" - Mostra il valore sia come valuta che come percentuale
*/
}
