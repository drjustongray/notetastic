using NotetasticApi.Notes;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_Constructor : NoteRepository_Base
	{
		public NoteRepository_Constructor(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void DoesNotAllowCreatingAdditionalRoot(string uid)
		{
			await Assert.ThrowsAnyAsync<MongoDB.Driver.MongoWriteException>(
				() => collection.InsertOneAsync(new NoteBook { UID = uid, IsRoot = true })
			);
			AssertCollectionEquals();
		}
	}
}