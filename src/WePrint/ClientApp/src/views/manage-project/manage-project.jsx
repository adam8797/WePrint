import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Tab, TabList, TabPanel, Tabs } from 'react-tabs';
import { BodyCard, StatusView, toastError } from '../../components';
import ProjectApi from '../../api/ProjectApi';
import './manage-project.scss';
import ManageUpdates from './components/manage-updates';
import ManagePledges from './components/manage-pledges';
import ProjectForm from './components/project-form';

function ManageProject() {
  const { projId } = useParams();

  const [project, setProject] = useState(null);

  useEffect(() => {
    ProjectApi.get(projId).subscribe(setProject, err => {
      console.error(err);
      toastError('There was an error loading the project, try again shortly');
    });
  }, [projId]);

  if (!project) {
    return (
      <BodyCard>
        <StatusView text="Project Loading..." icon="sync" spin />
      </BodyCard>
    );
  }

  return (
    <BodyCard className="manage-proj">
      <h1>
        Manage Project: <Link to={`/project/${project.id}`}>{project.title}</Link>
      </h1>
      <hr />
      <Tabs>
        <TabList>
          <Tab>
            <span>Details</span>
          </Tab>
          <Tab>
            <span>Updates</span>
          </Tab>
          <Tab>
            <span>Pledges</span>
          </Tab>
        </TabList>

        <TabPanel>
          <ProjectForm />
        </TabPanel>
        <TabPanel>
          <ManageUpdates />
        </TabPanel>
        <TabPanel>
          <ManagePledges />
        </TabPanel>
      </Tabs>
    </BodyCard>
  );
}

export default ManageProject;
