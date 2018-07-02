using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class ImageFile
    {
        public ImageFile()
        {
            Announcement = new HashSet<Announcement>();
            Category = new HashSet<Category>();
            User = new HashSet<User>();
        }

        public int IdImage { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] BinaryData { get; set; }

        public ICollection<Announcement> Announcement { get; set; }
        public ICollection<Category> Category { get; set; }
        public ICollection<User> User { get; set; }
    }
}
