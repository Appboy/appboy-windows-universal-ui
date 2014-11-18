using System;
using Windows.UI.Xaml.Data;
using AppboyPlatform.PCL.Utilities;

namespace AppboyUI.Universal.Converters {
  public sealed class PriceConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      decimal price = value as decimal? ?? 0m;
      return Formatter.FormatPrice(price);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
