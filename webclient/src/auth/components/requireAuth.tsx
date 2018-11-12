import React from "react"
import { Subscription } from "rxjs"
import Loading from "../../common-components/Loading";
import { AuthContext } from "../context";
import { AuthService, AuthState } from "../authService";
import TabbedAuthForm from "./TabbedAuthForm";

interface State {
	loggedIn: boolean | undefined
}

export default () => <T extends {}>(WrappedComponent: React.ComponentType<T>) => (
	class extends React.Component<T, State> {
		static contextType = AuthContext
		subscription: Subscription | undefined

		constructor(props: any) {
			super(props)
			this.update = this.update.bind(this)
			this.state = { loggedIn: undefined }
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
			if (authState.user && !this.state.loggedIn) {
				return this.setState({ loggedIn: true })
			}
			if (!authState.unknown && this.state.loggedIn != false) {
				return this.setState({ loggedIn: false })
			}
		}

		render() {
			const { loggedIn } = this.state
			if (loggedIn) {
				return <WrappedComponent {...this.props} />
			}
			if (loggedIn == undefined) {
				return <Loading />
			}
			const authService = this.context as AuthService
			return <TabbedAuthForm login={authService.login} register={authService.register} />
		}
	}
)