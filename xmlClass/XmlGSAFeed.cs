using System;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;


namespace MCPlusA.Google
{
	/// <summary>
	/// Summary description for XmlGSAFeed.
	/// 
	/// Creates an xml file according to Googles specs for a content feeder
	/// 
	/// </summary>
	public class XmlGSAFeed
	{

		protected static log4net.ILog log;
		protected XmlDocument doc = new XmlDocument();
		protected XmlNode root;
		protected XmlNode groupNode;		//node containing all recode nodes (content)
		protected string feederFolder = string.Empty;
		protected string dataSource = string.Empty;
		protected string feederType = string.Empty;
		protected int count = 0;

		#region "properties"
		
		public int Count
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
			}
		}

		public string DataSource
		{
			get{return dataSource;}
			set{dataSource = value;}
		}	
		public log4net.ILog Log
		{
			get{return log;}
			set{log = value;}
		}	
		#endregion

		public XmlGSAFeed()
		{

		}

        
		public XmlGSAFeed(log4net.ILog _log, string xmlFileFolder, string feedtype) 
		{
			log = _log;
			feederFolder = xmlFileFolder;
			feederType = feedtype;
		}
		
		public void BuildHeader()
		{
			try
			{
				//
				string test = "<?xml version='1.0' encoding='UTF-8'?><" + ConfigurationManager.AppSettings["GSADTD"] + "><gsafeed></gsafeed>";
				//set to null else .net will try to validate the xml against the dtd type and it will fail
				doc.XmlResolver = null;
				doc.LoadXml(test);
				
				
				root = doc.DocumentElement;

				//create header node
				XmlNode headerNode = doc.CreateNode(XmlNodeType.Element, "header", "");
				
				XmlElement datasrc  = doc.CreateElement("datasource");
				//build	 the GSA xml datashource from 
				//extract all no alphanumerics for datasource as some char can cause
				//the GSA to barf!!!
				
				Regex stripper = new Regex("[^a-zA-Z0-9]");
				datasrc.InnerText = HttpUtility.UrlEncode( stripper.Replace(DataSource, "" ));

				XmlElement feedtype = doc.CreateElement("feedtype");
				feedtype.InnerText = feederType;

				//insert data source and feedtype nodes into the neader node
				headerNode.InsertAfter(datasrc, headerNode.FirstChild);
				headerNode.InsertAfter(feedtype, headerNode.LastChild);

				//insert header into document
				root.InsertAfter(headerNode, root.FirstChild);

				//create the group node to contain our content records
				groupNode = doc.CreateNode(XmlNodeType.Element, "group", "");
				//create group attributes
				//XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, "lock", "");
				//attr.Value = "1";

				//Add the attribute to the document.
				//groupNode.Attributes.SetNamedItem(attr);

				//insert group into document
				root.InsertAfter(groupNode, root.FirstChild);
			}
			catch (Exception ex)
			{
				log.Error("Error  XmlGSAFeed.BuildHeader()");
				log.Error(ex.Message);
				log.Error(ex.StackTrace);
			}
		}

		public void WriteXMLToConsole ()
		{
			doc.Save(Console.Out);
		}


		/// <summary>
		/// Write contents of the xml document to disk
		/// The file name will be appended with the current time in ticks to make a unique
		/// file name. 
		/// Return the file name of file.
		/// </summary>
		/// <returns></returns>
		public string WriteXMLToFile ()
		{
			string filename = string.Empty;
			try
			{
				filename = feederFolder +"\\GSA" + DateTime.Now.Ticks+ ".xml";
				XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
				writer.Formatting = Formatting.Indented;
				doc.WriteTo( writer );
				writer.Flush();
				writer.Close();
			}
			catch (Exception ex)
			{
				log.Error("Error  XmlGSAFeed.WriteXMLToFile()");
				log.Error(ex.Message);
				log.Error(ex.StackTrace);
			}

			return filename;
		}

        /// <summary>
        /// Write contents of the xml document to disk
        /// The file name will be appended with the current time in ticks to make a unique
        /// file name. 
        /// Return the file name of file.
        /// </summary>
        /// <returns></returns>
        public string WriteXMLToFile(String uniqueId)
        {
            string filename = string.Empty;
            try
            {
                filename = feederFolder + "\\GSA" + uniqueId + DateTime.Now.Ticks + ".xml";
                XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                doc.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error  XmlGSAFeed.WriteXMLToFile()");
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
            }

            return filename;
        }

        public void DeleteRecord(string url)
        {
            try
            {
                //XmlNode groupNode = doc.CreateNode(XmlNodeType.Element, "group", "");
                //create the group node to contain our content records
                //insert group into document
                root.InsertAfter(groupNode, root.LastChild);
                XmlNode recordNode = doc.CreateNode(XmlNodeType.Element, "record", "");
                recordNode.Attributes.SetNamedItem(MakeRecordAttributenode("url", url));
                recordNode.Attributes.SetNamedItem(MakeRecordAttributenode("action", "delete"));
                recordNode.Attributes.SetNamedItem(MakeRecordAttributenode("mimetype", "text/html"));
                //insert recordNode into GroupNode
                this.groupNode.InsertAfter(recordNode, groupNode.FirstChild);
                //insert groupNode into document
                count++;
            }
            catch (Exception ex)
            {
                log.Error("Error  XmlGSAFeed.DeleteRecord()");
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
            }
        }

        private XmlNode MakeRecordAttributenode(string name, string val)
        {
            //lastmodified date attribute
            XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, name, "");
            attr.Value = val;
            //recordNode.Attributes.SetNamedItem( attrLDate );
            return attr;
        }

		public void AddRecord ( GSAContentItem item )
		{
			try
			{
				XmlNode recordNode = doc.CreateNode(XmlNodeType.Element, "record", "");
				//create group attributes
				XmlNode attrURL = doc.CreateNode(XmlNodeType.Attribute, "url", "");
				attrURL.Value = item.URL;
				//Add the attribute to the document.
				recordNode.Attributes.SetNamedItem( attrURL );

                if (item.DisplayURL != String.Empty)
                {
                    XmlNode attrDisUrl = doc.CreateNode(XmlNodeType.Attribute, "displayurl", "");
                    attrDisUrl.Value = item.DisplayURL;
                    //Add the attribute to the document.
                    recordNode.Attributes.SetNamedItem(attrDisUrl);

                }

				XmlNode attrMime = doc.CreateNode(XmlNodeType.Attribute, "mimetype", "");
				attrMime.Value = item.MimeType;
				recordNode.Attributes.SetNamedItem( attrMime );

				XmlNode attrModified = doc.CreateNode(XmlNodeType.Attribute, "last-modified", "");
				attrModified.Value = item.LastModifiedUTC();
				recordNode.Attributes.SetNamedItem( attrModified );

				XmlNode attrAuthMethod = doc.CreateNode(XmlNodeType.Attribute, "authmethod","");
				attrAuthMethod.Value = item.GSAAuthMethod.ToString();
				recordNode.Attributes.SetNamedItem( attrAuthMethod);

				if (item.Metadata.Count>0)
				{
					XmlNode metaData = doc.CreateNode(XmlNodeType.Element,"metadata","");
					GSAMetadataItem metaItem;
					for (int i=0; i<item.Metadata.Count;i++)
					{
						metaItem = (GSAMetadataItem)item.Metadata[i];
						if ((metaItem.Name.Length > 0) && (metaItem.Value.Length > 0))
						{
							XmlNode meta = doc.CreateNode(XmlNodeType.Element,"meta","");
							XmlNode metaAttribute = doc.CreateNode(XmlNodeType.Attribute,"name","");
							metaAttribute.Value = HttpUtility.HtmlEncode(metaItem.Name);
							meta.Attributes.SetNamedItem(metaAttribute);
							metaAttribute = doc.CreateNode(XmlNodeType.Attribute,"content","");
							metaAttribute.Value = HttpUtility.HtmlEncode(metaItem.Value);
							meta.Attributes.SetNamedItem(metaAttribute);
							metaData.AppendChild(meta);
						}
					}
					recordNode.AppendChild(metaData);

				}

				
				if (item.Content.Length>0)
				{
					//Assume content feed and add the content node
					XmlNode contentNode = doc.CreateNode(XmlNodeType.Element, "content", "");
                    if (item.Base64)
                    {
                        //Content is base64encoded
                        XmlNode base64 = doc.CreateNode(XmlNodeType.Attribute, "encoding", "");
                        base64.Value = "base64binary";
                        contentNode.Attributes.SetNamedItem(base64);
                    }
					//Add content and remove escape characters
					contentNode.InnerText = item.Content;//.Replace("&#30;","").Replace("&#28;","");
					recordNode.InsertAfter(contentNode, recordNode.FirstChild);
				} 

				//insert recordNode into groupNode
				groupNode.InsertAfter(recordNode, groupNode.LastChild);
			}
			catch (Exception ex)
			{
				log.Error("Error  XmlGSAFeed.AddRecord()");
				log.Error(ex.Message);
				log.Error(ex.StackTrace);
			}
            this.count++;
		}
	}


}
