import React from "react"
import { Link } from "react-router-dom";
import { INDEX, LOGIN, REGISTER, LOGOUT, ACCOUNT } from "../pages/links";
import { AuthContext } from "../auth/context";
import { Subscription } from "rxjs";
import { AuthService, AuthState } from "../auth/authService";
import User from "../auth/User";
import authConnect from "../auth/components/authConnect";

export interface LoggedInBarProps {
	username: string
}

export const LoggedInBar = ({ username }: LoggedInBarProps) => (
	<div>
		<Link to={ACCOUNT}>{username}</Link>
		<Link to={LOGOUT}>Log Out</Link>
	</div>
)

export const LoggedOutBar = () => (
	<div>
		<Link to={LOGIN}>Log In</Link>
		<Link to={REGISTER}>Register</Link>
	</div>
)

interface AppBarButtonsProps {
	username?: string
}

function mapAuthStateToProps(authState: AuthState) {
	return {
		username: authState.user && authState.user.username
	}
}

function renderAppBarButtons({ username }: AppBarButtonsProps) {
	if (username) {
		return <LoggedInBar username={username} />
	}
	return <LoggedOutBar />
}

export const AppBarButtons = authConnect(mapAuthStateToProps)<{}>(renderAppBarButtons)

export default () => (
	<nav>
		<Link to={INDEX}>Notetastic!</Link>
		<AppBarButtons />
	</nav>
)