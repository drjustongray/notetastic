export const NETWORK_ERROR = "Network Error";
export const SERVER_ERROR = "Server Error";
export const BAD_REQUEST = "Bad Request";
export const NOT_FOUND = "Not Found";
export const UNAUTHORIZED = "Unauthorized";

export function makeRejectedPromise(message: string) {
	return Promise.reject({ message });
}

export const networkErrorRejection = makeRejectedPromise(NETWORK_ERROR);
export const serverErrorRejection = makeRejectedPromise(SERVER_ERROR);
export const badRequestRejection = makeRejectedPromise(BAD_REQUEST);
export const notFoundRejection = makeRejectedPromise(NOT_FOUND);
export const unauthorizedRejection = makeRejectedPromise(UNAUTHORIZED);