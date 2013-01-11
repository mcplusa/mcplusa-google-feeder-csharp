using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;

using log4net;

namespace MCPlusA.Google
{
    public class FeedUtil
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger("fileappender");

        public static string GetDataFromURL(string url)
        {
            return _GetEncodedDataFromURL(false, url);
        }

        public static string GetEncodedDataFromURL(string url, ILog log)
        {
            return _GetEncodedDataFromURL(true, url);
        }

        public static string GetEncodedDataFromURL(string url, ILog log,NetworkCredential cred)
        {
            return _GetEncodedDataFromURL(true, url,cred);
        }

        /// <summary>
        /// get data from a URL
        /// return base 64 encoded or plain text in string
        /// </summary>
        /// <param name="encodeData"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string _GetEncodedDataFromURL(bool encodeData, string url,NetworkCredential cred)
        {
            log.Info(" GetEncodedDataFromURL -> URL " + url);
            byte[] arrBuffer = new byte[0];
            try
            {
                HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(url);
                HttpWReq.Credentials = cred;
                HttpWReq.KeepAlive = true;
                HttpWReq.Accept = "*/*";
                HttpWReq.Headers.Add("Accept-Language", "en-us");
                //HttpWReq.Headers.Add("Accept-Encoding", "gzip, deflate");
                HttpWReq.CookieContainer = new CookieContainer();
                HttpWReq.AllowAutoRedirect = true;
                HttpWReq.UnsafeAuthenticatedConnectionSharing = true;
                HttpWReq.MaximumAutomaticRedirections = 80;
                HttpWReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; InfoPath.1; .NET CLR 2.0.50727)";
                HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

                try
                {
                    if (!encodeData)
                    {
                        Stream receiveStream = HttpWResp.GetResponseStream();
                        StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                        log.Error("status of request " + HttpWResp.StatusCode + " " + HttpWResp.StatusDescription);
                        return readStream.ReadToEnd();
                    }
                    else
                    {

                        using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                        {
                            byte[] arrScratch = null;
                            while ((arrScratch = reader.ReadBytes(81920)).Length > 0)
                            {
                                if (arrBuffer.Length == 0)
                                {
                                    arrBuffer = arrScratch;
                                }
                                else
                                {
                                    byte[] arrTemp = new byte[arrBuffer.Length + arrScratch.Length];
                                    Array.Copy(arrBuffer, arrTemp, arrBuffer.Length);
                                    Array.Copy(arrScratch, 0, arrTemp, arrBuffer.Length, arrScratch.Length);
                                    arrBuffer = arrTemp;
                                }
                            }
                            reader.Close();
                        }
                    }
                }
                catch (WebException webEx)
                {
                    log.Error(" GetEncodedDataFromURL from url: " + url);
                    log.Error(" GetEncodedDataFromURL ", webEx);
                    log.Error(webEx.StackTrace);
                }
                catch (Exception ex)
                {
                    log.Error(" GetEncodedDataFromURL ", ex);
                    log.Error(ex.StackTrace);
                }
                finally
                {
                    HttpWResp.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(" GetEncodedDataFromURL ", ex);
                log.Error(ex.StackTrace);
            }

            return Convert.ToBase64String(arrBuffer);
        }


