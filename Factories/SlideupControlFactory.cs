using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppboyPlatform.PCL.Models.Incoming;
using AppboyPlatform.Universal.Managers;

namespace AppboyUI.Universal.Factories {
  public class SlideupControlFactory : ISlideupControlFactory {
    public UserControl GetSlideupControl(Slideup slideup) {
      var control = new Controls.Slideup();
      SetChevron(control, slideup);
      control.Message.Text = slideup.Message;
      return control;
    }

    private void SetChevron(Controls.Slideup control, Slideup slideup) {
      if (slideup.ClickAction == ClickAction.NONE) {
        control.Chevron.Visibility = Visibility.Collapsed;
      } else {
        control.Chevron.Visibility = Visibility.Visible;
      }
    }
  }
}
