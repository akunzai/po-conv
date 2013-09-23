using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using CsvHelper;

namespace PoConvert
{
    class CsvReader : IDisposable
    {
        #region Fields
        private CsvHelper.CsvReader _csvReader = null;
        private StreamReader _streamReader = null;
        private string _pathOfStreamReader = null;
        private string _csvPath = null;
        #endregion

        #region Private Methods
        private CsvHelper.CsvReader OpenCsvReader<T>(string path)
        {
            CloseCsvReader();
            
            /// Read content from file
            string newCsvContent = "";
            string oldCsvContent = null;
            using (StreamReader sr = new StreamReader(path))
            {
                oldCsvContent = sr.ReadToEnd();
            }
            
            /// Add header row
            Type type = typeof(T);
            PropertyInfo[] fields = type.GetProperties();
            for (int i = 0; i < fields.Length; i++)
            {
                if(newCsvContent.Length > 0)
                {
                    newCsvContent += ",";
                }
                newCsvContent += fields[i].Name;
            }
            newCsvContent += string.Format("\n{0}", oldCsvContent);

            /// Save to disk
            string newCsvPath = Path.Combine(Path.GetDirectoryName(path),
                                    string.Format("{0}.csv",Guid.NewGuid().ToString()));
            File.WriteAllText(newCsvPath, newCsvContent, Encoding.UTF8);

            ///
            _streamReader = new StreamReader(newCsvPath);
            _csvReader = new CsvHelper.CsvReader(_streamReader);
            _pathOfStreamReader = newCsvPath;

            return _csvReader;
        }

        private void CloseCsvReader()
        {
            if (_csvReader != null)
            {
                _csvReader.Dispose();
                _csvReader = null;
            }

            if (_streamReader != null)
            {
                _streamReader.Dispose();
                _streamReader = null;
            }

            if (_pathOfStreamReader != null)
            {
                File.Delete(_pathOfStreamReader);
                _pathOfStreamReader = null;
            }
        }
        #endregion

        #region Constructor and Destructor
        public CsvReader(string csvPath)
        {
            _csvPath = csvPath;
        }

        public void Dispose()
        {
            CloseCsvReader();
        }
        #endregion

        #region Public Methods
        public IEnumerable<T> GetRecords<T>()
        {
            IEnumerable<T> records = null;

            CsvHelper.CsvReader csvReader = OpenCsvReader<T>(_csvPath);
            records = csvReader.GetRecords<T>();

            return records;
        }
        #endregion
    }
}
