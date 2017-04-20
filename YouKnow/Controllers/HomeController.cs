using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace YouKnow.Controllers
{
    public class HomeController : Controller
    {
        /*
            一个Tab页连续刷新，线程处理是一个接一个的，list里面的内容还没有完全为空，所以只Run了一次。
            而多个Tab页访问，发现线程启动会慢些，此时list里面的内容都已经为空了，所以Run了多次，而到了后面，发现不会再Run多次了
        */
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} start");

            if (PdfConvertor.list.Count >= 10)
            {
                System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} pdf文件任务量已达最大，请稍后重试");
                return Content($"当前数量：{PdfConvertor.list.Count} pdf文件任务量已达最大，请稍后重试");
            }
            //为了不阻塞，这里使用Task来运行
            Task.Run(() => { PdfConvertor.AddTask(DateTime.Now.ToString()); });
            return Content($"当前数量：{PdfConvertor.list.Count} {DateTime.Now.ToString()}");
        }

        public class PdfConvertor
        {
            public static List<string> list = new List<string>();
            private static bool running = false;

            public static void AddTask(string pdfFile)
            {
                list.Add(pdfFile);

                System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} running:{running}");
                if (running == false)
                {
                    System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 没运行我要运行了");
                    running = true;
                    System.IO.File.AppendAllText("e:/YouKnow.txt", "run \r\n");
                    Run();
                }

            }

            public static void Run()
            {
                while (list.Any())
                {
                    var item = list[0];
                    System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} List：{list.Count}");
                    System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 开始暂停：{DateTime.Now}");
                    System.Threading.Thread.Sleep(30000);
                    System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 结束暂停：{DateTime.Now}");
                    System.IO.File.AppendAllText("e:/YouKnow.txt", item + "\r\n");

                    list.Remove(item);
                }
                System.Diagnostics.Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} List中没东西了");
                running = false;
            }
        }
    }
}