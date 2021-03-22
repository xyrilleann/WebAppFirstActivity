using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAppXyrille.Web.Areas.Manage.ViewModels.Products;
using WebAppXyrille.Web.Infrastructures.Domain.Data;
using WebAppXyrille.Web.Infrastructures.Domain.Enums;
using WebAppXyrille.Web.Infrastructures.Domain.Helpers;
using WebAppXyrille.Web.Infrastructures.Domain.Models;

namespace WebAppXyrille.Web.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductsController : Controller
    {
        private readonly DefaultDbContext _context;
        protected readonly IConfiguration _config;



        public ProductsController(DefaultDbContext context, IConfiguration iConfiguration)
        {
            _context = context;
            this._config = iConfiguration;



        }
        [HttpGet, Route("manage/products/index")]
        public IActionResult Index(int pageSize = 5, int pageIndex = 1, string keyword = "", string status = "Available")
        {
            Enum.TryParse(status, out ProductStatus productStatus); ;


            Page<Product> result = new Page<Product>();

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            IQueryable<Product> proQuery = (IQueryable<Product>)this._context.Products.Where(s => s.ProductStatus == productStatus);

            if (string.IsNullOrEmpty(keyword) == false)
            {
                proQuery = proQuery.Where(s => s.Name.Contains(keyword)
                                                 || s.Description.Contains(keyword));

            }

            long queryCount = proQuery.Count();

            int pageCount = (int)Math.Ceiling((decimal)(queryCount / pageSize));
            long mod = (queryCount % pageSize);

            if (mod > 0)
            {
                pageCount = pageCount + 1;
            }

            int skip = (int)(pageSize * (pageIndex - 1));
            List<Product> products = proQuery.ToList();

            result.Items = products.Skip(skip).Take((int)pageSize).ToList();
            result.PageCount = pageCount;
            result.PageSize = pageSize;
            result.QueryCount = queryCount;
            result.PageIndex = pageIndex;
            result.Keyword = keyword;

            return View(new IndexViewModel()
            {
                Products = result,
                productStatus = productStatus
            });
        }

        //[Authorize(Policy = "AuthorizeAdmin")]
        [HttpGet, Route("manage/products/delete/{productId}")]
        public IActionResult Delete(Guid? productId)
        {
            var product = this._context.Products.FirstOrDefault(s => s.Id == productId);

            if (product != null)
            {
                this._context.Products.Remove(product);
                this._context.SaveChanges();
            }

            return RedirectToAction("index");
        }


        [HttpGet, Route("manage/products/details/{id}")]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._context.Products.FirstOrDefault(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        [HttpGet, Route("manage/products/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, Route("manage/products/create")]
        public IActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Product pro = new Product()
            {

                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ProductStatus = Infrastructures.Domain.Enums.ProductStatus.Available,
                IsPublished = true,
                Quantity = model.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,



            };
            this._context.Products.Add(pro);
            this._context.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpGet, Route("manage/products/edit/{id}")]
        public IActionResult Edit(Guid? id)
        {

            var product = this._context.Products.FirstOrDefault(s => s.Id == id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }


            if (product != null)
            {
                return View(
                    new EditViewModel()
                    {
                        Id = product.Id.Value,
                        Name = product.Name,
                        Description = product.Description,
                        ProductStatus = product.ProductStatus,
                        Price = product.Price,
                        Quantity = product.Quantity
                    }
                );
            }

            return View();
        }
        [HttpPost, Route("manage/products/edit")]
        public IActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var product = this._context.Products.FirstOrDefault(s => s.Id == model.Id);

            if (product != null)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.ProductStatus = model.ProductStatus;
                product.Price = model.Price;
                product.Quantity = model.Quantity;

                this._context.Products.Update(product);
                this._context.SaveChanges();



                return RedirectToAction("Index", new { Id = model.Id });
            }

            return View();
        }

    }
}