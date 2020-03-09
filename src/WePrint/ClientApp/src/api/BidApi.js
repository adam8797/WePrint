import axios from 'axios-observable';
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus, ArrayDeepEquals } from './CommonApi';
import BidModel from '../models/BidModel';

export default class BidApi {
  // Returns an observable, which generates an array of all your bids
  static MyBids() {
    return axios.get(BuildUrl('bid')).pipe(ErrorOnBadStatus);
  }

  // Returns an observable, which returns a single bid matching the BidModel model.
  // If a bid is not found, the observable returns an error.
  static GetBid(id) {
    return axios.get(BuildUrl('bid', id)).pipe(ErrorOnBadStatus);
  }

  // Returns an observable, which returns an array of bids for all bids under a specified job.
  // If the job is not found, the observable returns an error.
  static GetBidsForJob(jobid) {
    return axios.get(BuildUrl('job', jobid, 'bid')).pipe(ErrorOnBadStatus);
  }

  // Returns an observable which emits the same data as GetBidsForJob.
  // Any time the result of GetBidsForJob changes, the observable will emit the new values, at most every pollInterval.
  // Note: THIS IS A POLLING CALL. Remember to unsubscribe when you're done with it, or it might just keep making network requests in the background forever.
  static TrackBidsForJob(jobid, pollInterval) {
    return timer(0, pollInterval).pipe(
      ErrorOnBadStatus,
      exhaustMap(() => BidApi.GetBidsForJob(jobid)),
      // IF it turns out the order of bids can change, we need to sort them, but I don't think it ever will.
      // map(bids => bids.sort((a,b) => a.id.localeCompare(b.id))),
      distinctUntilChanged((a, b) => ArrayDeepEquals(a, b, BidModel.AllPropertiesEqual))
    );
  }

  // Returns an observable, which emits a bid Id when the bid is created.
  // If the creation fails, the observable returns an error.
  static CreateBid(bidModel, jobId) {
    return axios.post(BuildUrl('job', jobId, 'bids'), bidModel).pipe(ErrorOnBadStatus);
  }

  // Returns an observable, which emits a bid Id when the bid is updated.
  // If the update fails, the observable returns an error.
  static UpdateBid(id, bidModel) {
    return axios.put(BuildUrl('bid', id), { params: bidModel }).pipe(ErrorOnBadStatus);
  }

  // Returns an observable, which completes when the bid is deleted.
  // If the deletion fails, the observable returns an error.
  static DeleteBid(id, bidModel) {
    return axios.put(BuildUrl('bid', id), { params: bidModel }).pipe(ErrorOnBadStatus);
  }

  // Returns an observable which emits the same data as GetBid.
  // Any time the result of GetBid changes, the observable will emit the new value, at most every pollInterval.
  // Note: THIS IS A POLLING CALL. Remember to unsubscribe when you're done with it, or it might just keep making network requests in the background forever.
  static TrackBid(id, pollInterval) {
    return timer(0, pollInterval).pipe(
      ErrorOnBadStatus,
      exhaustMap(() => BidApi.GetBid(id)),
      distinctUntilChanged(BidModel.AllPropertiesEqual)
    );
  }
}
