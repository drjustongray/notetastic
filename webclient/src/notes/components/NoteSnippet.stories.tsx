import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text } from "@storybook/addon-knobs";

import NoteSnippet from "./NoteSnippet";
import { HashRouter } from "react-router-dom";
import { action } from "@storybook/addon-actions";

const stories = storiesOf("NoteSnippet", module);

stories.addDecorator(withKnobs);

stories.add(
	"Standard List",
	() => (
		<HashRouter>
			<div>
				{Array.from({ length: 7 }, (_, i) => <NoteSnippet id={i + ""} key={i} title={text(`Title ${i}`, i + "")} type={text(`Type ${i}`, i + "")} />)}
			</div>
		</HashRouter>
	)
);