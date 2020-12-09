namespace ThePetDen.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Orders
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public int ProductID { get; set; }

        public string DateCreated { get; set; }
    }
}
