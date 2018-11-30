import { authAPI } from "./api";
import fetchMock from "fetch-mock";

describe("authAPI", () => {
	describe("when cannot connect to server", () => {
		const expectedError = { message: "Network Error" };
		window.fetch = jest.fn(() => Promise.reject("whatevs"));

		describe("login", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.login("", "", false)).rejects.toEqual(expectedError);
			});
		});

		describe("register", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.register("", "", false)).rejects.toEqual(expectedError);
			});
		});

		describe("getCurrentAuth", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.getCurrentAuth()).rejects.toEqual(expectedError);
			});
		});

		describe("changeUsername", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.changeUsername("", "", "")).rejects.toEqual(expectedError);
			});
		});

		describe("changePassword", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.changePassword("", "", "")).rejects.toEqual(expectedError);
			});
		});

		describe("logout", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.logout()).rejects.toEqual(expectedError);
			});
		});

		describe("logoutAll", () => {
			it("rejected with Network Error", async () => {
				await expect(authAPI.logoutAll("")).rejects.toEqual(expectedError);
			});
		});
	});

	describe("when can connect to server", () => {
		const username = "87";
		const password = "89as8d";
		const rememberMe = true;
		const basePath = "/api/auth";
		const token = "sometoken";
		const newPassword = "newpass";
		const newUsername = "newusern";

		const authBody = { username, password, rememberMe };
		const changePasswordBody = { password, newPassword };
		const changeUsernameBody = { password, newUsername };

		const badRequest = { message: "Bad Request" };
		const conflictResponse = { message: "Username already in use" };
		const missingAuth = { message: "Username or password missing" };
		const badAuth = { message: "Username or password missing or invalid" };
		const incorrectCredentials = { message: "Username and password combination incorrect" };
		const incorrectPassword = { message: "Password Incorrect" };
		const authResponse = { uid: "uid", username, token };

		describe("login", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().postOnce(basePath, {});
				window.fetch = mock;

				await authAPI.login(username, password, rememberMe);

				const init = mock.lastCall()[1];
				expect(JSON.parse(init.body as string)).toEqual(authBody);
				expect((init.headers as Record<string, string>)["Content-Type"]).toMatch("application/json");
			});

			it("rejects with bad request", async () => {
				window.fetch = fetchMock.sandbox().postOnce(basePath, 400);

				await expect(authAPI.login(username, password, rememberMe))
					.rejects.toEqual(missingAuth);
			});

			it("rejects with incorrect credentials message", async () => {
				window.fetch = fetchMock.sandbox().postOnce(basePath, 401);

				await expect(authAPI.login(username, password, rememberMe))
					.rejects.toEqual(incorrectCredentials);
			});

			it("resolves to auth response", async () => {
				window.fetch = fetchMock.sandbox().postOnce(basePath, authResponse);

				await expect(authAPI.login(username, password, rememberMe))
					.resolves.toEqual(authResponse);
			});
		});

		describe("register", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().putOnce(basePath, {});
				window.fetch = mock;

				await authAPI.register(username, password, rememberMe);

				const init = mock.lastCall()[1];
				expect(JSON.parse(init.body as string)).toEqual(authBody);
				expect((init.headers as Record<string, string>)["Content-Type"]).toMatch("application/json");
			});

			it("rejects with bad request", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, 400);

				await expect(authAPI.register(username, password, rememberMe))
					.rejects.toEqual(badAuth);
			});

			it("rejects with incorrect credentials message", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, 409);

				await expect(authAPI.register(username, password, rememberMe))
					.rejects.toEqual(conflictResponse);
			});

			it("resolves to auth response", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, authResponse);

				await expect(authAPI.register(username, password, rememberMe))
					.resolves.toEqual(authResponse);
			});
		});

		describe("getCurrentAuth", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().getOnce(basePath, {});
				window.fetch = mock;

				await authAPI.getCurrentAuth();

				const init = mock.lastCall()[1];
				expect(init.credentials).toEqual("same-origin");
			});

			it("resolves with undefined if bad request", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath, 400);

				await expect(authAPI.getCurrentAuth())
					.resolves.toEqual(undefined);
			});

			it("resolves with undefined if unauthorized", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath, 401);

				await expect(authAPI.getCurrentAuth())
					.resolves.toEqual(undefined);
			});

			it("resolves to auth response", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath, authResponse);

				await expect(authAPI.getCurrentAuth())
					.resolves.toEqual(authResponse);
			});
		});

		describe("changePassword", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().patchOnce(basePath, {});
				window.fetch = mock;

				await authAPI.changePassword(token, password, newPassword);

				const init = mock.lastCall()[1];
				// tslint:disable-next-line:no-string-literal
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(`Bearer ${token}`);
				expect(JSON.parse(init.body as string)).toEqual(changePasswordBody);
			});

			it("rejects with bad request", async () => {
				window.fetch = fetchMock.sandbox().patchOnce(basePath, 400);

				await expect(authAPI.changePassword(token, password, newPassword))
					.rejects.toEqual(badRequest);
			});

			it("rejests with incorrect password", async () => {
				window.fetch = fetchMock.sandbox().patchOnce(basePath, 401);

				await expect(authAPI.changePassword(token, password, newPassword))
					.rejects.toEqual(incorrectPassword);
			});

			it("resolves to auth response", async () => {
				window.fetch = fetchMock.sandbox().patchOnce(basePath, authResponse);

				await expect(authAPI.changePassword(token, password, newPassword))
					.resolves.toEqual(authResponse);
			});
		});

		describe("changeUsername", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().patchOnce(basePath, {});
				window.fetch = mock;

				await authAPI.changeUsername(token, password, newUsername);

				const init = mock.lastCall()[1];
				// tslint:disable-next-line:no-string-literal
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(`Bearer ${token}`);
				expect(JSON.parse(init.body as string)).toEqual(changeUsernameBody);
			});

			it("rejects with bad request", async () => {
				window.fetch = fetchMock.sandbox().patchOnce(basePath, 400);

				await expect(authAPI.changeUsername(token, password, newUsername))
					.rejects.toEqual(badRequest);
			});

			it("rejests with incorrect password", async () => {
				window.fetch = fetchMock.sandbox().patchOnce(basePath, 401);

				await expect(authAPI.changeUsername(token, password, newUsername))
					.rejects.toEqual(incorrectPassword);
			});

			it("resolves to auth response", async () => {
				window.fetch = fetchMock.sandbox().patchOnce(basePath, authResponse);

				await expect(authAPI.changeUsername(token, password, newUsername))
					.resolves.toEqual(authResponse);
			});
		});

		describe("logout", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().getOnce(basePath + "/logout", 204);
				window.fetch = mock;

				await authAPI.logout();

				const init = mock.lastCall()[1];
				expect(init.credentials).toEqual("same-origin");
			});

			it("resolves", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath + "/logout", 204);

				await expect(authAPI.logout())
					.resolves.toEqual(undefined);
			});
		});

		describe("logoutAll", () => {
			it("calls fetch with proper arguments", async () => {
				const mock = fetchMock.sandbox().getOnce(basePath + "/logoutall", 204);
				window.fetch = mock;

				await authAPI.logoutAll(token);

				const init = mock.lastCall()[1];
				// tslint:disable-next-line:no-string-literal
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(`Bearer ${token}`);
			});

			it("resolves", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath + "/logoutall", 204);

				await expect(authAPI.logoutAll(token))
					.resolves.toEqual(undefined);
			});
		});
	});
});
