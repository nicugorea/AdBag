using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class Categories
    {
        public Categories()
        {
            Announcements = new HashSet<Announcements>();
        }

        public int IdCategory { get; set; }
        public string Name { get; set; }

        public ICollection<Announcements> Announcements { get; set; }
    }
}
