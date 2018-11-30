import React from "react";
import omit from "lodash/omit";
import Loading from "../../components/Loading";
import { AuthContext } from "../context";
import { AuthService, AuthState } from "../authService";
import TabbedAuthForm from "./TabbedAuthForm";
import authConnect from "./authConnect";

interface RequireAuthProps {
	loggedIn: boolean | undefined;
}

function mapAuthToProps(authState: AuthState): RequireAuthProps {
	const loggedIn = authState.unknown ? undefined : !!authState.user;
	return { loggedIn };
}

export default function <T extends {}>(WrappedComponent: React.ComponentType<T>): React.ComponentClass<T> {
	return authConnect(mapAuthToProps)(class extends React.Component<T & RequireAuthProps> {
		public static contextType = AuthContext;
		public renderedOnce = false;

		public render() {
			const { loggedIn } = this.props;
			if (loggedIn) {
				return <WrappedComponent {...omit(this.props, "loggedIn")} />;
			}
			if (loggedIn === undefined && !this.renderedOnce) {
				return <Loading />;
			}
			this.renderedOnce = true;
			const authService = this.context as AuthService;
			return <TabbedAuthForm login={authService.login} register={authService.register} />;
		}
	});
}