import React from "react";
import { storiesOf } from "@storybook/react";

import Error from "./Error";

const stories = storiesOf("Error", module);

stories.add(
	"Standard",
	() => <Error message="Something bad happened" />
);