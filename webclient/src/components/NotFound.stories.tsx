import * as React from "react";
import { storiesOf } from "@storybook/react";
import NotFound from "./NotFound";

const stories = storiesOf("NotFound", module);

stories.add(
	"Standard",
	() => <NotFound />
);