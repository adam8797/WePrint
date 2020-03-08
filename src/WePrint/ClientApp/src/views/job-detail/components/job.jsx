import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Moment from 'react-moment';
import { Table } from '../../../components';
import JobApi from '../../../api/JobApi';
import JobPlaceholder from '../../../assets/images/job.png';

import './job.scss';

class Job extends Component {
  constructor(props) {
    super(props);
    this.state = {
      job: {},
    };
    this.filesTableCols = [
      {
        Header: 'Part Name',
        accessor: 'part',
      },
      {
        Header: 'File Size',
        accessor: 'size',
      },
      {
        Header: 'Dimensions',
        accessor: 'dimensions',
      },
      {
        Header: 'Filament Usage',
        accessor: 'filamentUsage',
      },
      {
        Header: 'Time Estimate',
        accessor: 'timeEst',
      },
    ];
    this.bidTableCols = [
      {
        Header: 'Bid',
        accessor: 'bid',
      },
      {
        Header: 'User',
        accessor: 'user',
      },
      {
        Header: 'Printer',
        accessor: 'printer',
      },
      {
        Header: 'Material',
        accessor: 'material',
      },
      {
        Header: 'Color',
        accessor: 'color',
      },
      {
        Header: 'Estimate', // time accepted to time it's put in the box
        accessor: 'estimate',
      },
      {
        Header: 'User Rating',
        accessor: 'rating',
      },
    ];
  }

  componentDidMount() {
    const { jobId } = this.props;
    this.subscription = JobApi.TrackJob(jobId, 1000).subscribe(job => {
      this.setState({ job });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    const { job } = this.state;
    if (!Object.keys(job).length) {
      return <div>Job Loading</div>;
    }
    let timeLeft;
    let bidDeadlineStyle;
    if (job) {
      bidDeadlineStyle = 'close';
      timeLeft = (
        <Moment fromNow ago>
          {job.bidClose}
        </Moment>
      );
    }
    const poster = job.customerId.split('ApplicationUsers-')[1];
    const status = job.status === 1 ? 'OPEN' : 'CLOSED';
    return (
      <div className="job">
        <div className="job__header">
          <span className="job__title">{job.name}</span>
          <span className="job__subtitle">
            <span>Posted by: {poster}</span>
            <h4>
              Bidding:&nbsp;
              <span className={`job__status--${status.toLowerCase()}`}>{status}</span>
            </h4>
          </span>
          <hr />
        </div>
        <div className="job__body">
          <img className="job__image" src={job.image || JobPlaceholder} alt="Job Preview" />
          <div className="job__detail">
            <span>
              <span className="job__section">Bid Deadline:</span>
              {job.bidClose}
              {timeLeft && (
                <span className={`job__deadline--${bidDeadlineStyle}`}>
                  &nbsp; ({timeLeft} left)
                </span>
              )}
            </span>
            <span>
              <span className="job__section">Printer Type:</span>
              {job.printerType}
            </span>
            <span>
              <span className="job__section">Material:</span>
              {job.materialColor}&nbsp;
              {job.materialType}
            </span>
            <span>
              <span className="job__section">Destination:</span>
              {job.address}
            </span>
            <span>
              <span className="job__section">Description:</span>
              <br />
              <span className="job__description">{job.description}</span>
            </span>
          </div>
        </div>
        <div className="job__bids">
          <Table title="Bids" columns={this.bidTableCols} data={[]} />
        </div>
        <div className="job__files">
          <Table title="Files" columns={this.filesTableCols} data={[]} />
        </div>
      </div>
    );
  }
}

Job.propTypes = {
  jobId: PropTypes.string.isRequired,
};

export default Job;
