using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace affilated_persons
{
    internal class CheckAffillated
    {
        /// <summary>
        /// По списку url на организации вытаскивает те, у которых есть ссылка на аффилированных лиц
        /// </summary>
        /// <param name="urlsList">Список url для работы</param>
        /// <param name="partKey"></param>
        /// <param name="outputFile">Итоговый файл</param>
        /// <param name="outputDirectory">Папка с итоговыми данными</param>
        /// <param name="proxy">Для использования прокси</param>
        /// <param name="checkBuhDocs">Для бухгалтерской отчетности</param>
        public void DoWork(List<string> urlsList,string partKey, string outputFile, string outputDirectory, WebProxy proxy = null, bool checkBuhDocs = false)
        {
            Dictionary<string, string> urlsDict = new Dictionary<string, string>();
            WebClient wc = new WebClient();
            if (proxy != null)
                wc.Proxy = proxy;
            string[] data = urlsList.ToArray();
            List<string> withAffilatedList = new List<string>();
            foreach (string one in data)
            {
                // мелкий блок для запихивания результатов обработки
                try
                {
                    urlsDict.Add(one, null);
                }
                catch {}
                // загнать в try
                string urlData;
                try
                {
                    urlData = wc.DownloadString(one);
                }
                catch (WebException e)
                {
                    Console.WriteLine("{0} WebException Error: {1}. Try new proxy...", one, e.Message);
                    urlsDict[one] = e.Message;
                    proxy = Proxy.GetProxy();
                    wc.Proxy = proxy;
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Error: {1}", one, e.Message);
                    urlsDict[one] = e.Message;
                    continue;
                }
                if (!checkBuhDocs)
                {
                    if (urlData.Contains("&type=6"))
                    {
                        urlsDict[one] = "affiliated found";
                        Console.WriteLine("{0} affilated found", one);
                        withAffilatedList.Add(one);
                    }
                    else
                    {
                        urlsDict[one] = "affiliated not found";
                        Console.WriteLine("{0} affilated not found", one);
                    }
                }
                else
                {
                    if (urlData.Contains("&type=3"))
                    {
                        urlsDict[one] = "buh docs found";
                        Console.WriteLine("{0} buh docs found", one);
                        withAffilatedList.Add(one);
                    }
                    else
                    {
                        urlsDict[one] = "buh docs not found";
                        Console.WriteLine("{0} buh docs not found", one);
                    }
                }
            }
            File.WriteAllLines(outputFile, withAffilatedList.ToArray());
            if (!checkBuhDocs)
            {
                using (StreamWriter file = new StreamWriter($"{outputDirectory}url_statuses_affiliated_{partKey}.txt"))
                    foreach (KeyValuePair<string, string> entry in urlsDict)
                        file.WriteLine("{0} {1}", entry.Key, entry.Value);
            }
            else
            {
                using (StreamWriter file = new StreamWriter($"{outputDirectory}url_statuses_buh_docs_{partKey}.txt"))
                    foreach (KeyValuePair<string, string> entry in urlsDict)
                        file.WriteLine("{0} {1}", entry.Key, entry.Value);
            }
        }
    }
}
