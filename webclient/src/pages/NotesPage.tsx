import React from "react";
import requireAuth from "../auth/components/requireAuth";
import { BaseNote } from "../notes/Note";
import Error from "../components/Error";
import Loading from "../components/Loading";
import { NoteService } from "../notes/noteService";
import { NoteContext } from "../notes/context";
import NoteSnippet from "../notes/components/NoteSnippet";
import { NEW_NOTE } from "./links";
import LinkCard from "../components/LinkCard";

interface State {
	error?: string;
	notes?: ReadonlyArray<BaseNote>;
}

function mapNoteToNoteSnippet(note: BaseNote) {
	const { id, title, type } = note;
	return <NoteSnippet id={id as string} key={id} title={title} type={type} />;
}

export class NotesPage extends React.Component<{}, State> {
	public static contextType = NoteContext;
	public state: State = {};

	public async componentDidMount() {
		const { getNoteList } = this.context as NoteService;
		try {
			const notes = await getNoteList();
			this.setState({ notes });
		} catch (e) {
			this.setState({ error: e.message });
		}
	}

	public render() {
		const { error, notes } = this.state;
		if (error) {
			return <Error message={error} />;
		}
		if (notes) {
			return (
				<div>
					<h2>All Notes</h2>
					<div>
						{notes.map(mapNoteToNoteSnippet)}
						<LinkCard to={NEW_NOTE}><h3>New Note</h3></LinkCard>
					</div>
				</div>
			);
		}
		return <Loading />;
	}
}


export default requireAuth(NotesPage);