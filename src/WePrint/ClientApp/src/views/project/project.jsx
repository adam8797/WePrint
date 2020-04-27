import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { withRouter } from 'react-router-dom';
import { Tabs, TabList, Tab, TabPanel } from 'react-tabs';
import OrganizationApi from '../../api/OrganizationApi';
import ProjectApi from '../../api/ProjectApi';
import { BodyCard, Button } from '../../components';
import './project.scss';
import UpdatesPanel from './components/updates-panel';

class Project extends Component {
  constructor(props) {
    super(props);
    this.state = {
      // TODO: find a better way to track this state
      error: false,
      orgErr: false,
      pledgesErr: false,
      project: null,
      organization: null,
      pledges: null,
    };
  }

  componentDidMount() {
    this.fetchProject();
  }

  fetchProject() {
    const { match } = this.props;
    const { projId } = match.params;
    ProjectApi.get(projId).subscribe(
      project => {
        this.setState({ project, error: false });
        OrganizationApi.get(project.organization).subscribe(
          organization => {
            this.setState({ organization, orgErr: false });
          },
          err => {
            console.error(err);
            this.setState({ orgErr: true });
          }
        );
      },
      err => {
        console.error(err);
        this.setState({ error: true });
      }
    );
  }

  fetchPledges() {
    const { match } = this.props;
    const { projId } = match.params;
    ProjectApi.pledgesFor(projId)
      .getAll()
      .subscribe(
        pledges => {
          this.setState({ pledges, pledgesErr: false });
        },
        err => {
          console.error(err);
          this.setState({ pledgesErr: true });
        }
      );
  }

  renderPledges(pledges, error) {
    if (error) {
      return 'Error Loading Pledges';
    }
    if (pledges === null) {
      return 'Loading Pledges...';
    }
    if (!pledges.length) {
      return 'No pledges yet';
    }
    return pledges.map(pledge => (
      <div className="pledge" key={pledge.id}>
        <p>
          Pledge for {pledge.quantity} on {pledge.created} by{' '}
          {pledge.anonymous ? 'Anonymous' : pledge.maker}
        </p>
        <p>
          Delivery Estimate: {pledge.deliveryDate} Currently: {pledge.status}
        </p>
      </div>
    ));
  }

  render() {
    const { error, project, organization, orgErr, pledges, pledgesErr } = this.state;
    const { match } = this.props;
    const { projId } = match.params;

    if (error) {
      return (
        <BodyCard>
          <div className="project__error">
            <span className="project__error-text">Could not load project with id {projId}</span>
            <FontAwesomeIcon icon={['far', 'frown']} />
          </div>
        </BodyCard>
      );
    }
    // TODO: better if empty check
    if (!project) {
      return (
        <BodyCard>
          <div className="project__loading">
            <span className="project__loading-text">Project Loading...</span>
            <FontAwesomeIcon icon="sync" spin />
          </div>
        </BodyCard>
      );
    }

    const {
      id,
      title,
      description,
      printingInstructions,
      shippingInstructions,
      attachments,
      address,
      goal,
      openGoal,
      progress,
    } = project;

    return (
      <BodyCard>
        <div className="project">
          <div className="project__header">
            <h1>{title}</h1>
          </div>
          <div className="project__overview">
            <div className="project__thumb">
              <img src={ProjectApi.getThumbnailUrl(id)} alt="Project Thumbnail" />
            </div>
            <div className="project__details">
              <div className="project__progress">
                <p>
                  {progress.Finished || 0}/{goal} {openGoal && '(open goal)'}
                </p>
              </div>
              <div className="project__overview-buttons">
                <Button
                  size={Button.Size.LARGE}
                  type={Button.Type.PRIMARY}
                  className="project__overview-buttons__pledge"
                >
                  Pledge Now!
                </Button>
                <Button size={Button.Size.LARGE} outline>
                  Share
                </Button>
              </div>
              <div className="project__org-info">
                <div className="project__org">
                  {/* TODO: this needs to be cleaned up, there's a chance for both to render */}
                  {orgErr && 'Error loading Org'}
                  {!orgErr && organization ? (
                    <>
                      <img src={OrganizationApi.getAvatarUrl(organization.id)} alt="Org Pic" />
                      <div className="project__org-name">{organization.name}</div>
                    </>
                  ) : (
                    <div className="project__org-name">Organization Loading...</div>
                  )}
                </div>
                <div className="project__org-addr">
                  {address.city}, {address.state}
                </div>
              </div>
            </div>
          </div>
          <Tabs>
            <TabList>
              <Tab>
                <span>Details</span>
              </Tab>
              <Tab>
                <span data-count={project.updates.length}>Updates</span>
              </Tab>
            </TabList>

            <TabPanel>
              <p>{description}</p>
              <h3>Delivery Instructions</h3>
              <p>{shippingInstructions}</p>
              <h3>Printing Instructions</h3>
              <p>{printingInstructions}</p>
              <p>{attachments}</p>
            </TabPanel>
            <TabPanel>
              <UpdatesPanel projId={id} />
            </TabPanel>
          </Tabs>

          <hr />

          <button onClick={() => this.fetchPledges()} type="button">
            Load Pledges
          </button>
          <div className="pledges">{this.renderPledges(pledges, pledgesErr)}</div>
        </div>
      </BodyCard>
    );
  }
}

Project.propTypes = {
  match: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.bool, PropTypes.object])
  ).isRequired,
};

export default withRouter(Project);
