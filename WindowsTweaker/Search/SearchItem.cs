namespace WindowsTweaker.Search {
    internal class SearchItem {
        private readonly string _mainTabName;
        private readonly string _subTabName;
        private readonly string _uiElementName;

        internal SearchItem(string mainTabName, string subTabName, string uiElementName) {
            this._mainTabName = mainTabName;
            this._subTabName = subTabName;
            this._uiElementName = uiElementName;
        }

        internal string MainTabName {
            get { return _mainTabName; }
        }

        internal string SubTabName {
            get { return _subTabName; }
        }

        internal string UiElementName {
            get { return _uiElementName; }
        }
    }
}
