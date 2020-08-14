using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Diplom.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http;


namespace Diplom.Controllers
{
    public class HomeController : Controller
    {

        UserContext db;

        

        public HomeController(UserContext context)
        {
            db = context;
        }

        public IActionResult Start()
        {
            return View(db.Suppliers.ToList());
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(db.Suppliers.ToList());
        }

        //Действие при нажатии Подробнее
        [HttpGet]
        public IActionResult Moredetails(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.Id = id;
            return View(db.Suppliers.Where(s => s.Id == id).ToList());
        }

        //Кнопка Оставить заявку 
        [HttpGet]
        public IActionResult Request()
        {
            return View();
        }

        [HttpPost]
        public string Request(Order order)
        {
            db.Orders.Add(order);
            // сохраняем в бд все изменения
            db.SaveChanges();
            return order.UserName + " " +"Спасибо за заявку.Наш специалист свяжется в Вами в ближайшее время!";
        }


        //Добавление поставщика
        [HttpGet]
        public IActionResult AddSuppliers()
        {
            return View();
        }
        [HttpPost]
        public string AddSuppliers(Suppliers suppliers)
        {
            db.Suppliers.Add(suppliers);
            // сохраняем в бд все изменения
            db.SaveChanges();
            return suppliers.Suppname + " " + "Спасибо за заявку.Наш специалист свяжется в Вами в ближайшее время!";
        } 

        public IActionResult Privacy()
        {
            return View();
        }

       
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
