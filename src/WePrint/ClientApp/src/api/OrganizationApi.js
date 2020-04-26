import axios from 'axios-observable';
import { CommonApi, BuildUrl } from './CommonApi';
import OrganizationModel from '../models/OrganizationModel';
import UserModel from '../models/UserModel';
import NestedApi from './NestedApi';
import ProjectModel from '../models/ProjectModel';

class OrgApi extends CommonApi {
  constructor() {
    super('organizations', OrganizationModel.AllPropsEqual);
  }

  usersFor(orgId) {
    return new NestedApi('organizations', orgId, 'users', 'users', UserModel.AllPropsEqual);
  }

  projectsFor(orgId) {
    return new NestedApi(
      'organizations',
      orgId,
      'projects',
      'projects',
      ProjectModel.AllPropsEqual
    );
  }

  getAvatar(id) {
    return super.wrapErrors(axios.get(this.getThumbnailUrl(id)));
  }

  getAvatarUrl(id) {
    return BuildUrl(this.apiPath, id, 'avatar');
  }
}

export default new OrgApi();
