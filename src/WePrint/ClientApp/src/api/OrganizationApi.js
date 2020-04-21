import { CommonApi } from './CommonApi';
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
    return new NestedApi('organizations', orgId, 'projects', 'projects', ProjectModel.AllPropsEqual);
  }
}

export default new OrgApi();
