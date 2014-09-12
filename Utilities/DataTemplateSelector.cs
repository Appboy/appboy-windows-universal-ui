using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppboyPlatform.PCL.Models.Incoming.Cards;

namespace AppboyUI.Universal.Utilities {
  public abstract class DataTemplateSelector : ContentControl {
    public virtual DataTemplate SelectTemplate(object item, DependencyObject container) {
      return null;
    }

    protected override void OnContentChanged(object oldContent, object newContent) {
      base.OnContentChanged(oldContent, newContent);
    }
  }
}
