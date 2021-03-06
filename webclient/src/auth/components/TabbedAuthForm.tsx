import React from "react";
import AuthForm, { AuthFunction } from "./AuthForm";
import styles from "./TabbedAuthForm.module.css";

export interface TabbedAuthFormProps {
	login: AuthFunction;
	register: AuthFunction;
}

interface TabbedAuthFormState {
	showRegisterForm: boolean;
}

export default class extends React.Component<TabbedAuthFormProps, TabbedAuthFormState> {
	constructor(props: TabbedAuthFormProps) {
		super(props);
		this.state = { showRegisterForm: false };
		this.showLogin = this.showLogin.bind(this);
		this.showRegister = this.showRegister.bind(this);
	}

	public showLogin() {
		if (this.state.showRegisterForm) {
			this.setState({ showRegisterForm: false });
		}
	}

	public showRegister() {
		if (!this.state.showRegisterForm) {
			this.setState({ showRegisterForm: true });
		}
	}

	public render() {
		const { showRegisterForm } = this.state;
		const { login, register } = this.props;
		const selectedTabStyle = `${styles.tab} ${styles.selected}`;
		const form = <AuthForm
			action={showRegisterForm ? register : login}
			title={showRegisterForm ? "Register" : "Login"} />;
		return (
			<div>
				<div className={styles.tabs}>
					<button className={showRegisterForm ? styles.tab : selectedTabStyle} onClick={this.showLogin}>Login</button>
					<button className={showRegisterForm ? selectedTabStyle : styles.tab} onClick={this.showRegister}>Register</button>
				</div>
				{form}
			</div>
		);
	}
}