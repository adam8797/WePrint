import axios from 'axios-observable';
import NestedApi from './NestedApi';
import { ErrorOnBadStatus } from './CommonApi';

export default class PledgeApi extends NestedApi {
    getCompletionUrl(pledgeId) {
        return `./recieved/${super.objId}/${pledgeId}`;
    }

    complete(subId) {
        return axios.patch(this.BuildSubUrl(subId, 'setstatus')).pipe(ErrorOnBadStatus);
    }
}