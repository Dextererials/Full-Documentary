using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking.BackgroundTransfer;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Full_Documentary
{
    class Internet
    {
        public static async Task<XDocument> RequestContent(string url)
        {
            HttpWebRequest request = null;
            WebResponse response = null;
            XDocument novelList = new XDocument();
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                response = await request.GetResponseAsync();
                StreamReader xmlReader;
                string xmlString = string.Empty;

                using (xmlReader = new StreamReader(response.GetResponseStream()))
                {
                    xmlString = xmlReader.ReadToEnd();
                }

                if (xmlString != string.Empty)
                {
                    try
                    {
                        novelList =  XDocument.Parse(xmlString);
                    }
                    catch
                    {
                        HtmlDocument fixMalformation = new HtmlDocument();
                        fixMalformation.OptionAutoCloseOnEnd = true;
                        fixMalformation.OptionCheckSyntax = false;
                        fixMalformation.OptionFixNestedTags = true;
                        fixMalformation.OptionOutputAsXml = true;
                        fixMalformation.LoadHtml(xmlString);
                        xmlString = fixMalformation.DocumentNode.InnerHtml.ToString();
                        fixMalformation.DetectEncodingHtml(xmlString);
                        HtmlAgilityPack.HtmlEntity.DeEntitize(xmlString);
                        novelList = XDocument.Parse(xmlString);
                    }
                    return novelList;
                }
                else
                {
                    return new XDocument();
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                request.Abort();

                if (response != null)
                    response.Dispose();

                return new XDocument();
            }
        }
    }
}



