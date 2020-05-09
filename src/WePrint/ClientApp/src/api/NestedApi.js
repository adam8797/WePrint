import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus } from './CommonApi';

export default class NestedApi {
  constructor(apiPathBeforeId, objId, apiPathAfterId, objPropertyName, itemEqualityComparer) {
    this.objId = objId;
    this.apiPathBeforeId = apiPathBeforeId;
    this.apiPathAfterId = apiPathAfterId;
    this.objPropertyName = objPropertyName;
    this.itemEqualityComparer = itemEqualityComparer;
  }

  BuildSubUrl(...path) {
    return BuildUrl(this.apiPathBeforeId, this.objId, this.apiPathAfterId, ...path);
  }

  getAll() {
    return axios.get(this.BuildSubUrl()).pipe(ErrorOnBadStatus);
  }

  get(subId) {
    return axios.get(this.BuildSubUrl(subId)).pipe(ErrorOnBadStatus);
  }

  add(subItem) {
    return axios.post(this.BuildSubUrl(), subItem).pipe(ErrorOnBadStatus);
  }

  remove(subId) {
    return axios.delete(this.BuildSubUrl(subId)).pipe(ErrorOnBadStatus);
  }
}
