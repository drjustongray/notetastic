import React from "react"
import ReactDOM from "react-dom"
import { BrowserRouter } from "react-router-dom";

import App from "./App"
import "./index.css"
import { AuthContext } from "./auth/context"
import { makeAuthService } from "./auth/authService";
import { authAPI } from "./auth/api";
import { validators } from "./auth/validators";
const { Provider } = AuthContext

const authService = makeAuthService(authAPI, validators)

ReactDOM.render(
	<Provider value={authService}>
		<BrowserRouter>
			<App />
		</BrowserRouter>
	</Provider>,
	document.getElementById("root") as HTMLElement
)
