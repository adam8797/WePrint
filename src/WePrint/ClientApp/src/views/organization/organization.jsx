import React, { Component } from 'react';
import ReactMarkdown from 'react-markdown';
import classNames from 'classnames';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Jdenticon from 'react-jdenticon';
import { withRouter } from 'react-router';

import OrgApi from '../../api/OrganizationApi';
import { isEmpty } from 'lodash';
import { BodyCard } from '../../components';
import Button from '../../components/button/button';
import './organization.scss';
import UserApi from '../../api/UserApi';

class Organization extends Component {
  constructor(props) {
    super(props);
    this.state = {
      organization: {},
      error: false,
      users: null,
      projects: null,
      user: null,
      edit: false,
    };
  }

  fetchOrg() {
    const { orgId } = this.props.match.params;
    OrgApi.get(orgId).subscribe(
      organization => {
        this.setState({ organization, error: false });
        OrgApi.users.getAll(orgId).subscribe(users => {
          this.setState({ users });
        }, console.error);
        OrgApi.projects.getAll(orgId).subscribe(projects => {
          this.setState({ projects });
        }, console.error);
      },
      err => {
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  componentDidMount() {
    this.fetchOrg();
    UserApi.CurrentUser().subscribe(user => {
      this.setState({ user });
    });
  }

  renderProjects(projects, isActive) {
    if (!projects) {
      return <div>Loading projects...</div>;
    }
    if (!projects.length) {
      return <div>No Projects</div>;
    }
    const projectClasses = classNames('organization__item', 'organization__project', {
      'organization__project--active': isActive,
    });
    const { history } = this.props;
    console.log(projects);
    return (
      <div>
        {projects.map(project => (
          <div className={projectClasses} onClick={() => history.push(`/project/${project.id}`)}>
            <div className="organization__project-info">
              <div className="organization__project-icon-container">
                <img className="organization__project-icon" src={project.thumbnail} />
              </div>
              <div className="organization__project-icon-container">
                {
                  // we need to display the progress cube for projects!
                  //<img className="organization__project-icon" src={project.??} />
                }
              </div>
            </div>
            <hr />
            <div className="organization__project-info">
              <div className="organization__project-detail">
                <span>{project.title}</span>
                <span className="organization__sub-info">
                  {project.address.city}, {project.address.state}
                </span>
              </div>
              <div className="organization__project-detail">
                <span>{project.closed ? 'Closed' : 'Open'}</span>
                <span className="organization__sub-info">
                  {Math.round((100 * (project.progress.Finished || 0)) / project.goal)}% Completed
                </span>
              </div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  renderPastProjects() {
    const { projects } = this.state;
    return this.renderProjects(projects && projects.filter(p => p.closed));
  }

  renderActiveProjects() {
    const { projects } = this.state;
    return this.renderProjects(projects && projects.filter(p => !p.closed), true);
  }

  renderUsers() {
    const { users } = this.state;
    if (!users) {
      return <div>Loading users...</div>;
    }
    if (!users.length) {
      return <div>No users...</div>;
    }

    return (
      <div>
        {users.map(user => (
          <div className="organization__item organization__user">
            <div className="organization__user-info">
              {user.avatar ? (
                <img className="organization__icon" src={user.avatar} />
              ) : (
                <Jdenticon className="organization__icon" value={user.username} size="75" />
              )}
              <div className="organization__user-name">
                {user.firstName} {user.lastName}
              </div>
            </div>
            <div className="organization__user-bio">{user.bio || <i>No user bio provided</i>}</div>
          </div>
        ))}
      </div>
    );
  }

  saveEdit() {
    this.setState({ organization: {}, edit: false });
    this.fetchOrg();
  }
  render() {
    const { error, organization, user, edit } = this.state;
    const { orgId } = this.props.match.params;
    if (error) {
      return (
        <BodyCard>
          <div className="organization__error">
            <span className="organization__error-text">
              Could not load organization with id {orgId}
            </span>
            <FontAwesomeIcon icon={['far', 'frown']} />
          </div>
        </BodyCard>
      );
    }
    if (isEmpty(organization)) {
      return (
        <BodyCard>
          <div className="organization__loading">
            <span className="organization__loading-text">Organization Loading...</span>
            <FontAwesomeIcon icon="sync" spin />
          </div>
        </BodyCard>
      );
    }
    const canEdit = user && organization.users.includes(user.id);
    // to be implemented when we implement editing orgs
    // if (edit && canEdit) {
    //   return (
    //     <OrganizationEdit organization={organization} onSave={this.saveEdit}></OrganizationEdit>
    //   );
    // }
    return (
      <BodyCard>
        <div className="organization">
          <div className="organization__header">
            <img className="organization__icon" src={organization.logo} alt="Organization Logo" />
            <div className="organization__title">
              <div className="organization__name">{organization.name}</div>
              <div className="organization__location">
                {organization.address.city}, {organization.address.state}
              </div>
            </div>
            {canEdit && (
              <div className="organization__actions">
                <Button
                  size={Button.Size.SMALL}
                  type={Button.Type.PRIMARY}
                  onClick={() => this.setState({ edit: true })}
                  icon="pen"
                >
                  Edit
                </Button>
              </div>
            )}
          </div>
          <hr />
          <div>
            <div className="organization__description">
              <div className="organization__section-title">Description</div>
              <div className="organization__description-content">
                <ReactMarkdown
                  source={organization.description || '*No organization description provided*'}
                />
              </div>
            </div>
            <hr />
            <div className="organization__references">
              <div className="organization__projects">
                <div className="organization__active-projects">
                  <div className="organization__section-title">Active Projects</div>
                  {this.renderActiveProjects()}
                </div>
                <hr />
                <div className="organization__past-projects">
                  <div className="organization__section-title">Past Projects</div>
                  {this.renderPastProjects()}
                </div>
              </div>
              <div className="organization__users">
                <div className="organization__section-title">The Team</div>
                {this.renderUsers()}
              </div>
            </div>
          </div>
        </div>
      </BodyCard>
    );
  }
}

export default withRouter(Organization);
