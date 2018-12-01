import * as React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import Form, { AuthFunction } from "./AuthForm";
import { withKnobs, text } from "@storybook/addon-knobs";

const stories = storiesOf("AuthForm", module);
stories.addDecorator(withKnobs);

const successfulLogin: AuthFunction = (username, password, rememberMe) => {
	action("login")(username, password, rememberMe);
	return Promise.resolve();
};

const failedLogin: AuthFunction = (username, password, rememberMe) => {
	action("login")(username, password, rememberMe);
	return Promise.reject({ message: "Sad" });
};

const successfulRegister: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe);
	return Promise.resolve();
};

const failedRegister: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe);
	return Promise.reject({ message: text("Error Message", ""), username: text("Username Error", ""), password: text("Password Error", "") });
};

stories.add(
	"With Successful Login",
	() => <Form action={successfulLogin} title="Login" />
);

stories.add(
	"With Failed Login",
	() => <Form action={failedLogin} title="Login" />
);
stories.add(
	"With Successful Register",
	() => <Form action={successfulRegister} title="Register" />
);

stories.add(
	"With Failed Register",
	() => <Form action={failedRegister} title="Register" />
);
