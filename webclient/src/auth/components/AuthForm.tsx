import * as React from "react";
import { Formik, Field, Form, FormikActions, FormikProps, ErrorMessage } from "formik";
import makeLabel from "../../components/makeLabel";
import Error from "../../components/Error";
import styles from "./AuthForm.module.css";

export type AuthFunction = (username: string, password: string, rememberMe: boolean) => Promise<any>;


export interface AuthFormProps {
	action: AuthFunction;
	title: string;
}

interface FormValues {
	username: string;
	password: string;
	rememberMe: boolean;
}

function makeError(message: string) {
	return <Error message={message} />;
}

const onSubmit = async (action: AuthFunction, values: FormValues, actions: FormikActions<FormValues>) => {
	try {
		await action(values.username, values.password, values.rememberMe);
		actions.setSubmitting(false);
	} catch (e) {
		actions.setSubmitting(false);
		actions.setErrors(e);
		actions.setStatus(e);
	}
};

const render = (title: string, { status, isSubmitting, values, handleBlur, handleChange }: FormikProps<FormValues>) => (
	<Form className={styles.form}>
		<h2 className={styles.title}>{title}</h2>
		<div className={styles.formGroup}>
			{makeLabel("auth-form-username", "Username")}
			<Field type="text" name="username" id="auth-form-username" placeholder="Username" />
			<ErrorMessage name="username" render={makeError} />
		</div>
		<div className={styles.formGroup}>
			{makeLabel("auth-form-password", "Password")}
			<Field type="password" name="password" id="auth-form-password" placeholder="Password" />
			<ErrorMessage name="password" render={makeError} />
		</div>
		<div className={styles.formGroup}>
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
		</div>
		<div className={styles.formGroup}>
			{status && status.message && <Error message={status.message} />}
		</div>
		<button className={styles.button} type="submit" disabled={isSubmitting}>
			Submit
		</button>
	</Form>
);

export default ({ action, title }: AuthFormProps) => (
	<Formik
		initialValues={{ username: "", password: "", rememberMe: true }}
		onSubmit={onSubmit.bind(null, action)}
		render={render.bind(null, title)}
	/>
);