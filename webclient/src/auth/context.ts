import * as React from "react"
import { AuthService, makeAuthService } from "./authService"
import { authAPI } from "./api"
import { validators } from "./validators"

export const authService = makeAuthService(authAPI, validators)
export const AuthContext = React.createContext<AuthService>(authService)