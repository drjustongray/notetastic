export const INDEX = "/"
export const ABOUT = "/about"
export const LOGIN = "/login"
export const REGISTER = "/register"
export const LOGOUT = "/logout"
export const LOGOUT_EVERYWHERE = "/logouteverywhere"
export const NOTES = "/notes"
export const ACCOUNT = "/account"
export const note = (id: string) => `${NOTES}/${id}`