using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThePetDen.Models
{
    public class Carts
    {

        public int Id { get; set; }

        public string UserEmail { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

    }
}