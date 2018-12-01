import * as React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import Form, { UpdateFunction } from "./AccountUpdateForm";
import { withKnobs, text } from "@storybook/addon-knobs";

const stories = storiesOf("AccountUpdateForm", module);
stories.addDecorator(withKnobs);

const changePasswordSuccess: UpdateFunction = (password, change) => {
	action("changePassword")(password, change);
	return Promise.resolve();
};

const changePasswordFail: UpdateFunction = (password, change) => {
	action("changePassword")(password, change);
	return Promise.reject({ message: text("Error Message", ":(") });
};

const changeUsernameSuccess: UpdateFunction = (password, change) => {
	action("changeUsername")(password, change);
	return Promise.resolve();
};

const changeUsernameFail: UpdateFunction = (password, change) => {
	action("changeUsername")(password, change);
	return Promise.reject({ message: text("Error Message", ":(") });
};

interface ExampleProps {
	action: UpdateFunction;
	type: string;
	name: string;
	current?: string;
}

class Example extends React.Component<ExampleProps, { open: boolean }> {
	public state = { open: false };
	public open = () => {
		this.setState({ open: true });
	}
	public close = () => {
		this.setState({ open: false });
	}
	public render() {
		return <Form {...this.props} isOpen={this.state.open} open={this.open} close={this.close} />;
	}
}

stories.add(
	"Change Password Success",
	() => <Example action={changePasswordSuccess} type="password" name="Password" />
);

stories.add(
	"Change Password Failure",
	() => <Example action={changePasswordFail} type="password" name="Password" />
);

stories.add(
	"Change Username Success",
	() => <Example action={changeUsernameSuccess} type="text" current="bob" name="Username" />
);

stories.add(
	"Change Username Failure",
	() => <Example action={changeUsernameFail} type="text" current="bob" name="Username" />
);