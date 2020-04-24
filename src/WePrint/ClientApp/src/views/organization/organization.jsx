import React, { Component } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import ReactMarkdown from 'react-markdown';
import { isEmpty } from 'lodash';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router-dom';

import OrgApi from '../../api/OrganizationApi';
import { BodyCard } from '../../components';
import Button from '../../components/button/button';
import UserApi from '../../api/UserApi';
import Users from './components/users';
import Projects from './components/projects';

import './organization.scss';

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

  componentDidMount() {
    this.fetchOrg();
    UserApi.CurrentUser().subscribe(user => {
      this.setState({ user });
    });
  }

  fetchOrg() {
    const { match } = this.props;
    const { orgId } = match.params;
    OrgApi.get(orgId).subscribe(
      organization => {
        this.setState({ organization, error: false });
        OrgApi.usersFor(orgId)
          .getAll()
          .subscribe(users => {
            this.setState({ users });
          }, console.error);
        OrgApi.projectsFor(orgId)
          .getAll()
          .subscribe(projects => {
            this.setState({ projects });
          }, console.error);
      },
      err => {
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  saveEdit() {
    this.setState({ organization: {}, edit: false });
    this.fetchOrg();
  }

  render() {
    const { error, organization, user, users, projects, edit } = this.state;
    const { match } = this.props;
    const { orgId } = match.params;
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
    if (edit && canEdit) {
      return (
        // <OrganizationEdit organization={organization} onSave={this.saveEdit}></OrganizationEdit>
        <div />
      );
    }
    return (
      <BodyCard>
        <div className="organization">
          <div className="organization__header">
            <img
              className="organization__icon"
              src={OrgApi.getDetailRoute(orgId, 'avatar')}
              alt="Organization Logo"
            />
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
                  <Projects projects={projects && projects.filter(p => !p.closed)} />
                </div>
                <hr />
                <div className="organization__past-projects">
                  <div className="organization__section-title">Past Projects</div>
                  <Projects projects={projects && projects.filter(p => p.closed)} />
                </div>
              </div>
              <div className="organization__users">
                <div className="organization__section-title">The Team</div>
                <Users users={users} />
              </div>
            </div>
          </div>
        </div>
      </BodyCard>
    );
  }
}

Organization.propTypes = {
  match: PropTypes.objectOf(PropTypes.string).isRequired,
};

export default withRouter(Organization);