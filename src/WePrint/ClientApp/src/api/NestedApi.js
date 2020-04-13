import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus } from './CommonApi'

export default class NestedApi {
    constructor(apiPathBeforeId, apiPathAfterId, objPropertyName, itemEqualityComparer) {
        this.apiPathBeforeId = apiPathBeforeId;
        this.apiPathAfterId = apiPathAfterId;
        this.objPropertyName = objPropertyName;
        this.itemEqualityComparer = itemEqualityComparer;
    }

    BuildSubUrl(id, ...path) {
        return BuildUrl(this.apiPathBeforeId, id, this.apiPathAfterId, ...path);
    }

    GetAll (id) {
        return axios.get(this.BuildSubUrl(id)).pipe(ErrorOnBadStatus);
    }
    
    Get (id, subId) {
        return axios.get(this.BuildSubUrl(id, subId)).pipe(ErrorOnBadStatus);
    }
    
    Add (id, subItem) {
        const patch = { op: 'add', path: `/${this.objPropertyName}/-`, value: subItem };
        return axios.patch(BuildUrl(this.apiPathBeforeId, id), patch).pipe(ErrorOnBadStatus);
    }
}