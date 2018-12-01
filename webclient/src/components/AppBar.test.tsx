import React from "react";
import TestRenderer from "react-test-renderer";

import { AppBarButtons, LoggedInBar, LoggedOutBar } from "./AppBar";
import { AuthService, AuthState } from "../auth/authService";
import { BehaviorSubject } from "rxjs";
import { AuthContext } from "../auth/context";
import User from "../auth/User";
import { BrowserRouter } from "react-router-dom";
const { Provider } = AuthContext;

describe("AppBarButtons", () => {
	let authService: AuthService;
	let authState: BehaviorSubject<AuthState>;

	beforeEach(() => {
		authState = new BehaviorSubject<AuthState>({ unknown: true });
		authService = { authState } as unknown as AuthService;
	});

	it("renders without crashing", () => {
		TestRenderer.create(<BrowserRouter><Provider value={authService}><AppBarButtons /></Provider></BrowserRouter>);
	});

	it("renders LoggedOutBar when not logged in", () => {
		const root = <BrowserRouter><Provider value={authService}><AppBarButtons /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		testRenderer.root.findByType(LoggedOutBar);
		expect(testRenderer.root.findAllByType(LoggedInBar)).toHaveLength(0);
		authState.next({});
		testRenderer.update(root);
		testRenderer.root.findByType(LoggedOutBar);
		expect(testRenderer.root.findAllByType(LoggedInBar)).toHaveLength(0);
	});

	it("renders LoggedInBar when logged in", () => {
		const username = "uzorn@m3";
		authState.next({ user: new User("uid", username) });
		const root = <BrowserRouter><Provider value={authService}><AppBarButtons /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		expect(testRenderer.root.findByType(LoggedInBar));
		expect(testRenderer.root.findAllByType(LoggedOutBar)).toHaveLength(0);
	});

	it("updates as the state changes", () => {
		const root = <BrowserRouter><Provider value={authService}><AppBarButtons /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		authState.next({ user: new User("uid", "username") });
		testRenderer.update(root);
		testRenderer.root.findByType(LoggedInBar);
		expect(testRenderer.root.findAllByType(LoggedOutBar)).toHaveLength(0);
		authState.next({});
		testRenderer.update(root);
		testRenderer.root.findByType(LoggedOutBar);
		expect(testRenderer.root.findAllByType(LoggedInBar)).toHaveLength(0);
		authState.next({ unknown: true });
		testRenderer.update(root);
		testRenderer.root.findByType(LoggedOutBar);
		expect(testRenderer.root.findAllByType(LoggedInBar)).toHaveLength(0);
	});

	it("stops subscription when unmounted", () => {
		const root = <BrowserRouter><Provider value={authService}><AppBarButtons /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		expect(authState.observers).toHaveLength(1);
		testRenderer.unmount();
		expect(authState.observers).toHaveLength(0);
	});
});