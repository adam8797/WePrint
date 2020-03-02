import axios from "axios-observable";
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged, map } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus, ArrayDeepEquals } from "./CommonApi";
import { BidModel } from "../models/BidModel";

export class BidApi {
    static MyBids() {
        return axios.get(BuildUrl('bid')).pipe(ErrorOnBadStatus);
    }

    static GetBid(id) {
        return axios.get(BuildUrl('bid', id)).pipe(ErrorOnBadStatus);
    }

    static GetBidsForJob(jobid) {
        return axios.get(BuildUrl('job', jobid, 'bid')).pipe(ErrorOnBadStatus);
    }
    
    static TrackBidsForJob(jobid, pollInterval) {
        return timer(0, pollInterval).pipe(
            exhaustMap(v => BidApi.GetBidsForJob(jobid)),
            // IF it turns out the order of bids can change, we need to sort them, but I don't think it ever will.
            // map(bids => bids.sort((a,b) => a.id.localeCompare(b.id))),
            distinctUntilChanged((a,b) => ArrayDeepEquals(a, b, BidModel.AllPropertiesEqual))
        );
    }

    static CreateBid(bidModel) {
        return axios.post(BuildUrl('bid'), { params: bidModel }).pipe(ErrorOnBadStatus);
    }

    static UpdateBid(id, bidModel) {
        return axios.put(BuildUrl('bid', id), { params: bidModel }).pipe(ErrorOnBadStatus);
    }

    // Tracks a bid, returning an observable that emits a value every time this bid changes
    static TrackBid(id, pollInterval) {
        return timer(0, pollInterval).pipe(
            exhaustMap(v => BidApi.GetBid(id)),
            distinctUntilChanged(BidModel.AllPropertiesEqual)
        );
    }
}
