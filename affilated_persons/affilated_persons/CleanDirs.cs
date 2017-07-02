using System.IO;

namespace affilated_persons
{
    internal class CleanDirs
    {
        public static void Clean()
        {
            // чистим каталоги
            DirectoryInfo di = new DirectoryInfo("tmp");
            foreach (FileInfo one in di.GetFiles())
                one.Delete();
            di = new DirectoryInfo("outputTmp");
            foreach (FileInfo one in di.GetFiles())
                one.Delete();
            di = new DirectoryInfo("fileList");
            foreach (FileInfo one in di.GetFiles())
                one.Delete();
            di = new DirectoryInfo("done");
            foreach (FileInfo one in di.GetFiles())
                one.Delete();
        }
    }
}
