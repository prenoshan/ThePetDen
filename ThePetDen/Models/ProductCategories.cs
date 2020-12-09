using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThePetDen.Models
{
    public class ProductCategories
    {

        [Key]
        public string ProductCategory { get; set; }

        public int OrderCount { get; set; }

    }
}