using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.Configuration;

namespace zakupki_ftp
{
    public class Zakupki
    {

        private static string ReverseString(string str)
        {
            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public void DoWork(string uri, string zipPath, string unzipPath, int days)
        {
            List<string> fileList = GetFileListFromFTP(uri, days); 
            if (fileList.Count > 0)
            {
                DownloadFileList(fileList, zipPath);
                DeleteEmptyFiles(zipPath);
                if (Directory.GetFiles(zipPath).Length > 0)
                    UnZip(zipPath, unzipPath);
            }
        }



        public List<string> GetFileListFromFTP(string uri, int days)
        {
            List<string> files = new List<string>();
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
            req.UsePassive = true;
            req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            try
            {
                FtpWebResponse resp = (FtpWebResponse)req.GetResponse();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        str = ReverseString(str);
                        str = str.Substring(0, str.IndexOf("   ")).Trim();
                        if (Convert.ToInt32(ReverseString(str).Split(' ')[0]) > 500)
                        {
                            string fUri = uri + ReverseString(str.Split(' ')[0]);
                            // получаем дату файла, если подходит - качаем
                            FtpWebRequest fDateReq = (FtpWebRequest)WebRequest.Create(fUri);
                            fDateReq.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                            FtpWebResponse fDateResp = (FtpWebResponse)fDateReq.GetResponse();
                            string dt = fDateResp.StatusDescription.Substring(fDateResp.StatusDescription.LastIndexOf(' ') + 1);
                            dt = dt.Substring(0, 4) + "-" + dt.Substring(4, 2) + "-" + dt.Substring(6, 2);
                            if (ConfigurationManager.AppSettings["UpdateDate"] == "now")
                            {
                                if (Convert.ToDateTime(dt) >= DateTime.Now.AddDays(-days))
                                    files.Add(fUri);
                            }
                            else
                                files.Add(fUri);
                        }
                    }
                }
            }
            catch
            {
               
            }
            return files;
        }

        public void DownloadFile(string uri, string output)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
                req.UsePassive = true;
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse resp = (FtpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                FileStream fstream = File.Create(output);
                byte[] buffer = new byte[resp.ContentLength];
                int bytesread;
                do
                {
                    bytesread = stream.Read(buffer, 0, buffer.Length);
                    fstream.Write(buffer, 0, bytesread);
                } while (bytesread > 0);
                stream.Close();
                fstream.Close();
                resp.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка DownloadFile: " + ex.Message);
            }

        }

        public void DownloadFileList(List<string> fileList, string outputPath)
        {
            foreach (string oneFile in fileList)
            {
                string fileName = ReverseString(ReverseString(oneFile).Split('/')[0]);
                DownloadFile(oneFile,outputPath + fileName);
            }
        }

        public void DeleteEmptyFiles(string path)
        {
            foreach (string oneFile in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(oneFile);
                if (fi.Length < 500) File.Delete(oneFile);
            }
        }

        public void UnZip(string zipPath, string unzipPath)
        {
            foreach (string zFile in Directory.GetFiles(zipPath))
            {
                FileInfo fi = new FileInfo(zFile);
                if (fi.Extension == ".zip")
                {
                    try
                    {
                        using (ZipFile zipFil = ZipFile.Read(zFile))
                        {
                            foreach (ZipEntry zEntry in zipFil)
                                zEntry.Extract(unzipPath, ExtractExistingFileAction.OverwriteSilently);
                        }
                        File.Delete(zFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка UnZip: " + ex.Message);
                    }
                }
            }
        }

    }
}
