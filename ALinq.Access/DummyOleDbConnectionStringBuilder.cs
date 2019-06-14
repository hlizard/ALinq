using System;

namespace ALinq.Access
{
	public class OleDbConnectionStringBuilder
	{
		public OleDbConnectionStringBuilder ()
		{
		}

		public OleDbConnectionStringBuilder (string str)
		{
		}

		public string DataSource { get; set;}
		public string Provider { get; set;}
		public string ConnectionString { get; set;}
	}
}

