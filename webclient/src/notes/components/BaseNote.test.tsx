import React from "react"
import TestRenderer from "react-test-renderer"

import { BaseNoteController } from "./BaseNote"
import { NoteContext } from "../context";
import { Bookmark, NoteType } from "../Note";
import { NoteService } from "../noteService";
import { BrowserRouter, Redirect } from "react-router-dom";
import ErrorView from "../../components/Error"
import Loading from "../../components/Loading";
import { NOTES } from "../../pages/links";

const NoteView = () => <div></div>

class TestController extends BaseNoteController<Bookmark>{
	renderNoteView(): React.ReactNode {
		return <NoteView />
	}
}

const NoteServiceProvider = NoteContext.Provider

let saveNote: jest.Mock<Promise<void>>
let noteService: NoteService
let note: Bookmark

let root: JSX.Element
let renderer: TestRenderer.ReactTestRenderer

function render() {
	root = (
		<BrowserRouter>
			<NoteServiceProvider value={noteService}>
				<TestController note={note} />
			</NoteServiceProvider>
		</BrowserRouter>
	)
	renderer = TestRenderer.create(root)
}

function update() {
	renderer.update(root)
}

function findByType(type: React.ReactType<any>) {
	return renderer.root.findByType(type)
}

function findController() {
	return findByType(TestController)
}

describe("BaseNoteController", () => {
	beforeEach(() => {
		saveNote = jest.fn(() => Promise.resolve())
		noteService = { saveNote } as any
		note = {
			id: Date.now() + "",
			type: NoteType.Bookmark,
			title: Date.now() + "title",
			url: Date.now() + "url",
			archived: (Date.now() & 1) == 0
		}
	})

	it("initialy has correct state and renders the note view", () => {
		render()
		expect(findController().instance.state).toEqual({ note })
		findByType(NoteView)
	})

	it("updateTitle saves the new version of the note and updates state", async () => {
		const title = "supercoolnewtitle"
		render()
		const { updateTitle } = findController().instance as TestController
		updateTitle(title)
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenCalledWith({ ...note, title })
		update()
		expect(findController().instance.state).toMatchObject({ note: { ...note, title } })
	})

	it("setArchived saves the new version of the note and updates state", async () => {
		const archived = !note.archived
		render()
		const { setArchived } = findController().instance as TestController
		setArchived(archived)
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenCalledWith({ ...note, archived })
		update()
		expect(findController().instance.state).toMatchObject({ note: { ...note, archived } })
	})

	it("update saves the new version of the note and updates state", async () => {
		const url = "newurl"
		render();
		(findController().instance as TestController).update({ ...note, url })
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenCalledWith({ ...note, url })
		update()
		expect(findController().instance.state).toMatchObject({ note: { ...note, url } })
	})

	it("delete deletes note and renders loading, redirect", async () => {
		noteService.deleteNote = jest.fn(() => Promise.resolve())
		render()
		const { deleteNote } = findController().instance as TestController
		deleteNote()
		update()
		expect(noteService.deleteNote).toBeCalledTimes(1)
		expect(noteService.deleteNote).toHaveBeenCalledWith(note.id)
		findByType(Loading)
		await Promise.resolve()
		update()
		const props = findByType(Redirect).props
		expect(props).toMatchObject({ to: NOTES })
	})

	it("does not save the note while a previous save is pending", async () => {
		const url = "newurl"
		const title = "newtitle"
		const archived = !note.archived
		render()
		const controller = findController().instance as TestController
		const { setArchived, updateTitle } = controller
		setArchived(archived)
		updateTitle(title)
		controller.update({ ...note, archived, title, url })
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenLastCalledWith({ ...note, archived })
		await new Promise(resolve => setTimeout(resolve, 1000))
		expect(saveNote).toBeCalledTimes(2)
		expect(saveNote).toHaveBeenLastCalledWith({ ...note, url, title, archived })
	})

	it("sets error state as necessary", async () => {
		const message = "an error!"
		noteService.saveNote = jest.fn(() => Promise.reject(new Error(message)))
		render()
		const { setArchived, deleteNote } = findController().instance as TestController
		setArchived(true)
		await Promise.resolve()
		update()
		expect(findController().instance.state).toMatchObject({ note: { ...note, archived: true }, error: message })
		noteService.saveNote = saveNote
		setArchived(false)
		await Promise.resolve()
		update()
		expect(findController().instance.state).toMatchObject({ note: { ...note, archived: false }, error: null })

		noteService.deleteNote = () => Promise.reject(new Error(message))
		deleteNote()
		await Promise.resolve()
		update()
		expect(findController().instance.state).toMatchObject({ note: { ...note, archived: false }, error: message })
		noteService.deleteNote = () => Promise.resolve()
		deleteNote()
		await Promise.resolve()
		update()
		expect(findController().instance.state).toMatchObject({ error: null })
	})
})