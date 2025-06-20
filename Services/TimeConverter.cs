using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProtocolEditor.Services;

public class TimeConverter : IValueConverter
{
    public static readonly TimeConverter Instance = new TimeConverter();

    public object Convert(object value, Type targerType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            // Преобразуем UTC в локальное время
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime.ToLocalTime().ToString("HH:mm:ss.fff");
                
            return dateTime.ToString("HH:mm:ss");
        }
        return value;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Для двусторонней привязки потребуется обратное преобразование
        if (value is string timeString && TimeSpan.TryParse(timeString, out var timeSpan))
        {
            return DateTime.Today.Add(timeSpan);
        }
        return value;
    }
}