import React from "react";
import { Link } from "react-router-dom";
import styles from "./LinkCard.module.css";

export interface LinkCardProps {
	to: string;
	children?: React.ReactNode;
}

export default function ({ to, children }: LinkCardProps) {
	return <Link to={to} className={styles.linkCard}>{children}</Link>;
}