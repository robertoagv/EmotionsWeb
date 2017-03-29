using EmotionsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmotionsWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.WelcomeMessage = "Hola Mundo";
            ViewBag.ValorEntero = 100;
            return View();
        }
        public  ActionResult IndexAlt()
        {
            var model = new Home();
            model.WelcomeMessage = "Hola Mundo desde el Modelo";
            
            return View(model);
        }
    }
}