import React from 'react';
import { useHistory } from 'react-router-dom';
import PropTypes from 'prop-types';
import classNames from 'classnames';

import ProjectApi from '../../../api/ProjectApi';
import ProjectModel from '../../../models/ProjectModel';
import './org-projects.scss';

function OrgProjects(props) {
  const { projects } = props;
  const history = useHistory();

  if (!projects) {
    return <div>Loading projects...</div>;
  }
  if (!projects.length) {
    return <div>No Projects</div>;
  }

  return (
    <>
      {projects.map(project => (
        <div
          className={classNames('org-project', { 'org-project--active': !project.closed })}
          onClick={() => history.push(`/project/${project.id}`)}
          key={project.id}
        >
          <div className="org-project__info">
            <div className="org-project__icon-container">
              <img
                className="org-project__icon"
                src={ProjectApi.getDetailRoute(project.id, 'thumbnail')}
                alt="Project Thumbnail"
              />
            </div>
            <div className="org-project__icon-container">
              {
                // we need to display the progress cube for projects!
                // <img className="organization__project-icon" src={project.??} />
              }
            </div>
          </div>
          <hr />
          <div className="org-project__info">
            <div className="org-project__detail">
              <span>{project.title}</span>
              <span className="org-project__sub-info">
                {project.address.city}, {project.address.state}
              </span>
            </div>
            <div className="org-project__detail">
              <span>{project.closed ? 'Closed' : 'Open'}</span>
              <span className="org-project__sub-info">
                {Math.round((100 * (project.progress.Finished || 0)) / project.goal)}% Completed
              </span>
            </div>
          </div>
        </div>
      ))}
    </>
  );
}

OrgProjects.propTypes = {
  projects: PropTypes.arrayOf(PropTypes.shape(ProjectModel)),
};

export default OrgProjects;
