using System;

namespace MCPlusA.Google
{
	/// <summary>
	/// Summary description for GSAMetadataItem.
	/// </summary>
	public class GSAMetadataItem
	{
		private string _Name = string.Empty;
		private string _Value = string.Empty;

		public GSAMetadataItem()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public override bool Equals(object test) {
            GSAMetadataItem item = (GSAMetadataItem)test;
            if ((item.Name.Equals(this.Name)) && (item.Value.Equals(this.Value)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public GSAMetadataItem(string name, string metavalue)
		{
			_Name = name;
			_Value = metavalue;
		}

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}

		}

		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
	}
}
