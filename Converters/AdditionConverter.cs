using System;
using Windows.UI.Xaml.Data;

namespace AppboyUI.Universal.Converters {
  public sealed class AdditionConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      Double input = Double.Parse(value.ToString());
      Double param = Double.Parse(parameter.ToString());
      return input + param;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }  
}
