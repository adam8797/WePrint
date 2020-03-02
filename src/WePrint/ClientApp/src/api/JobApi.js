import axios from "axios-observable";
import { timer } from 'rxjs';
import { exhaustMap, distinctUntilChanged } from 'rxjs/operators';
import { BuildUrl, ErrorOnBadStatus } from "./CommonApi";
import { JobModel } from "../models/JobModel";

export class JobApi {
    static MyJobs() {
        return axios.get(BuildUrl('job')).pipe(ErrorOnBadStatus);
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

    // Tracks a job, returning an observable that emits a value every time the job changes
    static TrackJob(id, pollInterval) {
        return timer(0, pollInterval).pipe(
            exhaustMap(v => JobApi.GetJob(id)),
            distinctUntilChanged(JobModel.AllPropsEquals)
        );
    }
}
