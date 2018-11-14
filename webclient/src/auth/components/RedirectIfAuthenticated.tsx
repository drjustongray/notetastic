import React from "react"
import { AuthState } from "../authService";
import { Redirect } from "react-router-dom";
import authConnect from "./authConnect";

interface RedirectIfAuthenticatedProps {
	redirect: boolean
	to: string
}

function ConditionalRedirect({ redirect, to }: RedirectIfAuthenticatedProps) {
	return redirect ? <Redirect to={to} /> : null
}

function mapAuthToProps(authState: AuthState) {
	return {
		redirect: !!authState.user
	}
}

export default authConnect(mapAuthToProps)<{ to: string }>(ConditionalRedirect)
