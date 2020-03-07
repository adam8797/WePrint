import axios from 'axios-observable';
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus, ArrayDeepEquals } from './CommonApi';
import JobModel from '../models/JobModel';

// See BidApi for documentation, this works the same way that does.
export default class JobApi {
  static MyJobs() {
    return axios.get(BuildUrl('job')).pipe(ErrorOnBadStatus);
  }

  static TrackMyJobs(pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => JobApi.MyJobs()),
      distinctUntilChanged((a, b) => ArrayDeepEquals(a, b, JobModel.AllPropsEquals))
    );
  }

  static GetJob(id) {
    return axios.get(BuildUrl('job', id)).pipe(ErrorOnBadStatus);
  }

  static CreateJob(jobModel) {
    return axios.post(BuildUrl('job'), { params: jobModel }).pipe(ErrorOnBadStatus);
  }

  static UpdateJob(id, jobModel) {
    return axios.put(BuildUrl('job', id), { params: jobModel }).pipe(ErrorOnBadStatus);
  }

  static SearchJobs(searchString) {
    return axios.get(BuildUrl('search'), { params: { q: searchString } }).pipe(ErrorOnBadStatus);
  }

  // Tracks a job, returning an observable that emits a value every time the job changes
  static TrackJob(id, pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => JobApi.GetJob(id)),
      distinctUntilChanged(JobModel.AllPropsEquals)
    );
  }
}
