using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThePetDen.Models;

namespace ThePetDen.Controllers
{
    public class AuthController : Controller
    {
        

        private bool checkEmail(string email)
        {

            bool exists = false;

            using(var usersDb = new MainDBContext())
            {

                if(usersDb.Users.Any(u => u.Email == email))
                {

                    exists = true;

                }

            }

            return exists;

        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users reg)
        {

            if (ModelState.IsValid)
            {

                using (var usersDB = new MainDBContext())
                {

                    if (checkEmail(reg.Email))
                    {

                        ModelState.AddModelError("UserExists", "User already exists");

                    }

                    else
                    {
                        //calls method from simple crypto nuget package to encrypt plain text password
                        var crypto = new SimpleCrypto.PBKDF2();
                        var encryptPass = crypto.Compute(reg.Password);

                        //creates a new user into the db
                        Users newUser = new Users();
                        newUser.Id = Guid.NewGuid().ToString();
                        newUser.Name = reg.Name;
                        newUser.Surname = reg.Surname;
                        newUser.Email = reg.Email;
                        newUser.Password = encryptPass;
                        newUser.Salt = crypto.Salt;
                        newUser.Role = "Customer";

                        //saving the new user to the db
                        usersDB.Users.Add(newUser);
                        usersDB.SaveChanges();

                        return RedirectToAction("Login", "Auth");

                    }

                }

            }

            return View(reg);
        }


        [HttpGet]
        public ActionResult Login()
        {

            return View();

        }


        [HttpPost]
        public ActionResult Login(Users users)
        {

            ModelState.Remove("Name");
            ModelState.Remove("Surname");

            string userRole;

            if (ModelState.IsValid)
            {

                if(isValid(users.Email, users.Password))
                {

                    using(var mainDB = new MainDBContext())
                    {

                        userRole = (from u in mainDB.Users.Where(u => u.Email == users.Email) select u.Role).FirstOrDefault().ToString();

                        var usersname = (from u in mainDB.Users.Where(u => u.Email == users.Email) select u.Name).FirstOrDefault().ToString();

                        HttpCookie usersnameCookie = new HttpCookie("usersname", usersname);

                        usersnameCookie.Expires = DateTime.Now.AddDays(1);

                        Response.Cookies.Add(usersnameCookie);

                    }

                    if(userRole == "Customer")
                    {

                        createAuthCookie(users, userRole);

                        return RedirectToAction("Index", "Home");

                    }

                    else if(userRole == "Employee")
                    {

                        createAuthCookie(users, userRole);

                        return RedirectToAction("Index", "Home");

                    }

                }

                else
                {
                    ModelState.AddModelError("", "Incorrect credentials, try again");
                }

            }

            return View(users);

        }

        private void createAuthCookie(Users users, string userRoleVal)
        {

            var email = Encoding.UTF8.GetBytes(users.Email);
            var emailEncrypted = Convert.ToBase64String(MachineKey.Protect(email, "emailProtected"));
            HttpCookie userCookie = new HttpCookie("user",emailEncrypted);
            userCookie.Expires = DateTime.Now.AddDays(2);
            Response.Cookies.Add(userCookie);

            var userRole = Encoding.UTF8.GetBytes(userRoleVal);
            var userRoleEncrypted = Convert.ToBase64String(MachineKey.Protect(userRole, "userRoleProtected"));
            HttpCookie userRoleCookie = new HttpCookie("userRole", userRoleEncrypted);
            userRoleCookie.Expires = DateTime.Now.AddDays(2);
            Response.Cookies.Add(userRoleCookie);

        }

        private bool isValid(string email, string password)
        {

            var crypto = new SimpleCrypto.PBKDF2();

            bool isValid = false;

            using(var usersDB = new MainDBContext())
            {

                var user = usersDB.Users.FirstOrDefault(u => u.Email == email);

                if(user != null)
                {

                    if(user.Password == crypto.Compute(password, user.Salt)){

                        isValid = true;

                    }

                }

            }

            return isValid;

        }

        public ActionResult LogOut()
        {

            if(Request.Cookies["user"] != null && Request.Cookies["userRole"] != null)
            {

                var u = new HttpCookie("user");
                u.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(u);

                var ur = new HttpCookie("userRole");
                ur.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ur);

                var uname = new HttpCookie("usersname");
                uname.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(uname);

            }

            return RedirectToAction("Index", "Home");

        }


    }
}