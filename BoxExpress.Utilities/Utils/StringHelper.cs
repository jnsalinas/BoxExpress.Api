
using System.Globalization;
using System.Linq;
using System.Text;

namespace BoxExpress.Utilities.Utils;

public static class StringHelper
{
    public static string NormalizeCity(string city)
    {
        if (string.IsNullOrEmpty(city)) return city;

        // Quitar tildes/acentos
        var normalized = city.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        // Convertir a mayúsculas y quitar espacios antes/después
        return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant().Trim();
    }
}
