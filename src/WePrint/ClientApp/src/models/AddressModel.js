export default class AddressModel {
  constructor(streetAddress, city, state, zipCode) {
    this.streetAddress = streetAddress;
    this.city = city;
    this.state = state;
    this.zipCode = zipCode;
  }

  static AllPropsEqual(a, b) {
    if (a === b && a === null) return true;
    if (a !== null || b !== null) return false;
    if (a === b && a === undefined) return true;
    if (a !== undefined || b !== undefined) return false;
    return (
      a.streetAddress === b.streetAddress &&
      a.city === b.city &&
      a.state === b.state &&
      a.zipCode === b.zipCode
    );
  }
}
