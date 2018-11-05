import AuthResponse from "./AuthResponse"

export interface AuthAPI {
	login: (username: string, password: string, persistSession: boolean) => Promise<AuthResponse>
	register: (username: string, password: string, persistSession: boolean) => Promise<AuthResponse>
	getCurrentAuth: () => Promise<AuthResponse | undefined>
	changePassword: (accessToken: string, password: string, newPassword: string) => Promise<AuthResponse>
	changeUsername: (accessToken: string, password: string, newUsername: string) => Promise<AuthResponse>
	logout: () => Promise<void>
	logoutAll: (accessToken: string) => Promise<void>
}

const path = "/api/auth"
const NETWORK_ERROR = "Network Error"

function makeRejectedPromise(message: string) {
	return Promise.reject({ message })
}

function makeRequestInit(method: string, body: object, accessToken?: string): RequestInit {
	const headers: Record<string, string> = {
		"Content-Type": "application/json; charset=utf-8",
		credentials: "omit"
	}
	if (accessToken) {
		headers.Authorization = `Bearer ${accessToken}`
	}
	return {
		method,
		headers,
		body: JSON.stringify(body)
	}
}

function makeAuthRequest(init: RequestInit) {
	return fetch(path, init).catch(e => makeRejectedPromise(NETWORK_ERROR))
}

function login(username: string, password: string, rememberMe: boolean): Promise<AuthResponse> {
	return makeAuthRequest(makeRequestInit("POST", { username, password, rememberMe }))
		.then(res => {
			if (res.ok) {
				return res.json()
			}
			if (res.status === 401) {
				return makeRejectedPromise("Username and password combination incorrect")
			}
			if (res.status === 400) {
				return makeRejectedPromise("Username or password missing")
			}
			return makeRejectedPromise("Server Error")
		})
}

function register(username: string, password: string, rememberMe: boolean): Promise<AuthResponse> {
	return makeAuthRequest(makeRequestInit("PUT", { username, password, rememberMe }))
		.then(res => {
			if (res.ok) {
				return res.json()
			}
			if (res.status === 409) {
				return makeRejectedPromise("Username already in use")
			}
			if (res.status === 400) {
				return makeRejectedPromise("Username or password missing or invalid")
			}
			return makeRejectedPromise("Server Error")
		})
}

function getCurrentAuth(): Promise<AuthResponse | undefined> {
	return makeAuthRequest({ credentials: "same-origin" }).then(res => {
		if (res.ok) {
			return res.json()
		}
		if (res.status === 400 || res.status === 401) {
			return
		}
		return makeRejectedPromise("Server Error")
	})
}

async function changePassword(accessToken: string, password: string, newPassword: string): Promise<AuthResponse> {
	const res = await makeAuthRequest(makeRequestInit("PATCH", { password, newPassword }, accessToken))
	if (res.ok) {
		return res.json()
	}
	if (res.status === 401) {
		return makeRejectedPromise("Password Incorrect") // making the assumption that the access token hasn't expired
	}
	if (res.status === 400) {
		return makeRejectedPromise("Bad Request")
	}
	return makeRejectedPromise("Server Error")
}

async function changeUsername(accessToken: string, password: string, newUsername: string): Promise<AuthResponse> {
	const res = await makeAuthRequest(makeRequestInit("PATCH", { password, newUsername }, accessToken))
	if (res.ok) {
		return res.json()
	}
	if (res.status === 401) {
		return makeRejectedPromise("Password Incorrect") // making the assumption that the access token hasn't expired
	}
	if (res.status === 400) {
		return makeRejectedPromise("Bad Request")
	}
	return makeRejectedPromise("Server Error")
}

async function logout(): Promise<void> {
	try {
		await fetch(`${path}/logout`, { credentials: "same-origin" })
	} catch (e) {
		return makeRejectedPromise(NETWORK_ERROR)
	}
}

async function logoutAll(accessToken: string): Promise<void> {
	try {
		const res = await fetch(`${path}/logoutall`, { headers: { "Authorization": `Bearer ${accessToken}` } })
		if (!res.ok) {
			return makeRejectedPromise("An error occurred")
		}
	} catch (e) {
		return makeRejectedPromise(NETWORK_ERROR)
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
}