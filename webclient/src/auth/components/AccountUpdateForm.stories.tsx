import * as React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import Form, { UpdateFunction } from "./AccountUpdateForm"

const stories = storiesOf("AccountUpdateForm", module)

const changePasswordSuccess: UpdateFunction = (password, change) => {
	action("changePassword")(password, change)
	return Promise.resolve()
}

const changePasswordFail: UpdateFunction = (password, change) => {
	action("changePassword")(password, change)
	return Promise.reject({ message: "something went wrong!" })
}

const changeUsernameSuccess: UpdateFunction = (password, change) => {
	action("changeUsername")(password, change)
	return Promise.resolve()
}

const changeUsernameFail: UpdateFunction = (password, change) => {
	action("changeUsername")(password, change)
	return Promise.reject({ message: "something went wrong!" })
}

interface ExampleProps {
	action: UpdateFunction
	type: string
	name: string
	current?: string
}

class Example extends React.Component<ExampleProps, { open: boolean }> {
	state = { open: false }
	open = () => {
		this.setState({ open: true })
	}
	close = () => {
		this.setState({ open: false })
	}
	render() {
		return <Form {...this.props} isOpen={this.state.open} open={this.open} close={this.close} />
	}
}

stories.add(
	"Change Password Success",
	() => <Example action={changePasswordSuccess} type="password" name="Password" />
)

stories.add(
	"Change Password Failure",
	() => <Example action={changePasswordFail} type="password" name="Password" />
)

stories.add(
	"Change Username Success",
	() => <Example action={changeUsernameSuccess} type="text" current="bob" name="Username" />
)

stories.add(
	"Change Username Failure",
	() => <Example action={changeUsernameFail} type="text" current="bob" name="Username" />
)