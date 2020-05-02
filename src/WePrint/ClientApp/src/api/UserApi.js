import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus, usersApiPath, CommonApi } from './CommonApi';
import UserModel from '../models/UserModel';

class UserApi extends CommonApi {
  constructor() {
    super(usersApiPath, UserModel.AllPropsEqual);
  }

  CurrentUser() {
    return axios.get(BuildUrl(usersApiPath, "current")).pipe(ErrorOnBadStatus);
    }

    UpdateUser(userModel) {
        return axios.put(BuildUrl(usersApiPath), userModel).pipe(ErrorOnBadStatus);
    }

  GetUser(id) {
    return axios.get(BuildUrl(usersApiPath, id)).pipe(ErrorOnBadStatus);
  }

  GetUserByUsername(username) {
    return axios.get(BuildUrl(usersApiPath, username)).pipe(ErrorOnBadStatus);
  }

  getAvatar(id) {
    return super.wrapErrors(axios.get(this.getThumbnailUrl(id)));
  }

  getAvatarUrl(id) {
    return BuildUrl(usersApiPath, id, 'avatar');
  }
}

export default new UserApi();
