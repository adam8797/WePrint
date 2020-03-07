import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus } from './CommonApi';

export default class UserApi {
  static CurrentUser() {
    return axios.get(BuildUrl('user')).pipe(ErrorOnBadStatus);
  }

  static GetUser(id) {
    return axios.get(BuildUrl('user', id)).pipe(ErrorOnBadStatus);
  }
}
