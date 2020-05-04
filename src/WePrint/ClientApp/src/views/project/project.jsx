import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router-dom';
import { Tabs, TabList, Tab, TabPanel } from 'react-tabs';
import moment from 'moment';

import OrganizationApi from '../../api/OrganizationApi';
import ProjectApi from '../../api/ProjectApi';
import { BodyCard, Button, Table, StatusView } from '../../components';
import UpdatesPanel from './components/updates-panel';
import CreatePledge from './components/create-pledge';

import './project.scss';

class Project extends Component {
  constructor(props) {
    super(props);
    this.state = {
      // TODO: find a better way to track this state
      error: false,
      orgErr: false,
      project: null,
      organization: null,
      pledges: null,
      pledgeModalOpen: false,
    };
  }

  componentDidMount() {
    this.fetchProject();
  }

  openModal = () => {
    this.setState({ pledgeModalOpen: true });
  };

  closeModal = () => {
    this.setState({ pledgeModalOpen: false });
  };

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
        this.fetchPledges();
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
          this.setState({ pledges });
        },
        err => {
          console.error(err);
          // TODO: do something with this
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
    const { error, project, organization, orgErr, pledges, pledgeModalOpen } = this.state;
    const { match } = this.props;
    const { projId } = match.params;

    if (error) {
      return (
        <BodyCard>
          <StatusView text={`Could not load project with id ${projId}`} icon={['far', 'frown']} />
        </BodyCard>
      );
    }
    // TODO: better if empty check
    if (!project) {
      return (
        <BodyCard>
          <StatusView text="Project Loading..." icon="sync" spin />
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

    const pledgeCols = [
      {
        Header: 'Date Pledged',
        accessor: 'created',
        Cell: ({ cell: { value } }) => moment(value).format('MM/DD/YYYY'),
      },
      {
        Header: 'Name',
        accessor: 'maker',
        Cell: ({ cell }) => {
          const { anonymous } = cell.row.original;
          return anonymous ? 'Anonymous' : cell.value;
        },
      },
      {
        Header: 'Amount Pledged',
        accessor: 'quantity',
      },
      {
        Header: 'Estimated Delivery',
        accessor: 'deliveryDate',
        Cell: ({ cell: { value } }) => moment(value).format('MM/DD/YYYY'),
      },
      {
        Header: 'Status',
        accessor: 'status',
      },
    ];

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
                  onClick={this.openModal}
                >
                  Pledge Now!
                </Button>
                {/* <Button
                  size={Button.Size.LARGE}
                  outline
                  className="project__overview-buttons__share"
                >
                  Share
                </Button> */}
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
          <div className="project__body">
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
            {/* TODO: add outputs for error and loading states */}
            <Table
              title="Donators"
              columns={pledgeCols}
              data={pledges || []}
              emptyMessage="There are no pledges yet, add yours now!"
            />
          </div>
        </div>
        <CreatePledge projId={id} modalOpen={pledgeModalOpen} closeModal={this.closeModal} />
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
