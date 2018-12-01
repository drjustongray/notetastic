import React from "react";
import { NoteType } from "../Note";
import styles from "./NoteTypeSelector.module.css";

export interface NoteTypeSelectorProps {
	onSelection: (type: NoteType) => any;
}

export default function ({ onSelection }: NoteTypeSelectorProps) {
	return (
		<div>
			<h2 className={styles.title}>Select Note Type</h2>
			<button className={styles.selection} onClick={onSelection.bind(null, NoteType.TextNote)}>Text Note</button>
			<button className={styles.selection} onClick={onSelection.bind(null, NoteType.Checklist)}>Check List</button>
			<button className={styles.selection} onClick={onSelection.bind(null, NoteType.Bookmark)}>Bookmark</button>
			<button className={styles.selection} onClick={onSelection.bind(null, NoteType.Location)}>Location</button>
		</div>
	);
}