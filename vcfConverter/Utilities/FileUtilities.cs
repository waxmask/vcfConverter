using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vcfConverter.Utilities
{
    internal class FileUtilities
    {
        public string GetNextFileName(string fullFileName)
        {
            if (fullFileName == null)
            {
                throw new ArgumentNullException("fullFileName");
            }

            if (!File.Exists(fullFileName))
            {
                return fullFileName;
            }

            string baseFileName = Path.GetFileNameWithoutExtension(fullFileName);
            string fileExtension = Path.GetExtension(fullFileName);

            string filePath = Path.GetDirectoryName(fullFileName);
            var numbersUsed = Directory.GetFiles(filePath, baseFileName + "*" + fileExtension)
                .Select(x => Path.GetFileNameWithoutExtension(x).Substring(baseFileName.Length))
                .Select(x =>
                {
                    return int.TryParse(x, out int result) ? result : 0;
                })
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var firstGap = numbersUsed
                .Select((x, i) => new { Index = i, Item = x })
                .FirstOrDefault(x => x.Index != x.Item);

            int numberToUse = firstGap != null ? firstGap.Item : numbersUsed.Count;

            return Path.Combine(filePath, baseFileName) + numberToUse + fileExtension;
        }
    }
}
