import { AuthAPI } from "./api"
import User from "./User"
import { Validators } from "./validators"
import { BehaviorSubject, Subscription } from "rxjs"

export interface AuthState {
	user?: Readonly<User>
	error?: string
	unknown?: boolean
}

export interface AuthService {
	authState: Readonly<{ value: Readonly<AuthState>, subscribe: (observer: (value: AuthState) => void) => Subscription }>
	login: (username: string, password: string, persistSession: boolean) => Promise<User>
	register: (username: string, password: string, persistSession: boolean) => Promise<User>
	getAccessToken: () => Promise<string>
	changePassword: (password: string, newPassword: string) => Promise<void>
	changeUsername: (password: string, newUsername: string) => Promise<void>
	logout: () => Promise<void>
	logoutAll: () => Promise<void>
}

export function makeAuthService(authAPI: AuthAPI, validators: Validators): Readonly<AuthService> {
	const authStateSub = new BehaviorSubject<AuthState>({ unknown: true })
	const authState = Object.freeze({
		get value() {
			return Object.freeze(authStateSub.value)
		},
		subscribe(observer: (value: AuthState) => void) {
			return authStateSub.subscribe(observer)
		}
	})
	let accessToken: string | undefined

	const login = async (username: string, password: string, persistSession: boolean): Promise<User> => {
		if (authState.value.user) {
			return Promise.reject({ message: "A user is already logged in." })
		}
		if (authState.value.unknown) {
			return Promise.reject({ message: "Authentication state is unknown." })
		}
		authStateSub.next({ unknown: true })
		try {
			const authRes = await authAPI.login(username, password, persistSession)
			accessToken = authRes.token
			const user = new User(authRes.uid, authRes.username)
			authStateSub.next({ user })
			return user
		} catch (e) {
			authStateSub.next({})
			throw e
		}
	}

	const register = async (username: string, password: string, persistSession: boolean): Promise<User> => {
		if (authState.value.user) {
			return Promise.reject({ message: "A user is already logged in." })
		}
		if (authState.value.unknown) {
			return Promise.reject({ message: "Authentication state is unknown." })
		}
		const usernameValid = validators.validateUsername(username)
		const passwordValid = validators.validatePassword(password)
		if (!passwordValid && !usernameValid) {
			return Promise.reject({ username: "Usernames must be at least 3 characters long and may not contain white space", password: "Passwords must be at least 8 characters long and may not contain white space" })
		}
		if (!passwordValid) {
			return Promise.reject({ password: "Passwords must be at least 8 characters long and may not contain white space" })
		}
		if (!usernameValid) {
			return Promise.reject({ username: "Usernames must be at least 3 characters long and may not contain white space" })
		}
		authStateSub.next({ unknown: true })
		try {
			const authRes = await authAPI.register(username, password, persistSession)
			accessToken = authRes.token
			const user = new User(authRes.uid, authRes.username)
			authStateSub.next({ user })
			return user
		} catch (e) {
			authStateSub.next({})
			throw e
		}
	}

	const getAccessToken = async (): Promise<string> => {
		if (authState.value.unknown) {
			return Promise.reject({ message: "Authentication state is unknown." })
		}
		if (!authState.value.user) {
			return Promise.reject({ message: "User is not logged in." })
		}
		if (validators.validateToken(accessToken)) {
			return accessToken as string
		}

		const authRes = await authAPI.getCurrentAuth()
		if (authRes) {
			accessToken = authRes.token as string
			if (authRes.username !== authState.value.user.username) {
				authStateSub.next({ user: new User(authRes.uid, authRes.username) })
			}
			return accessToken as string
		}
		authStateSub.next({})
		return Promise.reject({ message: "User is not logged in." })
	}

	const changePassword = async (password: string, newPassword: string): Promise<void> => {
		if (!validators.validatePassword(newPassword)) {
			return Promise.reject({ message: "Passwords must be at least 8 characters long and may not contain white space" })
		}
		const token = await getAccessToken()
		await authAPI.changePassword(token, password, newPassword)
	}

	const changeUsername = async (password: string, newUsername: string): Promise<void> => {
		if (!validators.validateUsername(newUsername)) {
			return Promise.reject({ message: "Usernames must be at least 3 characters long and may not contain white space" })
		}
		const token = await getAccessToken()
		await authAPI.changeUsername(token, password, newUsername)
		const user = authState.value.user
		if (user) {
			authStateSub.next({ user: new User(user.uid, newUsername) })
		}
	}

	const logout = async (): Promise<void> => {
		if (authState.value.unknown) {
			return Promise.reject({ message: "Authentication state is unknown." })
		}
		if (!authState.value.user) {
			return Promise.reject({ message: "User is not logged in." })
		}
		await authAPI.logout()
		authStateSub.next({})
	}

	const logoutAll = async (): Promise<void> => {
		const token = await getAccessToken()
		await authAPI.logoutAll(token)
		authStateSub.next({})
	}

	authAPI.getCurrentAuth().then(value => {
		if (value) {
			accessToken = value.token
			return authStateSub.next({ user: new User(value.uid, value.username) })
		}
		authStateSub.next({})
	}).catch(err => {
		authStateSub.next({ unknown: true, error: err.message })
	})

	return Object.freeze({
		authState,
		login,
		register,
		getAccessToken,
		changePassword,
		changeUsername,
		logout,
		logoutAll
	})
}