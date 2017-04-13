using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YouKnow.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            PdfConvertor.AddTask(DateTime.Now.ToString());
            return Content(DateTime.Now.ToString());
        }

        public class PdfConvertor
        {
            private static List<string> list = new List<string>();
            private static bool running = false;

            public static void AddTask(string pdfFile)
            {
                if (list.Count >= 2)
                    throw new Exception("pdf文件任务量已达最大，请稍后重试");

                list.Add(pdfFile);

                if (running == false)
                {
                    running = true;
                    System.IO.File.AppendAllText("e:/YouKnow.txt","run \r\n");
                    Run();
                }

            }

            public static void Run()
            {
                while (list.Any())
                {
                    var item = list[0];

                    System.Threading.Thread.Sleep(5000);
                    System.IO.File.AppendAllText("e:/YouKnow.txt", item + "\r\n");

                    list.Remove(item);
                }
                running = false;
            }
        }
    }
}