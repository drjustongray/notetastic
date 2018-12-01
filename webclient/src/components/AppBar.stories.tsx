import React from "react";
import { storiesOf } from "@storybook/react";

import AppBar from "./AppBar";
import { AuthContext } from "../auth/context";
import { BrowserRouter } from "react-router-dom";
import User from "../auth/User";
import { AuthState } from "../auth/authService";
import { BehaviorSubject } from "rxjs";

const loggedIn = new BehaviorSubject<AuthState>({ user: new User("uid", "username") });
const loggedOut = new BehaviorSubject<AuthState>({});

const stories = storiesOf("AppBar", module);

stories.add(
	"Logged in",
	() => <BrowserRouter><AuthContext.Provider value={{ authState: loggedIn } as any}><AppBar /></AuthContext.Provider></BrowserRouter>
);

stories.add(
	"Logged out",
	() => <BrowserRouter><AuthContext.Provider value={{ authState: loggedOut } as any}><AppBar /></AuthContext.Provider></BrowserRouter>
);