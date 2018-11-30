import AuthResponse from "./AuthResponse";
import { makeRequestInit } from "../api/makeRequestInit";
import { networkErrorRejection, makeRejectedPromise, serverErrorRejection, badRequestRejection } from "../api/rejectedPromises";

export interface AuthAPI {
	login: (username: string, password: string, persistSession: boolean) => Promise<AuthResponse>;
	register: (username: string, password: string, persistSession: boolean) => Promise<AuthResponse>;
	getCurrentAuth: () => Promise<AuthResponse | undefined>;
	changePassword: (accessToken: string, password: string, newPassword: string) => Promise<AuthResponse>;
	changeUsername: (accessToken: string, password: string, newUsername: string) => Promise<AuthResponse>;
	logout: () => Promise<void>;
	logoutAll: (accessToken: string) => Promise<void>;
}

const path = "/api/auth";

function makeAuthRequest(init: RequestInit) {
	return fetch(path, init).catch(e => networkErrorRejection);
}

function login(username: string, password: string, rememberMe: boolean): Promise<AuthResponse> {
	return makeAuthRequest(makeRequestInit("POST", { username, password, rememberMe }))
		.then(res => {
			if (res.ok) {
				return res.json();
			}
			if (res.status === 401) {
				return makeRejectedPromise("Username and password combination incorrect");
			}
			if (res.status === 400) {
				return makeRejectedPromise("Username or password missing");
			}
			return serverErrorRejection;
		});
}

function register(username: string, password: string, rememberMe: boolean): Promise<AuthResponse> {
	return makeAuthRequest(makeRequestInit("PUT", { username, password, rememberMe }))
		.then(res => {
			if (res.ok) {
				return res.json();
			}
			if (res.status === 409) {
				return makeRejectedPromise("Username already in use");
			}
			if (res.status === 400) {
				return makeRejectedPromise("Username or password missing or invalid");
			}
			return serverErrorRejection;
		});
}

function getCurrentAuth(): Promise<AuthResponse | undefined> {
	return makeAuthRequest({ credentials: "same-origin" }).then(res => {
		if (res.ok) {
			return res.json();
		}
		if (res.status === 400 || res.status === 401) {
			return;
		}
		return serverErrorRejection;
	});
}

async function changePassword(accessToken: string, password: string, newPassword: string): Promise<AuthResponse> {
	const res = await makeAuthRequest(makeRequestInit("PATCH", { password, newPassword }, accessToken));
	if (res.ok) {
		return res.json();
	}
	if (res.status === 401) {
		return makeRejectedPromise("Password Incorrect"); // making the assumption that the access token hasn't expired
	}
	if (res.status === 400) {
		return badRequestRejection;
	}
	return serverErrorRejection;
}

async function changeUsername(accessToken: string, password: string, newUsername: string): Promise<AuthResponse> {
	const res = await makeAuthRequest(makeRequestInit("PATCH", { password, newUsername }, accessToken));
	if (res.ok) {
		return res.json();
	}
	if (res.status === 401) {
		return makeRejectedPromise("Password Incorrect"); // making the assumption that the access token hasn't expired
	}
	if (res.status === 400) {
		return badRequestRejection;
	}
	return serverErrorRejection;
}

async function logout(): Promise<void> {
	try {
		await fetch(`${path}/logout`, { credentials: "same-origin" });
	} catch (e) {
		return networkErrorRejection;
	}
}

async function logoutAll(accessToken: string): Promise<void> {
	try {
		const res = await fetch(`${path}/logoutall`, { headers: { "Authorization": `Bearer ${accessToken}` } });
		if (!res.ok) {
			return makeRejectedPromise("An error occurred");
		}
	} catch (e) {
		return networkErrorRejection;
	}
}

export const authAPI: AuthAPI = {
	login,
	register,
	getCurrentAuth,
	changePassword,
	changeUsername,
	logout,
	logoutAll
};