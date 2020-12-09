using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThePetDen.Models;

namespace ThePetDen.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders
        public ActionResult Index()
        {

            if(Request.Cookies["user"] == null && Request.Cookies["userRole"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if(GetUserRole() != "Customer")
            {
                return RedirectToAction("Index", "Home");
            }

            List<string> orders = new List<string>();

            string userEmail = GetUserEmail();

            MainDBContext mainDB = new MainDBContext();

            var userOrders = (from o in mainDB.Orders.Where(o => o.UserEmail == userEmail) select o.DateCreated);

            foreach (var item in userOrders)
            {
                
                orders.Add(Convert.ToString(item));
                
            }

            ViewData["orders"] = orders;

            return View();
        }

        [HttpPost]
        public ActionResult OrderDetails(string dateCreated)
        {

            if (Request.Cookies["user"] == null && Request.Cookies["userRole"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (GetUserRole() != "Customer")
            {
                return RedirectToAction("Index", "Home");
            }

            string userEmail = GetUserEmail();
            List<int> prodIDs = new List<int>();
            List<Products> products = new List<Products>();

            using(var mainDB = new MainDBContext())
            {

                var userOrders = (from o in mainDB.Orders.Where(o => o.DateCreated == dateCreated && o.UserEmail == userEmail) select o.ProductID);

                foreach (var item in userOrders)
                {
                    prodIDs.Add(item);
                }

                foreach (var item in prodIDs)
                {
                    var userProducts = (from p in mainDB.Products.Where(p => p.Id == item) select p).FirstOrDefault();
                    products.Add(userProducts);
                }
            }

            if(products.Count <= 0)
            {
                return RedirectToAction("Index", "Orders");
            }

            return View(products);

        }

        private string GetUserEmail()
        {

            var bytes = Convert.FromBase64String(Request.Cookies["user"].Value);

            var output = MachineKey.Unprotect(bytes, "emailProtected");

            string result = Encoding.UTF8.GetString(output);

            return result;

        }

        private string GetUserRole()
        {

            var bytes = Convert.FromBase64String(Request.Cookies["userRole"].Value);

            var output = MachineKey.Unprotect(bytes, "userRoleProtected");

            string result = Encoding.UTF8.GetString(output);

            return result;

        }
    }
}