using System;
using System.Collections;
using System.Text;
using System.Xml;
using log4net;
using System.Net;
using System.IO;
using System.Web.Mail;
using System.Web;

namespace MCPlusA.Google
{
//    enum eFeedType: int
//    {
//        eFull,
//        eIncremental
//    }

    public enum eLogLevel : int
    {
        eInfo,
        eError,
        eFatal
    }

    public class GSAUploader
    {
        
        protected log4net.ILog log;
        protected bool performLog;  //it will automatically be initialized to false at runtime
        protected int webtimeout = 10*60*1000; //10 minutes

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GSAUploader"/> class.
        /// </summary>
        public GSAUploader()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GSAUploader"/> class.
        /// </summary>
        /// <param name="myLogger">A previous instance of log4net logger</param>
        public GSAUploader(ILog myLogger)
        {
            performLog = true;
            log = myLogger;
        }

        private bool PostToGSA(string gsaURL, byte[] data, string docURL)
        {
            HttpWebRequest gsaHttpRequest;
            HttpWebResponse gsaHttpResponse = null;
            Stream PostStream = null;
            Exception exToThrow = null;
            int statCode = 0;

            try
            {
                gsaHttpRequest = (HttpWebRequest)WebRequest.Create(gsaURL);
                // set web request parameters for multipart post
                gsaHttpRequest.ContentType = "multipart/form-data";
                gsaHttpRequest.Method = "POST";
                gsaHttpRequest.UserAgent = "Feeder Web Service";
                gsaHttpRequest.ContentLength = data.Length;
                gsaHttpRequest.Timeout = webtimeout;
                PostStream = gsaHttpRequest.GetRequestStream();
                PostStream.Write(data, 0, data.Length);
                gsaHttpResponse = (HttpWebResponse)gsaHttpRequest.GetResponse();
                statCode = (int)gsaHttpResponse.StatusCode;
            }
            catch (Exception ex)
            {
                log.Error("Error Pushing XML.");
                log.Error(ex.Message);
            }
            finally
            {
                if (null != PostStream) PostStream.Close();
                PostStream = null;
                if (null != gsaHttpResponse) gsaHttpResponse.Close();
                gsaHttpResponse = null;
                gsaHttpRequest = null;
            }

            // if statCode is 200 then post was successful
            if (statCode == 200)
            {
                return true;
            }
            else
            {
                log.Error("Failed to feed service, return code: " + statCode);
            }

            if (null != exToThrow)
            {
                throw exToThrow;
            }

            return false;
        }

        public bool FeedXML(string sourceURL, string gsaHost, bool ssl)
        {
            string gsaFeedURLPrimary = gsaHost;
            string feedtype = string.Empty;
            string datasource = string.Empty;
            
            FileStream fStrm = null;
            StreamReader treader = null;
            try
            {
                //read the file into a stream
                fStrm = File.OpenRead(sourceURL);
                Encoding tencode = System.Text.Encoding.GetEncoding("utf-8");
                treader = new StreamReader(fStrm, tencode);
                string filedata = HttpUtility.UrlEncode(treader.ReadToEnd());
                fStrm.Close();
                treader.Close();

                XmlDocument myXml = new XmlDocument();
                myXml.Load(sourceURL);


                if (myXml.SelectSingleNode("/gsafeed/header/feedtype").InnerText.ToLower() == GSAFeedType.FEEDTYPE_FULL)
                {
                    feedtype = GSAFeedType.FEEDTYPE_FULL;
                    Log(".....Performing Full Feed", eLogLevel.eInfo);
                }
                else if (myXml.SelectSingleNode("/gsafeed/header/feedtype").InnerText.ToLower() == GSAFeedType.FEEDTYPE_META)
                {
                    feedtype = GSAFeedType.FEEDTYPE_META;
                    Log(".....Performing Metafeed", eLogLevel.eInfo);
                }
                else
                {
                    feedtype = GSAFeedType.FEEDTYPE_INCREMENTAL;
                    Log(".....Performing Incremental Feed", eLogLevel.eInfo);
                }


                feedtype = "feedtype=" + feedtype;

                datasource = "datasource=" + myXml.SelectSingleNode("/gsafeed/header/datasource").InnerText; ;
                string data = "data=" + filedata;

                if (ssl == true)
                {
                    gsaFeedURLPrimary = "https://" + gsaFeedURLPrimary + ":19900/xmlfeed";
                }
                else
                {
                    gsaFeedURLPrimary = "http://" + gsaFeedURLPrimary + ":19900/xmlfeed";
                }
                // concatenate paramters
                char amp = '&';
                string feed = feedtype + amp + datasource + amp + data;
                // create byte array of data to post
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(feed);
                //byte[] bytes = new byte[TMPbytes.Length + arrBuffer.Length];
                //Array.Copy(TMPbytes, bytes, TMPbytes.Length);
                //Array.Copy(arrBuffer, 0, bytes,TMPbytes.Length, arrBuffer.Length);

                log.Info("posting " + bytes.Length.ToString() + " bytes of file " + sourceURL);
                // setup http request on GSA

                bool primaryFeed = true;

                if (!gsaFeedURLPrimary.Equals(string.Empty))
                {
                    try
                    {
                        primaryFeed = PostToGSA(gsaFeedURLPrimary, bytes, sourceURL);
                        log.Info("...Successfully posted to " + gsaFeedURLPrimary);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Feed to " + gsaFeedURLPrimary + " failed for " + sourceURL, ex);
                    }
                }

                return primaryFeed;
            }
            catch (Exception ex)
            {
                log.Error("GSAFeed", ex);
                if (null != fStrm) fStrm.Close();
                if (null != treader) treader.Close();
                throw ex;
            }
            finally
            {
                fStrm = null;
                treader = null;
            }
            
        }

        protected void Log(string logText, eLogLevel logLevel)
        {
            if (performLog)
            {
                switch (logLevel)
                {
                    case eLogLevel.eInfo:
                        log.Info(logText);
                        break;
                    case eLogLevel.eError:
                        log.Error(logText);
                        break;
                    case eLogLevel.eFatal:
                        log.Fatal(logText);
                        break;
                    default:
                        //do nothing
                        break;
                }

            }
        }

    
	}

    
}
