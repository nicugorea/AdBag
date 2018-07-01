using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBagWeb.Models
{
    public partial class AdComment
    {
        public Announcement Ad { get; set; }
        public string  Comment { get; set; }
    }
}
