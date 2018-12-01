import * as React from "react";
import { storiesOf } from "@storybook/react";
import LinkCard from "./LinkCard";
import { text, withKnobs } from "@storybook/addon-knobs";
import { BrowserRouter } from "react-router-dom";

const stories = storiesOf("LinkCard", module);
stories.addDecorator(withKnobs);

stories.add(
	"Standard",
	() => <BrowserRouter><LinkCard to={"somelink"}>{text("content", "stuff")}</LinkCard></BrowserRouter>
);