import * as React from "react";
import TestRenderer from "react-test-renderer";

import TabbedAuthForm from "./TabbedAuthForm";
import Loading from "../../components/Loading";
import requireAuth from "./requireAuth";
import { AuthContext } from "../context";
import { AuthService, AuthState } from "../authService";
import { BehaviorSubject } from "rxjs";
import User from "../User";

const { Provider } = AuthContext;

const WrappedComponent = () => <div />;
const Component = requireAuth(WrappedComponent);

describe("requireAuth", () => {
	let authService: AuthService;
	const login = () => undefined;
	const register = () => undefined;
	let authState: BehaviorSubject<AuthState>;

	beforeEach(() => {
		authState = new BehaviorSubject<AuthState>({ unknown: true });
		authService = { login, register, authState } as unknown as AuthService;
	});

	it("renders without crashing", () => {
		TestRenderer.create(<Provider value={authService}><Component /></Provider>);
	});

	it("renders the Loading component initially (auth state is unknown)", () => {
		const testRenderer = TestRenderer.create(<Provider value={authService}><Component /></Provider>);
		const component = testRenderer.root.findByType(Component);
		component.findByType(Loading);
	});

	it("renders the TabbedAuthForm when not logged in", () => {
		const root = <Provider value={authService}><Component /></Provider>;
		const testRenderer = TestRenderer.create(root);
		authState.next({});
		testRenderer.update(root);
		const component = testRenderer.root.findByType(Component);
		const form = component.findByType(TabbedAuthForm);
		expect(form.props).toEqual({ login, register });
	});

	it("renders the WrappedComponent when logged in, passing in props", () => {
		const props = { hi: "there", hello: 9, yes: true };
		const root = <Provider value={authService}><Component {...props} /></Provider>;
		const testRenderer = TestRenderer.create(root);
		authState.next({ user: new User("hhi", "hellooooo") });
		testRenderer.update(root);
		const component = testRenderer.root.findByType(Component);
		const wrapped = component.findByType(WrappedComponent);
		expect(wrapped.props).toEqual(props);
	});

	it("renders the AuthTabs when auth state becomes unknown again", () => {
		const root = <Provider value={authService}><Component /></Provider>;
		const testRenderer = TestRenderer.create(root);
		authState.next({});
		testRenderer.update(root);
		authState.next({ unknown: true });
		testRenderer.update(root);
		const component = testRenderer.root.findByType(Component);
		const form = component.findByType(TabbedAuthForm);
		expect(form.props).toEqual({ login, register });
	});

	it("stops subscription when unmounted", () => {
		const root = <Provider value={authService}><Component /></Provider>;
		const testRenderer = TestRenderer.create(root);
		expect(authState.observers).toHaveLength(1);
		testRenderer.unmount();
		expect(authState.observers).toHaveLength(0);
	});
});