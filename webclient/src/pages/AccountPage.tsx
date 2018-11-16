import React from "react"
import requireAuth from "../auth/components/requireAuth";
import { AuthContext } from "../auth/context";
import { AuthService } from "../auth/authService";
import { Link } from "react-router-dom";
import { LOGOUT_EVERYWHERE } from "./links";
import AccountUpdateForm from "../auth/components/AccountUpdateForm";

class AccountPage extends React.Component<{}, { usernameOpen: boolean, passwordOpen: boolean }> {
	static contextType = AuthContext

	constructor(props: {}) {
		super(props)
		this.state = { usernameOpen: false, passwordOpen: false }
		this.openPassword = this.openPassword.bind(this)
		this.openUsername = this.openUsername.bind(this)
		this.closePassword = this.closePassword.bind(this)
		this.closeUsername = this.closeUsername.bind(this)
	}

	openUsername() {
		this.setState({ usernameOpen: true, passwordOpen: false })
	}

	closeUsername() {
		this.setState({ usernameOpen: false })
	}

	openPassword() {
		this.setState({ usernameOpen: false, passwordOpen: true })
	}

	closePassword() {
		this.setState({ passwordOpen: false })
	}

	render() {
		const { usernameOpen, passwordOpen } = this.state
		const { changePassword, changeUsername, authState } = this.context as AuthService
		const username = authState.value.user!.username
		return (
			<div>
				<AccountUpdateForm isOpen={usernameOpen} open={this.openUsername} close={this.closeUsername} action={changeUsername} type="text" name="Username" current={username} />
				<AccountUpdateForm isOpen={passwordOpen} open={this.openPassword} close={this.closePassword} action={changePassword} type="password" name="Password" />
				<Link to={LOGOUT_EVERYWHERE}>Logout Everywhere</Link>
			</div>
		)
	}
}

export default requireAuth(AccountPage)