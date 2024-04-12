using System;
using System.IO;
using System.Net;
using System.Text;

namespace vcfConverter.Helpers
{
    public static class ImageHelper
    {
        public static string ConvertImageURLToBase64(string url)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] image = GetImage(url);
            stringBuilder.Append(Convert.ToBase64String(image, 0, image.Length));
            return stringBuilder.ToString();
        }

        private static byte[] GetImage(string url)
        {
            Stream stream;
            byte[] result;
            try
            {
                WebProxy webProxy = new WebProxy();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                stream = httpWebResponse.GetResponseStream();
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    int count = (int)httpWebResponse.ContentLength;
                    result = binaryReader.ReadBytes(count);
                    binaryReader.Close();
                }

                stream.Close();
                httpWebResponse.Close();
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
    }
}