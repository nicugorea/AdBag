using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class Announcement
    {
        public int IdAnnouncement { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int IdUser { get; set; }
        public int IdCategory { get; set; }

        public Category IdCategoryNavigation { get; set; }
        public User IdUserNavigation { get; set; }
    }
}
