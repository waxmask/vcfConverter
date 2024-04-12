using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vcfConverter.Helpers
{
    public static class CardHelper
    {
        #region CONSTANT
        private const string NewLine = "\r\n";

        private const string Separator = ";";

        private const string Header = "BEGIN:VCARD\r\nVERSION:3.0";

        private const string BDAYPrefix = "BDAY;VALUE=DATE:";

        private const string Name = "N:";

        private const string FormattedName = "FN:";

        private const string OrganizationName = "ORG:";

        private const string TitlePrefix = "TITLE:";

        private const string PhotoPrefix = "PHOTO;ENCODING=BASE64;JPEG:";

        private const string PhonePrefix = "TEL;TYPE=";

        private const string PhoneSubPrefix = ",VOICE:";

        private const string AddressPrefix = "ADR;TYPE=";

        private const string AddressSubPrefix = ":;;";

        private const string EmailPrefix = "EMAIL;TYPE=";

        private const string Footer = "END:VCARD";
        #endregion

        public static string CreateVCard(Contact contact)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Header);
            stringBuilder.Append(NewLine);
            if (!string.IsNullOrEmpty(contact.FirstName) || !string.IsNullOrEmpty(contact.LastName))
            {
                stringBuilder.Append("N:");
                stringBuilder.Append(contact.LastName);
                stringBuilder.Append(Separator);
                stringBuilder.Append(contact.FirstName);
                stringBuilder.Append(NewLine);
            }

            if (!string.IsNullOrEmpty(contact.FormattedName))
            {
                stringBuilder.Append("FN:");
                stringBuilder.Append(contact.FormattedName);
                stringBuilder.Append(NewLine);
            }

            if (!string.IsNullOrEmpty(contact.Organization))
            {
                stringBuilder.Append("ORG:");
                stringBuilder.Append(contact.Organization);
                stringBuilder.Append(NewLine);
            }

            if (!string.IsNullOrEmpty(contact.Title))
            {
                stringBuilder.Append("TITLE:");
                stringBuilder.Append(contact.Title);
                stringBuilder.Append(NewLine);
            }

            if (!string.IsNullOrEmpty(contact.DateOfBirth))
            {
                stringBuilder.Append("BDAY;VALUE=DATE:");
                stringBuilder.Append(contact.DateOfBirth);
                stringBuilder.Append(NewLine);
            }

            if (!string.IsNullOrEmpty(contact.Photo))
            {
                stringBuilder.Append("PHOTO;ENCODING=BASE64;JPEG:");
                stringBuilder.Append(ImageHelper.ConvertImageURLToBase64(contact.Photo));
                stringBuilder.Append(NewLine);
                stringBuilder.Append(NewLine);
            }

            foreach (Phone phone in contact.Phones)
            {
                stringBuilder.Append("TEL;TYPE=");
                stringBuilder.Append(phone.Type);
                stringBuilder.Append(":");
                stringBuilder.Append(phone.Number);
                stringBuilder.Append(NewLine);
            }

            if (contact.Addresses != null)
                foreach (Address address in contact.Addresses)
                {
                    stringBuilder.Append("ADR;TYPE=");
                    stringBuilder.Append(address.Type);
                    stringBuilder.Append(":;;");
                    stringBuilder.Append(address.Description.Replace("\n", ";"));
                    stringBuilder.Append(NewLine);
                }

            if (contact.Email != null)
                foreach (Email item in contact.Email)
                {
                    stringBuilder.Append("EMAIL;TYPE=");
                    stringBuilder.Append(item.Type);
                    stringBuilder.Append(":");
                    stringBuilder.Append(item.Mail);
                    stringBuilder.Append(NewLine);
                }

            stringBuilder.Append(Footer);
            return stringBuilder.ToString();
        }

        public static Contact ReadVCard(string contents)
        {
            StringReader stringReader = new StringReader(contents);
            Contact contact = new Contact();
            for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
            {
                if (!text.ToUpper().Equals("BEGIN:VCARD") || !text.ToUpper().Equals("BEGIN:VCARD"))
                {
                    if (text.ToUpper().Contains("BDAY;VALUE="))
                    {
                        string dateOfBirth = text.ToUpper().Replace("BDAY;VALUE=DATE:", "");
                        contact.DateOfBirth = dateOfBirth;
                    }
                    else if (text.ToUpper().Substring(0, 2).Contains("N:"))
                    {
                        text = text.Replace("N:", "");
                        text = text.Replace("n:", "");
                        string[] array = Split(';', text);
                        contact.FirstName = array[1];
                        contact.LastName = array[0];
                    }
                    else if (text.ToUpper().Substring(0, 3).Contains("FN:"))
                    {
                        text = text.Replace("FN:", "");
                        text = text.Replace("fn:", "");
                        contact.FormattedName = text;
                    }
                    else if (text.ToUpper().Substring(0, 4).Contains("ORG:"))
                    {
                        text = text.Replace("ORG:", "");
                        text = text.Replace("org:", "");
                        contact.Organization = text;
                    }
                    else if (text.ToUpper().Substring(0, 4).Contains("ADR;"))
                    {
                        text = text.Replace("ADR;", "");
                        text = text.Replace("adr;", "");
                        string[] array2 = Split(';', text);
                        string type = array2[0].Replace(":", "").Replace(";", "").Replace("TYPE=", "");
                        string description = array2[1].Replace(";", "\n");
                        if (contact.Addresses == null)
                        {
                            contact.Addresses = new List<Address>();
                        }

                        contact.Addresses.Add(new Address
                        {
                            Type = type,
                            Description = description
                        });
                    }
                    else if (text.ToUpper().Substring(0, 4).Contains("TEL;"))
                    {
                        text = text.Replace("TEL;", "");
                        text = text.Replace("tel;", "");
                        string[] array3 = Split(':', text);
                        string type2 = array3[0].Replace(":", "").Replace("TYPE=", "");
                        string number = array3[1];
                        if (contact.Phones == null)
                        {
                            contact.Phones = new List<Phone>();
                        }

                        contact.Phones.Add(new Phone
                        {
                            Type = type2,
                            Number = number
                        });
                    }
                    else if (text.ToUpper().Substring(0, 6).Contains("EMAIL;"))
                    {
                        text = text.Replace("EMAIL;", "");
                        text = text.Replace("email;", "");
                        string[] array4 = Split(':', text);
                        string type3 = array4[0].Replace("TYPE=", "").Replace(":", "");
                        string mail = array4[1];
                        if (contact.Email == null)
                        {
                            contact.Email = new List<Email>();
                        }

                        contact.Email.Add(new Email
                        {
                            Type = type3,
                            Mail = mail
                        });
                    }
                }
            }

            return contact;
        }

        private static string[] Split(char key, string param)
        {
            char[] separator = new char[2] { key, ' ' };
            int count = 2;
            return param.Split(separator, count, StringSplitOptions.None);
        }
    }
}