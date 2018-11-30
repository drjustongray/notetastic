import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { withKnobs, text, number, boolean } from "@storybook/addon-knobs";

import { TextNoteView } from "./TextNote";
import { LocationView } from "./Location";
import { BookmarkView } from "./Bookmark";
import { ChecklistView } from "./Checklist";

const stories = storiesOf("NoteViews", module);
stories.addDecorator(withKnobs);

const baseProps = () => ({
	title: text("Title", "title"),
	updateTitle: action("updateTitle"),
	archived: boolean("Archived", false),
	setArchived: action("setArchived"),
	deleteNote: action("deleteNote")
});

stories.add(
	"TextNoteView",
	() => <TextNoteView {...baseProps()} text={text("Text", "Hi")} updateText={action("updateText")} />
);

stories.add(
	"LocationView",
	() => <LocationView
		{...baseProps()}
		latitude={number("latitude", 33)}
		longitude={number("longitude", -97)}
		setToCurrentLocation={action("setToCurrentLocation")}
		setLocation={action("setLocation")}
		geoAvailable={boolean("geoAvailable", true)}
	/>
);

stories.add(
	"BookmarkView",
	() => <BookmarkView {...baseProps()} url={text("URL", "url")} updateURL={action("updateURL")} />
);

stories.add(
	"ChecklistView",
	() => <ChecklistView
		{...baseProps()}
		addItem={action("addItem")}
		removeItem={action("removeItem")}
		setItemChecked={action("setItemChecked")}
		updateItemText={action("updateItemText")}
		items={Array.from(
			{ length: number("Number of Items", 0, { min: 0, step: 1, range: true, max: 1000 }) },
			(_, i) => ({ checked: boolean(i + "checked", false), text: text(i + "text", "entry #" + i) })
		)}
	/>
);