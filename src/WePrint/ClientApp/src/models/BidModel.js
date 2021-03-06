import TimeModel from './TimeModel';
import { MaterialType, MaterialColor, FinishType } from './Enums';

export default class BidModel {
  constructor() {
    this.id = undefined;
    this.bidderId = null;
    this.jobId = null;
    this.price = 0;
    this.workTime = new TimeModel();
    this.notes = null;
    this.layerHeight = 0;
    this.shellThickness = 0;
    this.fillPercentage = 0;
    this.supportDensity = 0;
    this.printerId = null;
    this.materialType = MaterialType.ABS;
    this.materialColor = MaterialColor.Red;
    this.finishType = FinishType.None;
    this.accepted = false;
  }

  static IdEquals(a, b) {
    return a.id === b.id;
  }

  static AllPropertiesEqual(a, b) {
    return (
      BidModel.IdEquals(a, b) &&
      a.bidderId === b.bidderId &&
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
