using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTestExample.Models;

namespace TechTestExample
{
    class Program
    {
        static List<Company> Companies;
        static void Main(string[] args)
        {            
            Companies = new List<Company>();

            AddCompanies();
            int active = GetActiveCompanies();
            Console.WriteLine($"Active Companies: {active}");
            Console.ReadKey();

            Console.WriteLine("Reading file...");
            Company c = ReadCompanyFromFile(@"C:\Users\JonHowell\Source\Repos\TechTestExample\TechTestExample\AddCompany.txt");
            if (c != null)
                Companies.Add(c);
            Console.WriteLine("File read");

            Console.WriteLine("JSON read...");
            Company JsonCompany = ReadCompanyFromJsonFile(@"C:\Users\JonHowell\Source\Repos\TechTestExample\TechTestExample\Company.json");
            if (JsonCompany != null)
                Companies.Add(JsonCompany);
            Console.WriteLine("JSON Complete");

            Console.WriteLine("Get details for Company*");
            List<ContactDetails> details = GetContactDetailsForCompany("Company*");
            Console.WriteLine(JsonConvert.SerializeObject(details));

            Console.WriteLine("Get comapny by sector");
            List<Company> sectors = GetCompaniesBySector("Public");
            Console.WriteLine(JsonConvert.SerializeObject(sectors));

            Console.WriteLine("Deleting contact Jon");
            Console.WriteLine($"{DeleteContact("Jon", "Synertec")}");

            Console.WriteLine("Adding child company...");
            Companies.Add(new ChildCompany
            {
                CompanyName = "Child01",
                CompanyNumber = 999,
                Postcode = "TA7",
                Active = true,
                Sector = "Private",
                ParentCompany = "Synertec"
            });

            Console.WriteLine(@"List folders and subfolders in C:\temp");
            ListFolders(@"C:\temp\");

            Console.WriteLine("Hello World!");
        }

        static void ListFolders(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dirs = di.GetDirectories();

            foreach(var dir in dirs)
            {
                ListFolders(dir.FullName);
                Console.WriteLine(dir.FullName);
            }
        }

        static bool DeleteContact(string contactName, string companyName)
        {
            Company c = Companies.Where(s => s.CompanyName == companyName).FirstOrDefault();
            if(c != null)
            {
                //reverse iteration to prevent crash when removing from array
                for(int i = c.ContactDetails.Count; i != 0 ; i--)
                {
                    ContactDetails dt = c.ContactDetails[i];
                    if(dt.ContactName == contactName)
                    {
                        c.ContactDetails.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        static List<Company> GetCompaniesBySector(string sector)
        {
            List<Company> Return = new List<Company>();
            foreach(Company c in Companies)
            {
                //perform null check first
                if (c.Sector != null)
                {
                    if (string.Compare(c.Sector, sector) == 0)
                    {
                        Return.Add(c);
                    }
                }
            }

            return Return;
        }

        static List<ContactDetails> GetContactDetailsForCompany(string companyName)
        {
            ///return a list as we are likely to have more than one match
            int WildcardIndex = companyName.IndexOf("*");

            if(WildcardIndex == -1)
            {
                //perform exact search
                return Companies.Where(c => c.CompanyName == companyName)
                    .FirstOrDefault()
                    .ContactDetails;
            }
            else
            {
                List<ContactDetails> Details = new List<ContactDetails>();
                List<Company> c = null;
                if (WildcardIndex > 1)
                {
                    //fuzzy end
                    c = Companies.Where(c => c.CompanyName.StartsWith(companyName)).ToList();

                }
                else
                {
                    //fuzzy start
                    c = Companies.Where(c => c.CompanyName.EndsWith(companyName)).ToList();
                }

                if (c != null)
                {
                    foreach (var comp in c)
                    {
                        foreach (var contactDetails in comp.ContactDetails)
                            Details.Add(contactDetails);
                    }

                    return Details;
                }
                else
                    return null;
            }
        }

        static Company ReadCompanyFromJsonFile(string file)
        {
            string json = File.ReadAllText(file);

            return JsonConvert.DeserializeObject<Company>(json);
        }

        static Company ReadCompanyFromFile(string file)
        {
            Company c = null;
            using(StreamReader sr = new StreamReader(file))
            {
                //read headers from first line - we'll assume correct formatting
                string line = sr.ReadLine();
                List<string> headers = line.Split(',').ToList();
                int CompanyNameIndex = headers.IndexOf("CompanyName");
                int CompanyNumberIndex = headers.IndexOf("CompanyNumber");
                int PostcodeIndex = headers.IndexOf("PostCode");
                int ActiveIndex = headers.IndexOf("Active");
                int SectorIndex = headers.IndexOf("Sector");

                //now read through the file
                while(sr.Peek() != -1)
                {
                    string[] row = sr.ReadLine().Split(',');
                    c = new Company
                    {
                        CompanyName = row[CompanyNameIndex],
                        CompanyNumber = int.Parse(row[CompanyNumberIndex]),
                        Postcode = row[PostcodeIndex],
                        Active = bool.Parse(row[ActiveIndex]),
                        Sector = row[SectorIndex]
                    };
                }
            }

            return c;
        }

        static int GetActiveCompanies()
        {
            return Companies.Where(c => c.Active).Count();
        }

        static void AddCompanies()
        {
            for (int i = 0; i < 5; i++)
            {
                Company c = new Company
                {
                    CompanyName = $"Company{i}",
                    CompanyNumber = i,
                    Active = i % 2 > 0,
                    Sector = i % 2 > 0 ? "" : "PublicSector"
                };

                if(Companies.Find(x => x.CompanyName == c.CompanyName) == null)
                    Companies.Add(c);
            }
        }
    }
}
