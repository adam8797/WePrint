import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus, users_api_path } from './CommonApi';

export default class UserApi {
  static CurrentUser() {
    return axios.get(BuildUrl(users_api_path)).pipe(ErrorOnBadStatus);
  }

  static GetUser(id) {
    return axios.get(BuildUrl(users_api_path, 'by-id', id)).pipe(ErrorOnBadStatus);
  }

    static GetUserByUsername(username) {
        return axios.get(BuildUrl(users_api_path, 'by-name', username)).pipe(ErrorOnBadStatus);
    }
}
