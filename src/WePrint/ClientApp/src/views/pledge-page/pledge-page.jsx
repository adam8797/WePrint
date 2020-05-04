import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter, Link } from 'react-router-dom';
import { BodyCard, StatusView, Button } from '../../components';
import ProjectApi from '../../api/ProjectApi';
import { PledgeStatus } from '../../models/Enums';

const PledgePageStatus = {
  NOT_STARTED: 'NOT_STARTED',
  LOADING: 'LOADING',
  DONE: 'DONE',
  ERROR: 'ERROR',
};

class PledgePage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      pledge: null,
      project: null,
      pageStatus: PledgePageStatus.NOT_STARTED,
    };
  }

  componentDidMount() {
    const { match } = this.props;
    const { projId } = match.params;

    this.setState({ pageStatus: PledgePageStatus.LOADING });

    this.fetchPledge();
    ProjectApi.get(projId).subscribe(
      project => {
        this.setState(prevState => ({
          project,
          pageStatus: prevState.pledge ? PledgePageStatus.DONE : prevState.pageStatus,
        }));
      },
      err => {
        console.error(err);
        this.setState({ pageStatus: PledgePageStatus.ERROR });
      }
    );
  }

  setStatus = newStatus => {
    const { match } = this.props;
    const { projId, pledgeId } = match.params;

    ProjectApi.pledgesFor(projId)
      .setStatus(pledgeId, newStatus)
      .subscribe(
        () => {
          console.log('status updated');
          this.fetchPledge();
        },
        err => console.error(err)
      );
  };

  fetchPledge() {
    const { match } = this.props;
    const { projId, pledgeId } = match.params;

    this.setState({ pageStatus: PledgePageStatus.LOADING });

    ProjectApi.pledgesFor(projId)
      .get(pledgeId)
      .subscribe(
        pledge => {
          this.setState(prevState => ({
            pledge,
            pageStatus: prevState.project ? PledgePageStatus.DONE : prevState.pageStatus,
          }));
        },
        err => {
          console.error(err);
          this.setState({ pageStatus: PledgePageStatus.ERROR });
        }
      );
  }

  render() {
    const { match } = this.props;
    const { projId, pledgeId } = match.params;
    const { pledge, project, pageStatus } = this.state;

    if (pageStatus === PledgePageStatus.ERROR) {
      return (
        <BodyCard>
          <StatusView
            text={`Could not load pledge with id ${pledgeId} fro project with id ${projId}`}
            icon={['far', 'frown']}
          />
        </BodyCard>
      );
    }

    if (pageStatus === PledgePageStatus.NOT_STARTED || pageStatus === PledgePageStatus.LOADING) {
      return (
        <BodyCard>
          <StatusView text="Pledge Loading..." icon="sync" spin />
        </BodyCard>
      );
    }

    return (
      <BodyCard>
        <h1>
          Thanks for pledging to <Link to={`/project/${project.id}/`}>{project.title}</Link>
        </h1>
        <p>units: {pledge.quantity}</p>
        <p>pledged on: {pledge.created}</p>
        <p>est delivery on: {pledge.deliveryDate}</p>
        <p>Current status: {pledge.status}</p>
        <div className="status-buttons">
          <Button
            type={Button.Type.PRIMARY}
            onClick={() => this.setStatus(PledgeStatus.InProgress)}
            disabled={pledge.status !== PledgeStatus.NotStarted}
          >
            Set InProgress
          </Button>
          <Button
            type={Button.Type.PRIMARY}
            onClick={() => this.setStatus(PledgeStatus.Shipped)}
            disabled={pledge.status !== PledgeStatus.InProgress}
          >
            Set Shipped
          </Button>
          <Button
            type={Button.Type.SUCCESS}
            onClick={() => this.setStatus(PledgeStatus.Finished)}
            disabled={pledge.status !== PledgeStatus.Shipped}
          >
            Set Finished
          </Button>
          <Button
            type={Button.Type.DANGER}
            onClick={() => this.setStatus(PledgeStatus.Canceled)}
            disabled={pledge.status === PledgeStatus.Canceled}
          >
            Set Canceled
          </Button>
        </div>
      </BodyCard>
    );
  }
}

PledgePage.propTypes = {
  match: PropTypes.objectOf(
    PropTypes.oneOfType([PropTypes.string, PropTypes.bool, PropTypes.object])
  ).isRequired,
};

export default withRouter(PledgePage);
