import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus, authApiPath, CommonApi } from './CommonApi';

class AuthApi extends CommonApi {
  constructor() {
    super(authApiPath, null);
  }

  changePassword(payload) {
    return axios.post(BuildUrl(authApiPath, 'changePassword'), payload).pipe(ErrorOnBadStatus);
  }
}

export default new AuthApi();
