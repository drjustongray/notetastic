import React from "react";
import { getBaseNoteProps, BaseNoteProps, BaseNoteView, BaseNoteController } from "./BaseNote";
import { ChecklistItem as Item, Checklist } from "../Note";
import styles from "./Checklist.module.css";

export interface ChecklistViewProps extends BaseNoteProps {
	items: ReadonlyArray<Item>;
	addItem: () => any;
	removeItem: (index: number) => any;
	setItemChecked: (index: number, checked: boolean) => any;
	updateItemText: (index: number, text: string) => any;
}

interface ChecklistItemProps extends Item {
	remove: () => any;
	setChecked: (checked: boolean) => any;
	updateText: (text: string) => any;
}

class ChecklistItem extends React.Component<ChecklistItemProps> {

	constructor(props: ChecklistItemProps) {
		super(props);
		this.handleChange = this.handleChange.bind(this);
	}

	public handleChange(event: React.ChangeEvent<HTMLInputElement>) {
		const { type, value, checked } = event.currentTarget;
		if (type === "text") {
			this.props.updateText(value);
		} else if (type === "checkbox") {
			this.props.setChecked(checked);
		}
	}

	public render() {
		const { checked, text, remove } = this.props;
		const textClass = checked ? `${styles.itemText} ${styles.itemTextChecked}` : styles.itemText;
		return (
			<li className={styles.item}>
				<input type="checkbox" checked={checked} onChange={this.handleChange} />
				<input className={textClass} disabled={checked} type="text" value={text} onChange={this.handleChange} />
				<button onClick={remove}>Delete</button>
			</li>
		);
	}
}

export class ChecklistView extends React.Component<ChecklistViewProps> {

	constructor(props: ChecklistViewProps) {
		super(props);
		this.handleSetChecked = this.handleSetChecked.bind(this);
		this.handleUpdateText = this.handleUpdateText.bind(this);
		this.handleItemDelete = this.handleItemDelete.bind(this);
		this.handleItemCreate = this.handleItemCreate.bind(this);
		this.mapItemToElement = this.mapItemToElement.bind(this);
	}

	public handleUpdateText(index: number, text: string) {
		this.props.updateItemText(index, text);
	}

	public handleSetChecked(index: number, checked: boolean) {
		this.props.setItemChecked(index, checked);
	}

	public handleItemDelete(index: number) {
		this.props.removeItem(index);
	}

	public handleItemCreate() {
		this.props.addItem();
	}

	public mapItemToElement(item: Item, index: number) {
		return <ChecklistItem
			{...item}
			key={index}
			remove={this.handleItemDelete.bind(this, index)}
			setChecked={this.handleSetChecked.bind(this, index)}
			updateText={this.handleUpdateText.bind(this, index)}
		/>;
	}

	public render() {
		const { items } = this.props;
		const baseNoteProps = getBaseNoteProps(this.props);
		return (
			<BaseNoteView {...baseNoteProps} >
				<ul className={styles.list}>
					{items.map(this.mapItemToElement)}
				</ul>
				<button onClick={this.handleItemCreate}>New Item</button>
			</BaseNoteView>
		);
	}
}

export class ChecklistController extends BaseNoteController<Checklist>{

	constructor(props: { note: Checklist }) {
		super(props);
		this.addItem = this.addItem.bind(this);
		this.removeItem = this.removeItem.bind(this);
		this.setItemChecked = this.setItemChecked.bind(this);
		this.updateItemText = this.updateItemText.bind(this);
	}

	public addItem() {
		const { note } = this.state;
		const items = [...note.items, { checked: false, text: "" }];
		this.update({ ...note, items });
	}

	public removeItem(index: number) {
		const { note } = this.state;
		const items = note.items.filter((_, i) => i !== index);
		this.update({ ...note, items });
	}

	public setItemChecked(index: number, checked: boolean) {
		const { note } = this.state;
		const items = note.items.map((item, i) => i !== index ? item : { ...item, checked });
		this.update({ ...note, items });
	}

	public updateItemText(index: number, text: string) {
		const { note } = this.state;
		const items = note.items.map((item, i) => i !== index ? item : { ...item, text });
		this.update({ ...note, items });
	}

	public renderNoteView(): React.ReactNode {
		const { error } = this.state;
		const { title, archived, items } = this.state.note;
		const { updateTitle, setArchived, deleteNote, addItem, removeItem, setItemChecked, updateItemText } = this;
		const viewProps = { title, archived, items, updateTitle, setArchived, deleteNote, error, addItem, removeItem, setItemChecked, updateItemText };
		return <ChecklistView {...viewProps} />;
	}

}