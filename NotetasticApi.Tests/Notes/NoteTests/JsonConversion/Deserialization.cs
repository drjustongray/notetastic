using Xunit;
using Newtonsoft.Json;
using NotetasticApi.Notes;
using NotetasticApi.Users;
using System.Collections.Generic;

namespace NotetasticApi.Tests.Notes.NoteTests.JsonConversion
{
	public class Deserialization
	{
		NoteJsonConverter converter = new NoteJsonConverter();

		[Fact]
		public void HandlesMissingType()
		{
			var note = JsonConvert.DeserializeObject<Note>("{}", converter);
			Assert.Null(note);
		}

		[Fact]
		public void HandlesTypeNonString()
		{
			var note = JsonConvert.DeserializeObject<Note>("{'Type': 8}", converter);
			Assert.Null(note);
		}

		[Fact]
		public void DeserializesBookmarks()
		{
			var id = "someid";
			var uid = "someuid";
			var nbid = "somenbid";
			var archived = true;
			var title = "sometitle";
			var url = "someurl";
			var expected = new Bookmark
			{
				Id = id,
				UID = uid,
				NBID = nbid,
				Archived = archived,
				Title = title,
				URL = url
			};
			var json = $"{{ 'Id': '{id}', 'Type': 'Bookmark', 'UID': '{uid}', 'NBID': '{nbid}', 'Archived': {archived.ToString().ToLower()}, 'Title': '{title}', 'URL': '{url}' }}";
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var bookmark = Assert.IsAssignableFrom<Bookmark>(note);
			Assert.Equal(expected, bookmark);
		}

		[Fact]
		public void DeserializesChecklists()
		{
			var id = "someid";
			var uid = "someuid";
			var nbid = "somenbid";
			var archived = true;
			var title = "sometitle";
			var items = new List<CheckItem> { new CheckItem { Text = "grapes" }, new CheckItem { Checked = true, Text = "apples" } };
			var expected = new Checklist
			{
				Id = id,
				UID = uid,
				NBID = nbid,
				Archived = archived,
				Title = title,
				Items = items
			};
			var json = $"{{ 'Id': '{id}', 'Type': 'Checklist', 'UID': '{uid}', 'NBID': '{nbid}', 'Archived': {archived.ToString().ToLower()}, 'Title': '{title}', 'Items': [{{'Checked': false, 'Text': 'grapes'}}, {{'Checked': true, 'Text': 'apples'}}] }}";
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var checklist = Assert.IsAssignableFrom<Checklist>(note);
			Assert.Equal(expected, checklist);
		}

		[Fact]
		public void DeserializesLocations()
		{
			var id = "someid";
			var uid = "someuid";
			var nbid = "somenbid";
			var archived = true;
			var title = "sometitle";
			var lat = 89;
			var lon = -98;
			var expected = new Location
			{
				Id = id,
				UID = uid,
				NBID = nbid,
				Archived = archived,
				Title = title,
				Latitude = lat,
				Longitude = lon
			};
			var json = $"{{ 'Id': '{id}', 'Type': 'Location', 'UID': '{uid}', 'NBID': '{nbid}', 'Archived': {archived.ToString().ToLower()}, 'Title': '{title}', 'Latitude': '{lat}', 'Longitude': '{lon}' }}";
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var location = Assert.IsAssignableFrom<Location>(note);
			Assert.Equal(expected, location);
		}

		[Fact]
		public void DeserializesNotebooks()
		{
			var id = "someid";
			var uid = "someuid";
			var nbid = "somenbid";
			var archived = true;
			var title = "sometitle";
			var count = 7;
			var expected = new NoteBook
			{
				Id = id,
				UID = uid,
				NBID = nbid,
				Archived = archived,
				Title = title,
				Count = count
			};
			var json = $"{{ 'Id': '{id}', 'Type': 'NoteBook', 'UID': '{uid}', 'NBID': '{nbid}', 'Archived': {archived.ToString().ToLower()}, 'Title': '{title}', 'Count': '{count}' }}";
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var notebook = Assert.IsAssignableFrom<NoteBook>(note);
			Assert.Equal(expected, notebook);
		}

		[Fact]
		public void DeserializesTextNotes()
		{
			var id = "someid";
			var uid = "someuid";
			var nbid = "somenbid";
			var archived = true;
			var title = "sometitle";
			var text = "some text";
			var expected = new TextNote
			{
				Id = id,
				UID = uid,
				NBID = nbid,
				Archived = archived,
				Title = title,
				Text = text
			};
			var json = $"{{ 'Id': '{id}', 'Type': 'TextNote', 'UID': '{uid}', 'NBID': '{nbid}', 'Archived': {archived.ToString().ToLower()}, 'Title': '{title}', 'Text': '{text}' }}";
			var note = JsonConvert.DeserializeObject<Note>(json, converter);
			var textNote = Assert.IsAssignableFrom<TextNote>(note);
			Assert.Equal(expected, textNote);
		}
	}
}