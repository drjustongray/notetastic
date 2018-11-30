import React, { ReactNode } from "react";
import makeLabel from "../../components/makeLabel";
import Error from "../../components/Error";
import { Note } from "../Note";
import { NoteContext } from "../context";
import { NoteService } from "../noteService";
import Loading from "../../components/Loading";
import { Redirect } from "react-router-dom";
import { NOTES } from "../../pages/links";

export interface BaseNoteProps {
	title: string;
	archived: boolean;
	error?: string | null;
	updateTitle: (title: string) => any;
	setArchived: (archived: boolean) => any;
	deleteNote: () => any;
}

export function getBaseNoteProps(props: BaseNoteProps): BaseNoteProps {
	const { title, archived, updateTitle, setArchived, deleteNote, error } = props;
	return { title, archived, updateTitle, setArchived, deleteNote, error };
}

export class BaseNoteView extends React.Component<BaseNoteProps> {

	constructor(props: BaseNoteProps) {
		super(props);
		this.handleChange = this.handleChange.bind(this);
	}

	public handleChange(event: React.ChangeEvent<HTMLInputElement>) {
		const { name, value, checked } = event.currentTarget;
		if (name === "title") {
			this.props.updateTitle(value);
		} else if (name === "archived") {
			this.props.setArchived(checked);
		}
	}

	public render() {
		const { title, archived, deleteNote, error } = this.props;
		return (
			<article>
				<div>
					{makeLabel("title", "Title")}
					<input value={title} type="text" id="title" name="title" placeholder="Title" onChange={this.handleChange} />
				</div>
				<div>
					<input checked={archived} type="checkbox" id="archived" name="archived" onChange={this.handleChange} />
					{makeLabel("archived", "Archived")}
				</div>
				<button onClick={deleteNote}>Delete</button>
				{this.props.children}
				{error && <Error message={error} />}
			</article>
		);
	}
}

export interface BaseNoteControllerState<N> {
	note: N;
	waiting?: boolean;
	deleted?: boolean;
	error?: string | null;
}

export abstract class BaseNoteController<T extends Note> extends React.Component<{ note: T }, BaseNoteControllerState<T>> {
	public static contextType = NoteContext;

	public pendingRequest?: Promise<any>;
	public pendingNote?: T;

	constructor(props: { note: T }) {
		super(props);
		const { note } = this.props;
		this.state = { note: Object.assign({}, note) };
		this.updateTitle = this.updateTitle.bind(this);
		this.setArchived = this.setArchived.bind(this);
		this.deleteNote = this.deleteNote.bind(this);
	}

	public async update(note: T) {
		this.setState({ note, error: null });
		try {
			if (this.pendingRequest) {
				this.pendingNote = note;
				await this.pendingRequest;
				if (!this.pendingNote) {
					return;
				}
				note = this.pendingNote;
				this.pendingNote = undefined;
			}

			const { saveNote } = this.context as NoteService;
			this.pendingRequest = saveNote(note);
			await this.pendingRequest;
		} catch (e) {
			this.setState({ error: e.message });
		}
		this.pendingRequest = undefined;
	}

	public updateTitle(title: string) {
		const { note } = this.state;
		this.update(Object.assign({}, note, { title }));
	}

	public setArchived(archived: boolean) {
		const { note } = this.state;
		this.update(Object.assign({}, note, { archived }));
	}

	public async deleteNote() {
		const { deleteNote } = this.context as NoteService;
		const { id } = this.props.note;
		this.setState({ waiting: true, error: null });
		try {
			await deleteNote(id!);
			this.setState({ waiting: false, deleted: true });
		} catch (e) {
			this.setState({ error: e.message, waiting: false });
		}
	}

	public render() {
		const { waiting, deleted } = this.state;
		if (waiting) {
			return <Loading />;
		}
		if (deleted) {
			return <Redirect to={NOTES} />;
		}

		return this.renderNoteView();
	}

	public abstract renderNoteView(): ReactNode;
}