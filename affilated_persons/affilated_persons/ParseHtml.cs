using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HtmlAgilityPack;

namespace affilated_persons
{
    internal class ParseHtml
    {
        /// <summary>
        /// Парсит html страницы аффилированных лиц, вытаскивая данные по загружаемым файлам
        /// </summary>
        /// <param name="inputPath">Директория с сырыми html файлами</param>
        /// <param name="outputPath">Директория с результирующими данными</param>
        /// <param name="fromDate">Грузить документы с указанной даты</param>
        public void DoWork(string inputPath, string outputPath, DateTime fromDate)
        {
            foreach (string oneRaw in Directory.GetFiles(inputPath))
            {
                try
                {
                    FileInfo fi = new FileInfo(oneRaw);
                    List<string> collectedFiles = new List<string>();
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(oneRaw, Encoding.UTF8);
                    string orgName = doc.DocumentNode.SelectSingleNode("//body //div[@id='wrapper'] //div[@id='middle'] //div[@id='container'] //div[@id='content2'] //div[@class='infoblock'] //h2").InnerText;
                    collectedFiles.Add(orgName.Replace("&quot;", ""));
                    HtmlNode tableNode = doc.DocumentNode.SelectSingleNode("//body //div[@id='wrapper'] //div[@id='middle'] //div[@id='container'] //div[@id='content2'] //div[@id='cont_wrap'] //div[@class='spaceTbl'] //table[@class='zebra noBorderTbl centerHeader']");
                    foreach (HtmlNode row in tableNode.SelectNodes("tr"))
                    {
                        if (row.InnerHtml.Contains("FileLoad.ashx"))
                        {
                            string type = row.SelectNodes("td")[1].InnerText;
                            string date = row.SelectNodes("td")[4].InnerText;
                            string extension = row.SelectNodes("td")[5].InnerText.Split(',')[0];
                            string url = row.SelectNodes("td")[5].SelectSingleNode("a").Attributes["href"].Value;
                            string completeRow = $"{type};{date};{extension};{url}";
                            Console.WriteLine(completeRow);
                            if (Convert.ToDateTime(date) >= fromDate)
                                collectedFiles.Add(completeRow);
                        }
                    }
                    File.WriteAllLines($"fileList\\{fi.Name}", collectedFiles.ToArray());
                }
                catch 
                {
                    // если тут не срослость, пропускаем
                    continue;
                }

            }
        }
    }
}
