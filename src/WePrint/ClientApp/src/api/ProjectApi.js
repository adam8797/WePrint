import axios from 'axios-observable';
import { CommonApi, BuildUrl } from './CommonApi';
import NestedApi from './NestedApi';
import ProjectModel from '../models/ProjectModel';
import PledgeModel from '../models/PledgeModel';
import ProjectUpdateModel from '../models/ProjectUpdateModel';
import PledgeApi from './PledgeApi';

class ProjectApi extends CommonApi {
  constructor() {
    super('projects', ProjectModel.AllPropsEqual);
  }

  pledgesFor(projectId) {
    return new PledgeApi('projects', projectId, 'pledges', 'pledges', PledgeModel.AllPropsEqual);
  }

  updatesFor(projectId) {
    return new NestedApi('projects', projectId, 'updates', 'updates', ProjectUpdateModel.AllPropsEqual);
  }

  getThumbnail(id) {
    return super.wrapErrors(axios.get(this.getThumbnailUrl(id)));
  }

  getThumbnailUrl(id) {
    return BuildUrl(this.apiPath, id, 'thumbnail');
  }

  setThumbnail(id, file) {
    return super.wrapErrors(axios.post(BuildUrl(this.apiPath, id, 'thumbnail'), file));
  }

}

export default new ProjectApi();
