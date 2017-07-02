using System;
using System.Net;
using System.IO;

namespace affilated_persons
{
    internal class GetAffilatedPages
    {
        /// <summary>
        /// Вытаскивает html код страниц аффилированных лиц, разбивая их по файлам
        /// </summary>
        /// <param name="inputFile">Входной файл после обработки CheckAffilated</param>
        /// <param name="outputPath">Папка для загружаемых файлов</param>
        /// <param name="proxy"></param>
        /// <param name="checkBuhDocs">Для бухгалтерской отчетности</param>
        public void DoWork(string inputFile, string outputPath, WebProxy proxy = null, bool checkBuhDocs = false)
        {
            WebClient wc = new WebClient();
            string[] data = File.ReadAllLines(inputFile);
            if (proxy != null)
                wc.Proxy = proxy;
            foreach (string one in data)
            {
                try
                {
                    string id = one.Split('=')[1];
                    string url = $"http://www.e-disclosure.ru/portal/files.aspx?id={id}&type={(checkBuhDocs ? "3" : "6")}";
                    wc.DownloadFile(url, $"{outputPath}\\{id}.txt");
                    Console.WriteLine($"Donwload {id}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while download pages with files: {0}. Try again...", e.Message);
                    proxy = Proxy.GetProxy();
                }

            }
        }
    }
}
