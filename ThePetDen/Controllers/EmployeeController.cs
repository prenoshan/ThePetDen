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
    public class EmployeeController : Controller
    {
        // GET: Employee

        public ActionResult OrderStats()
        {

            if (Request.Cookies["user"] == null && Request.Cookies["userRole"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (GetUserRole() != "Employee")
            {
                return RedirectToAction("Index", "Home");
            }

            MainDBContext mainDB = new MainDBContext();

            List<int> ids = new List<int>();
            List<string> categories = new List<string>();

            var q_ids = (from o in mainDB.Orders select o.ProductID);

            foreach (var id in q_ids)
            {
                ids.Add(id);
            }

            foreach (var item in ids)
            {

                var q_category = (from p in mainDB.Products.Where(p => p.Id == item) select p.ProductCategory).SingleOrDefault();
                categories.Add(q_category);

            }

            var q_count = categories.GroupBy(x => x).Where(g => g.Count() >= 1).ToDictionary(x => x.Key, y => y.Count());

            Dictionary<string, int> data = new Dictionary<string, int>();

            data = q_count;

            List<string> xAxis = new List<string>();
            List<int> yAxis = new List<int>();

            xAxis = data.Select(kvp => kvp.Key).ToList();
            yAxis = data.Select(kvp => kvp.Value).ToList();

            ViewBag.XAXIS = xAxis;
            ViewBag.YAXIS = yAxis;

            return View();
        }

        private List<string> GetCategories(string petCategory)
        {

            List<string> categories = new List<string>();

            MainDBContext mainDB = new MainDBContext();

            var q_category = (from p in mainDB.Products.Where(p => p.PetCategory == petCategory) select p.ProductCategory);

            foreach (var item in q_category)
            {
                categories.Add(item);
            }

            return categories;
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