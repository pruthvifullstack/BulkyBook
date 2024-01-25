using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDBContext _db;

        public CategoryController(ApplicationDBContext db)
        {
            _db = db;
        }

        //GET
        [Authorize]
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            var sorted = objCategoryList.OrderBy(c => c.DisplayOrder).ToList();
            return View(sorted);
        }

        //GET
        [Authorize]
        public IActionResult Create() {
            Console.WriteLine("Creat clicked!!!!!!");
            return View();
        }

        //POST
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] // this avoid the cross site request forgery
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {   //this will be display in te summary all
                ModelState.AddModelError("name", "The display order cannot be same as the name!");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Creation was successfull";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var categoryFromDb = _db.Categories.Find(id);
            return View(categoryFromDb);
        }   

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] // this avoid the cross site request forgery
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {   //this will be display in te summary all
                ModelState.AddModelError("name", "The display order cannot be same as the name!");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Edit was successfull";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var categoryFromDb = _db.Categories.Find(id);
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Categories.Find(id);

            if(obj == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Delete was successfull";
            return RedirectToAction("Index");
        }
    }
}
