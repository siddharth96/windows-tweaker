using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WindowsTweaker.Search {
    internal class Search {
        private static Dictionary<string, List<SearchItem>> _termUiDictionary;
        private string _term;

        static Search() {
            _termUiDictionary = LoadSearchDictionary();
        }
        
        internal Search(string term) {
            this._term = term;
        }

        private static Dictionary<string, List<SearchItem>> LoadSearchDictionary() {
            Dictionary<string, List<SearchItem>> termUiDictionary =
                new Dictionary<string, List<SearchItem>>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "WindowsTweaker.Search.englishTermAndUiElementMap.json";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream)) {
                //System.Runtime.Serialization.
                //var jss = new JavaScriptSerializer();
            }
            return termUiDictionary;
        }
    }
}