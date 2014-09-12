using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Resources;

namespace AppboyUI.Universal.Assets.Localization {
  public class TranslationResourceProvider : CustomXamlResourceLoader {
    readonly ResourceLoader _resourceLoader = new ResourceLoader("AppboyUI/Resources");

    protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType) {
      return _resourceLoader.GetString(resourceId);
    }

    public string GetAppboyString(string resourceId) {
      return _resourceLoader.GetString(resourceId);
    }
  }
}