        /// <summary>
        /// get data from a URL
        /// return base 64 encoded or plain text in string
        /// </summary>
        /// <param name="encodeData"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string _GetEncodedDataFromURL(bool encodeData, string url)
        {
            log.Info(" GetEncodedDataFromURL -> URL " + url);
            byte[] arrBuffer = new byte[0];
            try
            {
                HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(url);
                HttpWReq.Credentials = CredentialCache.DefaultCredentials;
                HttpWReq.KeepAlive = true;
                HttpWReq.Accept = "*/*";
                HttpWReq.Headers.Add("Accept-Language", "en-us");
                //HttpWReq.Headers.Add("Accept-Encoding", "gzip, deflate");
                HttpWReq.CookieContainer = new CookieContainer();
                HttpWReq.AllowAutoRedirect = true;
                HttpWReq.UnsafeAuthenticatedConnectionSharing = true;
                HttpWReq.MaximumAutomaticRedirections = 80;
                HttpWReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; InfoPath.1; .NET CLR 2.0.50727)";
                HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

                try
                {
                    if (!encodeData)
                    {
                        Stream receiveStream = HttpWResp.GetResponseStream();
                        StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                        log.Error("status of request " + HttpWResp.StatusCode + " " + HttpWResp.StatusDescription);
                        return readStream.ReadToEnd();
                    }
                    else
                    {

                        using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                        {
                            byte[] arrScratch = null;
                            while ((arrScratch = reader.ReadBytes(81920)).Length > 0)
                            {
                                if (arrBuffer.Length == 0)
                                {
                                    arrBuffer = arrScratch;
                                }
                                else
                                {
                                    byte[] arrTemp = new byte[arrBuffer.Length + arrScratch.Length];
                                    Array.Copy(arrBuffer, arrTemp, arrBuffer.Length);
                                    Array.Copy(arrScratch, 0, arrTemp, arrBuffer.Length, arrScratch.Length);
                                    arrBuffer = arrTemp;
                                }
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(" GetEncodedDataFromURL ", ex);
                    log.Error(ex.StackTrace);
                }
                finally
                {
                    HttpWResp.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(" GetEncodedDataFromURL ", ex);
                log.Error(ex.StackTrace);
            }

            return Convert.ToBase64String(arrBuffer);
        }

        /// <summary>
        /// provided a data stream get document contents and convert to base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetEncodedDataFromURL(Stream str)
        {
            byte[] arrBuffer = new byte[0];
            try
            {
                using (BinaryReader reader = new BinaryReader(str))
                {
                    byte[] arrScratch = null;
                    while ((arrScratch = reader.ReadBytes(81920)).Length > 0)
                    {
                        if (arrBuffer.Length == 0)
                        {
                            arrBuffer = arrScratch;
                        }
                        else
                        {
                            byte[] arrTemp = new byte[arrBuffer.Length + arrScratch.Length];
                            Array.Copy(arrBuffer, arrTemp, arrBuffer.Length);
                            Array.Copy(arrScratch, 0, arrTemp, arrBuffer.Length, arrScratch.Length);
                            arrBuffer = arrTemp;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(" GetEncodedDataFromURL ", ex);
                log.Error(ex.StackTrace);
                throw new Exception("Unable to stream document from repository");
            }
            finally
            {
                str.Close();
            }

            return Convert.ToBase64String(arrBuffer);
        }

        public static string GetFile(string fileLoc, ILog log)
        {
            FileStream str = new FileStream(fileLoc, FileMode.Open);

            byte[] arrBuffer = new byte[0];
            try
            {
                using (BinaryReader reader = new BinaryReader(str))
                {
                    byte[] arrScratch = null;
                    while ((arrScratch = reader.ReadBytes(81920)).Length > 0)
                    {
                        if (arrBuffer.Length == 0)
                        {
                            arrBuffer = arrScratch;
                        }
                        else
                        {
                            byte[] arrTemp = new byte[arrBuffer.Length + arrScratch.Length];
                            Array.Copy(arrBuffer, arrTemp, arrBuffer.Length);
                            Array.Copy(arrScratch, 0, arrTemp, arrBuffer.Length, arrScratch.Length);
                            arrBuffer = arrTemp;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(" GetEncodedDataFromURL ", ex);
                log.Error(ex.StackTrace);
                throw new Exception("Unable to stream document from repository");
            }
            finally
            {
                str.Close();
            }

            return Convert.ToBase64String(arrBuffer);
        }

        public static bool IsExcludedDocType(string docName)
        {
            bool retVal = false;

            int index = docName.LastIndexOf(".");
            string docType = docName.Substring(index + 1).ToLower();
            string excludedTypes = ConfigurationManager.AppSettings["excludeType"];
            if (excludedTypes.IndexOf(docType) >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public static string GetMimeType(string docName)
        {
            ArrayList docMime = new ArrayList();
            string tmp = ConfigurationManager.AppSettings["mimetype"];
            string delimStr = "|";
            char[] delimiter = delimStr.ToCharArray();
            string[] mimeTmp = tmp.Split(delimiter);
            foreach (string str in mimeTmp)
            {
                mimeTypes tmpTok = new mimeTypes(str);
                docMime.Add(tmpTok);
            }

            //sort the mime types
            docMime.Sort(new mimeTypeSort());
            //extract the document extension
            int index = docName.LastIndexOf(".");
            string myMimeType = docName.Substring(index + 1);
            mimeTypes x = new mimeTypes(";" + myMimeType + ";");
            index = docMime.BinarySearch((object)x, new mimeTypeSort());
            if (index < 0)
            {
                //nothing found to match
                return "text/plain";
            }
            mimeTypes mimeToUse = new mimeTypes();
            mimeToUse = (mimeTypes)docMime[index];
            return mimeToUse.mime;
        }

        public static string StripHtml(string source)
        {
            return Regex.Replace(source, @"<(.|\n)*?>", string.Empty);
        }
    }

    class mimeTypes
    {
        string doctype = string.Empty;
        public string ext = string.Empty;
        public string mime = string.Empty;
        public mimeTypes() { }
        public mimeTypes(string str)
        {
            string delimStr = ";";
            char[] delimiter = delimStr.ToCharArray();
            string[] toks = str.Split(delimiter);
            doctype = toks[0];
            ext = toks[1];
            mime = toks[2];
        }
    }


    class mimeTypeSort : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            string xStr = ((mimeTypes)x).ext;
            string yStr = ((mimeTypes)y).ext;
            return String.Compare(xStr, yStr);
        }
    }
}
