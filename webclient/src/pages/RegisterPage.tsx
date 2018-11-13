import React from "react"
import RedirectIfAuthenticated from "../auth/components/RedirectIfAuthenticated";
import { NOTES } from "./links";
import { AuthContext } from "../auth/context";
import AuthForm from "../auth/components/AuthForm";
import { AuthService } from "../auth/authService";

export default class extends React.Component {
	static contextType = AuthContext
	render() {
		const { register } = this.context as AuthService
		return (
			<React.Fragment>
				<RedirectIfAuthenticated to={NOTES} />
				<AuthForm action={register} title="Register" />
			</React.Fragment>
		)
	}
}