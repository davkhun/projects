using System.Text;
using System.IO;

namespace zakupki_ftp
{
    public static class Logger
    {
        public static void WriteLog94(string logStr)
        {
            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)+ "\\log94.txt", true, Encoding.GetEncoding(1251)))
                sw.WriteLine(logStr);
        }

        public static void WriteLog223(string logStr)
        {
            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\log223.txt", true, Encoding.GetEncoding(1251)))
                sw.WriteLine(logStr);
        }

        public static void WriteLogB2B(string logStr)
        {
            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\logB2B.txt", true, Encoding.GetEncoding(1251)))
                sw.WriteLine(logStr);
        }

        public static void WriteLogGeneral(string logStr)
        {
            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\log.txt", true, Encoding.GetEncoding(1251)))
                sw.WriteLine(logStr);
        }
    }
}
