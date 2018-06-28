using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class Category
    {
        public Category()
        {
            Announcement = new HashSet<Announcement>();
        }

        public int IdCategory { get; set; }
        public string Name { get; set; }

        public ICollection<Announcement> Announcement { get; set; }
    }
}
