import React from "react"
import TestRenderer from "react-test-renderer"

import { TextNoteController, TextNoteView, TextNoteViewProps } from "./TextNote"
import { NoteType, TextNote } from "../Note"
import { NoteContext } from "../context";
import { BrowserRouter } from "react-router-dom";
import { BaseNoteController } from "./BaseNote";

const NoteServiceProvider = NoteContext.Provider

let note: TextNote

let root: JSX.Element
let renderer: TestRenderer.ReactTestRenderer

function render() {
	root = (
		<BrowserRouter>
			<NoteServiceProvider value={{} as any}>
				<TextNoteController note={note} />
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

describe("TextNoteController", () => {
	beforeEach(() => {
		note = {
			id: Date.now() + "",
			type: NoteType.TextNote,
			title: Date.now() + "title",
			text: Date.now() + "text",
			archived: (Date.now() & 1) == 0
		}
	})

	it("renders the TextNoteView with correct initial props", () => {
		const { text, title, archived } = note
		render()
		const props = findByType(TextNoteView).props as TextNoteViewProps
		expect(props).toMatchObject({ text, title, archived })
		expect(props.updateText).toBeInstanceOf(Function)
		expect(props.updateTitle).toBeInstanceOf(Function)
		expect(props.setArchived).toBeInstanceOf(Function)
		expect(props.deleteNote).toBeInstanceOf(Function)
	})

	it("updateText calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		const text = "supercoolnewurl"
		render()
		const { updateText } = findByType(TextNoteView).props as TextNoteViewProps
		updateText(text)
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({ ...note, text })
		BaseNoteController.prototype.update = _update
	})

	it("updateTitle calls super.updateTitle correctly", async () => {
		const mockUpdateTitle = jest.fn()
		const _updateTitle = BaseNoteController.prototype.updateTitle
		BaseNoteController.prototype.updateTitle = mockUpdateTitle
		const title = "supercoolnewtitle"
		render()
		const { updateTitle } = findByType(TextNoteView).props as TextNoteViewProps
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
		const { setArchived } = findByType(TextNoteView).props as TextNoteViewProps
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
		const { deleteNote } = findByType(TextNoteView).props as TextNoteViewProps
		deleteNote()
		expect(mockDeleteNote).toBeCalledTimes(1)
		BaseNoteController.prototype.deleteNote = _deleteNote
	})

	it("renders according to current state, using super.render", async () => {
		render()
		const instance = findByType(TextNoteController).instance as TextNoteController
		expect(instance.render).toBe(BaseNoteController.prototype.render)
		const title = Date.now() + "title"
		const text = Date.now() + "text"
		const archived = (Date.now() & 1) == 0
		const error = "::::PPPPPP"
		instance.setState({
			note: { ...note, title, text, archived },
			error
		})
		update()
		expect(findByType(TextNoteView).props).toMatchObject({ title, text, archived, error })
	})
})