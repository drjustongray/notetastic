import React from "react";
import requireAuth from "../auth/components/requireAuth";
import { Note, NoteType } from "../notes/Note";
import { RouteComponentProps } from "react-router-dom";
import { NoteContext } from "../notes/context";
import Loading from "../components/Loading";
import { NoteService } from "../notes/noteService";
import Error from "../components/Error";
import { BookmarkController } from "../notes/components/Bookmark";
import { ChecklistController } from "../notes/components/Checklist";
import { LocationController } from "../notes/components/Location";
import { TextNoteController } from "../notes/components/TextNote";

interface State {
	error?: string;
	note?: Note;
}

export class NotePage extends React.Component<{ id: string }, State> {
	public static contextType = NoteContext;

	constructor(props: { id: string }) {
		super(props);
		this.state = {};
	}

	public async componentDidMount() {
		const { getNote } = this.context as NoteService;
		try {
			this.setState({ note: await getNote(this.props.id) });
		} catch (e) {
			this.setState({ error: e.message });
		}
	}

	public render() {
		const { note, error } = this.state;
		if (note) {
			switch (note.type) {
				case NoteType.Bookmark:
					return <BookmarkController note={note} />;
				case NoteType.Checklist:
					return <ChecklistController note={note} />;
				case NoteType.Location:
					return <LocationController note={note} />;
				case NoteType.TextNote:
					return <TextNoteController note={note} />;
			}
		}
		if (error) {
			return <Error message={error} />;
		}
		return <Loading />;
	}
}

export const WrappedNotePage = requireAuth(NotePage);

export default function (props: RouteComponentProps<{ id: string }>) {
	return <WrappedNotePage id={props.match.params.id} />;
}