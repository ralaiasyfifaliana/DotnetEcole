using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GestionHospitaliere.Frontend.Converters;

public class ObjectMultiConverter : IMultiValueConverter
{
    // Permet d'inverser le résultat (utilisé pour NotEqual)
    public bool Invert { get; set; }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count < 2) return false;
        
        bool isEqual = Equals(values[0], values[1]);
        return Invert ? !isEqual : isEqual;
    }
}