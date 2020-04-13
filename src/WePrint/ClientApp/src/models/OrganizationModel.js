import { ArrayDeepEquals } from '../api/CommonApi';

export default class OrganizationModel {
	constructor() {
		this.id = "";
		this.name = "";
		this.logo = "";
		this.description = "";
		this.users = [];
		this.projects = [];
	}

	static AllPropsEqual(a, b) {
		return (
			a.id === b.id &&
			a.name === b.name &&
			a.logo === b.logo &&
			a.description === b.description &&
			ArrayDeepEquals(a.users, b.users, (x, y) => x === y) &&
			ArrayDeepEquals(a.projects, b.projects, (x, y) => x === y)
		);
	}
}
