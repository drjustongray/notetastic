export enum NoteType {
	Bookmark = "Bookmark",
	Checklist = "Checklist",
	Location = "Location",
	TextNote = "TextNote"
}

export interface Note {
	id?: string
	archived: boolean
	title: string
	type: NoteType
}

export interface Bookmark extends Note {
	type: NoteType.Bookmark
	url: string
}

export interface Checklist extends Note {
	type: NoteType.Checklist
	items: Array<{ Checked: boolean, Text: string }>
}

export interface Location extends Note {
	type: NoteType.Location
	latitude: number
	longitude: number
}

export interface TextNote extends Note {
	type: NoteType.TextNote
	text: string
}