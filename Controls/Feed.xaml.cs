using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using AppboyPlatform.PCL.Models.Incoming.Cards;
using AppboyPlatform.PCL.Utilities;
using AppboyPlatform.Universal;
using AppboyUI.Universal.ViewModels;

namespace AppboyUI.Universal.Controls {
  public sealed partial class Feed : UserControl {
    private readonly FeedAnalyticsTracker _feedAnalyticsTracker;
    private readonly FeedViewModel _feedViewModel;
    private readonly DispatcherTimer _impressionTimer;

    public Feed() {
      InitializeComponent();
      Loaded += Feed_Loaded;
      Unloaded += Feed_Unloaded;
      FeedListBox.IsHitTestVisible = true;
      _feedViewModel = new FeedViewModel();
      _feedAnalyticsTracker = new FeedAnalyticsTracker(Appboy.SharedInstance.EventLogger);
      _impressionTimer = new DispatcherTimer();
      _impressionTimer.Tick += CalculateCardImpressions;
      _impressionTimer.Interval = new TimeSpan(0, 0, 1);
      FeedProgressRing.DataContext = _feedViewModel;
      FeedListBox.DataContext = _feedViewModel.Cards;
      NetworkErrorMessage.DataContext = _feedViewModel;
      FeedEmptyMessage.DataContext = _feedViewModel;
    }

    public string InitialCategory { get; set; }

    public FeedViewModel FeedViewModel {
      get { return _feedViewModel; }
    }

    private void Feed_Loaded(object sender, RoutedEventArgs e) {
      var initialCategorySet = new HashSet<CardCategory>();
      if (InitialCategory != null) {
        initialCategorySet.Add((CardCategory)Enum.Parse(typeof(CardCategory), InitialCategory.ToUpper()));
      }
      _feedViewModel.GetFeed(initialCategorySet);
      _feedAnalyticsTracker.LogFeedDisplayed();
      _impressionTimer.Start();
    }

    private void Feed_Unloaded(object sender, RoutedEventArgs e) {
      _impressionTimer.Stop();
    }

    public void SetCategory(CardCategory category) {
      _feedViewModel.SetCategory(category);
    }

    public void SetCategories(HashSet<CardCategory> categories) {
      _feedViewModel.SetCategories(categories);
    }

    private async void Card_Tapped(object sender, RoutedEventArgs routedEventArgs) {
      string url = "";
      var card = ((FrameworkElement)sender).DataContext as BaseCard;
      string id = card.Id;
      Type cardType = card.GetType();
      if (cardType == typeof(Banner)) {
        url = ((Banner)card).Url;
      }
      if (cardType == typeof(CaptionedImage)) {
        url = ((CaptionedImage)card).Url;
      }
      if (cardType == typeof(ShortNews)) {
        url = ((ShortNews)card).Url;
      }
      if (cardType == typeof(TextAnnouncement)) {
        url = ((TextAnnouncement)card).Url;
      }
      if (!String.IsNullOrWhiteSpace(url)) {
        _feedAnalyticsTracker.LogCardClick(id);
        var uri = new Uri(url, UriKind.Absolute);
        await Launcher.LaunchUriAsync(uri);
      }
    }

    private async void CrossPromotionSmallPrice_Click(object sender, RoutedEventArgs e) {
      var card = ((FrameworkElement)sender).DataContext as BaseCard;
      var crossPromotionSmall = card as CrossPromotionSmall;
      if (crossPromotionSmall != null) {
        _feedAnalyticsTracker.LogCardClick(card.Id);
        var uri = new Uri("ms-windows-store:PDP?PFN=" + crossPromotionSmall.PackageFamilyName);
        if (crossPromotionSmall.Store == AppStore.WINDOWSPHONESTORE) {
          uri = new Uri("ms-windows-store:navigate?appid=" + crossPromotionSmall.AppId);
        }
        await Launcher.LaunchUriAsync(uri);
      }
    }

    private void CalculateCardImpressions(object sender, object e) {
      // Skipping impression testing if there are no cards in the view model.
      if (_feedViewModel.Cards.Count == 0) {
        return;
      }

      CalculateCardImpressionsViaContainerViewTesting();
    }

    /// <summary>
    ///   Calculates card impressions by simulating a click on the last row of pixels in the ListBox.
    ///   If a card would have been clicked, then it logs impressions for all cards in the feed up to
    ///   and including the clicked card. Otherwise, there must not be enough cards to fill up the
    ///   ListBox. Therefore, we log impressions for all the cards in the feed.
    /// </summary>
    private void CalculateCardImpressionsUsingHitTest() {
      // Performing a hit test at the bottom of the ListBox to get the last visible ListBoxItem.
      GeneralTransform feedListBoxTransform = FeedListBox.TransformToVisual(Window.Current.Content);
      Point feedListBoxOrigin = feedListBoxTransform.TransformPoint(new Point(0, 0));
      var feedListBoxBottomCenter = new Point(feedListBoxOrigin.X + FeedListBox.ActualWidth/2, feedListBoxOrigin.Y + FeedListBox.ActualHeight - 1);
      List<ListBoxItem> listBoxItems = VisualTreeHelper.FindElementsInHostCoordinates(feedListBoxBottomCenter, FeedListBox).OfType<ListBoxItem>().ToList<ListBoxItem>();

      switch (listBoxItems.Count) {
        case 0:
          // Log impressions for all cards.
          foreach (BaseCard card in _feedViewModel.Cards) {
            _feedAnalyticsTracker.LogCardImpression(card.Id);
          }
          break;
        case 1:
          var baseCard = listBoxItems[0].Content as BaseCard;
          if (baseCard != null) {
            // Log impressions for all cards up to and including this one.
            IEnumerable<BaseCard> targetCard = _feedViewModel.Cards.Where(card => card.Id == baseCard.Id);
            if (targetCard.Count() == 1) {
              int targetCardIndex = _feedViewModel.Cards.IndexOf(targetCard.ElementAt(0));
              for (int i = 0; i < targetCardIndex; i++) {
                _feedAnalyticsTracker.LogCardImpression(_feedViewModel.Cards[i].Id);
              }
            } else {
              Debug.WriteLine("Found more than one card in the view model with ID {0}.", baseCard.Id);
            }
          }
          break;
        default:
          Debug.WriteLine("Found more than one ListBoxItem during the hit test.");
          break;
      }
    }

    /// <summary>
    ///   Calculates card impressions by fetching the container for each card and performing an intersection
    ///   test with the ListBox. Impressions are logged for all cards that intersect the ListBox.
    /// </summary>
    private void CalculateCardImpressionsViaContainerViewTesting() {
      foreach (BaseCard card in FeedViewModel.Cards ?? Enumerable.Empty<BaseCard>()) {
        var element = FeedListBox.ItemContainerGenerator.ContainerFromItem(card) as FrameworkElement;
        if (IsInView(element, FeedListBox)) {
          _feedAnalyticsTracker.LogCardImpression(card.Id);
        }
      }
    }

    private static bool IsInView(FrameworkElement element, FrameworkElement container) {
      if (element == null || element.Visibility == Visibility.Collapsed) {
        return false;
      }
      Rect bounds = element.TransformToVisual(container).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
      var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
      return rect.Contains(new Point(bounds.X, bounds.Y)) || rect.Contains(new Point(bounds.X, bounds.Y + bounds.Height));
    }
  }
}
