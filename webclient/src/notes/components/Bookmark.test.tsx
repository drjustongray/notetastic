import React from "react"
import TestRenderer from "react-test-renderer"

import { BookmarkController, BookmarkView, BookmarkViewProps } from "./Bookmark"
import { Bookmark, NoteType } from "../Note"
import { NoteContext } from "../context";
import { BrowserRouter } from "react-router-dom";
import { BaseNoteController } from "./BaseNote";

const NoteServiceProvider = NoteContext.Provider

let note: Bookmark

let root: JSX.Element
let renderer: TestRenderer.ReactTestRenderer

function render() {
	root = (
		<BrowserRouter>
			<NoteServiceProvider value={{} as any}>
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

	it("updateURL calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		const url = "supercoolnewurl"
		render()
		const { updateURL } = findByType(BookmarkView).props as BookmarkViewProps
		updateURL(url)
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({ ...note, url })
		BaseNoteController.prototype.update = _update
	})

	it("updateTitle calls super.updateTitle correctly", async () => {
		const mockUpdateTitle = jest.fn()
		const _updateTitle = BaseNoteController.prototype.updateTitle
		BaseNoteController.prototype.updateTitle = mockUpdateTitle
		const title = "supercoolnewtitle"
		render()
		const { updateTitle } = findByType(BookmarkView).props as BookmarkViewProps
		updateTitle(title)
		expect(mockUpdateTitle).toBeCalledTimes(1)
		expect(mockUpdateTitle).toHaveBeenCalledWith(title)
		BaseNoteController.prototype.updateTitle = _updateTitle
	})

	it("setArchived calls super.setArchived correctly", async () => {
		const archived = !note.archived
		const mockSetArchived = jest.fn()
		const _setArchived = BaseNoteController.prototype.setArchived
		BaseNoteController.prototype.setArchived = mockSetArchived
		render()
		const { setArchived } = findByType(BookmarkView).props as BookmarkViewProps
		setArchived(archived)
		expect(mockSetArchived).toBeCalledTimes(1)
		expect(mockSetArchived).toHaveBeenCalledWith(archived)
		BaseNoteController.prototype.setArchived = _setArchived
	})

	it("deleteNote calls super.deleteNote correctly", async () => {
		const mockDeleteNote = jest.fn()
		const _deleteNote = BaseNoteController.prototype.deleteNote
		BaseNoteController.prototype.deleteNote = mockDeleteNote
		render()
		const { deleteNote } = findByType(BookmarkView).props as BookmarkViewProps
		deleteNote()
		expect(mockDeleteNote).toBeCalledTimes(1)
		BaseNoteController.prototype.deleteNote = _deleteNote
	})

	it("renders according to current state, using super.render", async () => {
		render()
		const instance = findByType(BookmarkController).instance as BookmarkController
		expect(instance.render).toBe(BaseNoteController.prototype.render)
		const title = Date.now() + "title"
		const url = Date.now() + "url"
		const archived = (Date.now() & 1) == 0
		const error = "::::PPPPPP"
		instance.setState({
			note: { ...note, title, url, archived },
			error
		})
		update()
		expect(findByType(BookmarkView).props).toMatchObject({ title, url, archived, error })
	})
})