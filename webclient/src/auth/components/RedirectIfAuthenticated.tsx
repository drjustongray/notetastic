import React from "react"
import { Subscription } from "rxjs"
import { AuthContext } from "../context";
import { AuthService, AuthState } from "../authService";
import { Redirect } from "react-router-dom";

export default class extends React.Component<{ to: string }, { redirect: boolean }> {
	static contextType = AuthContext
	subscription: Subscription | undefined

	constructor(props: any) {
		super(props)
		this.update = this.update.bind(this)
		this.state = { redirect: false }
	}

	componentDidMount() {
		const authService = this.context as AuthService
		this.subscription = authService.authState.subscribe(this.update)
	}

	componentWillUnmount() {
		if (this.subscription) {
			this.subscription.unsubscribe()
		}
	}

	update(authState: AuthState) {
		if (authState.user) {
			this.setState({ redirect: true })
		}
	}

	render() {
		if (this.state.redirect) {
			return <Redirect to={this.props.to} />
		}
		return null
	}
}