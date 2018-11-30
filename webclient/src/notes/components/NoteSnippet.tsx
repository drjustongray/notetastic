import React from "react";
import { Link } from "react-router-dom";
import { note } from "../../pages/links";

interface NoteSnippetProps {
	id: string;
	title: string;
	type: string;
}

export default function (props: NoteSnippetProps) {
	return <Link to={note(props.id)}><h3>{props.title} ({props.type})</h3></Link>;
}