using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using AppboyPlatform.Universal.Managers;
using AppboyPlatform.Universal.Utilities;
using AppboyUI.Universal.Controls;

namespace AppboyUI.Universal.Popups {
  public class FeedPopup : IAppboyModal {
    private Popup _popup;

    public void Open() {
      if (_popup == null) {
        Initialize();
      }
      if (!_popup.IsOpen) {
        _popup.Height = WindowHelper.CurrentWindow.Bounds.Height;
        var modalFeed = new ModalFeed(_popup);
        modalFeed.Height = WindowHelper.CurrentWindow.Bounds.Height;
        _popup.Child = modalFeed;
        _popup.IsOpen = true;
      }
    }

    public void Close() {
      _popup.IsOpen = false;
    }

    private void Initialize() {
      _popup = new Popup();
      WindowHelper.CurrentWindow.SizeChanged += CurrentWindow_SizeChanged;
      _popup.ChildTransitions = new TransitionCollection();
      _popup.ChildTransitions.Add(new PaneThemeTransition {
        Edge = EdgeTransitionLocation.Left
      });
      _popup.IsLightDismissEnabled = true;
    }

    private void CurrentWindow_SizeChanged(object sender, WindowSizeChangedEventArgs e) {
      if (_popup.IsOpen && (_popup.Child as Control) != null) {
        _popup.Height = WindowHelper.CurrentWindow.Bounds.Height;
        (_popup.Child as Control).Height = WindowHelper.CurrentWindow.Bounds.Height;
      }
    }
  }
}
