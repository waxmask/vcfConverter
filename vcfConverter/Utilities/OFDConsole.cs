using System;
using System.Runtime.InteropServices;

namespace vcfConverter.Helpers
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int flagsEx;
    }

    internal static class OFDConsole
    {
        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName(ref OpenFileName ofn);

        public static bool ShowDialog(out string filePath)
        {
            filePath = string.Empty;

            var ofn = new OpenFileName();

            ofn.lStructSize = Marshal.SizeOf(ofn);

            // Define Filter for your extensions (Excel, ...)
            ofn.lpstrFilter = "Excel Files (*.xlsx)\0*.xlsx\0All Files (*.*)\0*.*\0";
            ofn.lpstrFile = new string(new char[256]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[64]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Select EXCEL file...";

            if (GetOpenFileName(ref ofn))
            {
                filePath = ofn.lpstrFile;
                return true;
            }
            return false;
        }
    }
}
