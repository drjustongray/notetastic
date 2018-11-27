import React from "react"
import BaseNote, { BaseNoteProps, getBaseNoteProps } from "./BaseNote"

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