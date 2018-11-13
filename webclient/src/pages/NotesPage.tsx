import React from "react"
import requireAuth from "../auth/components/requireAuth";

export const NotesPage = () => (
	<div>
		<h2>All Notes</h2>
	</div>
)

export default requireAuth()(NotesPage)