using System;
using Windows.UI.Xaml.Data;
using AppboyPlatform.PCL.Utilities;

namespace AppboyUI.Universal.Converters {
  /// <summary>
  ///   Adds parentheses around the value string. If the value is not null, it first gets converted
  ///   to a string by having its ToString() called.
  /// </summary>
  /// <param name="value">The value that will be surrounded by parentheses</param>
  /// <param name="parameter">The default value to use if 'value' is null</param>
  /// <returns>
  ///   The value (or parameter) surrounded by parentheses. If both 'value' and 'parameter' are null, then an empty
  ///   string is returned
  /// </returns>
  public sealed class SurroundingParenthesesConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      if (value != null) {
        return Formatter.SurroundWithParentheses(value.ToString());
      }
      if (parameter != null) {
        return Formatter.SurroundWithParentheses(parameter.ToString());
      }
      return String.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
