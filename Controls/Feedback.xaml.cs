using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppboyPlatform.PCL.Utilities;
using AppboyPlatform.Universal;
using AppboyUI.Universal.Assets.Localization;

namespace AppboyUI.Universal.Controls {
  public sealed partial class Feedback : UserControl {
    public Feedback() {
      InitializeComponent();
      Loaded += Feedback_Loaded;
    }

    public event EventHandler OnCancel;
    public event EventHandler AfterSubmit;

    private void Feedback_Loaded(object sender, RoutedEventArgs e) {
      Appboy.SharedInstance.EventLogger.LogFeedbackDisplayed();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      if (OnCancel != null) {
        OnCancel(sender, null);
      }
    }

    private async void Submit_Click(object sender, RoutedEventArgs e) {
      string warning = "";
      string message = MessageTextBox.Text.Trim();
      string email = EmailTextBox.Text.Trim();
      var AppboynResourceProvider = new TranslationResourceProvider();
      if (message.Length == 0) {
        warning = AppboynResourceProvider.GetAppboyString("FeedbackEmptyMessageWarning");
      } else if (email.Length == 0) {
        warning = AppboynResourceProvider.GetAppboyString("FeedbackEmptyEmailWarning");
      } else if (!Validations.IsValidEmailAddress(EmailTextBox.Text.Trim())) {
        warning = AppboynResourceProvider.GetAppboyString("FeedbackInvalidEmailWarning");
      }

      if (warning.Length > 0) {
        await new MessageDialog(warning).ShowAsync();
        return;
      }

      Appboy.SharedInstance.SubmitFeedback(email, message, ReportingIssueCheckBox.IsChecked ?? false);
      if (AfterSubmit != null) {
        AfterSubmit(sender, null);
      }
    }
  }
}
