using System;
using System.Data.Common;
using Rebronx.Server.Services.Interfaces;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Data;

namespace Rebronx.Server.Services
{
	public class DatabaseService : IDatabaseService
	{
		private const string connectionString = "Host=localhost;Username=rebronx;Password=test;Database=rebronx;";

		private NpgsqlConnection conn;
		
		public DatabaseService()
		{
			conn = new NpgsqlConnection(connectionString);
		}

		public void ExecuteNonQuery(string sql, Dictionary<string, object> parameters) 
		{
			conn.Close();
			conn.Open();

			using (var cmd = new NpgsqlCommand())
			{
				cmd.Connection = conn;
				cmd.CommandText = sql;
				foreach(var parameter in parameters) 
				{
					cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
				}
				cmd.ExecuteNonQuery();
			}

			conn.Close();
		}

		public IDataReader ExecuteReader(string sql, Dictionary<string, object> parameters) 
		{
			var output = new List<IDataRecord>();
			conn.Close();
			conn.Open();

			using (var cmd = new NpgsqlCommand(sql, conn))
			{
				foreach (var parameter in parameters)
				{
					cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
				}
				var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				
				return reader;
			}
		}

		public T ExecuteScalar<T>(string sql, Dictionary<string, object> parameters)
		{
			conn.Close();
			conn.Open();

			T output;

			using (var cmd = new NpgsqlCommand(sql, conn))
			{
				foreach (var parameter in parameters)
				{
					cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
				}
				
				output = (T)cmd.ExecuteScalar();
			}

			conn.Close();

			return output;
		}
	}
}
