import React from "react"
import ReactDOM from "react-dom"
import { BrowserRouter } from "react-router-dom";

import App from "./App"
import "./index.css"
import { AuthContext, authService } from "./auth/context"
const { Provider } = AuthContext

ReactDOM.render(
	<Provider value={authService}>
		<BrowserRouter>
			<App />
		</BrowserRouter>
	</Provider>,
	document.getElementById("root") as HTMLElement
)
