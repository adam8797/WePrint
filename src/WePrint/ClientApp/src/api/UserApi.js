import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus, usersApiPath } from './CommonApi';

class UserApi {
  CurrentUser() {
    return axios.get(BuildUrl(usersApiPath)).pipe(ErrorOnBadStatus);
    }

    UpdateUser(userModel) {
        return axios.put(BuildUrl(usersApiPath), userModel).pipe(ErrorOnBadStatus);
    }

  GetUser(id) {
    return axios.get(BuildUrl(usersApiPath, 'by-id', id)).pipe(ErrorOnBadStatus);
  }

  GetUserByUsername(username) {
    return axios.get(BuildUrl(usersApiPath, 'by-name', username)).pipe(ErrorOnBadStatus);
  }
}

export default new UserApi();
