import React from "react"
import TestRenderer from "react-test-renderer"

import authConnect from "./authConnect"
import { AuthContext } from "../context"
import { AuthService, AuthState } from "../authService";
import { BehaviorSubject } from "rxjs";
import User from "../User";

const { Provider } = AuthContext

const WrappedComponent = () => <div></div>

describe("authConnect", () => {
	let authService: AuthService
	let authState: BehaviorSubject<AuthState>

	beforeEach(() => {
		authState = new BehaviorSubject<AuthState>({ unknown: true })
		authService = { authState } as unknown as AuthService
	})

	it("renders without crashing", () => {
		const Component = authConnect(() => ({}))(WrappedComponent)
		TestRenderer.create(<Provider value={authService}><Component /></Provider>)
	})

	it("renders the wrapped component with the correct props", () => {
		const authProps = { fun: 1, happy: "2" }
		const ownProps = { g: null, a: true }
		const Component = authConnect(() => (authProps))(WrappedComponent)
		const root = <Provider value={authService}><Component {...ownProps} /></Provider>
		const testRenderer = TestRenderer.create(root)
		expect(testRenderer.root.findByType(WrappedComponent).props).toEqual({ ...authProps, ...ownProps })
	})

	it("updates when the state changes", () => {
		const ownProps = { g: null, a: "true" }
		const Component = authConnect((authState) => ({ loggedIn: !!authState.user }))(WrappedComponent)
		const root = <Provider value={authService}><Component {...ownProps} /></Provider>
		const testRenderer = TestRenderer.create(root)
		expect(testRenderer.root.findByType(WrappedComponent).props).toEqual({ loggedIn: false, ...ownProps })
		authState.next({ user: new User("", "") })
		testRenderer.update(root)
		expect(testRenderer.root.findByType(WrappedComponent).props).toEqual({ loggedIn: true, ...ownProps })
	})

	it("foregoes update when mapFunction returns the same value as previously", () => {
		const WrappedComponent = jest.fn(() => <div></div>)
		const authProps = { fun: 1, happy: "2" }
		const ownProps = { g: null, a: "true" }
		const Component = authConnect(() => (authProps))(WrappedComponent)
		const root = <Provider value={authService}><Component {...ownProps} /></Provider>
		const testRenderer = TestRenderer.create(root)
		authState.next({ user: new User("", "") })
		testRenderer.update(root)
		expect(WrappedComponent).toHaveBeenCalledTimes(1)
	})

	it("subscribes and unsubscribes when mounted, unmounted", () => {
		const Component = authConnect(() => ({}))(WrappedComponent)
		const root = <Provider value={authService}><Component /></Provider>
		const testRenderer = TestRenderer.create(root)
		expect(authState.observers).toHaveLength(1)
		testRenderer.unmount()
		expect(authState.observers).toHaveLength(0)
	})
})