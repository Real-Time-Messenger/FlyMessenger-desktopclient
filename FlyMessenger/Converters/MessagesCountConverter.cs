﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyMessenger.Converters
{
    /// <summary>
    /// Messages count converter.
    /// </summary>
    public class MessagesCountConverter : IValueConverter
    {
        // Convert messages count to boolean.
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            // return true if value is more than 0
            return value != null && (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
