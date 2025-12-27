using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class ProductComposition
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Composition { get; set; }
    }
}
