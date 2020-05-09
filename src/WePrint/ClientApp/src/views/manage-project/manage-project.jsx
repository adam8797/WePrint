import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Tab, TabList, TabPanel, Tabs } from 'react-tabs';
import { BodyCard, StatusView, toastError } from '../../components';
import ProjectApi from '../../api/ProjectApi';
import './manage-project.scss';
import ManageUpdates from './components/manage-updates';
import ManagePledges from './components/manage-pledges';
import ProjectForm from './components/project-form';
import UserApi from '../../api/UserApi';

function ManageProject() {
  const { projId } = useParams();

  const [user, setUser] = useState(null);
  const [project, setProject] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    ProjectApi.get(projId).subscribe(setProject, err => {
      console.error(err);
      setError(true);
      toastError('There was an error loading the project, try again shortly');
    });
  }, [projId]);

  useEffect(() => {
    const sub = UserApi.CurrentUser().subscribe(u => {
      setUser(u);
    });
    return () => sub.unsubscribe();
  });

  if (error) {
    return (
      <BodyCard>
        <StatusView text="Could not load project" icon={['far', 'frown']} />
      </BodyCard>
    );
  }

  if (!project || !user) {
    return (
      <BodyCard>
        <StatusView text="Project Loading..." icon="sync" spin />
      </BodyCard>
    );
  }

  // can't manage this project
  if (user.organization !== project.organization) {
    // say "could not load" to help prevent searching for valid project hashes
    return (
      <BodyCard>
        <StatusView text="Could not load project" icon={['far', 'frown']} />
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
