import { PrinterType } from './Enums';

export default class PrinterModel {
  constructor() {
    this.id = undefined;
    this.name = null;
    this.xMax = 0;
    this.yMax = 0;
    this.zMax = 0;
    this.type = PrinterType.SLA;
    this.layerMin = 0;
  }

  static IdEquals(a, b) {
    return a.id === b.id;
  }

  static AllPropsEquals(a, b) {
    return (
      PrinterModel.IdEquals(a, b) &&
      a.name === b.name &&
      a.xMax === b.xMax &&
      a.yMax === b.yMax &&
      a.zMax === b.zMax &&
      a.type === b.type &&
      a.layerMin === b.layerMin
    );
  }
}
