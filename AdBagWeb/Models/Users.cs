using System;
using System.Collections.Generic;

namespace AdBagWeb.Models
{
    public partial class Users
    {
        public Users()
        {
            Announcements = new HashSet<Announcements>();
        }

        public int IdUser { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public Users IdUserNavigation { get; set; }
        public Users InverseIdUserNavigation { get; set; }
        public ICollection<Announcements> Announcements { get; set; }
    }
}
