import TimeModel from './TimeModel';

export default class BidModel {
  constructor() {
    this.id = null;
    this.bidderId = null;
    this.jobIdempotencyKey = 0;
    this.jobId = null;
    this.price = 0;
    this.workTime = new TimeModel();
    this.notes = null;
    this.layerHeight = 0;
    this.shellThickness = 0;
    this.fillPercentage = 0;
    this.supportDensity = 0;
    this.printerId = null;
    this.materialType = 'ABS';
    this.materialColor = 'Red';
    this.finishType = 'None';
    this.idempotencyKey = 0;
    this.accepted = false;
  }

  static IdEquals(a, b) {
    return a.id === b.id && a.idempotencyKey === b.idempotencyKey;
  }

  static AllPropertiesEqual(a, b) {
    return (
      BidModel.IdEquals(a, b) &&
      a.bidderId === b.bidderId &&
      a.jobIdempotencyKey === b.jobIdempotencyKey &&
      a.jobId === b.jobId &&
      a.price === b.price &&
      TimeModel.AllPropertiesEqual(a.workTime, b.workTime) &&
      a.notes === b.notes &&
      a.layerHeight === b.layerHeight &&
      a.shellThickness === b.shellThickness &&
      a.fillPercentage === b.fillPercentage &&
      a.supportDensity === b.supportDensity &&
      a.printerId === b.printerId &&
      a.materialType === b.materialType &&
      a.materialColor === b.materialColor &&
      a.finishType === b.finishType &&
      a.accepted === b.accepted
    );
  }
}
