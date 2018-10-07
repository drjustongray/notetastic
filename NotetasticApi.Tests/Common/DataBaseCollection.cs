using System;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace NotetasticApi.Tests.Common
{
	[CollectionDefinition("Database collection")]
	public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}

	public class DatabaseFixture : IDisposable
	{
		private readonly MongoDbRunner _runner;
		public DatabaseFixture()
		{
			_runner = MongoDbRunner.Start();
			Client = new MongoClient(_runner.ConnectionString);
		}

		public MongoClient Client { get; private set; }

		public void Dispose()
		{
			_runner.Dispose();
		}
	}
}