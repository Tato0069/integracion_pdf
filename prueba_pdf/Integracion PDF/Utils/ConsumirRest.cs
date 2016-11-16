using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;

namespace ConsumerClient
{
    public partial class _Default : System.Web.UI.Page
    {
    
        protected void Page_Load(object sender, EventArgs e)
        {
            WebRequest req = WebRequest.Create(@"http://cidi-demos.exe.cl/SKUProyect/rest/clasificador/texto/");

            req.Method = "GET";

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (Stream respStream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            else
            {
                Console.WriteLine(string.Format("Status Code: {0}, Status Description: {1}", resp.StatusCode, resp.StatusDescription));
            }
            Console.Read();
        }

    }
}