using AppboyPlatform.PCL.Utilities;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AppboyUI.Universal.Converters {
  public sealed class PriceConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
      var price = value as decimal? ?? 0m;
      return Formatter.FormatPrice(price);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
