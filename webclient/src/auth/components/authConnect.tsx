import React from "react"
import { Subscription } from "rxjs"
import isEqual from "lodash/isEqual"
import { AuthContext } from "../context";
import { AuthService, AuthState } from "../authService";

export type MapFunction<M extends {}> = (authState: AuthState) => M

export default function <M extends {}>(mapFunction: MapFunction<M>) {
	return function <T>(WrappedComponent: React.ComponentType<T & M>): React.ComponentClass<T> {
		return class extends React.Component<T> {
			static contextType = AuthContext
			subscription: Subscription | undefined
			mappedAuthState: M

			constructor(props: any) {
				super(props)
				this.update = this.update.bind(this)
				this.mappedAuthState = mapFunction({ unknown: true })
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
				const mappedAuthState = mapFunction(authState)
				if (!isEqual(this.mappedAuthState, mappedAuthState)) {
					this.mappedAuthState = mappedAuthState
					this.forceUpdate()
				}
			}

			render() {
				return <WrappedComponent {...this.mappedAuthState} {...this.props} />
			}
		}
	}
}