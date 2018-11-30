import React from "react";
import { storiesOf } from "@storybook/react";

import NoteTypeSelector from "./NoteTypeSelector";
import { action } from "@storybook/addon-actions";

const stories = storiesOf("NoteTypeSelector", module);

stories.add(
	"Standard",
	() => <NoteTypeSelector onSelection={action("Select Type")} />
);