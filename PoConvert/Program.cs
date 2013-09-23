using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using NDesk.Options;

namespace PoConvert
{
    class Program
    {
        #region Private Methods
        private static void ShowTryHelpHint()
        {
            Console.WriteLine("Unknown command\n");
            Console.WriteLine("Try `po-conv --help' for more information.");
        }

        private static void ShowHelp()
        {
            /// Options
            Console.WriteLine("\n Options:\n");
            Console.Write("  -h, --help\t\t\t");
            Console.WriteLine("output usage information");
            Console.Write("  -V, --version\t\t\t");
            Console.WriteLine("output the version number");
            Console.Write("  -s, --source\t\t\t");
            Console.WriteLine("path to read from");
            Console.Write("  -t, --target\t\t\t");
            Console.WriteLine("path to write to");

            /// Examples
            Console.WriteLine("\n Examples");
            Console.WriteLine("  - po to csv : ");
            Console.WriteLine("      po-conv -s en.po -t en.csv\n");
            Console.WriteLine("  - csv to po : ");
            Console.WriteLine("      po-conv -s en.csv -t en.po");
        }

        private static void ShowVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Console.WriteLine("{0}", version);
        }

        private static string ReadText(string poPath)
        {
            String retText = null;
            using (StreamReader sr = new StreamReader(poPath))
            {
                retText = sr.ReadToEnd();
            }
            return retText;
        }

        private static void po2csv(string poPath, string csvPath)
        {
            try
            {
                var items = PoReader.GetItems(poPath);
                using (StreamWriter streamWriter = new StreamWriter(csvPath))
                {
                    using (CsvHelper.CsvWriter csvWriter = new CsvHelper.CsvWriter(streamWriter))
                    {
                        foreach (var item in items)
                        {
                            csvWriter.WriteRecord(item);
                        }
                    }
                }
                Console.WriteLine("Convert to CSV file successfully.");
            }
            catch (PoConvert.Exception.InvalidFormatException e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void csv2po(string csvPath, string poPath)
        {
            try
            {
                using (CsvReader csvReader = new CsvReader(csvPath))
                {
                    List<PoItem> items = csvReader.GetRecords<PoItem>().ToList();
                    PoWriter.WriteAllItem(poPath, items);
                    Console.WriteLine("Convert to PO file successfully.");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        static void Main(string[] args)
        {
            string sourcePath = null;
            string targetPath = null;
            bool isShowHelp = false;
            bool isShowVersion = false;

            OptionSet options = new OptionSet();
            options = options.Add("s=|source=", s => sourcePath = s);
            options = options.Add("t=|target=", t => targetPath = t);
            options = options.Add("h|help", h => isShowHelp = (h != null));
            options = options.Add("V|version", v => isShowVersion = (v != null));
            
            try
            {
                options.Parse(args);
                if (isShowHelp)
                {
                    ShowHelp();
                }
                else if(isShowVersion)
                {
                    ShowVersion();
                }
                else if (string.IsNullOrEmpty(sourcePath) == false
                    && string.IsNullOrEmpty(targetPath) == false)
                {
                    string sourceExtension = Path.GetExtension(sourcePath).TrimStart('.');
                    if (sourceExtension.Equals("po", StringComparison.OrdinalIgnoreCase))
                    {
                        po2csv(sourcePath, targetPath);
                    }
                    else if (sourceExtension.Equals("csv", StringComparison.OrdinalIgnoreCase))
                    {
                        csv2po(sourcePath, targetPath);
                    }
                }
                else
                {
                    ShowTryHelpHint();
                }
            }
            catch (OptionException e)
            {
                ShowTryHelpHint();
                return;
            }
        }
    }
}
