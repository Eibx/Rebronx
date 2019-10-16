using System;
using System.Data.Common;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Data;

namespace Rebronx.Server.Services
{
    public class DatabaseService : IDatabaseService
    {
        private const string ConnectionString = "Host=localhost;Username=rebronx_role;Database=rebronx_database;";

        private readonly NpgsqlConnection conn;

        public DatabaseService()
        {
            conn = new NpgsqlConnection(ConnectionString);
        }

        public NpgsqlConnection GetConnection()
        {
            return conn;
        }
    }
}
