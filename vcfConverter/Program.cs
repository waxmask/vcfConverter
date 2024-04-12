using System;
using vcfConverter.Core;
using vcfConverter.Helpers;

namespace vcfConverter
{

    internal class Program
    {
        private static string filePath;

        static void Main()
        {
            GetInputFilePath();

            new SaveObject(filePath).SaveContactList();

            Console.WriteLine("Конец программы.");
            Console.ReadKey();
        }

        private static void GetInputFilePath()
        {
            bool isCorrectInput;
            do
            {
                isCorrectInput = OFDConsole.ShowDialog(out filePath);

                if (!filePath.Contains(".xl"))
                {
                    isCorrectInput = false;
                    Console.WriteLine("Ошибка открытия файла, выберите файл EXCEL.");
                }
            } while (!isCorrectInput);
        }
    }
}
