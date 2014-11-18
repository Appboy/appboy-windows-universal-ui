using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using AppboyPlatform.PCL.Models.Incoming.Cards;
using AppboyPlatform.PCL.Results;
using AppboyPlatform.Universal;
using AppboyUI.Universal.Annotations;

namespace AppboyUI.Universal.ViewModels {
  public class FeedViewModel : INotifyPropertyChanged {
    private readonly HashSet<BaseCard> _allCards;
    private HashSet<CardCategory> _categories;
    private bool _networkUnavailable;
    private bool _refreshingFeed;

    public FeedViewModel() {
      Cards = new ObservableCollection<BaseCard>();
      Cards.CollectionChanged += (sender, args) => OnPropertyChanged("EmptyFeed");
      _allCards = new HashSet<BaseCard>();
      _categories = new HashSet<CardCategory>();
    }

    public ObservableCollection<BaseCard> Cards { get; set; }

    public bool EmptyFeed {
      get { return NetworkUnavailable == false && RefreshingFeed == false && Cards.Count == 0; }
    }

    public bool NetworkUnavailable {
      get { return _networkUnavailable; }
      private set {
        if (value != _networkUnavailable) {
          _networkUnavailable = value;
          OnPropertyChanged("NetworkUnavailable");
        }
      }
    }

    public bool RefreshingFeed {
      get { return _refreshingFeed; }
      private set {
        if (value != _refreshingFeed) {
          _refreshingFeed = value;
          OnPropertyChanged("EmptyFeed");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void SetCategory(CardCategory category) {
      SetCategories(new HashSet<CardCategory> { category });
    }

    public void SetCategories(HashSet<CardCategory> categories) {
      _categories = categories;
      updateCardsWithCategories();
    }

    private void updateCardsWithCategories() {
      Cards.Clear();
      foreach (BaseCard card in _allCards.Where(
        card => _categories == null ||
                _categories.Count == 0 ||
                _categories.Contains(CardCategory.ALL) || (
                  (card.Categories == null || card.Categories.Length == 0) && (_categories.Contains(CardCategory.ALL) || _categories.Contains(CardCategory.NO_CATEGORY))) ||
                card.Categories.Intersect(_categories).Count() > 0)
        ) {
        Cards.Add(card);
      }
    }

    public void GetFeed() {
      GetFeed(new HashSet<CardCategory>());
    }

    public void GetFeed(HashSet<CardCategory> categories) {
      _categories = categories;
      RefreshingFeed = true;
      Action<Task<IResult>> bindCards = continuation => {
        NetworkUnavailable = continuation.Result.ErrorType == ErrorType.NetworkUnavailable;
        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
          foreach (BaseCard card in continuation.Result.Cards ?? Enumerable.Empty<BaseCard>()) {
            _allCards.Add(card);
          }
          updateCardsWithCategories();
        });
        RefreshingFeed = false;
      };
      Appboy.SharedInstance.RequestFeed().ContinueWith(bindCards);
    }

    [NotifyPropertyChangedInvocator]
    protected virtual async void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) {
        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { handler(this, new PropertyChangedEventArgs(propertyName)); });
      }
    }
  }
}
