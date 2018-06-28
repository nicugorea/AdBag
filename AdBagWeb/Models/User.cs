using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class User
    {
        public User()
        {
            Announcement = new HashSet<Announcement>();
        }

        public int IdUser { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public User IdUserNavigation { get; set; }
        public User InverseIdUserNavigation { get; set; }
        public ICollection<Announcement> Announcement { get; set; }
    }
}
