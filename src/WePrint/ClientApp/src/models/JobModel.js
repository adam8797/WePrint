import { isEqual } from 'lodash';
import AddressModel from './AddressModel';
import { JobStatus, PrinterType, MaterialType, MaterialColor } from './Enums';

export default class JobModel {
  constructor() {
    this.id = undefined;
    this.name = null;
    this.customerId = null;
    this.makerId = null;
    this.status = JobStatus.PendingOpen;
    this.description = '';
    this.printerType = PrinterType.SLA;
    this.materialType = MaterialType.ABS;
    this.materialColor = MaterialColor.Any;
    this.notes = '';
    this.bidClose = null;
    this.address = new AddressModel(null, null, null, 0);
    this.sliceReports = [];
  }

  static IdEquals(a, b) {
    return a.id === b.id;
  }

  static AllPropsEquals(a, b) {
    return (
      JobModel.IdEquals(a, b) &&
      a.name === b.name &&
      a.customerId === b.customerId &&
      a.makerId === b.makerId &&
      a.status === b.status &&
      a.description === b.description &&
      a.printerType === b.printerType &&
      a.materialType === b.materialType &&
      a.materialColor === b.materialColor &&
      a.notes === b.notes &&
      a.bidClose === b.bidClose &&
      isEqual(a.sliceReports, b.sliceReports) &&
      AddressModel.Equals(a.address, b.address)
    );
  }
}
