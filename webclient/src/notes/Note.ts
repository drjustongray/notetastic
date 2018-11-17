export interface Note {
	Id?: string
	Archived?: boolean
	Title: string
	Type: string
}

export interface Bookmark extends Note {
	Type: "bookmark"
	URL: string
}

export interface Checklist extends Note {
	Type: "checklist"
	Items: Array<{ Checked: boolean, Text: string }>
}

export interface Location extends Note {
	Type: "location"
	Latitude: number
	Longitude: number
}

export interface TextNote extends Note {
	Type: "text"
	Text: string
}