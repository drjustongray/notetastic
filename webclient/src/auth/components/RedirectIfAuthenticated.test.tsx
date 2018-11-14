import React from "react"
import TestRenderer from "react-test-renderer"
import { Redirect, BrowserRouter } from "react-router-dom"

import RedirectIfAuthenticated from "./RedirectIfAuthenticated"
import { AuthService, AuthState } from "../authService";
import { BehaviorSubject } from "rxjs";
import { AuthContext } from "../context"
import User from "../User";
const { Provider } = AuthContext

describe("RedirectIfAuthenticated", () => {
	let authService: AuthService
	let authState: BehaviorSubject<AuthState>

	beforeEach(() => {
		authState = new BehaviorSubject<AuthState>({ unknown: true })
		authService = { authState } as unknown as AuthService
	})

	it("renders without crashing", () => {
		TestRenderer.create(<Provider value={authService}><RedirectIfAuthenticated to="" /></Provider>)
	})

	it("renders nothing when not logged in", () => {
		const root = <Provider value={authService}><RedirectIfAuthenticated to="" /></Provider>
		const testRenderer = TestRenderer.create(root)
		expect(testRenderer.root.findAllByType(Redirect)).toHaveLength(0)
		authState.next({})
		testRenderer.update(root)
		expect(testRenderer.root.findAllByType(Redirect)).toHaveLength(0)
	})

	it("renders redirect when logged in", () => {
		const root = <BrowserRouter><Provider value={authService}><RedirectIfAuthenticated to="pineapple" /></Provider></BrowserRouter>
		const testRenderer = TestRenderer.create(root)
		authState.next({ user: new User("", "") })
		testRenderer.update(root)
		expect(testRenderer.root.findByType(Redirect).props).toMatchObject({ to: "pineapple" })
	})

	it("stops subscription when unmounted", () => {
		const root = <Provider value={authService}><RedirectIfAuthenticated to="pineapple" /></Provider>
		const testRenderer = TestRenderer.create(root)
		expect(authState.observers).toHaveLength(1)
		testRenderer.unmount()
		expect(authState.observers).toHaveLength(0)
	})
})