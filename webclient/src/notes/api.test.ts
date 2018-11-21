import { noteAPI } from "./api"
import fetchMock from "fetch-mock"
import { NETWORK_ERROR, BAD_REQUEST, NOT_FOUND, UNAUTHORIZED } from "../api/rejectedPromises";
import { Note, NoteType } from "./Note";

describe("noteAPI", () => {
	describe("when requeset cannot reach server", () => {
		const expectedError = { message: NETWORK_ERROR }
		window.fetch = jest.fn(() => Promise.reject("anything"))

		describe("putNote", () => {
			it("rejects with Network Error", async () => {
				await expect(noteAPI.putNote("", {} as Note)).rejects.toEqual(expectedError)
			})
		})

		describe("getNote", () => {
			it("rejects with Network Error", async () => {
				await expect(noteAPI.getNote("", "id")).rejects.toEqual(expectedError)
			})
		})

		describe("getNoteList", () => {
			it("rejects with Network Error", async () => {
				await expect(noteAPI.getNoteList("")).rejects.toEqual(expectedError)
			})
		})

		describe("deleteNote", () => {
			it("rejects with Network Error", async () => {
				await expect(noteAPI.deleteNote("", "id")).rejects.toEqual(expectedError)
			})
		})
	})

	describe("when request can reach server", () => {
		const noteList: Array<Note> = [
			{ id: "id1", title: "title1", type: NoteType.Bookmark, archived: false },
			{ id: "id2", title: "title2", type: NoteType.Checklist, archived: true },
			{ id: "id3", title: "title3", type: NoteType.Location, archived: false }
		]
		const note: Note = {
			id: "id",
			title: "title",
			type: NoteType.TextNote,
			archived: true
		}

		const badRequest = { message: BAD_REQUEST }
		const notFound = { message: NOT_FOUND }
		const unauthorized = { message: UNAUTHORIZED }

		const basePath = "/api/notes"
		const noteId = "id"
		const notePath = "/api/notes/id"

		const token = "tokentastic"
		const authHeader = `Bearer ${token}`

		describe("putNote", () => {
			it("calls fetch with correct arguments", async () => {
				const mock = fetchMock.sandbox().putOnce(basePath, note)
				window.fetch = mock
				try {
					await noteAPI.putNote(token, note)
				} catch (e) { }
				const init = mock.lastCall()[1]
				expect(JSON.parse(init.body as string)).toEqual(note)
				expect((init.headers as Record<string, string>)["Content-Type"]).toMatch("application/json")
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(authHeader)
			})

			it("handles bad request with rejection", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, 400)

				await expect(noteAPI.putNote(token, note)).rejects.toEqual(badRequest)
			})

			it("handles unauthorized with rejection", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, 401)

				await expect(noteAPI.putNote(token, note)).rejects.toEqual(unauthorized)
			})

			it("handles not found with rejection", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, 404)

				await expect(noteAPI.putNote(token, note)).rejects.toEqual(notFound)
			})

			it("handles ok with resolution", async () => {
				window.fetch = fetchMock.sandbox().putOnce(basePath, { ...note, Id: "newid" })

				await expect(noteAPI.putNote(token, note)).resolves.toEqual({ ...note, Id: "newid" })
			})
		})

		describe("getNote", () => {
			it("calls fetch with correct arguments", async () => {
				const mock = fetchMock.sandbox().getOnce(notePath, note)
				window.fetch = mock
				try {
					await noteAPI.getNote(token, noteId)
				} catch (e) { }
				const init = mock.lastCall()[1]
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(authHeader)
			})

			it("handles unauthorized with rejection", async () => {
				window.fetch = fetchMock.sandbox().getOnce(notePath, 401)

				await expect(noteAPI.getNote(token, noteId)).rejects.toEqual(unauthorized)
			})

			it("handles not found with rejection", async () => {
				window.fetch = fetchMock.sandbox().getOnce(notePath, 404)

				await expect(noteAPI.getNote(token, noteId)).rejects.toEqual(notFound)
			})

			it("handles ok with resolution", async () => {
				window.fetch = fetchMock.sandbox().getOnce(notePath, note)

				await expect(noteAPI.getNote(token, noteId)).resolves.toEqual(note)
			})
		})

		describe("getNoteList", () => {
			it("calls fetch with correct arguments", async () => {
				const mock = fetchMock.sandbox().getOnce(basePath, noteList)
				window.fetch = mock
				try {
					await noteAPI.getNoteList(token)
				} catch (e) { }
				const init = mock.lastCall()[1]
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(authHeader)
			})

			it("handles unauthorized with rejection", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath, 401)

				await expect(noteAPI.getNoteList(token)).rejects.toEqual(unauthorized)
			})

			it("handles ok with resolution", async () => {
				window.fetch = fetchMock.sandbox().getOnce(basePath, noteList)

				await expect(noteAPI.getNoteList(token)).resolves.toEqual(noteList)
			})
		})

		describe("deleteNote", () => {
			it("calls fetch with correct arguments", async () => {
				const mock = fetchMock.sandbox().deleteOnce(notePath, 200)
				window.fetch = mock
				try {
					await noteAPI.deleteNote(token, noteId)
				} catch (e) { }
				const init = mock.lastCall()[1]
				expect((init.headers as Record<string, string>)["Authorization"]).toEqual(authHeader)
			})

			it("handles unauthorized with rejection", async () => {
				window.fetch = fetchMock.sandbox().deleteOnce(notePath, 401)

				await expect(noteAPI.deleteNote(token, noteId)).rejects.toEqual(unauthorized)
			})

			it("handles not found with rejection", async () => {
				window.fetch = fetchMock.sandbox().deleteOnce(notePath, 404)

				await expect(noteAPI.deleteNote(token, noteId)).rejects.toEqual(notFound)
			})

			it("handles ok with resolution", async () => {
				window.fetch = fetchMock.sandbox().deleteOnce(notePath, 200)

				await expect(noteAPI.deleteNote(token, noteId)).resolves.toEqual(undefined)
			})
		})
	})
})