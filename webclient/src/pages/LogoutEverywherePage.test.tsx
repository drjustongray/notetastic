import React from "react";
import TestRenderer from "react-test-renderer";
import { BrowserRouter, Redirect } from "react-router-dom";

import LogoutEverywherePage from "./LogoutEverywherePage";
import Error from "../components/Error";
import { AuthContext } from "../auth/context";
import { AuthService, AuthState } from "../auth/authService";
import { INDEX } from "./links";
import Loading from "../components/Loading";
import { BehaviorSubject } from "rxjs";
import User from "../auth/User";
const { Provider } = AuthContext;

describe("LogoutEverywherePage", () => {
	let authService: AuthService;
	let authState: BehaviorSubject<AuthState>;
	let logoutAll: jest.Mock<Promise<void>>;

	beforeEach(() => {
		logoutAll = jest.fn(() => Promise.resolve());
		authState = new BehaviorSubject<AuthState>({ user: new User("", "") });
		authService = { logoutAll, authState } as unknown as AuthService;
	});

	it("calls logoutAll when mounted, displays loading while unresolved", () => {
		const testRenderer = TestRenderer.create(<BrowserRouter><Provider value={authService}><LogoutEverywherePage /></Provider></BrowserRouter>);
		expect(logoutAll).toHaveBeenCalledTimes(1);
		testRenderer.root.findByType(Loading);
	});

	it("redirects, does not call logoutAll if user not logged in", () => {
		authState.next({});
		const testRenderer = TestRenderer.create(<BrowserRouter><Provider value={authService}><LogoutEverywherePage /></Provider></BrowserRouter>);
		expect(logoutAll).toHaveBeenCalledTimes(0);
		expect(testRenderer.root.findByType(Redirect).props).toMatchObject({ to: INDEX });
	});

	it("redirects, does not call logoutAll if auth state unknown", () => {
		authState.next({ unknown: true });
		const testRenderer = TestRenderer.create(<BrowserRouter><Provider value={authService}><LogoutEverywherePage /></Provider></BrowserRouter>);
		expect(logoutAll).toHaveBeenCalledTimes(0);
		expect(testRenderer.root.findByType(Redirect).props).toMatchObject({ to: INDEX });
	});

	it("redirects when logoutAll resolves", async () => {
		const root = <BrowserRouter><Provider value={authService}><LogoutEverywherePage /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		await Promise.resolve();
		testRenderer.update(root);
		expect(testRenderer.root.findByType(Redirect).props).toMatchObject({ to: INDEX });
	});

	it("displays an error when logoutAll rejects", async () => {
		logoutAll = jest.fn(() => Promise.reject({ message: "some mice ate some wires" }));
		authService.logoutAll = logoutAll;
		const root = <BrowserRouter><Provider value={authService}><LogoutEverywherePage /></Provider></BrowserRouter>;
		const testRenderer = TestRenderer.create(root);
		await Promise.resolve();
		testRenderer.update(root);
		expect(testRenderer.root.findByType(Error).props).toMatchObject({ message: "some mice ate some wires" });
	});
});