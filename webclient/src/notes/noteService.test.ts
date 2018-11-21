import { makeNoteService } from "./noteService"
import { Note, NoteType } from "./Note"

describe("makeNoteService", () => {
	const note: Note = {
		id: "whatev",
		title: "tiittle",
		type: NoteType.Bookmark,
		archived: false,
		url: "url"
	}
	const NO_TOKEN = { message: "no token:(" }
	const getTokenRejection = Promise.reject(NO_TOKEN)
	const REQUEST_FAILED = { message: "request failed" }
	const apiRejection = Promise.reject(REQUEST_FAILED)
	let getAccessToken: () => Promise<string>
	let token: string
	beforeEach(() => {
		token = Date.now() + "token"
		getAccessToken = () => Promise.resolve(token)
	})

	describe("saveNote", () => {

		it("call api function with proper arguments", async () => {
			const putNote = jest.fn(() => Promise.resolve())
			const noteService = makeNoteService({ putNote } as any, getAccessToken)
			await noteService.saveNote(note)
			expect(putNote).toHaveBeenCalledWith(token, note)
		})

		it("resolves to what the api function does", async () => {
			const putNote = jest.fn(() => Promise.resolve({ ...note, Id: "id" }))
			const noteService = makeNoteService({ putNote } as any, getAccessToken)
			await expect(noteService.saveNote(note)).resolves.toEqual({ ...note, Id: "id" })
		})

		it("rejects when api function does", async () => {
			const putNote = jest.fn(() => apiRejection)
			const noteService = makeNoteService({ putNote } as any, getAccessToken)
			await expect(noteService.saveNote(note)).rejects.toEqual(REQUEST_FAILED)
		})

		it("rejects when getAccessToken does", async () => {
			getAccessToken = () => getTokenRejection
			const noteService = makeNoteService({} as any, getAccessToken)
			await expect(noteService.saveNote(note)).rejects.toEqual(NO_TOKEN)
		})
	})

	describe("getNote", () => {

		it("call api function with proper arguments", async () => {
			const getNote = jest.fn(() => Promise.resolve())
			const noteService = makeNoteService({ getNote } as any, getAccessToken)
			await noteService.getNote("id")
			expect(getNote).toHaveBeenCalledWith(token, "id")
		})

		it("resolves to what the api function does", async () => {
			const getNote = jest.fn(() => Promise.resolve(note))
			const noteService = makeNoteService({ getNote } as any, getAccessToken)
			await expect(noteService.getNote("id")).resolves.toEqual(note)
		})

		it("rejects when api function does", async () => {
			const getNote = jest.fn(() => apiRejection)
			const noteService = makeNoteService({ getNote } as any, getAccessToken)
			await expect(noteService.getNote("id")).rejects.toEqual(REQUEST_FAILED)
		})

		it("rejects when getAccessToken does", async () => {
			getAccessToken = () => getTokenRejection
			const noteService = makeNoteService({} as any, getAccessToken)
			await expect(noteService.getNote("id")).rejects.toEqual(NO_TOKEN)
		})
	})

	describe("getNoteList", () => {

		const noteList: Array<Note> = [
			{ id: "id1", title: "title1", type: NoteType.TextNote, archived: false, text: "text" },
			{ id: "id2", title: "title2", type: NoteType.Location, archived: false, longitude: 9, latitude: 89 },
			{ id: "id3", title: "title3", type: NoteType.Checklist, archived: true, items: [] }
		]

		it("call api function with proper arguments", async () => {
			const getNoteList = jest.fn(() => Promise.resolve())
			const noteService = makeNoteService({ getNoteList } as any, getAccessToken)
			await noteService.getNoteList()
			expect(getNoteList).toHaveBeenCalledWith(token)
		})

		it("resolves to what the api function does", async () => {
			const getNoteList = jest.fn(() => Promise.resolve(noteList))
			const noteService = makeNoteService({ getNoteList } as any, getAccessToken)
			await expect(noteService.getNoteList()).resolves.toEqual(noteList)
		})

		it("rejects when api function does", async () => {
			const getNoteList = jest.fn(() => apiRejection)
			const noteService = makeNoteService({ getNoteList } as any, getAccessToken)
			await expect(noteService.getNoteList()).rejects.toEqual(REQUEST_FAILED)
		})

		it("rejects when getAccessToken does", async () => {
			getAccessToken = () => getTokenRejection
			const noteService = makeNoteService({} as any, getAccessToken)
			await expect(noteService.getNoteList()).rejects.toEqual(NO_TOKEN)
		})
	})

	describe("deleteNote", () => {

		it("call api function with proper arguments", async () => {
			const deleteNote = jest.fn(() => Promise.resolve())
			const noteService = makeNoteService({ deleteNote } as any, getAccessToken)
			await noteService.deleteNote("id")
			expect(deleteNote).toHaveBeenCalledWith(token, "id")
		})

		it("resolves to what the api function does", async () => {
			const deleteNote = jest.fn(() => Promise.resolve())
			const noteService = makeNoteService({ deleteNote } as any, getAccessToken)
			await expect(noteService.deleteNote("id")).resolves.toEqual(undefined)
		})

		it("rejects when api function does", async () => {
			const deleteNote = jest.fn(() => apiRejection)
			const noteService = makeNoteService({ deleteNote } as any, getAccessToken)
			await expect(noteService.deleteNote("id")).rejects.toEqual(REQUEST_FAILED)
		})

		it("rejects when getAccessToken does", async () => {
			getAccessToken = () => getTokenRejection
			const noteService = makeNoteService({} as any, getAccessToken)
			await expect(noteService.deleteNote("id")).rejects.toEqual(NO_TOKEN)
		})
	})
})