import AddressModel from './AddressModel'

export default class JobModel {
	constructor() {
		this.id = "";
		this.name = "";
		this.customer = "";
		this.maker = "";
		this.status = "";
		this.description = "";
		this.printerType = "";
		this.materialType = "";
		this.materialColor = "";
		this.notes = "";
		this.bidClose = "";
		this.address = null;
	}

	static AllPropsEqual(a, b) {
		return (
			a.id === b.id &&
			a.name === b.name &&
			a.customer === b.customer &&
			a.maker === b.maker &&
			a.status === b.status &&
			a.description === b.description &&
			a.printerType === b.printerType &&
			a.materialType === b.materialType &&
			a.materialColor === b.materialColor &&
			a.notes === b.notes &&
			a.bidClose === b.bidClose &&
			AddressModel.AllPropsEqual(a.address, b.address)
		);
	}
}
