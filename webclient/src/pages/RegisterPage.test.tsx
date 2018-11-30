import React from "react";
import TestRenderer from "react-test-renderer";
import { BrowserRouter } from "react-router-dom";

import RegisterPage from "./RegisterPage";
import { BehaviorSubject } from "rxjs";
import { AuthState, AuthService } from "../auth/authService";
import { AuthContext } from "../auth/context";
import RedirectIfAuthenticated from "../auth/components/RedirectIfAuthenticated";
import { NOTES } from "./links";
import AuthForm from "../auth/components/AuthForm";
const { Provider } = AuthContext;

describe("RegisterPage", () => {
	it("renders RedirectIfAuthenticated and AuthForm with correct props", () => {
		const authState = new BehaviorSubject<AuthState>({ unknown: true });
		const register = () => undefined;
		const authService = { authState, register } as unknown as AuthService;
		const root = <BrowserRouter><Provider value={authService}><RegisterPage /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		expect(testRenderer.root.findByType(RedirectIfAuthenticated).props).toMatchObject({ to: NOTES });
		expect(testRenderer.root.findByType(AuthForm).props).toEqual({ action: register, title: "Register" });
	});
});