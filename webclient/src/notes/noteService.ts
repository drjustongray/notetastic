import { NoteAPI } from "./api";
import { Note, BaseNote } from "./Note";

export type SaveNote = <T extends Note>(note: T) => Promise<T>;
export type GetNote = (id: string) => Promise<Note>;
export type GetNoteList = () => Promise<BaseNote[]>;
export type DeleteNote = (id: string) => Promise<void>;

export interface NoteService {
	saveNote: SaveNote;
	getNote: GetNote;
	getNoteList: GetNoteList;
	deleteNote: DeleteNote;
}

export function makeNoteService(noteAPI: NoteAPI, getAccessToken: () => Promise<string>): Readonly<NoteService> {
	const saveNote: SaveNote = async function (note) {
		return await noteAPI.putNote(await getAccessToken(), note);
	};

	const getNote: GetNote = async function (id) {
		return await noteAPI.getNote(await getAccessToken(), id);
	};

	const getNoteList: GetNoteList = async function () {
		return await noteAPI.getNoteList(await getAccessToken());
	};

	const deleteNote: DeleteNote = async function (id) {
		return await noteAPI.deleteNote(await getAccessToken(), id);
	};

	return {
		saveNote,
		getNote,
		getNoteList,
		deleteNote
	};
}