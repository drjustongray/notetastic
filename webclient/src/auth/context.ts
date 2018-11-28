import * as React from "react"
import { AuthService } from "./authService"

export const AuthContext = React.createContext<AuthService>({} as AuthService)