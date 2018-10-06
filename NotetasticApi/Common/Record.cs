using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace NotetasticApi.Common
{
	public class Record
	{
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string Id { get; set; }
	}
}