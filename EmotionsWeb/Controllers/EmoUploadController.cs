using EmotionsWeb.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmotionsWeb.Models;
	

namespace EmotionsWeb.Controllers
{
    public class EmoUploadController : Controller
    {
        string serverFolderPath;
        EmotionHelper emoHelper;
        string key;
        EmotionsWebContext db = new EmotionsWebContext();
        

        public EmoUploadController()
        {
            serverFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"]; //obtenemos la ruta del folder donde estan las imagenes
            key = ConfigurationManager.AppSettings["EMOTION_KEY"];
            emoHelper = new EmotionHelper(key);
        }

        // GET: EmoUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {


            if (file != null && file.ContentLength > 0)
            {
                var pictureName = Guid.NewGuid().ToString();//genera una cadena aleatoria.
                pictureName += Path.GetExtension(file.FileName); //obtenemos la extension del archivo

                var route = Server.MapPath(serverFolderPath);//mapeamos en el servidor la ruta exacta, que esta en web.congig <appSettings>

                route = route + "/" + pictureName;

                file.SaveAs(route);

                var emoPicture = await emoHelper.detectAndExtractFaceAsync(file.InputStream);

                emoPicture.Name = file.FileName;
                emoPicture.Path = $"{serverFolderPath}/{pictureName}";

                db.EmoPictures.Add(emoPicture);
                await db.SaveChangesAsync();

                return RedirectToAction("Details", "EmoPictures", new { Id = emoPicture.Id });
               
            }

            return View();
        }


    }
}