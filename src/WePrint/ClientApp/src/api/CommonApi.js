import { switchMap } from 'rxjs/operators';
import { of, throwError } from 'rxjs';

// Root endpoint of the API. You can point this at different urls for development and whatnot
const ROOT_ENDPOINT = './api/';

export function BuildUrl(...components) {
  return ROOT_ENDPOINT + components.map(encodeURIComponent).join('/');
}

export const ErrorOnBadStatus = switchMap(res =>
  res.status >= 200 && res.status < 300 ? of(res.data) : throwError(res.status)
);

export function ArrayDeepEquals(a, b, elementComparer) {
  if (a.length !== b.length) return false;
  for (let i = 0; i < a.length; i++) if (!elementComparer(a[i], b[i])) return false;

  return true;
}
