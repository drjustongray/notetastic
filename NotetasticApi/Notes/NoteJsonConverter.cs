using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NotetasticApi.Notes
{
	public class NoteJsonConverter : JsonConverter<Note>
	{
		public const string TypeKey = "Type";
		public override Note ReadJson(JsonReader reader, Type objectType, Note existingValue, bool hasExistingValue, JsonSerializer serializer)
		{

			var jsonObject = JObject.Load(reader);
			if (jsonObject.ContainsKey(TypeKey))
				switch (jsonObject[TypeKey].Value<string>())
				{
					case nameof(Bookmark):
						return jsonObject.ToObject<Bookmark>();
					case nameof(Checklist):
						return jsonObject.ToObject<Checklist>();
					case nameof(Location):
						return jsonObject.ToObject<Location>();
					case nameof(Notebook):
						return jsonObject.ToObject<Notebook>();
					case nameof(TextNote):
						return jsonObject.ToObject<TextNote>();
				}
			return null;
		}

		public override void WriteJson(JsonWriter writer, Note value, JsonSerializer serializer)
		{
			var jsonObject = JObject.FromObject(value);
			jsonObject.Remove(nameof(Note.IsValid));
			jsonObject[TypeKey] = value.GetType().Name;
			jsonObject.WriteTo(writer);
		}
	}
}