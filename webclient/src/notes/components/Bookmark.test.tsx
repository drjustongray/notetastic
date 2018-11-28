import React from "react"
import TestRenderer from "react-test-renderer"

import { BookmarkController, BookmarkView, BookmarkViewProps } from "./Bookmark"
import { Bookmark, NoteType } from "../Note"
import { NoteService } from "../noteService"
import { NoteContext } from "../context";
import { NOTES } from "../../pages/links";
import { Redirect, BrowserRouter } from "react-router-dom";
import Loading from "../../components/Loading";
import ErrorView from "../../components/Error";

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
				<BookmarkController note={note} />
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

describe("BookmarkController", () => {
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

	it("renders the BookmarkView with correct initial props", () => {
		const { url, title, archived } = note
		render()
		const props = findByType(BookmarkView).props as BookmarkViewProps
		expect(props).toMatchObject({ url, title, archived })
		expect(props.updateURL).toBeInstanceOf(Function)
		expect(props.updateTitle).toBeInstanceOf(Function)
		expect(props.setArchived).toBeInstanceOf(Function)
		expect(props.deleteNote).toBeInstanceOf(Function)
	})

	it("updateURL saves the new version of the note and updates view", async () => {
		const { title, archived } = note
		const url = "supercoolnewurl"
		render()
		const { updateURL } = findByType(BookmarkView).props as BookmarkViewProps
		updateURL(url)
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenCalledWith({ ...note, url })
		update()
		const props = findByType(BookmarkView).props as BookmarkViewProps
		expect(props).toMatchObject({ url, title, archived })
	})

	it("updateTitle saves the new version of the note and updates view", async () => {
		const { url, archived } = note
		const title = "supercoolnewtitle"
		render()
		const { updateTitle } = findByType(BookmarkView).props as BookmarkViewProps
		updateTitle(title)
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenCalledWith({ ...note, title })
		update()
		const props = findByType(BookmarkView).props as BookmarkViewProps
		expect(props).toMatchObject({ url, title, archived })
	})

	it("setArchived saves the new version of the note and updates view", async () => {
		const { url, title } = note
		const archived = !note.archived
		render()
		const { setArchived } = findByType(BookmarkView).props as BookmarkViewProps
		setArchived(archived)
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenCalledWith({ ...note, archived })
		update()
		const props = findByType(BookmarkView).props as BookmarkViewProps
		expect(props).toMatchObject({ url, title, archived })
	})

	it("delete deletes note and renders loading, redirect", async () => {
		noteService.deleteNote = jest.fn(() => Promise.resolve())
		render()
		const { deleteNote } = findByType(BookmarkView).props as BookmarkViewProps
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
		const { setArchived, updateTitle, updateURL } = findByType(BookmarkView).props as BookmarkViewProps
		setArchived(archived)
		updateTitle(title)
		updateURL(url)
		expect(saveNote).toBeCalledTimes(1)
		expect(saveNote).toHaveBeenLastCalledWith({ ...note, archived })
		await new Promise(resolve => setTimeout(resolve, 1000))
		expect(saveNote).toBeCalledTimes(2)
		expect(saveNote).toHaveBeenLastCalledWith({ ...note, url, title, archived })
	})

	it("displays errors as necessary", async () => {
		const message = "an error!"
		noteService.saveNote = jest.fn(() => Promise.reject(new Error(message)))
		render()
		const { setArchived, deleteNote } = findByType(BookmarkView).props as BookmarkViewProps
		setArchived(true)
		await Promise.resolve()
		update()
		expect(findByType(ErrorView).props).toEqual({ message })
		noteService.saveNote = saveNote
		setArchived(false)
		await Promise.resolve()
		update()
		expect(renderer.root.findAllByType(ErrorView)).toHaveLength(0)

		noteService.deleteNote = () => Promise.reject(new Error(message))
		deleteNote()
		await Promise.resolve()
		update()
		expect(findByType(ErrorView).props).toEqual({ message })
		noteService.deleteNote = () => Promise.resolve()
		deleteNote()
		await Promise.resolve()
		update()
		expect(renderer.root.findAllByType(ErrorView)).toHaveLength(0)
	})
})