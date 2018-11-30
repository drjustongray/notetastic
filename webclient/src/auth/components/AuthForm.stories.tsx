import * as React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import Form, { AuthFunction } from "./AuthForm";

const stories = storiesOf("AuthForm", module);

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

const failedRegister1: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe);
	return Promise.reject({ message: "Sad" });
};

const failedRegister2: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe);
	return Promise.reject({ username: "bad username! bad!" });
};

const failedRegister3: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe);
	return Promise.reject({ password: "Bad password" });
};

const failedRegister4: AuthFunction = (username, password, rememberMe) => {
	action("register")(username, password, rememberMe);
	return Promise.reject({ message: "Sad", username: "bad username! bad!", password: "Bad password" });
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
	"With Failed Register (message)",
	() => <Form action={failedRegister1} title="Register" />
);

stories.add(
	"With Failed Register (username)",
	() => <Form action={failedRegister2} title="Register" />
);

stories.add(
	"With Failed Register (password)",
	() => <Form action={failedRegister3} title="Register" />
);

stories.add(
	"With Failed Register (all)",
	() => <Form action={failedRegister4} title="Register" />
);