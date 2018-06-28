using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdBagWeb.Models
{
    public partial class Category
    {
        public Category()
        {
            Announcement = new HashSet<Announcement>();
        }

        public int IdCategory { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Announcement> Announcement { get; set; }
    }
}
