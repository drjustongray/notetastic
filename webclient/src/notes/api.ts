import { Note } from "./Note"
import { makeNoBodyRequestInit, makeRequestInit } from "../api/makeRequestInit";
import { networkErrorRejection, serverErrorRejection, badRequestRejection, unauthorizedRejection, notFoundRejection } from "../api/rejectedPromises";

export type PutNote = <T extends Note>(accessToken: string, note: T) => Promise<T>
export type GetNote = (accessToken: string, id: string) => Promise<Note>
export type GetNoteList = (accessToken: string) => Promise<Array<Note>>
export type DeleteNote = (accessToken: string, id: string) => Promise<void>

export interface NoteAPI {
	putNote: PutNote
	getNote: GetNote
	getNoteList: GetNoteList
	deleteNote: DeleteNote
}

const path = "/api/notes"

function makeNoteRequest(init: RequestInit, id?: string) {
	let reqPath = id ? `${path}/${id}` : path
	return fetch(reqPath, init).catch(e => networkErrorRejection)
}

function handleNotOKResponse(res: Response) {
	if (res.status == 400) {
		return badRequestRejection
	}
	if (res.status == 401) {
		return unauthorizedRejection
	}
	if (res.status == 404) {
		return notFoundRejection
	}
	return serverErrorRejection
}

const putNote: PutNote = async function <T extends Note>(accessToken: string, note: T) {
	const res = await makeNoteRequest(makeRequestInit("PUT", note, accessToken))
	if (res.ok) {
		return res.json()
	}
	return handleNotOKResponse(res)
}

const getNote: GetNote = async function (accessToken: string, id: string) {
	const res = await makeNoteRequest(makeNoBodyRequestInit("GET", accessToken), id)
	if (res.ok) {
		return res.json()
	}
	return handleNotOKResponse(res)
}

const getNoteList: GetNoteList = async function (accessToken: string) {
	const res = await makeNoteRequest(makeNoBodyRequestInit("GET", accessToken))
	if (res.ok) {
		return res.json()
	}
	return handleNotOKResponse(res)
}

const deleteNote: DeleteNote = async function (accessToken: string, id: string) {
	const res = await makeNoteRequest(makeNoBodyRequestInit("DELETE", accessToken), id)
	if (res.ok) {
		return
	}
	return handleNotOKResponse(res)
}

export const noteAPI: NoteAPI = {
	putNote,
	getNote,
	getNoteList,
	deleteNote
}