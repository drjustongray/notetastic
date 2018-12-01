import React from "react";
import styles from "./Error.module.css";

export interface ErrorProps {
	message: string;
}

export default function ({ message }: ErrorProps) {
	return <div className={styles.errorView}>Error: {message}</div>;
}