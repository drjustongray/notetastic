using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace NotetasticApi.Notes
{
	public class NoteJsonConverter : JsonConverter<Note>
	{
		public const string TypeKey = "Type";
		public const string LowerTypeKey = "type";
		public static readonly JsonSerializer serializer = new JsonSerializer
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		};

		public override Note ReadJson(JsonReader reader, Type objectType, Note existingValue, bool hasExistingValue, JsonSerializer serializer)
		{

			var jsonObject = JObject.Load(reader);
			var type = jsonObject.ContainsKey(TypeKey) ? jsonObject[TypeKey].Value<string>() : jsonObject.ContainsKey(LowerTypeKey) ? jsonObject[LowerTypeKey].Value<string>() : null;
			if (type != null)
				switch (type)
				{
					case nameof(Bookmark):
						return jsonObject.ToObject<Bookmark>();
					case nameof(Checklist):
						return jsonObject.ToObject<Checklist>();
					case nameof(Location):
						return jsonObject.ToObject<Location>();
					case nameof(TextNote):
						return jsonObject.ToObject<TextNote>();
				}
			return null;
		}

		public override void WriteJson(JsonWriter writer, Note value, JsonSerializer serializer)
		{
			var jsonObject = JObject.FromObject(value, NoteJsonConverter.serializer);
			jsonObject.Remove(nameof(Note.IsValid));
			jsonObject[LowerTypeKey] = value.GetType().Name;
			jsonObject.WriteTo(writer);
		}
	}
}