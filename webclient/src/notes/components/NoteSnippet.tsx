import React from "react";
import { note } from "../../pages/links";
import LinkCard from "../../components/LinkCard";

interface NoteSnippetProps {
	id: string;
	title: string;
	type: string;
}

export default function (props: NoteSnippetProps) {
	return <LinkCard to={note(props.id)}><h3>{props.title}</h3><h4>{props.type}</h4></LinkCard>;
}