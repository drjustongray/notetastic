import React from "react";
import { BaseNoteProps, getBaseNoteProps, BaseNoteView, BaseNoteController } from "./BaseNote";
import { Bookmark } from "../Note";
import styles from "./Bookmark.module.css";

export interface BookmarkViewProps extends BaseNoteProps {
	url: string;
	updateURL: (url: string) => any;
}

export class BookmarkView extends React.Component<BookmarkViewProps> {

	constructor(props: BookmarkViewProps) {
		super(props);
		this.handleChange = this.handleChange.bind(this);
	}

	public handleChange(event: React.ChangeEvent<HTMLInputElement>) {
		this.props.updateURL(event.currentTarget.value);
	}

	public render() {
		const { url } = this.props;
		const baseNoteProps = getBaseNoteProps(this.props);
		return (
			<BaseNoteView {...baseNoteProps} >
				<input className={styles.url} type="text" value={url} onChange={this.handleChange} />
			</BaseNoteView>
		);
	}
}

export class BookmarkController extends BaseNoteController<Bookmark> {

	constructor(props: { note: Bookmark }) {
		super(props);
		this.updateURL = this.updateURL.bind(this);
	}

	public updateURL(url: string) {
		const { note } = this.state;
		this.update({ ...note, url });
	}

	public renderNoteView() {
		const { error } = this.state;
		const { title, archived, url } = this.state.note;
		const { updateTitle, updateURL, setArchived, deleteNote } = this;
		const viewProps = { title, archived, url, updateTitle, updateURL, setArchived, deleteNote, error };
		return <BookmarkView {...viewProps} />;
	}
}