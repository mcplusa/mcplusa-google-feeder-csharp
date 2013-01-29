MC+A C# SDK for Feeding Google Search Appliance
============================

A C# Library for developing simple feeders to the Google Search Appliance.

# Usage
 
```
 log4net.ILog log;
 GSAUploader uploader = new GSAUploader(log);
 XmlGSAFeed myFeed = new XmlGSAFeed(log, ConfigurationManager.AppSettings["GSAFeedFolder"], GSAFeedType.FEEDTYPE_FULL);
 myFeed.DataSource = ConfigurationManager.AppSettings["datasource"];
 myFeed.BuildHeader();
 GSAContentItem item = new GSAContentItem();
 item.URL = "https://www.mcplusa.com/";
 item.DisplayURL = item.URL;
 item.GSAAuthMethod = "none";
 item.AddMetadata("author", "Michael Cizmar");
 item.Content = "Your html or binary here";
 myFeed.AddRecord(item);
 feedFile = myFeed.WriteXMLToFile();
 uploader.FeedXML(feedFile, ConfigurationManager.AppSettings["gsahost"], false);
 if (Boolean.Parse(ConfigurationManager.AppSettings["deletefeed"]))
 {
   File.Delete(feedFile);
   log.Info("Deleted file " + feedFile);
 }
```

# Dependencies
* Log4net

# Related Projects
[GSALib C# SDK for Query GSA](http://gsalib.codeplex.com)

Copyright 2007 Michael Cizmar + Associates Ltd.[MC+A](http://www.mcplusa.com/)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
