using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class Comment
    {
        public int IdComment { get; set; }
        public string Text { get; set; }
        public DateTime PostTime { get; set; }
        public int IdUser { get; set; }
        public int IdAnnouncement { get; set; }

        public Announcement IdAnnouncementNavigation { get; set; }
        public User IdUserNavigation { get; set; }
    }
}
