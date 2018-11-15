import React from "react"

import { AuthContext } from "../auth/context"
import { AuthService } from "../auth/authService";
import { Redirect } from "react-router-dom";
import { INDEX } from "./links";
import Error from "../components/Error";
import Loading from "../components/Loading";

interface State {
	loggedOut?: boolean
	error?: string
}

export default class extends React.Component<{}, State> {
	static contextType = AuthContext
	state: State = {}

	async componentDidMount() {
		if (!(this.context as AuthService).authState.value.user) {
			return
		}
		try {
			await (this.context as AuthService).logout()
			this.setState({ loggedOut: true })
		} catch (e) {
			this.setState({ error: e.message })
		}
	}
	render() {
		const loggedOut = this.state.loggedOut || !(this.context as AuthService).authState.value.user
		if (loggedOut) {
			return <Redirect to={INDEX} />
		}
		if (this.state.error) {
			return <Error message={this.state.error} />
		}
		return <Loading />
	}
}