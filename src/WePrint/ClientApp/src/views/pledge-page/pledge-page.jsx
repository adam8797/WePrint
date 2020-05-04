import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withRouter, Link } from 'react-router-dom';
import moment from 'moment';
import { BodyCard, StatusView, Button, SectionTitle } from '../../components';
import ProjectApi from '../../api/ProjectApi';
import { PledgeStatus } from '../../models/Enums';
import './pledge-page.scss';

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
      <BodyCard className="pledge-page">
        <div className="pledge-page__header">
          <h1>
            Thanks for pledging to <Link to={`/project/${project.id}/`}>{project.title}</Link>
          </h1>
          <Button
            className="pledge-page__cancel"
            type={Button.Type.DANGER}
            size={Button.Size.SMALL}
            onClick={() => this.setStatus(PledgeStatus.Canceled)}
            disabled={
              pledge.status === PledgeStatus.Canceled ||
              pledge.status === PledgeStatus.Shipped ||
              pledge.status === PledgeStatus.Finished
            }
          >
            Cancel Pledge
          </Button>
        </div>
        <div className="pledge-page__info">
          <div className="pledge-page__stats">
            <div className="pledge-page__stat">Units: {pledge.quantity}</div>
            <div className="pledge-page__stat">
              Pledged on: {moment(pledge.created).format('MMM, Do YYYY')}
            </div>
            <div className="pledge-page__stat">
              Estimated Delivery: {moment(pledge.deliveryDate).format('MMM, Do YYYY')}
            </div>
            <div className="pledge-page__stat">Current status: {pledge.status}</div>
          </div>
          <div className="pledge-page__status">
            <Button
              type={Button.Type.PRIMARY}
              onClick={() => this.setStatus(PledgeStatus.InProgress)}
              disabled={pledge.status !== PledgeStatus.NotStarted}
            >
              Started
            </Button>
            <Button
              type={Button.Type.PRIMARY}
              onClick={() => this.setStatus(PledgeStatus.Shipped)}
              disabled={pledge.status !== PledgeStatus.InProgress}
            >
              Shipped
            </Button>
            <Button
              type={Button.Type.SUCCESS}
              onClick={() => this.setStatus(PledgeStatus.Finished)}
              disabled={pledge.status !== PledgeStatus.Shipped}
            >
              Done
            </Button>
          </div>
        </div>
        <div className="pledge-page__project-row">
          <div className="pledge-page__project-info">
            <SectionTitle title="Printing Instructions" />
            <p>{project.printingInstructions}</p>
          </div>
          <div className="pledge-page__project-info">
            <SectionTitle title="Files" />
            <p>{project.attachments ? project.attachments.map(a => a) : 'No Files'}</p>
          </div>
        </div>
        <div className="pledge-page__project-row">
          <div className="pledge-page__project-info">
            <SectionTitle title="Delivery Instructions" />
            <p>{project.shippingInstructions}</p>
          </div>
          <div className="pledge-page__project-info">
            <SectionTitle title="Delivery Identifier" />
            <div className="pledge-page__qr-section">
              <img
                src={`/api/qrcodes/generate/${pledge.id}`}
                alt="Pledge delivery QR code"
                className="pledge-page__qr-code"
              />
              <p>
                Include this code with your delivery to allow the organization to mark your pledge
                as complete.
                {/* Click to enlarge for in person delivery or click below to print it out. */}
              </p>
              {/* <Button className="pledge-page__qr-print">Print</Button> */}
            </div>
          </div>
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
