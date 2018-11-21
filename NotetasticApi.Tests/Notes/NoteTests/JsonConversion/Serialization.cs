using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests.JsonConversion
{
	public class Serialization
	{
		NoteJsonConverter converter = new NoteJsonConverter();

		[Fact]
		public void SerializesBookmarks()
		{
			var id = "someid";
			var uid = "someuid";
			var archived = true;
			var title = "sometitle";
			var url = "someurl";
			var expected = new Bookmark
			{
				Id = id,
				UID = uid,
				Archived = archived,
				Title = title,
				URL = url
			};
			var json = JsonConvert.SerializeObject(expected, converter);
			//deserializing works, so testing this way
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var bookmark = Assert.IsAssignableFrom<Bookmark>(note);
			Assert.Equal(expected, bookmark);
		}

		[Fact]
		public void SerializesChecklists()
		{
			var id = "someid";
			var uid = "someuid";
			var archived = true;
			var title = "sometitle";
			var items = new List<CheckItem> { new CheckItem { Text = "grapes" }, new CheckItem { Checked = true, Text = "apples" } };
			var expected = new Checklist
			{
				Id = id,
				UID = uid,
				Archived = archived,
				Title = title,
				Items = items
			};
			var json = JsonConvert.SerializeObject(expected, converter);
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var checklist = Assert.IsAssignableFrom<Checklist>(note);
			Assert.Equal(expected, checklist);
		}

		[Fact]
		public void SerializesLocations()
		{
			var id = "someid";
			var uid = "someuid";
			var archived = true;
			var title = "sometitle";
			var lat = 89;
			var lon = -98;
			var expected = new Location
			{
				Id = id,
				UID = uid,
				Archived = archived,
				Title = title,
				Latitude = lat,
				Longitude = lon
			};
			var json = JsonConvert.SerializeObject(expected, converter);
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var location = Assert.IsAssignableFrom<Location>(note);
			Assert.Equal(expected, location);
		}

		[Fact]
		public void SerializesTextNotes()
		{
			var id = "someid";
			var uid = "someuid";
			var archived = true;
			var title = "sometitle";
			var text = "some text";
			var expected = new TextNote
			{
				Id = id,
				UID = uid,
				Archived = archived,
				Title = title,
				Text = text
			};
			var json = JsonConvert.SerializeObject(expected, converter);
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var textNote = Assert.IsAssignableFrom<TextNote>(note);
			Assert.Equal(expected, textNote);
		}

		[Fact]
		public void CamelCasesProperties()
		{
			var id = "someid";
			var uid = "someuid";
			var archived = true;
			var title = "sometitle";
			var url = "someurl";
			var expected = new Bookmark
			{
				Id = id,
				UID = uid,
				Archived = archived,
				Title = title,
				URL = url
			};
			var json = JsonConvert.SerializeObject(expected, converter);
			var jsonObject = JObject.Parse(json);
			Assert.True(jsonObject.ContainsKey("id"));
			Assert.True(jsonObject.ContainsKey("uid"));
			Assert.True(jsonObject.ContainsKey("archived"));
			Assert.True(jsonObject.ContainsKey("title"));
			Assert.True(jsonObject.ContainsKey("url"));
			Assert.True(jsonObject.ContainsKey("type"));
		}
	}
}