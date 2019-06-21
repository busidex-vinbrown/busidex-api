using System.Collections.Generic;
using System.IO;
using System.Web;
using Busidex.DomainModels;
using Busidex.Services.Interfaces;

namespace Busidex.Services
{
    public class ContactsRepository : IContactsRepository
    {
        public List<ContactData> ReadContacts(HttpPostedFile data)
        {
            var csvreader = new StreamReader(data.InputStream);
            var users = new List<ContactData>();
            int iLastName, iEmail, iCompany;
            int iFirstName = iLastName = iEmail = iCompany = 0;

            bool firstPass = true;
            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                if (line != null)
                {
                    var values = line.Split(',');
                    if (iFirstName == 0)
                    {
                        iFirstName = findItemIndex(values, "first");
                    }
                    if (iLastName == 0)
                    {
                        iLastName = findItemIndex(values, "last");
                    }
                    if (iEmail == 0)
                    {
                        iEmail = findItemIndex(values, "mail");
                    }
                    if (iCompany == 0)
                    {
                        iCompany = findItemIndex(values, "company");
                    }

                    if (!firstPass)
                    {
                        if (iEmail >= 0 && iFirstName <= values.Length && iLastName <= values.Length && iCompany <= values.Length && iEmail <= values.Length)
                        {
                            users.Add(
                                new ContactData
                                {
                                    FirstName = iFirstName >= 0 ? values[iFirstName] : string.Empty,
                                    LastName = iLastName >= 0 ? values[iLastName] : string.Empty,
                                    Company = iCompany >= 0 ? values[iCompany] : string.Empty,
                                    Email = values[iEmail]
                                });
                        }
                    }
                    else
                    {
                        firstPass = false;
                    }
                }
            }

            return users;
        }

        private int findItemIndex(string[] line, string exp)
        {
            for (int idx=0; idx< line.Length; idx++)
            {
                var item = line[idx].ToLower();
                if (item.Contains(exp))
                {
                    return idx;
                }
            }

            return -1;
        }
    }
}