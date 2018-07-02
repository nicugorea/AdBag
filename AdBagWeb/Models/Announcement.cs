using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class Announcement
    {
        public Announcement()
        {
            Comment = new HashSet<Comment>();
        }

        public int IdAnnouncement { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int IdUser { get; set; }
        public int IdCategory { get; set; }
        public string Type { get; set; }
        public int? IdImage { get; set; }

        public Category IdCategoryNavigation { get; set; }
        public ImageFile IdImageNavigation { get; set; }
        public User IdUserNavigation { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
