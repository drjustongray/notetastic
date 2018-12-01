import React from "react";
import { FormikActions, Formik, Form, Field, ErrorMessage, FormikProps } from "formik";
import makeLabel from "../../components/makeLabel";
import styles from "./AccountUpdateForm.module.css";
import Error from "../../components/Error";

export type UpdateFunction = (password: string, change: string) => Promise<any>;

export interface AccountUpdateFormProps {
	isOpen: boolean;
	open: () => any;
	close: () => any;
	action: UpdateFunction;
	type: string;
	name: string;
	current?: string;
}

interface FormValues {
	password: string;
	change: string;
}

async function onSubmit(action: UpdateFunction, close: () => any, values: FormValues, actions: FormikActions<FormValues>) {
	try {
		await action(values.password, values.change);
		actions.setSubmitting(false);
		close();
	} catch (e) {
		actions.setSubmitting(false);
		actions.setStatus(e);
	}
}

function render(title: string, type: string, label: string, close: () => any, { status, isSubmitting }: FormikProps<FormValues>) {
	return (
		<Form className={styles.form}>
			<h2 className={styles.title}>{title}</h2>
			<div className={styles.formGroup}>
				{makeLabel("update-form-change", label)}
				<Field type={type} name="change" id="update-form-change" placeholder={label} />
				<ErrorMessage name="change" render={makeLabel.bind(null, "update-form-change")} />
			</div>
			<div className={styles.formGroup}>
				{makeLabel("update-form-password", "Current Password")}
				<Field type="password" name="password" id="update-form-password" placeholder="Password" />
				<ErrorMessage name="password" render={makeLabel.bind(null, "update-form-password")} />
			</div>
			<div>{status && <Error message={status.message} />}</div>
			<div className={styles.buttons}>
				<button type="submit" disabled={isSubmitting}>
					Submit
				</button>
				<button type="button" onClick={close}>
					Cancel
				</button>
			</div>
		</Form>
	);
}

export default function ({ isOpen, open, close, action, type, current, name }: AccountUpdateFormProps) {
	const title = "Change " + name;
	const label = "New " + name;
	if (isOpen) {
		return <Formik
			initialValues={{ password: "", change: "" }}
			onSubmit={onSubmit.bind(null, action, close)}
			render={render.bind(null, title, type, label, close)}
		/>;
	}
	return <div className={styles.entryPoint}>{name}{current && ": " + current}<button onClick={open}>{title}</button></div>;
}