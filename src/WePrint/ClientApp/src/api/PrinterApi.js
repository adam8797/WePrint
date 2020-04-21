import CommonApi from './CommonApi';
import PrinterModel from '../models/PrinterModel';

class PrinterApi extends CommonApi {
  constructor() {
    super('devices', PrinterModel.AllPropsEqual);
  }
}

export default new PrinterApi();