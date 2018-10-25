using System.Threading.Tasks;
using MongoDB.Driver;

namespace NotetasticApi.Common
{
	public static class CollectionExtensions
	{
		public static async Task<TDocument> FindOneAsync<TDocument>(this IMongoCollection<TDocument> collection, FilterDefinition<TDocument> filter)
		{
			return await (await collection.FindAsync(filter)).FirstOrDefaultAsync();
		}
	}
}