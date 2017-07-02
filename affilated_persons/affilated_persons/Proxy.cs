using System;
using System.Collections.Generic;
using System.Net;

namespace affilated_persons
{
    public class Proxy
    {
        public static string proxyUrl;

        public static WebProxy GetProxy()
        {
            Console.WriteLine("Start to find proxies");
            List<string> proxyList = new List<string>();
            CWebClient wc = new CWebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;
            string proxyStr;
            try
            {
                proxyStr = wc.DownloadString(proxyUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while download proxies: {0}. Try again...", e.Message);
                wc = new CWebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                proxyStr = wc.DownloadString(proxyUrl);
            }


            foreach (string oneProxy in proxyStr.Split('\r'))
            {
                if (oneProxy.Length > 5)
                    proxyList.Add(oneProxy.Replace("\n", ""));
            }
            Console.WriteLine($"FOUND PROXIES: {proxyList.Count}");
            foreach (string oneProxy in proxyList)
            {
                string currentAddress = oneProxy.Split(':')[0];
                int currentPort = Convert.ToInt32(oneProxy.Split(':')[1]);
                try
                {
                    wc.Proxy = new WebProxy(currentAddress, currentPort);

                    Console.Write($"try: {currentAddress}:{currentPort}");
                    DateTime dt = DateTime.Now;
                    wc.DownloadString("http://www.e-disclosure.ru");
                    TimeSpan ts = DateTime.Now - dt;
                    Console.WriteLine(". Elapsed: {0}", ts.TotalMilliseconds);
                    if (ts.TotalMilliseconds < 4000)
                    {
                        Console.WriteLine($"\r\nFOUND Proxy: {currentAddress}:{currentPort}");
                        return new WebProxy(currentAddress, currentPort);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(". Result: {0}", e.Message);
                }
            }
            return null;
        }

    }
}
