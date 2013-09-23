using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PoConvert
{
    class PoWriter
    {
        #region Private Methods
        private static string PoFieldString(string str)
        {
            return str.Replace("\"", "\\\"");
        }
        
        private static string GetSection(PoItem item)
        {
            string section = "";

            /// msgid
            section += string.Format("msgid \"{0}\"", PoFieldString(item.MsgId));

            /// new line
            section += "\n";

            /// msgstr
            var lines = item.MsgStr.Split('\n').ToList();
            if (lines.Count == 1)
            {
                section += string.Format("msgstr \"{0}\"", PoFieldString(lines[0]));
            }
            else
            {
                section += string.Format("msgstr \"\"");
                foreach (var line in lines)
                {
                    section += string.Format("\n\"{0}\"", PoFieldString(line));
                }
            }

            return section;
        }
        #endregion

        #region Public Methods
        public static void WriteAllItem(string path, List<PoItem> items)
        {
            string poContent = "";
            foreach (PoItem item in items)
            {
                string itemSection = GetSection(item);
                if (itemSection != null)
                {
                    if (poContent.Length > 0)
                    {
                        poContent += "\n\n";
                    }
                    poContent += itemSection;
                }
            }

            /// Write to file
            File.WriteAllText(path, poContent, Encoding.UTF8);
        }
        #endregion
    }
}
