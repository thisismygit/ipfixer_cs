using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace IpFixer
{
    class Program
    {

        static void Main(string[] args)
        {
            // this "Main" function, is where our program begins
            // those args are command line arguments that were used to call the program
            // eg   C:\thisProgram.exe -argument0 -argument1
            //
            // args[0] = "-argument0";
            //
            int IPCheckInterval = 10000;  // every ten second

            // example usage:
            // ipfixer http://www.domain.com/dev/ipfixer.php hostA

            // if http:// then do http method
            // else if ftp:// then do ftp method

            // first argument is the host to connect to, including arguments
            // second argument is

            if (args.Length == 1)
                if (args[0] == "help" || args[0] == "h" || args[0] == "-h" || args[0] == "/h" || args[0] == "/help" || args[0] == "-help")
                {
                    Console.WriteLine("help not implimented yet.");
                    return;
                }
                else
                    Console.WriteLine("plz specify a web address to interact with and a hostname as arguments.");


            string address = "";
            string hostName = "";

            if (args.Length == 2)
            {
                address = args[0];
                hostName = args[1];
            }
            else
            {
                Console.WriteLine("plz specify a web address to interact with and a hostname as arguments.");
                return;
            }



            address += "?hostname=" + hostName + "&field=publicfield";
            string ipData = "";
           

            while (true)
            {
                //  scrape

                string curIP = DoWeb.GetExternalIp().ToString();
                if (ipData != curIP)
                {
                    ipData = curIP;
                    DoWeb.BrowseToPage(address);
                    Console.Write(ipData);
                }

                System.Threading.Thread.Sleep(IPCheckInterval);
            }
        }
    }



    class DoWeb
    {
        public static void BrowseToPage(string Address)
        {
            // string url = "http://www.excelsiorcarpetone.com/clay/ipfixer.php?field=publicfield" + "&host=" + hostName;
            WebRequest request = WebRequest.Create(Address);

            request.ContentType = "Content-type: text/xml";
            request.Method = "GET";

            // Fire off the request and get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // System.Diagnostics.EventLog.WriteEntry("aaaMyIPFixer", "IP Fixer just tried to request a webpage.   Status:  " + response.StatusDescription);

            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            //string responseFromServer = reader.ReadToEnd();
            //System.Diagnostics.EventLog.WriteEntry("aaaMyIPFixer", "IP Fixer msg:  " + responseFromServer.Trim());
        }




        /// <summary>
        /// This writes a string of text to an FTP server
        /// Just specify the msg to write...
        /// Later I'll make one that let's you specify the FTP server, the username, the pass, and the file path to write to.
        /// </summary>
        /// <returns>returns null if something goes wrong</returns>
        public static void WriteToFTP(string msg, string FilePath, string Username, string Password, string ftpAddress)
        {
            string FTPAddress = "ftp://ftp.domain.net";
            string filePath = FTPAddress + "/" + "castt/" + "update.txt";
            string password = "yourpasswordgoeshere";
            string username = "yourusernamegoes here";


            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(filePath);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;


            // convert msg to byte[] so it can be put on the FTP server
            byte[] myExternalIPAddress;
            UTF8Encoding encoding = new System.Text.UTF8Encoding();
            myExternalIPAddress = encoding.GetBytes(msg);


            // write to the FTP server
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(myExternalIPAddress, 0, myExternalIPAddress.Length);
            reqStream.Close();
        }

        /// <summary>
        /// This returns your IP address by navigating to an online IP reporter website; downloading the string it reports. 
        /// Btw, you can get at the string by GetExternalIp().ToString()
        /// Relys on http://www.whatismyip.com/automation/n09230945.asp working right
        /// </summary>
        /// <returns>returns null if something goes wrong</returns>
        public static IPAddress GetExternalIp()
        {
            string whatIsMyIp = "http://www.whatismyip.com/automation/n09230945.asp";      // this is their special page that simply returns the string consisting of your IP address
            WebClient wc = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            string requestHtml = "";

            try
            {
                requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
            }
            catch (WebException we)
            {
                // do something with exception
                Console.Write(we.ToString());
            }


            int L1 = requestHtml.Length;    // Get length of IP address
            int L2 = requestHtml.Replace(".", "").Length;  // remove decimal points and get new length

            bool didFindValidIP;
            if (L1 - L2 == 3)       // were there 3 decimal points?  That means it worked!
                didFindValidIP = true;
            else
                didFindValidIP = false;


            IPAddress externalIp = null;
            if (didFindValidIP)
            {
                externalIp = IPAddress.Parse(requestHtml.ToString());
            }
            return externalIp;
        }
    }
}
