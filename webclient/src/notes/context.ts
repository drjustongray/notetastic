import React from "react";
import { NoteService } from "./noteService";

export const NoteContext = React.createContext<NoteService>({} as NoteService);