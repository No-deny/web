using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplom.Models
{
    public class Category
    {
        public int CategoryId { set; get; }
        public string categoryName { set; get; }
        public string desc { set; get; }
        public int SupplierId { get; set; }
    }
}
