using Microsoft.Ajax.Utilities;
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
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            MainDBContext mainDB = new MainDBContext();

            var petCategories = mainDB.Products.Select(p => p.PetCategory).Distinct();

            var productCategories = mainDB.Products.Select(p => p.ProductCategory).Distinct();

            ViewData["PetCategories"] = new SelectList(petCategories);

            ViewData["ProductCategories"] = new SelectList(productCategories);

            string petCategory = "";
            string productCategory = "";
            string productSearch = "";


            if (Request.QueryString.Count > 0)
            {
                if (Request.QueryString["pet"] != null)
                {
                    petCategory = Request.QueryString["pet"].ToString();
                }

                if (Request.QueryString["prodCat"] != null)
                {
                    productCategory = Request.QueryString["prodCat"].ToString();
                }

                if (Request.QueryString["prodSearch"] != null)
                {
                    productSearch = Request.QueryString["prodSearch"].ToString();
                }
            }

            var products = from p in mainDB.Products select p;

            if (!String.IsNullOrEmpty(petCategory))
            {
                products = from p in mainDB.Products.Where(p => p.PetCategory == petCategory) select p;
            }

            if (!String.IsNullOrEmpty(petCategory) && !String.IsNullOrEmpty(productCategory))
            {
                products = from p in mainDB.Products.Where(p => p.PetCategory == petCategory && p.ProductCategory == productCategory) select p;

            }

            if (!String.IsNullOrEmpty(productSearch))
            {
                products = from p in mainDB.Products.Where(p => p.Name.Contains(productSearch) && p.PetCategory == petCategory && p.ProductCategory == productCategory) select p;
            }

            return View(products);
        }

        [HttpPost]
        public ActionResult Index(string productSearch, string productCat, string petCat)
        {

            return RedirectToAction("Index", new { pet = petCat, prodCat = productCat, prodSearch = productSearch });

        }

        [HttpPost]
        public ActionResult AddToCart(string ddl_Quantity, int prodID)
        {

            if(Request.Cookies["user"] != null && Request.Cookies["userRole"] != null)
            {

                if (GetUserRole() != "Customer")
                {
                    return RedirectToAction("Index", "Home");
                }

                var bytes = Convert.FromBase64String(Request.Cookies["user"].Value);
                var output = MachineKey.Unprotect(bytes, "emailProtected");
                string result = Encoding.UTF8.GetString(output);

                string userEmail = result;
                int productID = prodID;

                using (var mainDB = new MainDBContext())
                {

                    var prodIDExists = (from c in mainDB.Carts.Where(c => c.UserEmail == userEmail && c.ProductID == prodID) select c.ProductID).SingleOrDefault();

                        if(prodIDExists == productID)
                        {
                        TempData["prodExists"] = "You have already added that product to your cart";

                        return RedirectToAction("Index", "Products");

                        }

                    else
                    {
                        Carts newCart = new Carts();

                        newCart.ProductID = productID;
                        newCart.Quantity = Convert.ToInt32(ddl_Quantity);
                        newCart.UserEmail = userEmail;

                        mainDB.Carts.Add(newCart);
                        mainDB.SaveChanges();

                        TempData["itemAdded"] = "Item added to cart!!!";

                    }


                }

            }

            else
            {

                return RedirectToAction("Login", "Auth");

            }

            return RedirectToAction("Index", "Products");

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