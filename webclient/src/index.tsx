import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";

import App from "./App";
import "./index.css";
import { AuthContext } from "./auth/context";
import { makeAuthService } from "./auth/authService";
import { authAPI } from "./auth/api";
import { validators } from "./auth/validators";
import { makeNoteService } from "./notes/noteService";
import { noteAPI } from "./notes/api";
import { NoteContext } from "./notes/context";
const AuthProvider = AuthContext.Provider;
const NoteProvider = NoteContext.Provider;

const authService = makeAuthService(authAPI, validators);
const noteService = makeNoteService(noteAPI, authService.getAccessToken);

ReactDOM.render(
	<AuthProvider value={authService}>
		<NoteProvider value={noteService}>
			<BrowserRouter>
				<App />
			</BrowserRouter>
		</NoteProvider>
	</AuthProvider>,
	document.getElementById("root") as HTMLElement
);
