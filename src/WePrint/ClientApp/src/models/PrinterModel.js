
export default class PrinterModel {
	constructor() {
		this.id = "";
		this.ownerId = "";
		this.name = "";
		this.type = "";
		this.xMax = "";
		this.yMax = "";
		this.zMax = "";
		this.layerMin = "";
	}

	static AllPropsEqual(a, b) {
		return (
			a.id === b.id &&
			a.ownerId === b.ownerId &&
			a.name === b.name &&
			a.type === b.type &&
			a.xMax === b.xMax &&
			a.yMax === b.yMax &&
			a.zMax === b.zMax &&
			a.layerMin === b.layerMin
		);
	}
}
