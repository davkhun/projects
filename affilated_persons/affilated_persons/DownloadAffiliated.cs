using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Security.Cryptography;


namespace affilated_persons
{
    internal class DownloadAffiliated
    {

        private static string CleanDirectory(string directoryName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(directoryName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        /// <summary>
        /// Скачивание файлов по спискам URL
        /// </summary>
        /// <param name="inputDirectory">Папка с файлами URL</param>
        /// <param name="outputDirectory">Папка с выходными файлами</param>
        /// <param name="proxy"></param>
        /// <param name="justFirstFile">Забирать только первый файл из списка</param>
        public void DoWork(string inputDirectory, string outputDirectory, WebProxy proxy = null, bool justFirstFile = false)
        {
            WebClient wc = new WebClient();
            if (proxy != null)
                wc.Proxy = proxy;

                foreach (string oneFile in Directory.GetFiles(inputDirectory))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(oneFile);
                        string id = fi.Name.Split('.')[0];
                        string[] data = File.ReadAllLines(oneFile);
                        string orgName = $"{data[0]}_{id}";
                        orgName = CleanDirectory(orgName);
                        // создаем временную директорию для хранения проверяемых файлов
                        if (!Directory.Exists("outputTmp"))
                            Directory.CreateDirectory("outputTmp");

                        int count = data.Length;
                        if (justFirstFile)
                            count = 2;
                        for (int i = 1; i < count; i++)
                        {
                            string[] tokens = data[i].Split(';');
                            string unique = DateTime.Now.Ticks.ToString();
                            string outFileName = $"{orgName}_{tokens[1]}_{tokens[0]}_{unique}.{tokens[2]}";
                            string tmpDirFile = $"outputTmp\\{outFileName}";
                            // скачиваем во временную директорию, проверяем, если норм, перемещаем файл, если нет - удаляем и качаем дальше
                            wc.DownloadFile(tokens[3], tmpDirFile);
                            bool isInserted = InsertAndCheckData(tmpDirFile, id, data[0], tokens[1]);
                            if (isInserted)
                            {
                                // если всё хорошо, создаем конечную директорию, куда и переносим файл
                                if (!File.Exists($"{outputDirectory}\\{outFileName}"))
                                    File.Move(tmpDirFile, $"{outputDirectory}\\{outFileName}");
                            }
                            else
                                File.Delete(tmpDirFile);
                            Console.WriteLine("{0} - {1}. ID: {2}", tokens[3], isInserted, id);
                        }
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine("Error while downloading {0}. Try new proxy...", e.Message);
                        proxy = Proxy.GetProxy();
                        wc.Proxy = proxy;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }



        }

        public bool InsertAndCheckData(string fileName, string orgID, string orgName, string fileDate)
        {
            string md5Str;
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    md5Str = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
            bool isInserted=true;
            // TODO: database insertion (insert ignore)
            return isInserted;
        }
    }
}
