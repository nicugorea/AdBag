using AdBagWeb.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBagWeb.ViewModels
{
    public class AnnouncementViewModel
    {
        public Announcement Ad { set; get; }
        public IFormFile Image { set; get; }
    }
}
