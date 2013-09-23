using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoConvert
{
    public class PoItem
    {
        #region Properties
        public string MsgId {get; set;}
        public string MsgStr { get; set; }
        #endregion

        #region Constructor
        public PoItem()
        {
            MsgId = "";
            MsgStr = "";
        }

        public PoItem(string id, string str)
        {
            MsgId = id;
            MsgStr = str;
        }
        #endregion
    }
}
