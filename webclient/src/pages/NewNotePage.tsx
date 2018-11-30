import React from "react";
import requireAuth from "../auth/components/requireAuth";
import { NoteType, TextNote, Checklist, Bookmark, Location } from "../notes/Note";
import NoteTypeSelector from "../notes/components/NoteTypeSelector";
import { NoteContext } from "../notes/context";
import { NoteService } from "../notes/noteService";
import Loading from "../components/Loading";
import { Redirect } from "react-router";
import { note } from "./links";
import Error from "../components/Error";

interface State {
	pending?: boolean;
	error?: string;
	noteId?: string;
}

function createNewNote(type: NoteType): TextNote | Checklist | Bookmark | Location {
	switch (type) {
		case NoteType.TextNote:
			return {
				title: "",
				type,
				archived: false,
				text: ""
			};
		case NoteType.Checklist:
			return {
				title: "",
				type,
				archived: false,
				items: []
			};
		case NoteType.Bookmark:
			return {
				title: "",
				type,
				archived: false,
				url: ""
			};
		case NoteType.Location:
			return {
				title: "",
				type,
				archived: false,
				latitude: 0,
				longitude: 0
			};
	}
}

export class NewNotePage extends React.Component<{}, State> {

	public static contextType = NoteContext;

	constructor(props: {}) {
		super(props);
		this.state = {};
		this.createNote = this.createNote.bind(this);
	}

	public async createNote(type: NoteType) {
		const { saveNote } = this.context as NoteService;
		this.setState({ pending: true });
		try {
			const noteId = (await saveNote(createNewNote(type))).id;
			this.setState({ pending: false, noteId });
		} catch (e) {
			this.setState({ pending: false, error: e.message });
		}
	}

	public render() {
		const { pending, error, noteId } = this.state;
		if (pending) {
			return <Loading />;
		}
		if (noteId) {
			return <Redirect to={note(noteId)} />;
		}
		const errorMessage = error && <Error message={error} />;
		return <div>{errorMessage}<NoteTypeSelector onSelection={this.createNote} /></div>;
	}
}

export default requireAuth(NewNotePage);