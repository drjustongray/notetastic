import React from "react"

export interface ErrorProps {
	message: string
}

export default function ({ message }: ErrorProps) {
	return <div>Error: {message}</div>
}