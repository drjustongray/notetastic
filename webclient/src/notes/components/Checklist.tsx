import React from "react"
import BaseNote, { getBaseNoteProps, BaseNoteProps } from "./BaseNote"
import { ChecklistItem as Item } from "../Note";

export interface ChecklistViewProps extends BaseNoteProps {
	items: ReadonlyArray<Item>
	addItem: () => any
	removeItem: (index: number) => any
	setItemChecked: (index: number, checked: boolean) => any
	updateItemText: (index: number, text: string) => any
}

interface ChecklistItemProps extends Item {
	remove: () => any
	setChecked: (checked: boolean) => any
	updateText: (text: string) => any
}

class ChecklistItem extends React.Component<ChecklistItemProps> {

	constructor(props: ChecklistItemProps) {
		super(props)
		this.handleChange = this.handleChange.bind(this)
	}

	handleChange(event: React.ChangeEvent<HTMLInputElement>) {
		const { type, value, checked } = event.currentTarget
		if (type == "text") {
			this.props.updateText(value)
		} else if (type == "checkbox") {
			this.props.setChecked(checked)
		}
	}

	render() {
		const { checked, text, remove } = this.props
		return (
			<li>
				<input type="checkbox" checked={checked} onChange={this.handleChange} />
				<input type="text" value={text} onChange={this.handleChange} />
				<button onClick={remove}>Delete</button>
			</li>
		)
	}
}

export class ChecklistView extends React.Component<ChecklistViewProps> {

	constructor(props: ChecklistViewProps) {
		super(props)
		this.handleSetChecked = this.handleSetChecked.bind(this)
		this.handleUpdateText = this.handleUpdateText.bind(this)
		this.handleItemDelete = this.handleItemDelete.bind(this)
		this.handleItemCreate = this.handleItemCreate.bind(this)
		this.mapItemToElement = this.mapItemToElement.bind(this)
	}

	handleUpdateText(index: number, text: string) {
		this.props.updateItemText(index, text)
	}

	handleSetChecked(index: number, checked: boolean) {
		this.props.setItemChecked(index, checked)
	}

	handleItemDelete(index: number) {
		this.props.removeItem(index)
	}

	handleItemCreate() {
		this.props.addItem()
	}

	mapItemToElement(item: Item, index: number) {
		return <ChecklistItem
			{...item}
			key={index}
			remove={this.handleItemDelete.bind(this, index)}
			setChecked={this.handleSetChecked.bind(this, index)}
			updateText={this.handleUpdateText.bind(this, index)}
		/>
	}

	render() {
		const { items } = this.props
		const baseNoteProps = getBaseNoteProps(this.props)
		return (
			<BaseNote {...baseNoteProps} >
				<ul>
					{items.map(this.mapItemToElement)}
				</ul>
				<button onClick={this.handleItemCreate}>New Item</button>
			</BaseNote>
		)
	}
}