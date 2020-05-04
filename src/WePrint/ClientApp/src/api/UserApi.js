import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus, usersApiPath, CommonApi } from './CommonApi';
import UserModel from '../models/UserModel';

class UserApi extends CommonApi {
  constructor() {
    super(usersApiPath, UserModel.AllPropsEqual);
  }

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

  getAvatar(id) {
    return super.wrapErrors(axios.get(this.getThumbnailUrl(id)));
  }

  getAvatarUrl(id) {
    return BuildUrl(this.apiPath, 'by-id', id, 'avatar');
  }

  getPledges(id) {
    if (id) {
      return axios.get(BuildUrl(usersApiPath, 'pledges', id)).pipe(ErrorOnBadStatus);
    }
    return axios.get(BuildUrl(usersApiPath, 'pledges')).pipe(ErrorOnBadStatus);
  }
}

export default new UserApi();
