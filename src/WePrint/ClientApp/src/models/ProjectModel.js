import { ArrayDeepEquals } from '../api/CommonApi';
import AddressModel from './AddressModel';

export default class ProjectModel {
  constructor() {
    this.id = '';
    this.description = '';
    this.goal = '';
    this.shippingInstructions = '';
    this.printingInstructions = '';
    this.thumbnail = '';
    this.address = null;
    this.closed = '';
    this.openGoal = '';
    this.organization = '';
    this.pledges = [];
    this.updates = [];
    this.attachments = [];
  }

  static AllPropsEqual(a, b) {
    return (
      a.id === b.id &&
      a.description === b.description &&
      a.goal === b.goal &&
      a.shippingInstructions === b.shippingInstructions &&
      a.printingInstructions === b.printingInstructions &&
      a.thumbnail === b.thumbnail &&
      AddressModel.AllPropsEqual(a.address, b.address) &&
      a.closed === b.closed &&
      a.openGoal === b.openGoal &&
      a.organization === b.organization &&
      ArrayDeepEquals(a.pledges, b.pledges, (x, y) => x === y) &&
      ArrayDeepEquals(a.updates, b.updates, (x, y) => x === y) &&
      ArrayDeepEquals(a.attachments, b.attachments, (x, y) => x === y)
    );
  }
}
