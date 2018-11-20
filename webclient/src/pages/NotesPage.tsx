import React from "react"
import requireAuth from "../auth/components/requireAuth";
import { Note } from "../notes/Note";
import Error from "../components/Error";
import Loading from "../components/Loading";
import { NoteService } from "../notes/noteService";
import { NoteContext } from "../notes/context";
import NoteSnippet from "../notes/components/NoteSnippet";
import { NEW_NOTE } from "./links";
import { Link } from "react-router-dom";

interface State {
	error?: string
	notes?: ReadonlyArray<Note>
}

function mapNoteToNoteSnippet(note: Note) {
	const { Id, Title, Type } = note
	return <NoteSnippet id={Id as string} key={Id} title={Title} type={Type} />
}

export class NotesPage extends React.Component<{}, State> {
	static contextType = NoteContext
	state: State = {}

	async componentDidMount() {
		const { getNoteList } = this.context as NoteService
		try {
			const notes = await getNoteList()
			this.setState({ notes })
		} catch (e) {
			this.setState({ error: e.message })
		}
	}

	render() {
		const { error, notes } = this.state
		if (error) {
			return <Error message={error} />
		}
		if (notes) {
			return (
				<div>
					<h2>All Notes</h2>
					{notes.map(mapNoteToNoteSnippet)}
					<Link to={NEW_NOTE} />
				</div>
			)
		}
		return <Loading />
	}
}


export default requireAuth(NotesPage)