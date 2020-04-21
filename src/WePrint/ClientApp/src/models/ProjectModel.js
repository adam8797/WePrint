import { ArrayDeepEquals } from '../api/CommonApi';
import AddressModel from './AddressModel';

export default class ProjectModel {
  constructor() {
    this.id = '';
    this.title = '';
    this.description = '';
    this.goal = '';
    this.progress = [];
    this.shippingInstructions = '';
    this.printingInstructions = '';
    this.address = null;
    this.closed = '';
    this.openGoal = '';
    this.organization = '';
    this.pledges = [];
    this.updates = [];
    this.attachments = [];
    this.deleted = '';
  }

  static AllPropsEqual(a, b) {
    return (
      a.id === b.id &&
      a.title === b.title &&
      a.description === b.description &&
      a.goal === b.goal &&
      ArrayDeepEquals(a.progress, b.progress, (x, y) => x === y) &&
      a.shippingInstructions === b.shippingInstructions &&
      a.printingInstructions === b.printingInstructions &&
      AddressModel.AllPropsEquals(a.address, b.address) &&
      a.closed === b.closed &&
      a.openGoal === b.openGoal &&
      a.organization === b.organization &&
      ArrayDeepEquals(a.pledges, b.pledges, (x, y) => x === y) &&
      ArrayDeepEquals(a.updates, b.updates, (x, y) => x === y) &&
      ArrayDeepEquals(a.attachments, b.attachments, (x, y) => x === y) &&
      a.deleted === b.deleted
    );
  }
}
