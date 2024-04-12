using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vcfConverter.Utilities;

namespace vcfConverter.Core
{
    internal class SaveObject : DataReader
    {
        private readonly string _vcfFileName;
        private const string vcfEX = ".vcf";


        public SaveObject(string inputDataFile,string vcfFileName = "contactList") : base (inputDataFile)
        {
            _vcfFileName = vcfFileName + vcfEX;
        }

        public void SaveContactList()
        {
            string myExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
            string fullFileName = System.IO.Path.Combine(myExeDir, _vcfFileName);

            string newfileName = new FileUtilities().GetNextFileName(fullFileName);

            foreach (var elem in GetCorrectData())
            {
                var vcfCon = Helpers.CardHelper.CreateVCard(elem) + "\r\n";
                System.IO.File.AppendAllText(newfileName, vcfCon);
            }
        }
    }
}
