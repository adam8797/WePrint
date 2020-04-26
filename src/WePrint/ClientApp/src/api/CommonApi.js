import axios from 'axios-observable';
import { of, throwError, timer } from 'rxjs';
import { switchMap, exhaustMap, distinctUntilChanged } from 'rxjs/operators';

// Root endpoint of the API. You can point this at different urls for development and whatnot
export const ROOT_ENDPOINT = './api/';

export function BuildUrl(...components) {
  return ROOT_ENDPOINT + components.map(encodeURIComponent).join('/');
}

export const ErrorOnBadStatus = switchMap(res =>
  res.status >= 200 && res.status < 300 ? of(res.data) : throwError(res.status)
);

export function ObjectToPatch(obj) {
  const keys = Object.keys(obj);
  return keys.map(prop => ({ op: 'replace', path: prop, value: obj[prop] }));
}

export function ArrayDeepEquals(a, b, elementComparer) {
  if (a.length !== b.length) return false;
  for (let i = 0; i < a.length; i++) if (!elementComparer(a[i], b[i])) return false;

  return true;
}

export class CommonApi {
  constructor(apiPath, itemEqualityComparer) {
    this.apiPath = apiPath;
    this.itemEqualityComparer = itemEqualityComparer;
  }

  wrapErrors(observable) {
    return observable.pipe(ErrorOnBadStatus);
  }

  // Returns an observable, which generates an array of all items at this endpoint
  getAll() {
    return axios.get(BuildUrl(this.apiPath)).pipe(ErrorOnBadStatus);
  }

  // Returns an observable, which returns a single object.
  // If an id is not found, the observable returns an error.
  get(id) {
    return axios.get(BuildUrl(this.apiPath, id)).pipe(ErrorOnBadStatus);
  }

  // Create the given item on the server, returning an observable which emits the whole saved object
  create(item) {
    return axios.post(BuildUrl(this.apiPath), item).pipe(ErrorOnBadStatus);
  }

  // HTTP-PUT, replace the item with the given id with the provided item in its entirety.
  replace(id, item) {
    return axios.put(BuildUrl(this.apiPath, id), item).pipe(ErrorOnBadStatus);
  }

  // PUT/PATCH hybrid, take a list of fields, and patch the object with the provdied id with the values of these fields.
  // For example, given an object: { foo: 'someFoo', bar: 'someBar' }, if you correct with { foo: 'newValue' }, you get { foo: 'newValue', bar: 'someBar' }
  // Note that this does not support adding new fields, nor removing existing fields, only updating fields with new values.
  correct(id, itemUpdates) {
    return axios
      .patch(BuildUrl(this.apiPath, id), ObjectToPatch(itemUpdates))
      .pipe(ErrorOnBadStatus);
  }

  // HTTP-PATCH, takes a JSON-PATCH object and applies it to the given object.
  patch(id, patchInstructions) {
    return axios.patch(BuildUrl(this.apiPath, id), patchInstructions).pipe(ErrorOnBadStatus);
  }

  // Delete the item with the given ID.
  delete(id) {
    return axios.delete(BuildUrl(this.apiPath), id).pipe(ErrorOnBadStatus);
  }

  // Returns an observable which emits the same data as Get(id).
  // Any time the result of Get(id) changes, the observable will emit the new values, at most every pollInterval.
  // Note: THIS IS A POLLING CALL. Remember to unsubscribe when you're done with it, or it might just keep making network requests in the background forever.
  track(id, pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => this.get(id)),
      distinctUntilChanged(this.itemEqualityComparer)
    );
  }

  // Returns an observable which emits the same data as GetAll().
  // Any time the result of GetAll() changes, the observable will emit the new values, at most every pollInterval.
  // Note: THIS IS A POLLING CALL. Remember to unsubscribe when you're done with it, or it might just keep making network requests in the background forever.
  trackAll(pollInterval) {
    return timer(0, pollInterval).pipe(
      exhaustMap(() => this.getAll()),
      distinctUntilChanged((a, b) => ArrayDeepEquals(a, b, this.itemEqualityComparer))
    );
  }
}

export const bidsApiPath = 'bids';
export const jobsApiPath = 'jobs';
export const usersApiPath = 'users';
export const devicesApiPath = 'devices';
