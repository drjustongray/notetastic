import React from "react"
import { NoteType } from "../Note";

export interface NoteTypeSelectorProps {
	onSelection: (type: NoteType) => any
}

export default function ({ onSelection }: NoteTypeSelectorProps) {
	return (
		<div>
			<h2>Select Note Type</h2>
			<button onClick={onSelection.bind(null, NoteType.TextNote)}>Text Note</button>
			<button onClick={onSelection.bind(null, NoteType.Checklist)}>Check List</button>
			<button onClick={onSelection.bind(null, NoteType.Bookmark)}>Bookmark</button>
			<button onClick={onSelection.bind(null, NoteType.Location)}>Location</button>
		</div>
	)
}