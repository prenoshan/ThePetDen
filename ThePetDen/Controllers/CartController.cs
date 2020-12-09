using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThePetDen.Models;

namespace ThePetDen.Controllers
{

    public class CartController : Controller
    {
        public ActionResult Index()
        {
            if (Request.Cookies["user"] == null && Request.Cookies["userRole"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (GetUserRole() != "Customer")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(GetUserProducts());

        }

        [HttpPost]
        public ActionResult RemoveFromCart(int prodID)
        {

            using (var mainDB = new MainDBContext())
            {

                string userEmail = GetUserEmail();

                var itemToRemove = (from c in mainDB.Carts.Where(c => c.ProductID == prodID && c.UserEmail == userEmail) select c).SingleOrDefault();

                if (itemToRemove != null)
                {
                    mainDB.Carts.Remove(itemToRemove);
                    mainDB.SaveChanges();
                }

            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public ActionResult CreateOrder()
        {

            using (var mainDB = new MainDBContext())
            {

                DateTime date = DateTime.Now;

                foreach (var item in GetUserProductIDs())
                {

                    Orders orders = new Orders();

                    orders.UserEmail = GetUserEmail();
                    orders.ProductID = item;
                    orders.DateCreated = date.ToString();

                    mainDB.Orders.Add(orders);
                    mainDB.SaveChanges();


                }


            }

            return RedirectToAction("Index", "Orders");

        }

        private string GetUserEmail()
        {

            var bytes = Convert.FromBase64String(Request.Cookies["user"].Value);

            var output = MachineKey.Unprotect(bytes, "emailProtected");

            string result = Encoding.UTF8.GetString(output);

            return result;

        }

        private List<Products> GetUserProducts()
        {

            List<int> productIDs = new List<int>();

            List<Products> products = new List<Products>();

            string userEmail = GetUserEmail();

            var mainDB = new MainDBContext();

            var usersCart = (from c in mainDB.Carts.Where(c => c.UserEmail == userEmail) select c.ProductID);

            foreach (var item in usersCart)
            {
                productIDs.Add(item);
            }

            foreach (var item in productIDs)
            {

                var prodItems = (from p in mainDB.Products.Where(p => p.Id == item) select p).FirstOrDefault();

                products.Add(prodItems);

            }

            return products;

        }

        private List<int> GetUserProductIDs()
        {

            List<int> productIDs = new List<int>();

            string userEmail = GetUserEmail();

            var mainDB = new MainDBContext();

            var usersCart = (from c in mainDB.Carts.Where(c => c.UserEmail == userEmail) select c.ProductID);

            foreach (var item in usersCart)
            {
                productIDs.Add(item);
            }

            return productIDs;

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