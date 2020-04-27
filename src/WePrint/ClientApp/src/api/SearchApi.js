import axios from 'axios-observable';
import { BuildUrl, ErrorOnBadStatus } from './CommonApi';

// See BidApi for documentation, this works the same way that does.
export default class SearchApi {
    static Search(q) {
        return axios.get(BuildUrl('search'), { params: { q } }).pipe(ErrorOnBadStatus);
    }
}
