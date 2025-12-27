using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class SaleHistory
    {
        [Key]
        public int Id { get; set; }
        public string BuyerName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
