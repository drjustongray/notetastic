import * as React from "react"
import { storiesOf } from '@storybook/react';
import Loading from "./Loading"

const stories = storiesOf("Loading", module)

stories.add(
	"Standard",
	() => <Loading />
)