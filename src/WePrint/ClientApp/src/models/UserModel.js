
export default class UserModel {
	constructor() {
		this.id = "";
		this.firstName = "";
		this.lastName = "";
		this.bio = "";
		this.username = "";
	}

	static AllPropsEqual(a, b) {
		return (
			a.id === b.id &&
			a.firstName === b.firstName &&
			a.lastName === b.lastName &&
			a.bio === b.bio &&
			a.username === b.username
		);
	}
}
