using System.Collections.Generic;

namespace WindowsTweaker.Search {
    internal sealed class SearchItemEqualityComparer : IEqualityComparer<SearchItem> {
        public bool Equals(SearchItem x, SearchItem y) {
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.GetType() == y.GetType() && x.UiElement.Equals(y.UiElement);
        }

        public int GetHashCode(SearchItem obj) {
            return obj.MainNavItem.GetHashCode() ^ obj.SubTab.GetHashCode() ^ obj.SubTabControl.GetHashCode() ^ obj.UiElement.GetHashCode();
        }
    }
}
