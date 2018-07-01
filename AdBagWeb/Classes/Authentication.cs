using AdBagWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBagWeb.Classes
{
    public class Authentication
    {

        private static Authentication _instance = null;
        public static Authentication Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Authentication();
                return _instance;
            }
            private set { }
        }

        private User LocalUser { get; set; }

        public void Login(User user)
        {
            LocalUser = user;
        }

        public void Logout() { LocalUser = null; }

        public bool IsUserLoggedIn() { return LocalUser != null; }

        public bool IsAdmin()
        {
            return IsUserLoggedIn() && LocalUser.Role == "admin";
        }

        public bool IsUser()
        {
            return IsUserLoggedIn() && LocalUser.Role == "user";
        }

        public int? GetId()
        {
            if (IsUserLoggedIn())
                return LocalUser.IdUser;
            return null;
        }
    }


}

