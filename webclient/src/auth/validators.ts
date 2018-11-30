export interface Validators {
	/**
	 * Returns true if the provided value contains no white space and is at least 3 unicode codepoints in length
	 * @param username the value to be tested
	 */
	validateUsername(username: string): boolean;
	/**
	 * Returns true if the provided value contains no white space and is at least 8 unicode codepoints in length
	 * @param password the value to be tested
	 */
	validatePassword(password: string): boolean;
	/**
	 * Returns true if the provided value is a base64(url) encoded JSON object that, when taken as a JWT payload, has not yet expired
	 * @param token The value to be tested
	 */
	validateToken(token: string | undefined): boolean;
}

const whiteSpaceRegex = /^\S*$/;

function validateUsername(username: string): boolean {
	return [...username].length > 2 && whiteSpaceRegex.test(username);
}

function validatePassword(password: string): boolean {
	return [...password].length > 7 && whiteSpaceRegex.test(password);
}

function validateToken(token: string | undefined): boolean {
	if (token) {
		try {
			const exp = JSON.parse(atob(token.replace(/-/g, "+").replace(/_/g, "/"))).exp;
			if (exp >= Date.now() / 1000) {
				return true;
			}
			// tslint:disable-next-line:no-empty
		} catch (e) { }
	}
	return false;
}

export const validators: Validators = {
	validatePassword,
	validateToken,
	validateUsername
};