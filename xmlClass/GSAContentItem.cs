using System;
using System.Collections;
using System.Configuration;

using MCPlusA.Google.Constants;

namespace MCPlusA.Google
{
	/// <summary>
	/// Summary description for GSAContentItem.
	/// contains elements of data necesary to create a GSA feeder content item record
	/// </summary>
	public class GSAContentItem
	{
		
		private string		url			= string.Empty;
        private string      displayurl  = string.Empty;
		private string		mimetype	= string.Empty;
		private string		content		= string.Empty;
		private bool		base64 = false;
		private DateTime	lastModified;
		private ArrayList   metaData;
		private string authMethod = GSAAuthMethodTypes.AUTH_METHOD_NONE;

		public GSAContentItem()
		{
			metaData = new ArrayList();
		}
		#region "properties"
		public string URL
		{
			get{return url;}
			set
			{
				//Removing any url encoding
				string cleanedUrl =value;
				url = cleanedUrl.Replace("%3a",":").Replace("%2f","/").Replace("+","%20").Replace("%3f","?").Replace("%3d","=");
			}
		}
        public string DisplayURL
        {
            get { return displayurl; }
            set
            {
                //Removing any url encoding
                string cleanedUrl = value;
                displayurl = cleanedUrl.Replace("%3a", ":").Replace("%2f", "/").Replace("+", "%20").Replace("%3f", "?").Replace("%3d", "=");
            }
        }
		public string MimeType
		{
			get{return mimetype;}
			set{mimetype = value;}
		}	
		public string Content
		{
			get{return content;}
			set{content = value;}
		}
		public bool Base64
		{
			get{return this.base64;}
			set{base64 = value;}
		}
		/// <summary>
		/// set the last mod date as DateTime using the local time zone
		/// 
		/// </summary>
		public DateTime LastModified
		{
			get{return lastModified;}
			set{lastModified = value;}
		}

		/// <summary>
		/// return last modified time as string 
		/// formated as UTC time as specified in rfc822 
		/// </summary>
		/// <returns></returns>
		public string LastModifiedUTC ()
		{
			return LastModified.ToUniversalTime().ToString("R");
		}

		public ArrayList Metadata
		{
			get
			{
				return this.metaData;
			}
		}
		
		#endregion

		public void test()
		{
			TimeZone localZone = TimeZone.CurrentTimeZone;
			//			localZone.StandardName;

		}

		public string GSAAuthMethod
		{
			get
			{
				return authMethod;
			}

			set
			{
				authMethod = value;
			}
		}


        /// <summary>
        /// Adds the metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="metavalue">The metavalue.</param>
		public bool AddMetadata(string name, string metavalue)
		{	
			GSAMetadataItem meta = new GSAMetadataItem(name,metavalue);
            return this.AddMetadata(meta);
		}

        public bool AddMetadata(GSAMetadataItem meta)
        {
            
            //Check if there is an existing name/value pair with the exact
            //match.  We are not concerned with duplicate names as there could
            //be cause for duplicate attributes.
            if (this.Metadata.Contains(meta))
            {
                //Do nothing - skip
                return false;
            }
            else
            {
                this.Metadata.Add(meta);
                return true;
            }
        }
	}
}
