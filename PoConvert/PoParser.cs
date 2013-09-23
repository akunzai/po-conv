using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoConvert
{
    class PoParser
    {
        #region Constants
        private const string TOKEN_MSGID = "msgid";
        private const string TOKEN_MSGSTR = "msgstr";
        private const string NEW_LINE_TOKEN = "\n";
        private enum TokenType { UNKNOWN, MSG_ID, MSG_STR, STRING };
        private enum ParsingState : int
        {
            UNKNOWN = 0x0,
            WAIT_MSG_ID = 0x01,
            WAIT_MSG_STR = 0x010,
            WAIT_STRING = 0x0100
        };
        #endregion

        #region Fields
        private LineSeparator _lineSeparator = null;
        private int _state = (int)ParsingState.UNKNOWN;
        private string _msgId = null;
        private string _msgStr = null;
        #endregion

        #region Delegates
        public delegate void DidFindPairDelegate(string key, string value);
        public delegate void DidFailWithError(int lineNumber, string errorMessage);
        public delegate void DidFinish();
        #endregion

        #region Properties
        public event DidFindPairDelegate DidFindPairEvent = null;
        public event DidFailWithError DidFailWithErrorEvent = null;
        public event DidFinish DidFinishEvent = null;
        #endregion

        #region Private Methods
        private string NextNonEmptyLine()
        {
            string line = null;
            do
            {
                line = _lineSeparator.NextLine();
                if (line != null)
                {
                    line = line.Trim(' ');
                }
            }
            while (line != null && line.Length <= 0);

            return line;
        }

        private TokenType ParseLine(string line, out string value)
        {
            TokenType tokenType = TokenType.UNKNOWN;

            if (line.StartsWith(TOKEN_MSGID))
            {
                tokenType = TokenType.MSG_ID;
                value = line.Substring(TOKEN_MSGID.Length).Trim(' ').Trim('"');
            }
            else if (line.StartsWith(TOKEN_MSGSTR))
            {
                tokenType = TokenType.MSG_STR;
                value = line.Substring(TOKEN_MSGSTR.Length).Trim(' ').Trim('"');
            }
            else if (line.StartsWith("\""))
            {
                tokenType = TokenType.STRING;
                value = line.Trim(' ').Trim('"');
            }
            else
            {
                tokenType = TokenType.UNKNOWN;
                value = null;
            }

            return tokenType;
        }

        private string GetErrorMessage(int state)
        {
            string msg = null;

            if (state == (int)ParsingState.WAIT_MSG_ID)
            {
                msg = "A keyword msgid is expected";
            }
            else if (state == (int)ParsingState.WAIT_MSG_STR)
            {
                msg = "A Keyword msgstr is expected";
            }
            else if(state == ((int)ParsingState.WAIT_MSG_ID | (int)ParsingState.WAIT_STRING))
            {
                msg = "A string or a keyword msgid is expected";
            }
            else
            {
                msg = "Invalid format";
            }

            return msg;
        }
        #endregion

        #region Constructor
        public PoParser(string poSource)
        {
            _lineSeparator = new LineSeparator(poSource, NEW_LINE_TOKEN);
        }
        #endregion
        
        #region Public Methods
        public void Parse()
        {
            string line = null;
            _state = (int)ParsingState.WAIT_MSG_ID;

            bool hasError = false;
            while ((line = NextNonEmptyLine()) != null)
            {
                string value = null;
                TokenType tokenType = ParseLine(line, out value);
                switch(tokenType)
                {
                    case TokenType.MSG_ID:
                        if ((_state & (int)ParsingState.WAIT_MSG_ID) != 0)
                        {
                            if(_msgStr != null)
                            {
                                if(DidFindPairEvent != null)
                                {
                                    DidFindPairEvent.Invoke(_msgId, _msgStr);
                                }
                                _msgId = _msgStr = null;
                            }

                             _state = (int)ParsingState.WAIT_MSG_STR;
                             _msgId = value;
                        }
                        else
                        {
                            hasError = true;
                        }
                        break;

                    case TokenType.MSG_STR:
                        if ((_state & (int)ParsingState.WAIT_MSG_STR) != 0)
                        {
                            _state = (int)ParsingState.WAIT_STRING | (int)ParsingState.WAIT_MSG_ID;
                            _msgStr = value;
                        }
                        else
                        {
                            hasError = true;
                        }
                        break;

                    case TokenType.STRING:
                        if((_state & (int)ParsingState.WAIT_STRING) != 0)
                        {
                            if (_msgStr.Length > 0)
                            {
                                _msgStr += NEW_LINE_TOKEN;
                            }
                            _msgStr += value;
                        }
                        else
                        {
                            hasError = true;
                        }
                        break;

                    default:
                        hasError = true;
                        break;
                }

                if(hasError)
                {
                    break;
                }
            }// end of while

            /// Last Item
            if (_msgStr != null)
            {
                if (DidFindPairEvent != null)
                {
                    DidFindPairEvent.Invoke(_msgId, _msgStr);
                }
                _msgId = _msgStr = null;
            }

            /// Response
            if (hasError)
            {
                if (DidFailWithErrorEvent != null)
                {
                    DidFailWithErrorEvent.Invoke(_lineSeparator.LineNumber, GetErrorMessage((int)_state));
                }
            }
            else
            {
                if (DidFinishEvent != null)
                {
                    DidFinishEvent.Invoke();
                }
            }

        }
        #endregion
    }
}
