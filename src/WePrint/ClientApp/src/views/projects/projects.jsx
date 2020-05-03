import React, { Component } from 'react';

import ProjectApi from '../../api/ProjectApi';
import { ToggleableDisplay, BodyCard, StatusView, CardTypes } from '../../components';

class Projects extends Component {
  constructor(props) {
    super(props);
    this.state = {
      projects: null,
      error: false,
    };

    this.columns = [
      {
        Header: 'Title',
        accessor: 'title',
      },
      {
        Header: 'Status',
        accessor: 'status',
      },
      {
        Header: 'Location',
        accessor: 'location',
      },
      {
        Header: 'Goal',
        accessor: 'goal',
      },
      {
        Header: 'Progress',
        accessor: 'progressDisplay',
      },
    ];
  }

  componentDidMount() {
    ProjectApi.getAll().subscribe(
      projects => {
        const projsWithLinks = projects
          .filter(proj => !proj.deleted)
          .map(proj => {
            return this.formatForTable(proj);
          });
        this.setState({ projects: projsWithLinks });
      },
      err => {
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  formatForTable(project) {
    return {
      ...project,
      link: `/project/${project.id}`,
      status: project.closed ? 'Closed' : 'Open',
      location: `${project.address.city}, ${project.address.state}`,
      progressDisplay: `${Math.round(
        (100 * (project.progress.Finished || 0)) / project.goal
      )} % Completed`,
    };
  }

  render() {
    const { projects, error } = this.state;
    if (error) {
      return (
        <BodyCard>
          <StatusView text="Could not load projects" icon={['far', 'frown']} />
        </BodyCard>
      );
    }
    if (projects === null) {
      return (
        <BodyCard>
          <StatusView text="Loading projects..." icon="sync" spin />
        </BodyCard>
      );
    }

    return (
      <BodyCard>
        <ToggleableDisplay
          title="Projects"
          data={projects}
          cardType={CardTypes.PROJECT}
          columns={this.columns}
        />
      </BodyCard>
    );
  }
}

export default Projects;
