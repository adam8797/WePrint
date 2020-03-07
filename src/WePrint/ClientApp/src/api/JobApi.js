import axios from 'axios-observable';
import { timer, Subject } from 'rxjs';
import { exhaustMap, distinctUntilChanged, switchMap, auditTime } from 'rxjs/operators';
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
    return axios.post(BuildUrl('job'), jobModel).pipe(ErrorOnBadStatus);
  }

  static UpdateJob(id, jobModel) {
    return axios.put(BuildUrl('job', id), jobModel).pipe(ErrorOnBadStatus);
  }

  // Returns an input object and output object. Subscribe to the output object to get search results.
  // Submit searches by calling .next("queryString") on the input object.
  // Requests will be automatically debounced, and old requests automatically cancelled.
  static BuildSearchStream() {
    const input = new Subject();
    return { input, output: JobApi.SearchJobsStream(input) };
  }

  // Probably most useful as a helper for BuildSearchStream();
  static SearchJobsStream(inputObservable) {
    return inputObservable.pipe(auditTime(1000), switchMap(JobApi.SearchJobs));
  }

  static SearchJobs(q) {
    return axios.get(BuildUrl('search'), { params: { q } }).pipe(ErrorOnBadStatus);
  }

  // Tracks a job, returning an observable that emits a value every time the job changes
  static TrackJob(id, pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => JobApi.GetJob(id)),
      distinctUntilChanged(JobModel.AllPropsEquals)
    );
  }

  //
  // File Operations
  //

  static GetJobFiles(jobId) {
    return axios.get(BuildUrl('job', jobId, 'files')).pipe(ErrorOnBadStatus);
  }

  static GetFile(jobId, fileName) {
    return axios.get(BuildUrl('job', jobId, 'files', fileName)).pipe(ErrorOnBadStatus);
  }

  static CreateFile(jobId, file, onUploadProgress) {
    return axios
      .post(BuildUrl('job', jobId, 'files'), file, {
        onUploadProgress: ProgressEvent => onUploadProgress(ProgressEvent, file),
      })
      .pipe(ErrorOnBadStatus);
  }

  static DeleteFile(jobId, fileName) {
    return axios.delete(BuildUrl('job', jobId, 'files', fileName)).pipe(ErrorOnBadStatus);
  }
}
