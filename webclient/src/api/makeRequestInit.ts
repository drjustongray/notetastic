export function makeRequestInit(method: string, body: object, accessToken?: string): RequestInit {
	const headers: Record<string, string> = {
		"Content-Type": "application/json; charset=utf-8",
		credentials: "omit"
	};
	if (accessToken) {
		headers.Authorization = `Bearer ${accessToken}`;
	}
	return {
		method,
		headers,
		body: JSON.stringify(body)
	};
}

export function makeNoBodyRequestInit(method: string, accessToken?: string) {
	const headers: Record<string, string> = {
		credentials: "omit"
	};
	if (accessToken) {
		headers.Authorization = `Bearer ${accessToken}`;
	}
	return {
		method,
		headers
	};
}