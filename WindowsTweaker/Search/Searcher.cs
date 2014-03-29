using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using EnglishStemmer;

namespace WindowsTweaker.Search {
    internal class Searcher {
        private readonly Dictionary<string, List<SearchItem>> _termUiDictionary;
        private readonly MainWindow _window;

        internal Searcher(MainWindow window) {
            _window = window;
            _termUiDictionary = LoadSearchDictionary();
            _termUiDictionary = AddTextToSearchItems(_termUiDictionary);
        }

        internal List<SearchItem> Search(string term, int count=10) {
            if (_termUiDictionary == null || !_termUiDictionary.Any()) return null;
            Dictionary<SearchItem, int> searchItemAndCntDict = GetMatches(term.Split(' '));
            return searchItemAndCntDict != null && searchItemAndCntDict.Any() ? 
                (from entry in searchItemAndCntDict orderby entry.Value descending select entry).
                Select(keyValuePair => keyValuePair.Key).ToList() : null;
        }

        private Dictionary<SearchItem, int> GetMatches(string[] individualTerms) {
            Dictionary<SearchItem, int> searchItemAndCntDict = new Dictionary<SearchItem, int>(new SearchItemEqualityComparer());
            foreach (string individualTerm in individualTerms) {
                string stemmedTxt = StemTerm(individualTerm);
                List<SearchItem> searchItems = GetMatchingSearchItems(stemmedTxt);
                if (searchItems == null || !searchItems.Any()) continue;
                foreach (SearchItem searchItem in searchItems) {
                    if (searchItemAndCntDict.ContainsKey(searchItem)) {
                        searchItemAndCntDict[searchItem]++;
                    } else {
                        searchItemAndCntDict[searchItem] = 1;
                    }
                }
            }
            return searchItemAndCntDict;
        }

        private string StemTerm(string term) {
            string massagedTerm = term.Trim().ToLower();
            if (String.IsNullOrEmpty(massagedTerm)) return null;
            EnglishWord englishWord = new EnglishWord(term);
            return englishWord.Stem;
        }

        private List<SearchItem> GetMatchingSearchItems(string stemmedTxt) {
            if (_termUiDictionary == null || !_termUiDictionary.Any()) return null;
            if (_termUiDictionary.ContainsKey(stemmedTxt)) return _termUiDictionary[stemmedTxt];
            List<SearchItem> matchedSearchItems = new List<SearchItem>();
            foreach (KeyValuePair<string, List<SearchItem>> keyValuePair in _termUiDictionary.
                    Where(keyValuePair => keyValuePair.Key.StartsWith(stemmedTxt) || stemmedTxt.StartsWith(keyValuePair.Key))) {
                matchedSearchItems.AddRange(keyValuePair.Value);
            }
            return matchedSearchItems;
        }

        private Dictionary<string, List<SearchItem>> AddTextToSearchItems(Dictionary<string, List<SearchItem>> termUiDictionary) {
            if (termUiDictionary == null || !termUiDictionary.Any())
                return null;
            foreach (KeyValuePair<string, List<SearchItem>> keyValuePair in termUiDictionary) {
                List<SearchItem> searchItemList = keyValuePair.Value;
                foreach (SearchItem searchItem in searchItemList) {
                    object uiControl = _window.FindName(searchItem.UiElement);
                    if (uiControl is CheckBox) {
                        CheckBox chkBox = (CheckBox)uiControl;
                        searchItem.UiElementText = chkBox.Content is TextBlock ? ((TextBlock)chkBox.Content).Text : chkBox.Content.ToString();
                    } else if (uiControl is RadioButton) {
                        RadioButton rbtn = (RadioButton)uiControl;
                        searchItem.UiElementText = rbtn.Content is TextBlock ? ((TextBlock)rbtn.Content).Text : rbtn.Content.ToString();
                    } else if (uiControl is TextBlock) {
                        searchItem.UiElementText = ((TextBlock)uiControl).Text;
                    } else if (uiControl is TabItem) {
                        searchItem.UiElementText = ((TabItem)uiControl).Header.ToString();
                    }
                }
            }
            return termUiDictionary;
        }

        private Dictionary<string, List<SearchItem>> LoadSearchDictionary() {
            Dictionary<string, List<SearchItem>> termUiDictionary;
            Assembly assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "WindowsTweaker.Search.englishTermAndUiElementMap.json";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
                if (stream == null) return null;
                using (StreamReader reader = new StreamReader(stream)) {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    termUiDictionary = javaScriptSerializer.Deserialize<Dictionary<string, List<SearchItem>>>(reader.ReadToEnd());
                }
            }
            return termUiDictionary;
        }
    }
}