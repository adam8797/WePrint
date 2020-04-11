import axios from 'axios-observable';
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus, ArrayDeepEquals, devices_api_path } from './CommonApi';
import PrinterModel from '../models/PrinterModel';


export default class PrinterApi {
  static MyPrinters() {
    return axios.get(BuildUrl(devices_api_path)).pipe(ErrorOnBadStatus);
  }

  static TrackMyPrinters(pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => PrinterApi.MyPrinters()),
      distinctUntilChanged((a, b) => ArrayDeepEquals(a, b, PrinterModel.AllPropsEquals))
    );
  }

  static GetPrinter(id) {
    return axios.get(BuildUrl(devices_api_path, id)).pipe(ErrorOnBadStatus);
  }

  static CreatePrinter(printerModel) {
    return axios.post(BuildUrl(devices_api_path), printerModel).pipe(ErrorOnBadStatus);
  }

  static UpdatePrinter(id, printerModel) {
    return axios.put(BuildUrl(devices_api_path, id), printerModel).pipe(ErrorOnBadStatus);
  }

  static DeletePrinter(id) {
    return axios.delete(BuildUrl(devices_api_path, id)).pipe(ErrorOnBadStatus);
  }
}
