export enum NoteType {
	Bookmark = "Bookmark",
	Checklist = "Checklist",
	Location = "Location",
	TextNote = "TextNote"
}

export interface BaseNote {
	id?: string;
	archived: boolean;
	title: string;
	type: NoteType;
}

export interface Bookmark extends BaseNote {
	type: NoteType.Bookmark;
	url: string;
}

export interface Checklist extends BaseNote {
	type: NoteType.Checklist;
	items: ChecklistItem[];
}

export interface ChecklistItem {
	checked: boolean;
	text: string;
}

export interface Location extends BaseNote {
	type: NoteType.Location;
	latitude: number;
	longitude: number;
}

export interface TextNote extends BaseNote {
	type: NoteType.TextNote;
	text: string;
}

export type Note = Bookmark | Checklist | Location | TextNote;