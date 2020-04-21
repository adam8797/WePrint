export default class SearchModel {
  constructor() {
    this.title = '';
    this.description = '';
    this.imageUrl = '';
    this.id = '';
    this.type = '';
  }

  static AllPropsEqual(a, b) {
    return (
      a.title === b.title &&
      a.description === b.description &&
      a.imageUrl === b.imageUrl &&
      a.id === b.id &&
      a.type === b.type
    );
  }
}
