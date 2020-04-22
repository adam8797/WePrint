export default class ProjectUpdateModel {
  constructor() {
    this.id = '';
    this.timestamp = '';
    this.editTimestamp = '';
    this.body = '';
    this.title = '';
    this.postedBy = '';
    this.editedBy = '';
    this.project = '';
    this.deleted = '';
  }

  static AllPropsEqual(a, b) {
    return (
      a.id === b.id &&
      a.timestamp === b.timestamp &&
      a.editTimestamp === b.editTimestamp &&
      a.body === b.body &&
      a.title === b.title &&
      a.postedBy === b.postedBy &&
      a.editedBy === b.editedBy &&
      a.project === b.project &&
      a.deleted === b.deleted
    );
  }
}
