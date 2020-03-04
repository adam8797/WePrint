import React, { Component } from 'react';
import { Table } from '../../../components';
import { JobApi } from '../../../api/JobApi';
import JobPlaceholder from '../../../assets/images/job.png';
import Moment from 'react-moment';

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
    this.subscription = JobApi.TrackJob(this.props.jobId, 1000).subscribe(job => {
      this.setState({ job });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    if (!Object.keys(this.state.job).length) {
      return <div>Job Loading</div>;
    }
    var timeLeft, bidDeadlineStyle;
    if (this.state.job) {
      bidDeadlineStyle = 'close';
      timeLeft = (
        <Moment fromNow ago>
          {this.state.job.bidClose}
        </Moment>
      );
    }
    const poster = this.state.job.customerId.split('ApplicationUsers-')[1];
    const status = this.state.job.status === 1 ? 'OPEN' : 'CLOSED';
    return (
      <div className="job">
        <div className="job__header">
          <span className="job__title">{this.state.job.name}</span>
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
          <img
            className="job__image"
            src={this.state.job.image || JobPlaceholder}
            alt="Job Preview"
          />
          <div className="job__detail">
            <span>
              <span className="job__section">Bid Deadline:</span>
              {this.state.job.bidClose}
              {timeLeft && (
                <span className={`job__deadline--${bidDeadlineStyle}`}>
                  &nbsp; ({timeLeft} left)
                </span>
              )}
            </span>
            <span>
              <span className="job__section">Printer Type:</span>
              {this.state.job.printerType}
            </span>
            <span>
              <span className="job__section">Material:</span>
              {this.state.job.materialColor}&nbsp;
              {this.state.job.materialType}
            </span>
            <span>
              <span className="job__section">Destination:</span>
              {this.state.job.address.zipCode}
            </span>
            <span>
              <span className="job__section">Description:</span>
              <br />
              <span className="job__description">{this.state.job.description}</span>
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

export default Job;
