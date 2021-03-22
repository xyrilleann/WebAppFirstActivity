using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppXyrille.Web.Infrastructures.Domain.Enums;
using WebAppXyrille.Web.Infrastructures.Domain.Helpers;
using WebAppXyrille.Web.Infrastructures.Domain.Models;

namespace WebAppXyrille.Web.Areas.Manage.ViewModels.Products
{
    public class IndexViewModel
    {
        public Page<Product> Products { get; set; }

        public ProductStatus productStatus { get; set; }

        public List<ProductStatus> productStatuses
        {
            get
            {
                return Enum.GetValues(typeof(ProductStatus)).Cast<ProductStatus>().ToList();
            }
        }
    }
}