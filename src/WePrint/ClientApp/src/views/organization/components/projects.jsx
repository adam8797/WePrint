import React from 'react';
import { useHistory } from 'react-router-dom';
import PropTypes from 'prop-types';

import ProjectApi from '../../../api/ProjectApi';
import './projects.scss';

function Projects(props) {
  const { projects } = props;
  const history = useHistory();

  if (!projects) {
    return <div>Loading projects...</div>;
  }
  if (!projects.length) {
    return <div>No Projects</div>;
  }

  return (
    <div>
      {projects.map(project => (
        <div
          className={`project project${project.closed ? '--inactive' : '--active'}`}
          onClick={() => history.push(`/project/${project.id}`)}
          onKeyDown={() => history.push(`/project/${project.id}`)}
        >
          <div className="project__info">
            <div className="project__icon-container">
              <img
                className="project__icon"
                src={ProjectApi.getDetailRoute(project.id, 'thumbnail')}
                alt="Project Thumbnail"
              />
            </div>
            <div className="project__icon-container">
              {
                // we need to display the progress cube for projects!
                // <img className="organization__project-icon" src={project.??} />
              }
            </div>
          </div>
          <hr />
          <div className="project__info">
            <div className="project__detail">
              <span>{project.title}</span>
              <span className="project__sub-info">
                {project.address.city}, {project.address.state}
              </span>
            </div>
            <div className="project__detail">
              <span>{project.closed ? 'Closed' : 'Open'}</span>
              <span className="project__sub-info">
                {Math.round((100 * (project.progress.Finished || 0)) / project.goal)}% Completed
              </span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}

Projects.propTypes = {
  projects: PropTypes.arrayOf(
    PropTypes.objectOf(PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object]))
  ).isRequired,
};

export default Projects;
