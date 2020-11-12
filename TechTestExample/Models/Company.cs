using System;
using System.Collections.Generic;
using System.Text;

namespace TechTestExample.Models
{
    public class Company
    {
        public string CompanyName { get; set; }
        public int CompanyNumber { get; set; }
        public string Postcode { get; set; }
        public bool Active { get; set; }
        public string Sector { get; set; }

        public List<ContactDetails> ContactDetails = new List<ContactDetails>();
    }

    public class ChildCompany : Company
    {
        public string ParentCompany { get; set; }
    }
}
