using AdBagWeb.Classes;
using AdBagWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AdBagWeb.Controllers
{
    public class AccountController : Controller
    {
        #region Properties
        private readonly AdBagWebDBContext _context;
        #endregion

        #region Constructor
        public AccountController(AdBagWebDBContext context)
        {
            _context = context;
        }
        #endregion

        #region Index
        [HttpGet]
        public IActionResult Index()
        {
            if (Authentication.Instance.IsUserLoggedIn())
                return RedirectToAction(nameof(MyAccount));
            else
                return RedirectToAction(nameof(Login));

        }
        #endregion

        #region Register
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
            if (user != null && !string.IsNullOrEmpty(user.Password) && !string.IsNullOrEmpty(user.Username))
            {
                user.Role = "user";
                user.Password = Authentication.ComputeSha256Hash(user.Password);
                _context.Add(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        #endregion

        #region Login
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
                    var localPass = Authentication.ComputeSha256Hash(user.Password);
                    if (dbUser.Password != localPass)
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
        #endregion

        #region Logout
        [HttpGet]
        public IActionResult Logout()
        {
            Authentication.Instance.Logout();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region MyAccount
        [HttpGet]
        public IActionResult MyAccount()
        {
            return View(_context.User.Find(Authentication.Instance.GetId()));
        }
        #endregion

        #region Edit
        [HttpGet]
        public IActionResult Edit()
        {
            return View(_context.User.Find(Authentication.Instance.GetId()));
        }
        #endregion

    }
}