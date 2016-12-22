using System;
using Rebronx.Server.Services.Interfaces;
using StackExchange.Redis;

namespace Rebronx.Server.Services
{
	public class DatabaseService : IDatabaseService
	{
		private readonly IConnectionMultiplexer connectionMultiplexer;
		public DatabaseService()
		{
			try {
				connectionMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
			}
			catch (Exception e)
			{
				Console.WriteLine("Couldn't connect to database!");
				throw e;
			}
			
		}

		public IDatabase GetDatabase() 
		{
			return connectionMultiplexer.GetDatabase();
		}
	}
}