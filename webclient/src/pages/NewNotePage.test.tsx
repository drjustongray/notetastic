import React from "react";
import TestRenderer from "react-test-renderer";
import { BrowserRouter, Redirect } from "react-router-dom";

import WrappedNewNotePage, { NewNotePage } from "./NewNotePage";
import { Note, NoteType, TextNote, Checklist, Location, Bookmark } from "../notes/Note";
import { NoteService } from "../notes/noteService";
import { AuthContext } from "../auth/context";
import { NoteContext } from "../notes/context";
import Loading from "../components/Loading";
import { note } from "./links";
import Error from "../components/Error";
import { BehaviorSubject } from "rxjs";
import { AuthState, AuthService } from "../auth/authService";
import User from "../auth/User";
import TabbedAuthForm from "../auth/components/TabbedAuthForm";
import NoteTypeSelector from "../notes/components/NoteTypeSelector";

const NoteServiceProvider = NoteContext.Provider;
const AuthServiceProvider = AuthContext.Provider;

let saveNote: jest.Mock<Promise<{ id: string }>>;
let noteService: NoteService;

function makeRoot() {
	return (
		<BrowserRouter>
			<NoteServiceProvider value={noteService}>
				<NewNotePage />
			</NoteServiceProvider>
		</BrowserRouter>
	);
}

let root: JSX.Element;
let testRenderer: TestRenderer.ReactTestRenderer;

function renderAndGetSelector() {
	root = makeRoot();
	testRenderer = TestRenderer.create(root);
	return testRenderer.root.findByType(NoteTypeSelector);
}

describe("NewNotePage", () => {

	let id: string;

	beforeEach(() => {
		id = Date.now() + "";
		saveNote = jest.fn(() => Promise.resolve({ id }));
		noteService = { saveNote } as any;
	});

	it("renders NoteTypeSelector", () => {
		renderAndGetSelector();
	});

	it("calls saveNote correctly (TextNote)", () => {
		renderAndGetSelector().props.onSelection(NoteType.TextNote);
		expect(saveNote).toHaveBeenCalledTimes(1);
		const expected: TextNote = { title: "", type: NoteType.TextNote, text: "", archived: false };
		expect(saveNote).toHaveBeenCalledWith(expected);
	});

	it("calls saveNote correctly (Checklist)", () => {
		renderAndGetSelector().props.onSelection(NoteType.Checklist);
		expect(saveNote).toHaveBeenCalledTimes(1);
		const expected: Checklist = { title: "", type: NoteType.Checklist, items: [], archived: false };
		expect(saveNote).toHaveBeenCalledWith(expected);
	});

	it("calls saveNote correctly (Location)", () => {
		renderAndGetSelector().props.onSelection(NoteType.Location);
		expect(saveNote).toHaveBeenCalledTimes(1);
		const expected: Location = { title: "", type: NoteType.Location, latitude: 0, longitude: 0, archived: false };
		expect(saveNote).toHaveBeenCalledWith(expected);
	});

	it("calls saveNote correctly (Bookmark)", () => {
		renderAndGetSelector().props.onSelection(NoteType.Bookmark);
		expect(saveNote).toHaveBeenCalledTimes(1);
		const expected: Bookmark = { title: "", type: NoteType.Bookmark, url: "", archived: false };
		expect(saveNote).toHaveBeenCalledWith(expected);
	});

	it("shows loading while promise pending", () => {
		renderAndGetSelector().props.onSelection(NoteType.TextNote);
		testRenderer.update(root);
		testRenderer.root.findByType(Loading);
		expect(testRenderer.root.findAllByType(NoteTypeSelector)).toHaveLength(0);
	});

	it("redirects when the promise resolves", async () => {
		renderAndGetSelector().props.onSelection(NoteType.TextNote);
		await Promise.resolve();
		testRenderer.update(root);
		expect(testRenderer.root.findByType(Redirect).props).toMatchObject({ to: note(id) });
	});

	it("displays an error when promise rejects", async () => {
		noteService.saveNote = () => Promise.reject({ message: ":(" });
		renderAndGetSelector().props.onSelection(NoteType.TextNote);
		await Promise.resolve();
		testRenderer.update(root);
		expect(testRenderer.root.findByType(Error).props).toEqual({ message: ":(" });
		testRenderer.root.findByType(NoteTypeSelector);
	});
});

describe("WrappedNewNotePage", () => {
	const authState = new BehaviorSubject<AuthState>({});
	const authService: AuthService = { authState } as any;

	const root = (
		<BrowserRouter>
			<NoteServiceProvider value={noteService}>
				<AuthServiceProvider value={authService}>
					<WrappedNewNotePage />
				</AuthServiceProvider>
			</NoteServiceProvider>
		</BrowserRouter>
	);

	it("shows NotesPage when authenticated", () => {
		authState.next({ user: new User("", "") });
		testRenderer = TestRenderer.create(root);
		testRenderer.root.findByType(NewNotePage);
	});

	it("shows auth form if not authenticated", () => {
		authState.next({});
		testRenderer = TestRenderer.create(root);
		testRenderer.root.findByType(TabbedAuthForm);
	});
});