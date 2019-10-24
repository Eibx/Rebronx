using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Configuration;
using Npgsql;
using System.Linq;
using System.Data;

namespace Rebronx.Server.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly NpgsqlConnection _conn;

        public DatabaseService()
        {
            _conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
        }

        public NpgsqlConnection GetConnection()
        {
            return _conn;
        }
    }
}
