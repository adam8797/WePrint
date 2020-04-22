import { ArrayDeepEquals } from '../api/CommonApi';
import AddressModel from './AddressModel';

export default class OrganizationModel {
  constructor() {
    this.id = '';
    this.name = '';
    this.logo = '';
    this.description = '';
    this.users = [];
    this.projects = [];
    this.address = null;
    this.deleted = '';
  }

  static AllPropsEqual(a, b) {
    return (
      a.id === b.id &&
      a.name === b.name &&
      a.logo === b.logo &&
      a.description === b.description &&
      ArrayDeepEquals(a.users, b.users, (x, y) => x === y) &&
      ArrayDeepEquals(a.projects, b.projects, (x, y) => x === y) &&
      AddressModel.AllPropsEquals(a.address, b.address) &&
      a.deleted === b.deleted
    );
  }
}
