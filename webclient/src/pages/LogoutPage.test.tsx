import React from "react"
import TestRenderer from "react-test-renderer"
import { BrowserRouter, Redirect } from "react-router-dom";

import LogoutPage from "./LogoutPage"
import Error from "../components/Error"
import { AuthContext } from "../auth/context";
import { AuthService } from "../auth/authService";
import { INDEX } from "./links";
import Loading from "../components/Loading";
const { Provider } = AuthContext

describe("LogoutPage", () => {
	let authService: AuthService
	let logout: jest.Mock<Promise<void>>

	beforeEach(() => {
		logout = jest.fn(() => Promise.resolve())
		authService = { logout } as unknown as AuthService
	})

	it("calls logout when mounted, displays loading while unresolved", () => {
		const testRenderer = TestRenderer.create(<BrowserRouter><Provider value={authService}><LogoutPage /></Provider></BrowserRouter>)
		expect(logout).toHaveBeenCalledTimes(1)
		testRenderer.root.findByType(Loading)
	})

	it("redirects when logout resolves", async () => {
		const root = <BrowserRouter><Provider value={authService}><LogoutPage /></Provider></BrowserRouter>
		const testRenderer = TestRenderer.create(root)
		await Promise.resolve()
		testRenderer.update(root)
		expect(testRenderer.root.findByType(Redirect).props).toMatchObject({ to: INDEX })
	})

	it("displays an error when logout rejects", async () => {
		logout = jest.fn(() => Promise.reject({ message: "some mice ate some wires" }))
		authService.logout = logout
		const root = <BrowserRouter><Provider value={authService}><LogoutPage /></Provider></BrowserRouter>
		const testRenderer = TestRenderer.create(root)
		await Promise.resolve()
		testRenderer.update(root)
		expect(testRenderer.root.findByType(Error).props).toMatchObject({ message: "some mice ate some wires" })
	})
})