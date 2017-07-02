using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SKYPE4COMLib;
using System.IO;
using System.Net;

namespace skype_bot_win
{
    public partial class Main : Form
    {
        private Skype service;

        public class CFileContent
        {
            public string send_to { get; set; }
            public string to { get; set; }
            public string message { get; set; }

            public CFileContent(string data)
            {
                string[] arr = data.Split('|');
                send_to = arr[0];
                to = arr[1];
                message = arr[2];
            }

        }



        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            service = new Skype();
            if (!service.Client.IsRunning)
                service.Client.Start(true, true);
            service.Attach(7,true);
            service.MessageStatus += service_MessageStatus;
            listBox1.Items.Add("Skype connected...");
        }

        private void service_MessageStatus(ChatMessage pMessage, TChatMessageStatus Status)
        {
            try
            {
                listBox1.Items.Add(String.Format("Date: {0},Chat: {1},Send from: {2}, status: {3}", DateTime.Now, pMessage.ChatName, pMessage.Sender.Handle, Status));
                if (Status == TChatMessageStatus.cmsReceived)
                {
                    if (pMessage.Body.IndexOf("!") == 0)
                    {

                        // Remove trigger string and make lower case
                        string command = pMessage.Body.Remove(0, 1).ToLower();

                        // Send processed message back to skype chat window
                        if (pMessage.ChatName.Contains('#'))
                            service.Chat[pMessage.ChatName].SendMessage(ProcessCommand(command));
                        else
                            service.SendMessage(pMessage.Sender.Handle, ProcessCommand(command));
                        listBox1.Items.Add(String.Format("Date: {0},Chat: {1},Send from: {2}, Message: {3}", DateTime.Now, pMessage.ChatName, pMessage.Sender.Handle, pMessage.Body));
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("process_log.txt", true))
                    sw.WriteLine("{0}.Command: {1} - Error: {2}", DateTime.Now, pMessage.Body, ex.Message);
            }

        }


        private string ProcessCommand(string str)
        {
                string result = null;
                StringBuilder strb = new StringBuilder();
                if (str.Equals("help"))
                {
                    strb.AppendLine("!help - current help.");
                    strb.AppendLine("!bash - random bash quotes.");
                    result = strb.ToString();
                }
                else if (str.Equals("bash"))
                {
                    WebClient wc = new WebClient();
                    string bq = wc.DownloadString("http://bash.im/random");
                    bq = bq.Substring(bq.IndexOf("div class=\"text\"") + 17);
                    bq = bq.Substring(0, bq.IndexOf("</div>"));
                    bq = bq.Replace("<br>", "\r\n").Replace("<br />", "\r\n").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;","\"");
                    result = bq;
                }
               
                return result;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\request";
            string donePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\request\\done";
            if (Directory.GetFiles(path).Length > 0)
            {
                foreach (string oneFile in Directory.GetFiles(path))
                {
                    try
                    {
                        CFileContent cont = new CFileContent(File.ReadAllText(oneFile));
                        if (cont.send_to.Equals("person"))
                            service.SendMessage(cont.to, cont.message);
                        else if (cont.send_to.Equals("chat"))
                            service.Chat[cont.to].SendMessage(cont.message);
                        // moving
                        FileInfo fi = new FileInfo(oneFile);
                        File.Move(oneFile, donePath + "\\" + DateTime.Now.Ticks + "_" + fi.Name);
                        listBox1.Items.Add(String.Format("Date: {0},Chat: {1},Send from: file, Message: {2}", DateTime.Now, cont.to, cont.message.Substring(0, 60)));
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = new StreamWriter("log.txt", true))
                            sw.WriteLine("{0} - Error:{1}", DateTime.Now, ex.Message);
                    }
                }
            }
        }

    }
}
