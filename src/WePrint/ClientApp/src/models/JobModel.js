import AddressModel from './AddressModel';
import { JobStatus, PrinterType, MaterialType, MaterialColor } from './Enums';

export default class JobModel {
  constructor() {
    this.id = null;
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
  }

  static IdEquals(a, b) {
    return a.id === b.id && a.idempotencyKey === b.idempotencyKey;
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
      AddressModel.Equals(a.address, b.address)
    );
  }
}
