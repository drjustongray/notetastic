import * as React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import TabbedFrom from "./TabbedAuthForm"
import { AuthFunction } from './AuthForm';

const stories = storiesOf("TabbedAuthForm", module)

const login: AuthFunction = (username, password, rememberMe) => {
	action("login")(username, password, rememberMe)
	return Promise.resolve()
}

const register: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe)
	return Promise.resolve()
}

stories.add(
	"Successful register and login",
	() => <TabbedFrom login={login} register={register} />
)