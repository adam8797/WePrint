import axios from 'axios-observable';
import NestedApi from './NestedApi';
import { ErrorOnBadStatus } from './CommonApi';

export default class PledgeApi extends NestedApi {
  getCompletionUrl(pledgeId) {
    return `./recieved/${super.objId}/${pledgeId}`;
  }

  setStatus(subId, newStatus) {
    return axios
      .patch(this.BuildSubUrl(subId, 'setstatus'), null, { params: { newStatus } })
      .pipe(ErrorOnBadStatus);
  }
}
