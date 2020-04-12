import axios from 'axios-observable';
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus, ArrayDeepEquals, devicesApiPath } from './CommonApi';
import PrinterModel from '../models/PrinterModel';


export default class PrinterApi {
  static MyPrinters() {
    return axios.get(BuildUrl(devicesApiPath)).pipe(ErrorOnBadStatus);
  }

  static TrackMyPrinters(pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => PrinterApi.MyPrinters()),
      distinctUntilChanged((a, b) => ArrayDeepEquals(a, b, PrinterModel.AllPropsEqual))
    );
  }

  static GetPrinter(id) {
    return axios.get(BuildUrl(devicesApiPath, id)).pipe(ErrorOnBadStatus);
  }

  static CreatePrinter(printerModel) {
    return axios.post(BuildUrl(devicesApiPath), printerModel).pipe(ErrorOnBadStatus);
  }

  static UpdatePrinter(id, printerModel) {
    return axios.put(BuildUrl(devicesApiPath, id), printerModel).pipe(ErrorOnBadStatus);
  }

  static DeletePrinter(id) {
    return axios.delete(BuildUrl(devicesApiPath, id)).pipe(ErrorOnBadStatus);
  }
}
