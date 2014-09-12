using AppboyPlatform.Universal.Managers;
using AppboyPlatform.Universal.Utilities;
using AppboyUI.Universal.Controls;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace AppboyUI.Universal.Popups {
  public class FeedPopup : IAppboyModal {
    private Popup _popup;

    private void Initialize() {
      _popup = new Popup();
      WindowHelper.CurrentWindow.SizeChanged += CurrentWindow_SizeChanged;
      _popup.ChildTransitions = new TransitionCollection();
      _popup.ChildTransitions.Add(new PaneThemeTransition {
        Edge = EdgeTransitionLocation.Left
      });
      _popup.IsLightDismissEnabled = true;
    }

    public void Open() {
      if (_popup == null) {
        Initialize();
      }
      if (!_popup.IsOpen) {
        _popup.Height = WindowHelper.CurrentWindow.Bounds.Height;
        ModalFeed modalFeed = new ModalFeed(_popup);
        modalFeed.Height = WindowHelper.CurrentWindow.Bounds.Height;
        _popup.Child = modalFeed;
        _popup.IsOpen = true;
      }
    }

    public void Close() {
      _popup.IsOpen = false;
    }

    private void CurrentWindow_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {
      if (_popup.IsOpen && (_popup.Child as Control) != null) {
        _popup.Height = WindowHelper.CurrentWindow.Bounds.Height;
        (_popup.Child as Control).Height = WindowHelper.CurrentWindow.Bounds.Height;
      }
    }
  }
}
