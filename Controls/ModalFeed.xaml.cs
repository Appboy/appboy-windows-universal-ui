using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace AppboyUI.Universal.Controls {
  public sealed partial class ModalFeed : UserControl {
    private Popup _popup;

    public ModalFeed(Popup popup) {
      this.InitializeComponent();
      _popup = popup;
    }

    private void Close_Click(object sender, RoutedEventArgs e) {
      _popup.IsOpen = false;
    }
  }
}
