import React from "react"
import { BaseNoteProps, getBaseNoteProps, BaseNoteView, BaseNoteController } from "./BaseNote"
import { TextNote } from "../Note";

export interface TextNoteViewProps extends BaseNoteProps {
	text: string
	updateText: (text: string) => any
}

export class TextNoteView extends React.Component<TextNoteViewProps> {

	constructor(props: TextNoteViewProps) {
		super(props)
		this.handleChange = this.handleChange.bind(this)
	}

	handleChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
		this.props.updateText(event.currentTarget.value)
	}

	render() {
		const { text } = this.props
		const baseNoteProps = getBaseNoteProps(this.props)
		return (
			<BaseNoteView {...baseNoteProps} >
				<textarea value={text} onChange={this.handleChange} />
			</BaseNoteView>
		)
	}
}

export class TextNoteController extends BaseNoteController<TextNote> {

	constructor(props: { note: TextNote }) {
		super(props)
		this.updateText = this.updateText.bind(this)
	}

	updateText(text: string) {
		const { note } = this.state
		this.update({ ...note, text })
	}

	renderNoteView(): React.ReactNode {
		const { error } = this.state
		const { title, archived, text } = this.state.note
		const { updateTitle, updateText, setArchived, deleteNote } = this
		const viewProps = { title, archived, text, updateTitle, updateText, setArchived, deleteNote, error }
		return <TextNoteView {...viewProps} />
	}

}