using System;
using System.Collections.Generic;
using System.IO;

namespace affilated_persons
{
    public class ParseFile
    {
        public static Dictionary<string, List<string>> Parse(string fileName)
        {
            Dictionary<string, List<string>> urlsDict = new Dictionary<string, List<string>>();
            List<string> partList = new List<string>();
            string[] urlArr = File.ReadAllLines(fileName);
            for (int i = 0; i < urlArr.Length; i++)
            {
                if (i % 25 == 0)
                {
                    if (partList.Count > 0)
                    {
                        urlsDict.Add(Guid.NewGuid().ToString(), partList);
                        partList = new List<string>();
                    }
                }
                partList.Add(urlArr[i]);
            }
            if (partList.Count > 0)
                urlsDict.Add(DateTime.Now.Ticks.ToString(), partList);
            return urlsDict;
        }
    }
}
