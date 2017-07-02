using System;
using System.IO;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace zakupki_ftp
{
    class Program
    {

        static void Main()
        {
            string[] cmdLine = Environment.GetCommandLineArgs();
            string path = ConfigurationManager.AppSettings["WorkFolder"];
            string path223 = ConfigurationManager.AppSettings["223Folder"];
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(path223))
                Directory.CreateDirectory(path223);
            if (cmdLine.Length > 1)
            {

                try
                {

                    MySqlConnection conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnString"]);
                    MySqlCommand comm;
                    Zakupki zak = new Zakupki();

                    if (cmdLine[1] == "/223")
                    {
                        Notification223fz notif223 = new Notification223fz(path223);
                        DateTime d1 = DateTime.Now;
                        Logger.WriteLog223("Начали работать по 223 " + d1);
                        foreach (Notification223fz.Surls oneUrl in notif223.urls)
                        {
                            zak.DoWork(oneUrl.url, path223, path223, 4);
                            notif223.region = oneUrl.region;
                            Logger.WriteLog223("Регион " + notif223.region);
                            notif223.DoWork();
                        }

                        Logger.WriteLog223("Оптимизируем таблицы...");
                        Logger.WriteLog223("Закончили работать. Потратили: " + (DateTime.Now - d1).TotalSeconds);
                    }
                    else if (cmdLine[1] == "/94")
                    {
                        Notification94fz notif94 = new Notification94fz(path);
                        DateTime d1 = DateTime.Now;
                        Logger.WriteLog94("Начали работать по 44 " + d1);
                        foreach (Notification94fz.Surls oneUrl in notif94.urls)
                        {
                            zak.DoWork(oneUrl.url, path, path, 2);
                            notif94.region = oneUrl.region;
                            Logger.WriteLog94("Регион " + notif94.region);
                            Console.WriteLine("Работаем с {0}", notif94.region);
                            notif94.DoWork();
                            Console.WriteLine("Elapsed {0}: {1}", notif94.region, (DateTime.Now - d1).TotalSeconds);
                        }
                        Logger.WriteLog94("Оптимизируем таблицы...");
                        Logger.WriteLog94("Закончили работать. Потратили: " + (DateTime.Now - d1).TotalSeconds);
                        Console.WriteLine("Elapsed: {0}", (DateTime.Now - d1).TotalSeconds);
                    }

                    else if (cmdLine[1] == "/b2b")
                    {
                        NotificationB2B notifB2B = new NotificationB2B(path);
                        DateTime d1 = DateTime.Now;
                        Logger.WriteLogB2B("Начали работать по b2b" + d1);
                        notifB2B.DoWork();

                        Logger.WriteLogB2B("Закончили работать. Потратили: " + (DateTime.Now - d1).TotalSeconds);
                        Console.WriteLine("Elapsed: {0}", (DateTime.Now - d1).TotalSeconds);

                    }
                    else if (cmdLine[1] == "/trunc")
                    {
                        comm = new MySqlCommand("truncate tenderbase.notification;truncate tenderbase.products;truncate tenderbase.lot;truncate tenderbase.documents;", conn);
                        conn.Open(); comm.ExecuteNonQuery(); conn.Close();
                    }
                    else if (cmdLine[1] == "/fraud")
                    {
                        FraudNotice fNotice = new FraudNotice();
                        fNotice.InsertFraudNotice();
                    }
                    else if (cmdLine[1] == "/optimize")
                    {
                        comm = new MySqlCommand("optimize table tenderbase.notification, tenderbase.lot, tenderbase.products", conn);
                        conn.Open(); comm.ExecuteNonQuery(); conn.Close();
                    }
                    
                }
                catch (Exception ex)
                {
                    Logger.WriteLogGeneral(DateTime.Now + ": " + cmdLine[1] + ". " + ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                  Console.WriteLine("Usage:");
                  Console.WriteLine("/96 - 96 federal law import.");
                  Console.WriteLine("/223 - 223 federal law import.");
                  Console.WriteLine("/b2b - b2b-center tenders import.");
                  Console.ReadKey();
            }
        }
    }
}
