import * as React from "react"
import { Formik, Field, Form, FormikActions, FormikProps, ErrorMessage } from "formik"

export type AuthFunction = (username: string, password: string, rememberMe: boolean) => Promise<any>


export interface AuthFormProps {
	action: AuthFunction
	title: string
}

interface FormValues {
	username: string
	password: string
	rememberMe: boolean
}

const onSubmit = async (action: AuthFunction, values: FormValues, actions: FormikActions<FormValues>) => {
	try {
		await action(values.username, values.password, values.rememberMe)
		actions.setSubmitting(false)
	} catch (e) {
		actions.setSubmitting(false)
		actions.setErrors(e)
		actions.setStatus(e)
	}
}

const makeLabel = (forWhat: string, text: string) => <label htmlFor={forWhat}>{text}</label>

const render = (title: string, { status, isSubmitting, values, handleBlur, handleChange }: FormikProps<FormValues>) => (
	<Form>
		<h2>{title}</h2>
		<div>
			{makeLabel("auth-form-username", "Username")}
			<Field type="text" name="username" id="auth-form-username" placeholder="Username" />
			<ErrorMessage name="username" render={makeLabel.bind(null, "auth-form-username")} />
		</div>
		<div>
			{makeLabel("auth-form-password", "Password")}
			<Field type="password" name="password" id="auth-form-password" placeholder="Password" />
			<ErrorMessage name="password" render={makeLabel.bind(null, "auth-form-password")} />
		</div>
		<div>
			<input
				type="checkbox"
				name="rememberMe"
				id="auth-form-remember-me"
				onChange={handleChange}
				onBlur={handleBlur}
				checked={values.rememberMe}
			/>
			<label htmlFor="auth-form-remember-me">Remember Me</label>
		</div>
		<div>{status && status.message}</div>
		<button type="submit" disabled={isSubmitting}>
			Submit
		</button>
	</Form>
)

export default ({ action, title }: AuthFormProps) => (
	<Formik
		initialValues={{ username: "", password: "", rememberMe: true }}
		onSubmit={onSubmit.bind(null, action)}
		render={render.bind(null, title)}
	/>
)