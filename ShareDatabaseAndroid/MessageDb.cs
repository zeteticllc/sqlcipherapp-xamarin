using System;
using System.IO;
using Mono.Data.Sqlite;

namespace ShareDatabase
{
	public class MessageDb
	{
		public const string MIME_TYPE = "application/x-net-zetetic-messagedb";

		public string FilePath {get; set;}
		//public string Password {private get; set;}

		public MessageDb (string filePath)
		{
			FilePath = filePath;
		}

		public SqliteConnection GetConnection() 
		{
			var connection = new SqliteConnection(String.Format("Data Source={0}", FilePath));
			connection.Open();
			return connection;
		}

		public string LoadMessage() 
		{
			if(File.Exists(FilePath)) 
			{
				using(var connection = GetConnection())
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandText = "SELECT content FROM message WHERE id = 0";
						return command.ExecuteScalar() as string;
					}
				}
			}
			return null;
		}

		public void SaveMessage(string message) 
		{
			File.Delete(FilePath);
			using(var connection = GetConnection())
			{	
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "CREATE TABLE message(id INTEGER PRIMARY KEY, content TEXT)";
					command.ExecuteNonQuery();

					command.CommandText = "INSERT OR REPLACE INTO message (id, content) VALUES (0, @content)";
					var p = command.CreateParameter();
					p.ParameterName = "@content";
					p.Value = message;
					command.Parameters.Add(p);
					command.ExecuteNonQuery();
				}
			}

		}
	}
}

