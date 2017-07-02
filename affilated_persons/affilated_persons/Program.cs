using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Configuration;

namespace affilated_persons
{
    class Program
    {
        static void Main(string[] args)
        {
            Proxy.proxyUrl = "proxy list url";
            bool hasFile = false;
            foreach (string oneFile in Directory.GetFiles(ConfigurationManager.AppSettings["inputDir"]).Where(oneFile => oneFile.Contains("urls")))
            {
                hasFile = true;
                break;
            }
            if (!hasFile)
                return;
            // чистим каталоги
            CleanDirs.Clean();
            WebProxy proxy = Proxy.GetProxy();
            if (proxy != null)
            {
                DownloadAffiliated download = new DownloadAffiliated();
                CheckAffillated checker = new CheckAffillated();
                GetAffilatedPages getterHtml = new GetAffilatedPages();
                ParseHtml parserHtml = new ParseHtml();
                // для каждого подходящего файла в каталоге
                foreach (string oneFile in Directory.GetFiles(ConfigurationManager.AppSettings["inputDir"]).Where(oneFile => oneFile.Contains("urls")))
                {
                    // чек для бухформ
                    bool isBuhDocs = oneFile.EndsWith("urls_buh");
                    string outputDir = isBuhDocs ? ConfigurationManager.AppSettings["outputDirectoryBuh"] : ConfigurationManager.AppSettings["outputDirectory"];
                    Dictionary<string, List<string>> urlsDict = ParseFile.Parse(oneFile);
                    Console.WriteLine("Parsed file: {0}. Parts: {1}", oneFile, urlsDict.Count);
                    if (urlsDict.Count > 0)
                    {
                        foreach (KeyValuePair<string, List<string>> onePart in urlsDict)
                        {
                            checker.DoWork(onePart.Value, onePart.Key, "onlyWithFiles.txt", outputDir, proxy, isBuhDocs);
                            getterHtml.DoWork("onlyWithFiles.txt", "tmp", proxy, isBuhDocs);
                            File.Delete("onlyWithFiles.txt");
                            parserHtml.DoWork("tmp", "fileList", new DateTime(2017, 1, 1));
                            download.DoWork("fileList", outputDir, proxy);
                            Console.WriteLine("done with part {0}", onePart.Key);
                            // чистим каталоги и ищем новую проксю
                            CleanDirs.Clean();
                            proxy = Proxy.GetProxy();
                        }
                    }
                    // переименовываем файл
                    FileInfo fi = new FileInfo(ConfigurationManager.AppSettings["inputFile"]);
                    string dir = fi.DirectoryName;
                    File.Move(oneFile, $"{dir}\\{DateTime.Now.Ticks}.txt");
                }
            }
        }
    }
}
