import React from "react"
import BaseNote, { BaseNoteProps, getBaseNoteProps } from "./BaseNote"
import { NoteContext } from "../context";
import { Bookmark } from "../Note";
import { NoteService } from "../noteService";
import Loading from "../../components/Loading";
import { Redirect } from "react-router-dom";
import { NOTES } from "../../pages/links";

export interface BookmarkViewProps extends BaseNoteProps {
	url: string
	updateURL: (url: string) => any
}

export class BookmarkView extends React.Component<BookmarkViewProps> {

	constructor(props: BookmarkViewProps) {
		super(props)
		this.handleChange = this.handleChange.bind(this)
	}

	handleChange(event: React.ChangeEvent<HTMLInputElement>) {
		this.props.updateURL(event.currentTarget.value)
	}

	render() {
		const { url } = this.props
		const baseNoteProps = getBaseNoteProps(this.props)
		return (
			<BaseNote {...baseNoteProps} >
				<input type="text" value={url} onChange={this.handleChange} />
			</BaseNote>
		)
	}
}

export class BookmarkController extends React.Component<{ note: Bookmark }, { note: Bookmark, waiting?: boolean, deleted?: boolean, error?: string | null }> {
	static contextType = NoteContext

	pendingRequest?: Promise<any>
	pendingNote?: Bookmark

	constructor(props: { note: Bookmark }) {
		super(props)
		const { note } = this.props
		this.state = { note: { ...note } }
		this.updateURL = this.updateURL.bind(this)
		this.updateTitle = this.updateTitle.bind(this)
		this.setArchived = this.setArchived.bind(this)
		this.deleteNote = this.deleteNote.bind(this)
	}

	async update(note: Bookmark) {
		this.setState({ note, error: null })
		try {
			if (this.pendingRequest) {
				this.pendingNote = note
				await this.pendingRequest
				if (!this.pendingNote) {
					return
				}
				note = this.pendingNote
				this.pendingNote = undefined
			}

			const { saveNote } = this.context as NoteService
			this.pendingRequest = saveNote(note)
			await this.pendingRequest
		} catch (e) {
			this.setState({ error: e.message })
		}
		this.pendingRequest = undefined
	}

	updateURL(url: string) {
		const { note } = this.state
		this.update({ ...note, url })
	}

	updateTitle(title: string) {
		const { note } = this.state
		this.update({ ...note, title })
	}

	setArchived(archived: boolean) {
		const { note } = this.state
		this.update({ ...note, archived })
	}

	async deleteNote() {
		const { deleteNote } = this.context as NoteService
		const { id } = this.props.note
		this.setState({ waiting: true, error: null })
		try {
			await deleteNote(id!)
			this.setState({ waiting: false, deleted: true })
		} catch (e) {
			this.setState({ error: e.message, waiting: false })
		}
	}

	render() {
		const { waiting, deleted, error } = this.state
		if (waiting) {
			return <Loading />
		}
		if (deleted) {
			return <Redirect to={NOTES} />
		}
		const { title, archived, url } = this.state.note
		const { updateTitle, updateURL, setArchived, deleteNote } = this
		const viewProps = { title, archived, url, updateTitle, updateURL, setArchived, deleteNote, error }
		return <BookmarkView {...viewProps} />
	}
}