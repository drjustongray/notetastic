import React from "react"
import TestRenderer from "react-test-renderer"
import { BrowserRouter, Link } from "react-router-dom"

import WrappedNotesPage, { NotesPage } from "./NotesPage"
import { Note, NoteType } from "../notes/Note";
import { NoteService } from "../notes/noteService";
import { AuthContext } from "../auth/context";
import { NoteContext } from "../notes/context";
import Loading from "../components/Loading";
import NoteSnippet from "../notes/components/NoteSnippet";
import { NEW_NOTE } from "./links";
import Error from "../components/Error";
import { BehaviorSubject } from "rxjs";
import { AuthState, AuthService } from "../auth/authService";
import User from "../auth/User";
import TabbedAuthForm from "../auth/components/TabbedAuthForm";

const NoteServiceProvider = NoteContext.Provider
const AuthServiceProvider = AuthContext.Provider

let getNoteList: () => Promise<Array<Note>>
let noteService: NoteService
const noteList: Array<Note> = [
	{ Id: "id1", Title: "title1", Type: NoteType.Checklist, Archived: true },
	{ Id: "id2", Title: "title2", Type: NoteType.Location, Archived: false },
	{ Id: "id3", Title: "title3", Type: NoteType.TextNote }
]

function makeRoot() {
	return (
		<BrowserRouter>
			<NoteServiceProvider value={noteService}>
				<NotesPage />
			</NoteServiceProvider>
		</BrowserRouter>
	)
}

describe("NotesPage", () => {

	beforeEach(() => {
		getNoteList = () => Promise.resolve(noteList)
		noteService = { getNoteList } as any
	})

	it("shows loading when loading", () => {
		const root = makeRoot()
		const testRenderer = TestRenderer.create(root)
		testRenderer.root.findByType(Loading)
	})
	it("shows NoteSnippets and link to create note when loading done", async () => {
		const root = makeRoot()
		const testRenderer = TestRenderer.create(root)
		await Promise.resolve()
		testRenderer.update(root)
		const noteSnippets = testRenderer.root.findAllByType(NoteSnippet)
		expect(noteSnippets).toHaveLength(noteList.length)
		noteSnippets.forEach((component, i) => {
			const { Id, Title, Type } = noteList[i]

			expect(component.props).toEqual({ id: Id, title: Title, type: Type })
		})
		expect(testRenderer.root.findAllByType(Link).find(node => node.props.to == NEW_NOTE)).toBeDefined()
	})
	it("shows error message on error", async () => {
		noteService.getNoteList = () => Promise.reject({ message: "NOOOOOOO!!!!!" })
		const root = makeRoot()
		const testRenderer = TestRenderer.create(root)
		await Promise.resolve()
		testRenderer.update(root)
		expect(testRenderer.root.findByType(Error).props).toEqual({ message: "NOOOOOOO!!!!!" })
	})
})

describe("WrappedNotesPage", () => {
	const authState = new BehaviorSubject<AuthState>({})
	const authService: AuthService = { authState } as any

	const root = (
		<BrowserRouter>
			<NoteServiceProvider value={noteService}>
				<AuthServiceProvider value={authService}>
					<WrappedNotesPage />
				</AuthServiceProvider>
			</NoteServiceProvider>
		</BrowserRouter>
	)

	it("shows NotesPage when authenticated", () => {
		authState.next({ user: new User("", "") })
		const testRenderer = TestRenderer.create(root)
		testRenderer.root.findByType(NotesPage)
	})

	it("shows auth form if not authenticated", () => {
		authState.next({})
		const testRenderer = TestRenderer.create(root)
		testRenderer.root.findByType(TabbedAuthForm)
	})
})