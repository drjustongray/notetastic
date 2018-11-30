import * as React from "react";
import "./App.css";
import AppBar from "./components/AppBar";
import { Route } from "react-router-dom";
import { INDEX, LOGIN, REGISTER, NOTES, LOGOUT, ABOUT, ACCOUNT, LOGOUT_EVERYWHERE, NEW_NOTE } from "./pages/links";
import IndexPage from "./pages/IndexPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import NotesPage from "./pages/NotesPage";
import LogoutPage from "./pages/LogoutPage";
import AboutPage from "./pages/AboutPage";
import AccountPage from "./pages/AccountPage";
import LogoutEverywherePage from "./pages/LogoutEverywherePage";
import NewNotePage from "./pages/NewNotePage";
import NotePage from "./pages/NotePage";

export default function () {
	return (
		<div className="App">
			<AppBar />
			<Route exact path={INDEX} component={IndexPage} />
			<Route exact path={LOGIN} component={LoginPage} />
			<Route exact path={REGISTER} component={RegisterPage} />
			<Route exact path={NOTES} component={NotesPage} />
			<Route exact path={NEW_NOTE} component={NewNotePage} />
			<Route exact path={LOGOUT} component={LogoutPage} />
			<Route exact path={LOGOUT_EVERYWHERE} component={LogoutEverywherePage} />
			<Route exact path={ABOUT} component={AboutPage} />
			<Route exact path={ACCOUNT} component={AccountPage} />
			<Route exact path={`${NOTES}/:id`} component={NotePage} />
		</div>
	);
}