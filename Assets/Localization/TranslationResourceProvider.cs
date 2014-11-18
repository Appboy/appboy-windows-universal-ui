using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Resources;

namespace AppboyUI.Universal.Assets.Localization {
  public class TranslationResourceProvider : CustomXamlResourceLoader {
    private readonly ResourceLoader _resourceLoader = new ResourceLoader("AppboyUI/Resources");

    protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType) {
      return _resourceLoader.GetString(resourceId);
    }

    public string GetAppboyString(string resourceId) {
      return _resourceLoader.GetString(resourceId);
    }
  }
}
