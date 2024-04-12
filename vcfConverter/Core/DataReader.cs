using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using vcfConverter.Extensions;

namespace vcfConverter.Core
{
    internal class DataReader : IDisposable
    {
        private readonly string _filePath;
        private int _MinPhoneLength;
        public int MinPhoneLength
        {
            get { return _MinPhoneLength; }
            set
            {
                _MinPhoneLength = value > 0 ? value : 0;
            }
        }

        public DataReader(string filePath, int minPhoneLength = 3)
        {
            _filePath = filePath;
            MinPhoneLength = minPhoneLength;
        }

        public IEnumerable<string> GetDataLines()
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workBook = excel.Workbooks.Open(_filePath);

            var fileName = _filePath.RemoveCharacters(',', '.') + ".csv";
            workBook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlCSVWindows);

            workBook.Close();
            excel.Quit();

            using (StreamReader sr = new StreamReader(path: fileName, encoding: Encoding.GetEncoding(1251)))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    yield return line;
                }
            }
            new FileInfo(fileName).Delete();
        }
        public IEnumerable<Contact> GetData()
        {
            var lines = GetDataLines();
            foreach (var line in lines)
            {
                var contact = new Contact
                {
                    FirstName = line.Split(',').First(),
                    Phones = new System.Collections.Generic.List<Phone>() { { new Phone { Number = line.Split(',').Last(), Type = "Work" } } },
                };

                yield return contact;
            }
        }
        public IEnumerable<Contact> GetCorrectData()
        {
            var contactList = GetData();

            foreach (var contact in contactList)
            {
                var phoneNumber = new string(contact?.Phones.FirstOrDefault().Number?.Where(c => char.IsWhiteSpace(c) || char.IsDigit(c)).ToArray());

                if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length >= _MinPhoneLength)
                {
                    if (phoneNumber.Length == 11 && phoneNumber[0] == '8')
                    {
                        phoneNumber = "7" + phoneNumber.Substring(1);
                    }
                    if (phoneNumber.Length == 10)
                    {
                        phoneNumber = "7" + phoneNumber;
                    }

                    contact.Phones.FirstOrDefault().Number = phoneNumber;
                    yield return contact;
                }
            }
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
