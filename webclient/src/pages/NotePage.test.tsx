import React from "react"
import TestRenderer from "react-test-renderer"

import NotePageWithRouteParams, { NotePage, WrappedNotePage } from "./NotePage"
import { NoteType, TextNote, Checklist, Location, Bookmark, Note } from "../notes/Note";
import { NoteService } from "../notes/noteService";
import { AuthContext } from "../auth/context";
import { NoteContext } from "../notes/context";
import Loading from "../components/Loading";
import Error from "../components/Error";
import { BehaviorSubject } from "rxjs";
import { AuthState, AuthService } from "../auth/authService";
import User from "../auth/User";
import TabbedAuthForm from "../auth/components/TabbedAuthForm";
import { TextNoteController } from "../notes/components/TextNote";
import { BookmarkController } from "../notes/components/Bookmark";
import { ChecklistController } from "../notes/components/Checklist";
import { LocationController } from "../notes/components/Location";
import { RouteComponentProps } from "react-router";

const NoteServiceProvider = NoteContext.Provider
const AuthServiceProvider = AuthContext.Provider

let noteService: NoteService

let root: JSX.Element
let renderer: TestRenderer.ReactTestRenderer

function createNodeMock(element: React.ReactElement<any>) {
	if (element.type === "div") {
		return document.createElement("div")
	}
}

function render(id: string) {
	root = (
		<NoteServiceProvider value={noteService}>
			<NotePage id={id} />
		</NoteServiceProvider>
	)
	renderer = TestRenderer.create(root, { createNodeMock })
}

function update() {
	renderer.update(root)
}

function findByType(type: React.ReactType<any>) {
	return renderer.root.findByType(type)
}

describe("NotePage", () => {
	beforeEach(() => {
		noteService = {} as NoteService
	})

	it("renders Loading initially, calls getNote correctly", () => {
		noteService.getNote = jest.fn(() => Promise.resolve({}))
		render("someid")
		findByType(Loading)
		expect(noteService.getNote).toBeCalledTimes(1)
		expect(noteService.getNote).toHaveBeenCalledWith("someid")
	})

	it("renders TextNoteController", async () => {
		const note: TextNote = {
			id: Date.now() + "",
			type: NoteType.TextNote,
			title: Date.now() + "title",
			text: Date.now() + "text",
			archived: (Date.now() & 1) == 0
		}
		noteService.getNote = () => Promise.resolve(note)
		render("asdf")
		await Promise.resolve()
		update()
		expect(findByType(TextNoteController).props).toEqual({ note })
	})

	it("renders BookmarkController", async () => {
		const note: Bookmark = {
			id: Date.now() + "",
			type: NoteType.Bookmark,
			title: Date.now() + "title",
			url: Date.now() + "url",
			archived: (Date.now() & 1) == 0
		}
		noteService.getNote = () => Promise.resolve(note)
		render("asdf")
		await Promise.resolve()
		update()
		expect(findByType(BookmarkController).props).toEqual({ note })
	})

	it("renders ChecklistController", async () => {
		const note: Checklist = {
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
		noteService.getNote = () => Promise.resolve(note)
		render("asdf")
		await Promise.resolve()
		update()
		expect(findByType(ChecklistController).props).toEqual({ note })
	})

	it("renders LocationController", async () => {
		const note: Location = {
			id: Date.now() + "",
			type: NoteType.Location,
			title: Date.now() + "title",
			longitude: 67,
			latitude: -34,
			archived: (Date.now() & 1) == 0
		}
		noteService.getNote = () => Promise.resolve(note)
		render("asdf")
		await Promise.resolve()
		update()
		expect(findByType(LocationController).props).toEqual({ note })
	})

	it("renders errors", async () => {
		const message = "fasoiudfasodo"
		noteService.getNote = () => Promise.reject({ message })
		render("id3")
		await Promise.resolve()
		update()
		expect(findByType(Error).props).toEqual({ message })
	})
})

describe("WrappedNotePage", () => {
	const authState = new BehaviorSubject<AuthState>({})
	const authService: AuthService = { authState } as any

	const root = (
		<NoteServiceProvider value={noteService}>
			<AuthServiceProvider value={authService}>
				<WrappedNotePage id={"1234"} />
			</AuthServiceProvider>
		</NoteServiceProvider>
	)

	it("shows NotesPage when authenticated", () => {
		noteService = {} as NoteService
		noteService.getNote = () => Promise.resolve({} as Note)
		authState.next({ user: new User("", "") })
		const testRenderer = TestRenderer.create(root)
		expect(testRenderer.root.findByType(NotePage).props).toEqual({ id: "1234" })
	})

	it("shows auth form if not authenticated", () => {
		authState.next({})
		const testRenderer = TestRenderer.create(root)
		testRenderer.root.findByType(TabbedAuthForm)
	})
})

describe("NotePageWithRouteParams", () => {
	it("should render the WrappedNotePage", () => {
		const authState = new BehaviorSubject<AuthState>({})
		const authService: AuthService = { authState } as any

		const props = { match: { params: { id: "1234" } } } as RouteComponentProps<{ id: string }>

		const root = (
			<NoteServiceProvider value={noteService}>
				<AuthServiceProvider value={authService}>
					<NotePageWithRouteParams {...props} />
				</AuthServiceProvider>
			</NoteServiceProvider>
		)

		const testRenderer = TestRenderer.create(root)
		expect(testRenderer.root.findByType(WrappedNotePage).props).toEqual({ id: "1234" })
	})
})