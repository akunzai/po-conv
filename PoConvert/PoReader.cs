using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PoConvert
{
    class PoReader
    {
        #region Fields
        private static List<PoItem> _items = null;
        #endregion

        #region PoParser Events
        private static void DidFindPair(string msgid, string msgstr)
        {
            Console.WriteLine(string.Format("DidFindPair : [{0}, {1}]", msgid, msgstr));

            _items.Add(new PoItem(msgid, msgstr));
        }

        private static void DidFailWithError(int lineNumber, string errorMessage)
        {
            string msg = string.Format("{0} : line {1}", errorMessage, lineNumber);
            PoConvert.Exception.InvalidFormatException e = new PoConvert.Exception.InvalidFormatException(msg);

            _items = null;
            throw e;
        }

        private static void DidFinish()
        {
            if (_items.Count <= 0)
            {
                _items = null;
            }
        }
        #endregion

        #region Public Methods
        public static List<PoItem> GetItems(string poPath)
        {
            _items = new List<PoItem>();

            String poContent = null;
            using (StreamReader sr = new StreamReader(poPath))
            {
                poContent = sr.ReadToEnd();
            }
            PoParser poParser = new PoParser(poContent);
            poParser.DidFindPairEvent += DidFindPair;
            poParser.DidFailWithErrorEvent += DidFailWithError;
            poParser.DidFinishEvent += DidFinish;
            poParser.Parse();

            return _items;
        }
        #endregion
    }
}
