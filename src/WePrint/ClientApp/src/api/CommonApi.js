import { switchMap } from 'rxjs/operators';
import { of, throwError } from 'rxjs';

const ROOT_ENDPOINT = './api/';

export function BuildUrl(...components) {
    return ROOT_ENDPOINT + components.map(encodeURIComponent).join('/');
}

export const ErrorOnBadStatus = switchMap(res => 
    res.status >= 200 && res.status < 300 ? 
    of(res.data) :
    throwError(res.status));