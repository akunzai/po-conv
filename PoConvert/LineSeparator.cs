using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoConvert
{
    class LineSeparator
    {
        #region Fields
        private string _source = null;
        private int _curIndex = 0;
        private readonly string _newLinePattern = null;
        private readonly int _sourceLen = 0;
        #endregion

        #region Properties
        /// <summary>
        /// 1-indexed
        /// </summary>
        public int LineNumber = 0;
        #endregion

        #region Private Methods
        private string DoNextLine(bool moveCurrentIndex)
        {
            string line = null;
            if (_curIndex >= _sourceLen) return null;

            int foundIndex = _source.IndexOf(_newLinePattern, _curIndex);
            if (foundIndex >= _curIndex)
            {
                int lineLen = foundIndex - _curIndex;
                line = _source.Substring(_curIndex, lineLen);
                if (moveCurrentIndex)
                {
                    _curIndex += (lineLen + 1);
                    LineNumber++;
                }
            }
            else
            {
                if (_curIndex < _sourceLen)
                {
                    line = _source.Substring(_curIndex, _sourceLen - _curIndex);
                    if (moveCurrentIndex)
                    {
                        _curIndex = _sourceLen;
                        LineNumber++;
                    }
                }
            }

            return line;
        }
        #endregion

        #region Constructor
        public LineSeparator(string source, string newLinePattern)
        {
            _source = source;
            _curIndex = 0;
            _newLinePattern = newLinePattern;
            _sourceLen = _source.Length;
        }
        #endregion

        #region Public Methods
        public void Rewind()
        {
            _curIndex = 0;
        }

        public string PreviewNextLine()
        {
            return DoNextLine(false);
        }

        public string NextLine()
        {
            return DoNextLine(true);
        }
        #endregion
    }
}
