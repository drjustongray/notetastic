import React from "react"
import { NoteService, makeNoteService } from "./noteService"
import { noteAPI } from "./api"
import { authService } from "../auth/context"

export const noteService = makeNoteService(noteAPI, authService.getAccessToken)
export const NoteContext = React.createContext<NoteService>(noteService)