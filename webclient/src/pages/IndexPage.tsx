import React from "react"
import RedirectIfAuthenticated from "../auth/components/RedirectIfAuthenticated";
import { NOTES } from "./links";
import AboutPage from "./AboutPage";

export default () => (<React.Fragment><RedirectIfAuthenticated to={NOTES} /><AboutPage /></React.Fragment>)