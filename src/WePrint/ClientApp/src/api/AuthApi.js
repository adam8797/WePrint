import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus } from './CommonApi'

const apiPath = "auth";
export class AuthApi {

  wrapErrors(observable) {
    return observable.pipe(ErrorOnBadStatus);
  }

  login(username, password, remember) {
    if(remember === undefined || remember === null) {
        // eslint-disable-next-line no-param-reassign
        remember = false;
    }
    return axios.post(BuildUrl(apiPath, 'login'), { username, password, remember }).pipe(ErrorOnBadStatus);
  }

  register(email, username, firstName, lastName, password) {
    return axios.post(BuildUrl(apiPath, 'register'), { email, username, password, firstName, lastName }).pipe(ErrorOnBadStatus);
  }
}

export default new AuthApi();