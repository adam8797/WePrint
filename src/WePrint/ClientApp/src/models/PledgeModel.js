
export default class PledgeModel {
  constructor() {
    this.id = '';
    this.deliveryDate = '';
    this.created = '';
    this.quantity = '';
    this.status = '';
    this.anonymous = '';
    this.project = '';
    this.maker = '';
    this.deleted = '';
  }

  static AllPropsEqual(a, b) {
    return (
      a.id === b.id &&
      a.deliveryDate === b.deliveryDate &&
      a.created === b.created &&
      a.quantity === b.quantity &&
      a.status === b.status &&
      a.anonymous === b.anonymous &&
      a.project === b.project &&
      a.maker === b.maker &&
      a.deleted === b.deleted
    );
  }
}
