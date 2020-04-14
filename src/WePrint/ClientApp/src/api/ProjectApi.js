import { CommonApi } from "./CommonApi";
import NestedApi from "./NestedApi";
import ProjectModel from "../models/ProjectModel";
import PledgeModel from "../models/PledgeModel";
import UpdateModel from "../models/UpdateModel";

class ProjectApi extends CommonApi {
    constructor() {
        super("projects", ProjectModel.AllPropsEqual);

        // Used to get/add/remove pledges from a project
        this.pledges = new NestedApi("projects", "pledges", "pledges", PledgeModel.AllPropsEqual);
        // Used to get/add/remove updates from a project
        this.updates = new NestedApi("projects", "updates", "updates", UpdateModel.AllPropsEqual);
    }
}

const projApi = new ProjectApi();
export default projApi;