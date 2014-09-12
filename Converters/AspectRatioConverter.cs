using System;
using Windows.UI.Xaml.Data;

namespace AppboyUI.Universal.Converters {
  public class AspectRatioConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      Double actualWidth = Double.Parse(value.ToString());
      Double multiplier = Double.Parse(parameter.ToString());
      return actualWidth * multiplier;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
