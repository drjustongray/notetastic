import { makeAuthService } from "./authService"
import { AuthAPI } from "./api"
import { Validators } from "./validators"
import User from "./User"
import AuthResponse from "./AuthResponse"

describe("makeAuthService", () => {
	let getCurrentAuth: jest.Mock<Promise<AuthResponse | undefined>>
	let validateToken: jest.Mock<boolean>
	let validateUsername: jest.Mock<boolean>
	let validatePassword: jest.Mock<boolean>

	beforeEach(() => {
		getCurrentAuth = jest.fn(() => Promise.resolve(undefined))
		validateToken = jest.fn(() => true)
		validatePassword = jest.fn(() => true)
		validateUsername = jest.fn(() => true)
	})

	it("checks auth status with server", () => {
		makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
		expect(getCurrentAuth).toHaveBeenCalledTimes(1)
	})

	it("saves the access token when initial status check is successful", async () => {
		const user = new User("uid", "username")
		getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
		validateToken = jest.fn(value => value === "token")
		const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateToken } as unknown as Validators)
		await Promise.resolve()
		await expect(authService.getAccessToken()).resolves.toBe("token")
	})

	describe("authState", () => {
		it("has the correct initial value", () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			expect(authService.authState.value).toEqual({ unknown: true })
		})

		it("calls observer with current state", () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			const observer = jest.fn()
			authService.authState.subscribe(observer)
			expect(observer).toHaveBeenLastCalledWith({ unknown: true })
		})

		it("updates correctly if getCurrentAuth resolves to undefined", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			const observer = jest.fn()
			authService.authState.subscribe(observer)
			await Promise.resolve()
			expect(authService.authState.value).toEqual({})
			expect(observer).toHaveBeenLastCalledWith({})
		})

		it("updates correctly if getCurrentAuth resolves to AuthResponse", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			const observer = jest.fn()
			authService.authState.subscribe(observer)
			await Promise.resolve()
			expect(authService.authState.value).toEqual({ user })
			expect(observer).toHaveBeenLastCalledWith({ user })
		})

		it("updates correctly if getCurrentAuth rejects", async () => {
			const error = "sad times"
			getCurrentAuth = jest.fn(() => Promise.reject({ message: error }))
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			const observer = jest.fn()
			authService.authState.subscribe(observer)
			await Promise.resolve()
			expect(authService.authState.value).toEqual({ unknown: true, error })
			expect(observer).toHaveBeenLastCalledWith({ unknown: true, error })
		})
	})

	describe("login", () => {

		it("resolves if authAPI.login resolves", async () => {
			const user = new User("8", "username")
			const token = "sometoken"
			const login = jest.fn(() => Promise.resolve({ ...user, token }))

			const authService = makeAuthService({ getCurrentAuth, login } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.login(user.username, "apassword", true)).resolves.toEqual(user)
		})

		it("rejects if authAPI.login rejects", async () => {
			const rejectsWith = { message: "some error message" }
			const login = jest.fn(() => Promise.reject(rejectsWith))

			const authService = makeAuthService({ getCurrentAuth, login } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.login("fasdfad", "asdfdsa", false)).rejects.toEqual(rejectsWith)
		})

		it("rejects and does nothing if user is logged in", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.login("username1", "passworkdd", false)).rejects.toEqual({ message: "A user is already logged in." })
		})

		it("rejects and does nothing if auth state is unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await expect(authService.login("username1", "passworkdd", false)).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("updates authState", async () => {
			const user = new User("8", "username")
			const token = "sometoken"
			let login = jest.fn(() => Promise.resolve({ ...user, token }))
			let authService = makeAuthService({ getCurrentAuth, login } as unknown as AuthAPI, {} as unknown as Validators)
			await Promise.resolve()

			let observer = jest.fn()
			const sub = authService.authState.subscribe(observer)
			await authService.login("username1", "passworkdd", false)
			expect(observer).toHaveBeenCalledWith({ unknown: true })
			expect(observer).toHaveBeenLastCalledWith({ user })
			expect(observer).toHaveBeenCalledTimes(3)
			sub.unsubscribe()

			login = jest.fn(() => Promise.reject({ message: "whatevs" }))
			authService = makeAuthService({ getCurrentAuth, login } as unknown as AuthAPI, {} as unknown as Validators)
			await Promise.resolve()
			observer = jest.fn()
			authService.authState.subscribe(observer)
			try {
				await authService.login("username1", "passworkdd", false)
			} catch (e) { }
			expect(observer).toHaveBeenCalledWith({ unknown: true })
			expect(observer).toHaveBeenLastCalledWith({})
			expect(observer).toHaveBeenCalledTimes(3)
		})

		describe("getAccessToken", () => {
			it("returns token retrieved when logging in", async () => {
				const user = new User("8", "username")
				const token = "sometoken"
				const login = jest.fn(() => Promise.resolve({ ...user, token }))
				const authService = makeAuthService({ getCurrentAuth, login } as unknown as AuthAPI, { validateToken } as unknown as Validators)
				await Promise.resolve()
				await authService.login("", "", false)

				await expect(authService.getAccessToken()).resolves.toBe(token)
			})
		})
	})

	describe("register", () => {

		it("resolves if authAPI.register resolves", async () => {
			const user = new User("8", "username")
			const token = "sometoken"
			const register = jest.fn(() => Promise.resolve({ ...user, token }))

			const authService = makeAuthService({ getCurrentAuth, register } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.register(user.username, "apassword", true)).resolves.toEqual(user)
		})

		it("rejects if authAPI.register rejects", async () => {
			const rejectsWith = { message: "some error message" }
			const register = jest.fn(() => Promise.reject(rejectsWith))

			const authService = makeAuthService({ getCurrentAuth, register } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.register("fasdfad", "asdfdsa", false)).rejects.toEqual(rejectsWith)
		})

		it("rejects and does nothing if user is logged in", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.register("username1", "passworkdd", false)).rejects.toEqual({ message: "A user is already logged in." })
		})

		it("rejects and does nothing if auth state is unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await expect(authService.register("username1", "passworkdd", false)).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("rejects if username invalid", async () => {
			validateUsername = jest.fn(() => false)
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.register("username1", "passworkdd", false)).rejects.toEqual({ username: "Usernames must be at least 3 characters long and may not contain white space" })
		})

		it("rejects if password invalid", async () => {
			validatePassword = jest.fn(() => false)
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.register("username1", "passworkdd", false)).rejects.toEqual({ password: "Passwords must be at least 8 characters long and may not contain white space" })
		})

		it("rejects if both username and password invalid", async () => {
			validateUsername = jest.fn(() => false)
			validatePassword = jest.fn(() => false)
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.register("username1", "passworkdd", false)).rejects.toEqual({ username: "Usernames must be at least 3 characters long and may not contain white space", password: "Passwords must be at least 8 characters long and may not contain white space" })
		})

		it("updates authState", async () => {
			const user = new User("8", "username")
			const token = "sometoken"
			let register = jest.fn(() => Promise.resolve({ ...user, token }))
			let authService = makeAuthService({ getCurrentAuth, register } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()

			let observer = jest.fn()
			const sub = authService.authState.subscribe(observer)
			await authService.register("username1", "passworkdd", false)
			expect(observer).toHaveBeenCalledWith({ unknown: true })
			expect(observer).toHaveBeenLastCalledWith({ user })
			expect(observer).toHaveBeenCalledTimes(3)
			sub.unsubscribe()

			register = jest.fn(() => Promise.reject({ message: "whatevs" }))
			authService = makeAuthService({ getCurrentAuth, register } as unknown as AuthAPI, { validatePassword, validateUsername } as unknown as Validators)
			await Promise.resolve()
			observer = jest.fn()
			authService.authState.subscribe(observer)
			try {
				await authService.register("username1", "passworkdd", false)
			} catch (e) { }
			expect(observer).toHaveBeenCalledWith({ unknown: true })
			expect(observer).toHaveBeenLastCalledWith({})
			expect(observer).toHaveBeenCalledTimes(3)
		})

		describe("getAccessToken", () => {
			it("returns token retrieved when registering", async () => {
				const user = new User("8", "username")
				const token = "sometoken"
				const register = jest.fn(() => Promise.resolve({ ...user, token }))
				const authService = makeAuthService({ getCurrentAuth, register } as unknown as AuthAPI, { validatePassword, validateUsername, validateToken } as unknown as Validators)
				await Promise.resolve()
				await authService.register("", "", false)

				await expect(authService.getAccessToken()).resolves.toBe(token)
			})
		})
	})

	describe("getAccessToken", () => {
		it("rejects if auth state unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await expect(authService.getAccessToken()).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("rejects if no user is logged in", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.getAccessToken()).rejects.toEqual({ message: "User is not logged in." })
		})

		it("calls api function to get new token if logged in and token invalid", async () => {
			const token2 = "token2++"
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn()
			getCurrentAuth
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" }))
				.mockReturnValueOnce(Promise.resolve({ ...user, token: token2 }))

			validateToken = jest.fn(value => value === token2)

			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateToken } as unknown as Validators)

			await Promise.resolve()

			await expect(authService.getAccessToken()).resolves.toEqual(token2)
			await expect(authService.getAccessToken()).resolves.toEqual(token2)
		})

		it("updates auth state and rejects if user no longer logged in", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn()
			getCurrentAuth
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" }))
				.mockReturnValue(Promise.resolve(undefined))

			validateToken = jest.fn(() => false)

			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateToken } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.getAccessToken()).rejects.toEqual({ message: "User is not logged in." })
			expect(authService.authState.value).toEqual({})
		})

		it("updates auth state if username has changed", async () => {
			const token2 = "token2++"
			const user = new User("uid", "username2")
			getCurrentAuth = jest.fn()
			getCurrentAuth
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token", username: "username1" }))
				.mockReturnValueOnce(Promise.resolve({ ...user, token: token2 }))

			validateToken = jest.fn(value => value === token2)

			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateToken } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.getAccessToken()).resolves.toBe(token2)

			expect(authService.authState.value).toEqual({ user })
		})
	})

	describe("changePassword", () => {

		it("rejects if password invalid", async () => {
			validatePassword = jest.fn(value => value !== "newepass")
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.changePassword("oldepass", "newepass")).rejects.toEqual({ message: "Passwords must be at least 8 characters long and may not contain white space" })
		})

		it("rejects if auth state unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword } as unknown as Validators)
			await expect(authService.changePassword("oldepass", "newepass")).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("rejects if no user is logged in", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validatePassword } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.changePassword("oldepass", "newepass")).rejects.toEqual({ message: "User is not logged in." })
		})

		it("calls api function", async () => {
			const user = new User("uid", "username")
			const tokens = ["toke1", "token2", "take3"]
			getCurrentAuth = jest.fn()
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" })) as any
			for (const token of tokens) {
				getCurrentAuth.mockReturnValueOnce(Promise.resolve({ ...user, token }))
			}
			validateToken = jest.fn(value => false)
			const changePassword = jest.fn(() => Promise.resolve())

			const authService = makeAuthService({ getCurrentAuth, changePassword } as unknown as AuthAPI, { validatePassword, validateToken } as unknown as Validators)
			await Promise.resolve()
			for (const token of tokens) {
				await authService.changePassword("pass" + token, "newpass" + token)
				expect(changePassword).toHaveBeenLastCalledWith(token, "pass" + token, "newpass" + token)
			}
		})

		it("rejects with API call", async () => {
			const user = new User("uid", "username")
			const tokens = ["toke1", "token2", "take3"]
			getCurrentAuth = jest.fn()
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" })) as any
			for (const token of tokens) {
				getCurrentAuth.mockReturnValueOnce(Promise.resolve({ ...user, token }))
			}
			validateToken = jest.fn(value => false)

			const changePassword = jest.fn(() => Promise.reject({ message: "Oh noes!" }))

			const authService = makeAuthService({ getCurrentAuth, changePassword } as unknown as AuthAPI, { validatePassword, validateToken } as unknown as Validators)
			await Promise.resolve()
			for (const token of tokens) {
				await expect(authService.changePassword("pass" + token, "newpass" + token)).rejects.toEqual({ message: "Oh noes!" })
			}
		})
	})

	describe("changeUsername", () => {

		it("rejects if username invalid", async () => {
			validateUsername = jest.fn(value => value !== "newusername")
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.changeUsername("passwordia", "newusername")).rejects.toEqual({ message: "Usernames must be at least 3 characters long and may not contain white space" })
		})

		it("rejects if auth state unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateUsername } as unknown as Validators)
			await expect(authService.changeUsername("passwordia", "newusername")).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("rejects if no user is logged in", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, { validateUsername } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.changeUsername("passwordia", "newusername")).rejects.toEqual({ message: "User is not logged in." })
		})

		it("calls api function", async () => {
			const user = new User("uid", "username")
			const tokens = ["toke1", "token2", "take3"]
			getCurrentAuth = jest.fn()
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" })) as any
			for (const token of tokens) {
				getCurrentAuth.mockReturnValueOnce(Promise.resolve({ ...user, token }))
			}
			validateToken = jest.fn(value => false)
			const changeUsername = jest.fn(() => Promise.resolve())

			const authService = makeAuthService({ getCurrentAuth, changeUsername } as unknown as AuthAPI, { validateUsername, validateToken } as unknown as Validators)
			await Promise.resolve()
			for (const token of tokens) {
				await authService.changeUsername("pass" + token, "newusername" + token)
				expect(changeUsername).toHaveBeenLastCalledWith(token, "pass" + token, "newusername" + token)
			}
		})

		it("updates authState", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn()
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" })) as any
			const changeUsername = jest.fn(() => Promise.resolve())

			const authService = makeAuthService({ getCurrentAuth, changeUsername } as unknown as AuthAPI, { validateUsername, validateToken } as unknown as Validators)
			await Promise.resolve()
			await authService.changeUsername("passrd", "newusername")
			expect(authService.authState.value).toEqual({ user: new User("uid", "newusername") })
		})

		it("rejects with API call", async () => {
			const user = new User("uid", "username")
			const tokens = ["toke1", "token2", "take3"]
			getCurrentAuth = jest.fn()
				.mockReturnValueOnce(Promise.resolve({ ...user, token: "token" })) as any
			for (const token of tokens) {
				getCurrentAuth.mockReturnValueOnce(Promise.resolve({ ...user, token }))
			}
			validateToken = jest.fn(value => false)

			const changeUsername = jest.fn(() => Promise.reject({ message: "Oh noes!" }))

			const authService = makeAuthService({ getCurrentAuth, changeUsername } as unknown as AuthAPI, { validateUsername, validateToken } as unknown as Validators)
			await Promise.resolve()
			for (const token of tokens) {
				await expect(authService.changeUsername("pass" + token, "newusername" + token)).rejects.toEqual({ message: "Oh noes!" })
			}
		})
	})

	describe("logout", () => {
		let logout: jest.Mock<Promise<void>>

		beforeEach(() => {
			logout = jest.fn(() => Promise.resolve())
		})

		it("rejects if user not logged in", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.logout()).rejects.toEqual({ message: "User is not logged in." })
		})

		it("rejects if auth state unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await expect(authService.logout()).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("calls api function", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth, logout } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await authService.logout()
			expect(logout).toHaveBeenCalledTimes(1)
		})

		it("rejects with api call", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			logout = jest.fn(() => Promise.reject({ message: "ARRGGHHH!" }))
			const authService = makeAuthService({ getCurrentAuth, logout } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.logout()).rejects.toEqual({ message: "ARRGGHHH!" })
		})

		it("updates auth state", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth, logout } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await authService.logout()
			expect(authService.authState.value).toEqual({})
		})
	})

	describe("logoutAll", () => {
		let logoutAll: jest.Mock<Promise<void>>

		beforeEach(() => {
			logoutAll = jest.fn(() => Promise.resolve())
		})

		it("rejects if user not logged in", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await Promise.resolve()
			await expect(authService.logoutAll()).rejects.toEqual({ message: "User is not logged in." })
		})

		it("rejects if auth state unknown", async () => {
			const authService = makeAuthService({ getCurrentAuth } as unknown as AuthAPI, {} as Validators)
			await expect(authService.logoutAll()).rejects.toEqual({ message: "Authentication state is unknown." })
		})

		it("calls api function", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth, logoutAll } as unknown as AuthAPI, { validateToken } as unknown as Validators)
			await Promise.resolve()
			await authService.logoutAll()
			expect(logoutAll).toHaveBeenCalledTimes(1)
			expect(logoutAll).toHaveBeenLastCalledWith("token")
		})

		it("rejects with api call", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			logoutAll = jest.fn(() => Promise.reject({ message: "ARRGGHHH!" }))
			const authService = makeAuthService({ getCurrentAuth, logoutAll } as unknown as AuthAPI, { validateToken } as unknown as Validators)
			await Promise.resolve()
			await expect(authService.logoutAll()).rejects.toEqual({ message: "ARRGGHHH!" })
		})

		it("updates auth state", async () => {
			const user = new User("uid", "username")
			getCurrentAuth = jest.fn(() => Promise.resolve({ ...user, token: "token" }))
			const authService = makeAuthService({ getCurrentAuth, logoutAll } as unknown as AuthAPI, { validateToken } as unknown as Validators)
			await Promise.resolve()
			await authService.logoutAll()
			expect(authService.authState.value).toEqual({})
		})
	})
})