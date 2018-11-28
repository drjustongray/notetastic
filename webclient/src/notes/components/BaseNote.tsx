import React from "react"
import makeLabel from "../../components/makeLabel";
import Error from "../../components/Error";

export interface BaseNoteProps {
	title: string
	archived: boolean
	error?: string | null
	updateTitle: (title: string) => any
	setArchived: (archived: boolean) => any
	deleteNote: () => any
}

export function getBaseNoteProps(props: BaseNoteProps): BaseNoteProps {
	const { title, archived, updateTitle, setArchived, deleteNote, error } = props
	return { title, archived, updateTitle, setArchived, deleteNote, error }
}

export default class extends React.Component<BaseNoteProps> {

	constructor(props: BaseNoteProps) {
		super(props)
		this.handleChange = this.handleChange.bind(this)
	}

	handleChange(event: React.ChangeEvent<HTMLInputElement>) {
		const { name, value, checked } = event.currentTarget
		if (name == "title") {
			this.props.updateTitle(value)
		} else if (name == "archived") {
			this.props.setArchived(checked)
		}
	}

	render() {
		const { title, archived, deleteNote, error } = this.props
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
		)
	}
}