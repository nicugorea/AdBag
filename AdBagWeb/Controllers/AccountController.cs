using AdBagWeb.Classes;
using AdBagWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AdBagWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AdBagWebDBContext _context;



        public AccountController(AdBagWebDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (Authentication.Instance.IsUserLoggedIn())
                return RedirectToAction(nameof(MyAccount));
            else
                return RedirectToAction(nameof(Login));

        }

        [HttpGet]
        public IActionResult Register()
        {
            if (Authentication.Instance.IsUserLoggedIn())
                return RedirectToAction(nameof(MyAccount));
            return View();

        }


        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (Authentication.Instance.IsUserLoggedIn())
                return RedirectToAction(nameof(MyAccount));
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var dbUser = _context.User.First(u => u.Username == user.Username);
                    if (dbUser == null)
                        return View(user);
                    if (dbUser.Password != user.Password)
                        return View(user);
                    Authentication.Instance.Login(dbUser);
                }
                catch
                {
                    return View(user);

                }

            }
            return RedirectToAction(nameof(MyAccount));

        }

        [HttpGet]
        public IActionResult Logout()
        {
            Authentication.Instance.Logout();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult MyAccount()
        {
            return View();
        }



    }
}