import axios from "axios-observable";
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus } from "./CommonApi";
import { JobModel } from "./JobModel";
import { BidModel } from "./BidModel";

export class BidApi {
    static MyBids() {
        return axios.get(BuildUrl('bid')).pipe(ErrorOnBadStatus);
    }

    static GetBid(id) {
        return axios.get(BuildUrl('bid', id)).pipe(ErrorOnBadStatus);
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
