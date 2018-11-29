import React from "react"
import TestRenderer from "react-test-renderer"

import { ChecklistController, ChecklistView, ChecklistViewProps } from "./Checklist"
import { NoteType, Checklist } from "../Note"
import { NoteContext } from "../context";
import { BrowserRouter } from "react-router-dom";
import { BaseNoteController } from "./BaseNote";

const NoteServiceProvider = NoteContext.Provider

let note: Checklist

let root: JSX.Element
let renderer: TestRenderer.ReactTestRenderer

function render() {
	root = (
		<BrowserRouter>
			<NoteServiceProvider value={{} as any}>
				<ChecklistController note={note} />
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

describe("ChecklistController", () => {
	beforeEach(() => {
		note = {
			id: Date.now() + "",
			type: NoteType.Checklist,
			title: Date.now() + "title",
			items: [
				{ checked: false, text: "text1" },
				{ checked: true, text: "text2" },
				{ checked: false, text: "text3" },
				{ checked: false, text: "text4" }
			],
			archived: (Date.now() & 1) == 0
		}
	})

	it("renders the ChecklistView with correct initial props", () => {
		const { items, title, archived } = note
		render()
		const props = findByType(ChecklistView).props as ChecklistViewProps
		expect(props).toMatchObject({ items, title, archived })
		expect(props.addItem).toBeInstanceOf(Function)
		expect(props.removeItem).toBeInstanceOf(Function)
		expect(props.setItemChecked).toBeInstanceOf(Function)
		expect(props.updateItemText).toBeInstanceOf(Function)
		expect(props.updateTitle).toBeInstanceOf(Function)
		expect(props.setArchived).toBeInstanceOf(Function)
		expect(props.deleteNote).toBeInstanceOf(Function)
	})

	it("addItem calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		render()
		const { addItem } = findByType(ChecklistView).props as ChecklistViewProps
		addItem()
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({
			...note, items: [
				{ checked: false, text: "text1" },
				{ checked: true, text: "text2" },
				{ checked: false, text: "text3" },
				{ checked: false, text: "text4" },
				{ checked: false, text: "" }
			]
		})
		BaseNoteController.prototype.update = _update
	})

	it("removeItem calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		render()
		const { removeItem } = findByType(ChecklistView).props as ChecklistViewProps
		removeItem(2)
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({
			...note, items: [
				{ checked: false, text: "text1" },
				{ checked: true, text: "text2" },
				{ checked: false, text: "text4" }
			]
		})
		BaseNoteController.prototype.update = _update
	})

	it("setItemChecked calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		render()
		const { setItemChecked } = findByType(ChecklistView).props as ChecklistViewProps
		setItemChecked(2, true)
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({
			...note, items: [
				{ checked: false, text: "text1" },
				{ checked: true, text: "text2" },
				{ checked: true, text: "text3" },
				{ checked: false, text: "text4" }
			]
		})
		BaseNoteController.prototype.update = _update
	})

	it("updateItemText calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		render()
		const { updateItemText } = findByType(ChecklistView).props as ChecklistViewProps
		updateItemText(1, "newtext")
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({
			...note, items: [
				{ checked: false, text: "text1" },
				{ checked: true, text: "newtext" },
				{ checked: false, text: "text3" },
				{ checked: false, text: "text4" }
			]
		})
		BaseNoteController.prototype.update = _update
	})

	it("updateTitle calls super.updateTitle correctly", async () => {
		const mockUpdateTitle = jest.fn()
		const _updateTitle = BaseNoteController.prototype.updateTitle
		BaseNoteController.prototype.updateTitle = mockUpdateTitle
		const title = "supercoolnewtitle"
		render()
		const { updateTitle } = findByType(ChecklistView).props as ChecklistViewProps
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
		const { setArchived } = findByType(ChecklistView).props as ChecklistViewProps
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
		const { deleteNote } = findByType(ChecklistView).props as ChecklistViewProps
		deleteNote()
		expect(mockDeleteNote).toBeCalledTimes(1)
		BaseNoteController.prototype.deleteNote = _deleteNote
	})

	it("renders according to current state, using super.render", async () => {
		render()
		const instance = findByType(ChecklistController).instance as ChecklistController
		expect(instance.render).toBe(BaseNoteController.prototype.render)
		const title = Date.now() + "title"
		const items = [
			{ checked: true, text: "te  xt1" },
			{ checked: false, text: "tesdfsazdfxt2" },
			{ checked: true, text: "tegasdfgfxt3" }
		]
		const archived = (Date.now() & 1) == 0
		const error = "::::PPPPPP"
		instance.setState({
			note: { ...note, title, items, archived },
			error
		})
		update()
		expect(findByType(ChecklistView).props).toMatchObject({ title, items, archived, error })
	})
})