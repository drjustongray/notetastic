import React from "react"
import TestRenderer from "react-test-renderer"

import AccountPage from "./AccountPage"
import AccountUpdateForm from "../auth/components/AccountUpdateForm"
import TabbedAuthForm from "../auth/components/TabbedAuthForm"
import { AuthService, AuthState } from "../auth/authService";
import { AuthContext } from "../auth/context";
import { BehaviorSubject } from "rxjs";
import User from "../auth/User";
import { BrowserRouter, Link } from "react-router-dom";
import { LOGOUT_EVERYWHERE } from "./links";

const { Provider } = AuthContext

describe("AccountPage", () => {
	let authService: AuthService
	let authState: BehaviorSubject<AuthState>

	beforeEach(() => {
		authState = new BehaviorSubject<AuthState>({ user: new User("uid1", "user123") })
		authService = { authState, changePassword: () => { }, changeUsername: () => { } } as unknown as AuthService
	})

	it("renders TabbedAuthForm when not logged in", () => {
		authState.next({})
		const root = <BrowserRouter><Provider value={authService}><AccountPage /></Provider></BrowserRouter>
		const testRenderer = TestRenderer.create(root)
		expect(testRenderer.root.findAllByType(AccountUpdateForm)).toHaveLength(0)
		testRenderer.root.findByType(TabbedAuthForm)
	})

	it("renders two AccountUpdateForms and a Link with right props", () => {
		const root = <BrowserRouter><Provider value={authService}><AccountPage /></Provider></BrowserRouter>
		const testRenderer = TestRenderer.create(root)
		const forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms).toHaveLength(2)
		expect(forms[0].props).toMatchObject({ isOpen: false, action: authService.changeUsername, type: "text", name: "Username", current: "user123" })
		expect(forms[1].props).toMatchObject({ isOpen: false, action: authService.changePassword, type: "password", name: "Password" });
		[0, 1].forEach(element => {
			expect(typeof forms[element].props.open).toBe("function")
			expect(typeof forms[element].props.close).toBe("function")
		})
		expect(testRenderer.root.findByType(Link).props).toMatchObject({ to: LOGOUT_EVERYWHERE })
	})

	it("updates props when open is called", () => {
		const root = <BrowserRouter><Provider value={authService}><AccountPage /></Provider></BrowserRouter>
		let testRenderer = TestRenderer.create(root)
		let forms = testRenderer.root.findAllByType(AccountUpdateForm)

		forms[0].props.open()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(true)
		expect(forms[1].props.isOpen).toBe(false)

		forms[0].props.open()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(true)
		expect(forms[1].props.isOpen).toBe(false)

		forms[1].props.open()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(false)
		expect(forms[1].props.isOpen).toBe(true)

		testRenderer = TestRenderer.create(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		forms[1].props.open()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(false)
		expect(forms[1].props.isOpen).toBe(true)

		forms[1].props.open()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(false)
		expect(forms[1].props.isOpen).toBe(true)

		forms[0].props.open()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(true)
		expect(forms[1].props.isOpen).toBe(false)
	})

	it("updates props when close is called", () => {
		const root = <BrowserRouter><Provider value={authService}><AccountPage /></Provider></BrowserRouter>
		let testRenderer = TestRenderer.create(root)
		let forms = testRenderer.root.findAllByType(AccountUpdateForm)
		forms[0].props.close()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(false)
		expect(forms[1].props.isOpen).toBe(false)

		forms[0].props.open()
		testRenderer.update(root)
		forms[0].props.close()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(false)
		expect(forms[1].props.isOpen).toBe(false)

		forms[1].props.open()
		testRenderer.update(root)
		forms[1].props.close()
		testRenderer.update(root)
		forms = testRenderer.root.findAllByType(AccountUpdateForm)
		expect(forms[0].props.isOpen).toBe(false)
		expect(forms[1].props.isOpen).toBe(false)
	})
})