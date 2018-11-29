import React from "react"
import { map, Map, marker, Marker, LeafletMouseEvent, tileLayer, icon } from "leaflet"
import "leaflet/dist/leaflet.css"
import markerRetinaUrl from "leaflet/dist/images/marker-icon-2x.png"
import markerUrl from "leaflet/dist/images/marker-icon.png"
import markerShadowUrl from "leaflet/dist/images/marker-shadow.png"
import { BaseNoteProps, getBaseNoteProps, BaseNoteView } from "./BaseNote"

const markerIcon = icon({
	iconAnchor: [12, 41],
	iconSize: [25, 41],
	shadowSize: [41, 41],
	iconRetinaUrl: markerRetinaUrl,
	iconUrl: markerUrl,
	shadowUrl: markerShadowUrl
})

export interface LocationViewProps extends BaseNoteProps {
	latitude: number
	longitude: number
	setToCurrentLocation: () => any
	setLocation: (latitude: number, longitude: number) => any
}

export class LocationView extends React.Component<LocationViewProps> {
	map!: Map
	marker!: Marker
	mapRef = React.createRef<HTMLDivElement>()

	componentDidMount() {
		const { latitude, longitude } = this.props
		const latlng: [number, number] = [latitude, longitude]
		this.map = map(this.mapRef.current as HTMLElement)
			.setView(latlng, 13)
			.addEventListener("click", (event) => {
				this.props.setLocation(
					(event as LeafletMouseEvent).latlng.lat,
					(event as LeafletMouseEvent).latlng.lng
				)
			})
		tileLayer(`https://api.tiles.mapbox.com/v4/mapbox.streets/{z}/{x}/{y}.png?access_token=${process.env.REACT_APP_MAPBOX_KEY}`, {
			maxZoom: 18,
			attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, ' +
				'<a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
				'Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
		}).addTo(this.map)
		this.marker = marker(latlng, { icon: markerIcon }).addTo(this.map)
	}

	componentDidUpdate() {
		const { latitude, longitude } = this.props
		const latlng: [number, number] = [latitude, longitude]
		this.map.setView(latlng, 13)
		this.marker.setLatLng(latlng)
	}

	componentWillUnmount() {
		this.map.remove()
	}

	render() {
		const { latitude, longitude, setToCurrentLocation } = this.props
		const baseNoteProps = getBaseNoteProps(this.props)
		return (
			<BaseNoteView {...baseNoteProps} >
				<div ref={this.mapRef} style={{ height: "400px", maxWidth: "600px" }} />
				<div>({latitude}, {longitude})</div>
				<button onClick={setToCurrentLocation}>Set to current location</button>
				<div>Click or touch map to set to that location</div>
			</BaseNoteView>
		)
	}
}