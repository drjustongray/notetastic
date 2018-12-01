import React from "react";
import { Link } from "react-router-dom";
import { INDEX, LOGIN, REGISTER, LOGOUT, ACCOUNT } from "../pages/links";
import { AuthState } from "../auth/authService";
import authConnect from "../auth/components/authConnect";
import styles from "./AppBar.module.css";

export interface LoggedInBarProps {
	username: string;
}

export const LoggedInBar = () => (
	<React.Fragment>
		<Link className={styles.link} to={INDEX}>My Notes</Link>
		<div>
			<Link className={styles.link} to={ACCOUNT}>My Account</Link>
			<Link className={styles.link} to={LOGOUT}>Log Out</Link>
		</div>
	</React.Fragment>
);

export const LoggedOutBar = () => (
	<React.Fragment>
		<Link className={styles.link} to={INDEX}>Notetastic!</Link>
		<div>
			<Link className={styles.link} to={LOGIN}>Log In</Link>
			<Link className={styles.link} to={REGISTER}>Register</Link>
		</div>
	</React.Fragment>
);

interface AppBarButtonsProps {
	loggedIn?: boolean;
}

function mapAuthStateToProps(authState: AuthState) {
	return {
		loggedIn: !!authState.user
	};
}

function renderAppBarButtons({ loggedIn }: AppBarButtonsProps) {
	if (loggedIn) {
		return <LoggedInBar />;
	}
	return <LoggedOutBar />;
}

export const AppBarButtons = authConnect(mapAuthStateToProps)<{}>(renderAppBarButtons);

export default () => (
	<nav className={styles.bar}>
		<AppBarButtons />
	</nav>
);