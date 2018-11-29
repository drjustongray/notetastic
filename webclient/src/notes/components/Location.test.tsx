import React from "react"
import TestRenderer from "react-test-renderer"

import { LocationController, LocationView, LocationViewProps } from "./Location"
import { NoteType, Location } from "../Note"
import { NoteContext } from "../context";
import { BrowserRouter } from "react-router-dom";
import { BaseNoteController } from "./BaseNote";

const NoteServiceProvider = NoteContext.Provider

let note: Location

let root: JSX.Element
let renderer: TestRenderer.ReactTestRenderer

function createNodeMock(element: React.ReactElement<any>) {
	if (element.type === "div") {
		return document.createElement("div")
	}
}

function render() {
	root = (
		<BrowserRouter>
			<NoteServiceProvider value={{} as any}>
				<LocationController note={note} />
			</NoteServiceProvider>
		</BrowserRouter>
	)
	renderer = TestRenderer.create(root, { createNodeMock })
}

function update() {
	renderer.update(root)
}

function findByType(type: React.ReactType<any>) {
	return renderer.root.findByType(type)
}

describe("LocationController", () => {
	beforeEach(() => {
		note = {
			id: Date.now() + "",
			type: NoteType.Location,
			title: Date.now() + "title",
			longitude: 67,
			latitude: -34,
			archived: (Date.now() & 1) == 0
		}
	})

	it("renders the LocationView with correct initial props", () => {
		const { longitude, latitude, title, archived } = note
		render()
		const props = findByType(LocationView).props as LocationViewProps
		expect(props).toMatchObject({ longitude, latitude, title, archived })
		expect(props.setLocation).toBeInstanceOf(Function)
		expect(props.setToCurrentLocation).toBeInstanceOf(Function)
		expect(props.updateTitle).toBeInstanceOf(Function)
		expect(props.setArchived).toBeInstanceOf(Function)
		expect(props.deleteNote).toBeInstanceOf(Function)
	})

	it("sets geoAvailable correctly", () => {
		(navigator as any).geolocation = {}
		render()
		expect(findByType(LocationView).props).toMatchObject({ geoAvailable: true })
		delete (navigator as any).geolocation
		render()
		expect(findByType(LocationView).props).toMatchObject({ geoAvailable: false })
	})

	it("setLocation calls super.update correctly", async () => {
		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		const latitude = -63
		const longitude = 23
		render()
		const { setLocation } = findByType(LocationView).props as LocationViewProps
		setLocation(latitude, longitude)
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({ ...note, latitude, longitude })
		BaseNoteController.prototype.update = _update
	})

	it("setToCurrentLocation uses browser geolocation api and calls super.update correctly", () => {
		const latitude = -6
		const longitude = 3
		const getCurrentPosition = (successCallback: (pos: Position) => any) => successCallback({ coords: { latitude, longitude } } as Position);
		(navigator as any).geolocation = { getCurrentPosition }

		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		render()
		const { setToCurrentLocation } = findByType(LocationView).props as LocationViewProps
		setToCurrentLocation()
		expect(updateNote).toBeCalledTimes(1)
		expect(updateNote).toHaveBeenCalledWith({ ...note, latitude, longitude })
		BaseNoteController.prototype.update = _update
	})

	it("setToCurrentLocation sets error state when getCurrentPosition fails, does not call super.update", () => {
		const getCurrentPosition = (_: (pos: Position) => any, failureCallback: (error: PositionError) => any) => failureCallback({} as PositionError);
		(navigator as any).geolocation = { getCurrentPosition }

		const updateNote = jest.fn()
		const _update = BaseNoteController.prototype.update
		BaseNoteController.prototype.update = updateNote
		render()
		const { setToCurrentLocation } = findByType(LocationView).props as LocationViewProps
		setToCurrentLocation()
		expect(updateNote).toBeCalledTimes(0)
		update()
		expect(findByType(LocationController).instance.state).toMatchObject({ error: "Could not retrieve your current location" })
		BaseNoteController.prototype.update = _update
	})

	it("updateTitle calls super.updateTitle correctly", async () => {
		const mockUpdateTitle = jest.fn()
		const _updateTitle = BaseNoteController.prototype.updateTitle
		BaseNoteController.prototype.updateTitle = mockUpdateTitle
		const title = "supercoolnewtitle"
		render()
		const { updateTitle } = findByType(LocationView).props as LocationViewProps
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
		const { setArchived } = findByType(LocationView).props as LocationViewProps
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
		const { deleteNote } = findByType(LocationView).props as LocationViewProps
		deleteNote()
		expect(mockDeleteNote).toBeCalledTimes(1)
		BaseNoteController.prototype.deleteNote = _deleteNote
	})

	it("renders according to current state, using super.render", async () => {
		render()
		const instance = findByType(LocationController).instance as LocationController
		expect(instance.render).toBe(BaseNoteController.prototype.render)
		const title = Date.now() + "title"
		const latitude = -63
		const longitude = 23
		const archived = (Date.now() & 1) == 0
		const error = "::::PPPPPP"
		instance.setState({
			note: { ...note, title, archived, latitude, longitude },
			error
		})
		update()
		expect(findByType(LocationView).props).toMatchObject({ title, latitude, longitude, archived, error })
	})
})