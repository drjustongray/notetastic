export enum NoteType {
	Bookmark = "Bookmark",
	Checklist = "Checklist",
	Location = "Location",
	TextNote = "TextNote"
}

export interface Note {
	Id?: string
	Archived?: boolean
	Title: string
	Type: NoteType
}

export interface Bookmark extends Note {
	Type: NoteType.Bookmark
	URL: string
}

export interface Checklist extends Note {
	Type: NoteType.Checklist
	Items: Array<{ Checked: boolean, Text: string }>
}

export interface Location extends Note {
	Type: NoteType.Location
	Latitude: number
	Longitude: number
}

export interface TextNote extends Note {
	Type: NoteType.TextNote
	Text: string
}