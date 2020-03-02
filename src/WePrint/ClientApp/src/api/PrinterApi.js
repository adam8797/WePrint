import axios from "axios-observable";
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus, ArrayDeepEquals } from "./CommonApi";
import { PrinterModel } from "../models/PrinterModel";

export class PrinterApi {
    static MyPrinters() {
        return axios.get(BuildUrl('printer')).pipe(ErrorOnBadStatus);
    }

    static TrackMyPrinters(pollInterval) {
        return timer(0, pollInterval).pipe(
            exhaustMap(v => PrinterApi.MyPrinters()),
            distinctUntilChanged((a,b) => ArrayDeepEquals(a, b, PrinterModel.AllPropsEquals))
        );
    }

    static GetPrinter(id) {
        return axios.get(BuildUrl('printer', id)).pipe(ErrorOnBadStatus);
    }

    static CreatePrinter(printerModel) {
        return axios.post(BuildUrl('printer'), { params: printerModel }).pipe(ErrorOnBadStatus);
    }

    static UpdatePrinter(id, printerModel) {
        return axios.put(BuildUrl('printer', id), { params: printerModel }).pipe(ErrorOnBadStatus);
    }
    
    static DeletePrinter(id) {
        return axios.delete(BuildUrl('printer', id)).pipe(ErrorOnBadStatus);
    }
}
