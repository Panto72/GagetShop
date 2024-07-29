using Microsoft.AspNetCore.Mvc;
using MyGagetsWebApp.Models;
using MyGagetsWebApp.Services;

namespace MyGagetsWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(i => i.Id).ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is requried");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);
            string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = imageFullPath,
                CeatedAt = DateTime.Now,
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CeatedAt"] = product.CeatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CeatedAt"] = product.CeatedAt.ToString("MM/dd/yyyy");
                return View(productDto);
            }
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                newFileName += Path.GetExtension(productDto.ImageFile!.FileName);
                string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }


                string oldImageFullPath = Path.Combine(environment.WebRootPath, "Products", product.ImageFileName);
                System.IO.File.Delete(oldImageFullPath);
            }

            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Brand = productDto.Brand;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }


        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            string oldImageFullPath = Path.Combine(environment.WebRootPath, "Products", product.ImageFileName);
            System.IO.File.Delete(oldImageFullPath);

            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
    }
}
