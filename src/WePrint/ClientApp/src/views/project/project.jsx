import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter, Link } from 'react-router-dom';
import { Tabs, TabList, Tab, TabPanel } from 'react-tabs';
import moment from 'moment';
import { Progress } from 'reactstrap';

import OrganizationApi from '../../api/OrganizationApi';
import ProjectApi from '../../api/ProjectApi';
import UserApi from '../../api/UserApi';
import { BodyCard, Button, Table, StatusView, toastError, TableUser } from '../../components';
import UpdatesPanel from './components/updates-panel';
import CreatePledge from './components/create-pledge';
import MyPledges from './components/my-pledges';
import { PledgeStatus } from '../../models/Enums';
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
      myPledges: null,
      canPledge: false,
      pledgeModalOpen: false,
      loggedIn: true,
    };
  }

  componentDidMount() {
    this.fetchProject();
    this.fetchUser();
  }

  openModal = () => {
    const { canPledge } = this.state;
    if (canPledge) {
      this.setState({ pledgeModalOpen: true });
    } else {
      toastError(
        'You are not able to pledge right now. Make sure you are logged in and do not have an active pledge'
      );
    }
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

  fetchUser() {
    UserApi.CurrentUser().subscribe(
      u => {
        this.setState({ loggedIn: !!u, user: u });
      },
      err => {
        if (err.response.status === 401) {
          this.setState({ loggedIn: false });
          return;
        }
        console.error(err);
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
    UserApi.getPledges(projId).subscribe(
      myPledges => this.setState({ myPledges, canPledge: !this.checkActivePledge(myPledges) }),
      err => console.error(err)
    );
  }

  checkActivePledge(myPledges) {
    // return true if any pledges have status 'NotStarted' or 'InProgress'
    return (
      myPledges.filter(
        pledge =>
          pledge.status === PledgeStatus.NotStarted || pledge.status === PledgeStatus.InProgress
      ).length > 0
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
    const {
      user,
      error,
      project,
      organization,
      orgErr,
      pledges,
      myPledges,
      canPledge,
      pledgeModalOpen,
      loggedIn,
    } = this.state;
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
          return anonymous ? 'Anonymous' : <TableUser user={cell.value} />;
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

    const progFinished = Math.round((progress.Finished ? progress.Finished / goal : 0) * 100);
    const progInProg = Math.round(
      (progress.InProgress || progress.Shipped
        ? ((progress.InProgress || 0) + (progress.Shipped || 0)) / goal
        : 0) * 100
    );

    const canManage = user && project && user.organization === project.organization;

    return (
      <BodyCard centered>
        <div className="project">
          <div className="project__header">
            <h1>{title}</h1>
            {canManage && <Link to={`/manage-project/${projId}`}>Manage Project</Link>}
          </div>
          <div className="project__overview">
            <div className="project__thumb">
              <img src={ProjectApi.getThumbnailUrl(id)} alt="Project Thumbnail" />
            </div>
            <div className="project__details">
              <div className="project__progress">
                <div className="project__progress-bar">
                  <span>0</span>
                  <Progress multi>
                    <Progress bar color="success" value={progFinished}>
                      {progress.Finished}
                    </Progress>
                    <Progress bar color="primary" value={progInProg}>
                      {(progress.InProgress || 0) + (progress.Shipped || 0)}
                    </Progress>
                  </Progress>
                  <span>{project.goal}</span>
                </div>
                {openGoal && ' (open goal)'}
              </div>
              <div className="project__overview-buttons">
                <Button
                  size={Button.Size.LARGE}
                  type={Button.Type.PRIMARY}
                  className="project__overview-buttons__pledge"
                  onClick={this.openModal}
                  disabled={!canPledge}
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
            <MyPledges
              projId={id}
              pledges={myPledges}
              openPledgeModal={this.openModal}
              canPledge={canPledge}
              loggedIn={loggedIn}
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
