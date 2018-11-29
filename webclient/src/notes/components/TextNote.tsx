import React from "react"
import { BaseNoteProps, getBaseNoteProps, BaseNoteView } from "./BaseNote"

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