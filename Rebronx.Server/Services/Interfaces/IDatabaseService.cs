using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace Rebronx.Server.Services.Interfaces
{
	public interface IDatabaseService
	{
		void ExecuteNonQuery(string sql, Dictionary<string, object> parameters);
		IDataReader ExecuteReader(string sql, Dictionary<string, object> parameters);
		T ExecuteScalar<T>(string sql, Dictionary<string, object> parameters);
	}
}
