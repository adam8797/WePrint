export default class ErrorModel {
  constructor() {
    this.requestId = '';
    this.showRequestId = '';
  }

  static AllPropsEqual(a, b) {
    return a.requestId === b.requestId && a.showRequestId === b.showRequestId;
  }
}
