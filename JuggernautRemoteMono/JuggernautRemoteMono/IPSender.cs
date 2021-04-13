using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace JuggernautControlAndroid
{
    public class IPSender
    {


        public static string jsIPID = "306fc0a4-9ae4-11eb-ad57-0242ac110002";
        public static string jsBin = "ccfbc521785e";
        public static string getIP()
        {
            using (var client = new System.Net.WebClient())
            {
                client.Headers["Security-key"] = jsIPID;
                return System.Text.Encoding.Default.GetString(client.DownloadData("https://json.extendsclass.com/bin/" + jsBin)).Split(':')[1].Replace("\"", "").Replace("}", "").Replace(" ", "");
            }
        }

    }
}
