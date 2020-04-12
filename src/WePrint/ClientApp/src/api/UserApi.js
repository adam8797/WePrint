import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus, usersApiPath } from './CommonApi';

export default class UserApi {
  static CurrentUser() {
    return axios.get(BuildUrl(usersApiPath)).pipe(ErrorOnBadStatus);
  }

  static GetUser(id) {
    return axios.get(BuildUrl(usersApiPath, 'by-id', id)).pipe(ErrorOnBadStatus);
  }

    static GetUserByUsername(username) {
        return axios.get(BuildUrl(usersApiPath, 'by-name', username)).pipe(ErrorOnBadStatus);
    }
}
