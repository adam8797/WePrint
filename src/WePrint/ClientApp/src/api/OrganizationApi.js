import { CommonApi } from "./CommonApi";
import OrganizationModel from "../models/OrganizationModel"
import UserModel from "../models/UserModel"
import NestedApi from "./NestedApi";
import ProjectModel from "../models/ProjectModel";

class OrgApi extends CommonApi {
    constructor() {
        super("organizations", OrganizationModel.AllPropsEqual);

        // Used to get/add/remove users to an organization
        this.users = new NestedApi("organizations", "users", "users", UserModel.AllPropsEqual);
        // Used to get/add/remove projects to an organization
        this.projects = new NestedApi("organizations", "projects", "projects", ProjectModel.AllPropsEqual);
    }
}

const orgApi = new OrgApi();
export default orgApi;