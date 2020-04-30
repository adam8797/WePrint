import React, { Component } from 'react';
import JobApi from '../../api/JobApi';

import { BodyCard, ToggleableDisplay, CardTypes } from '../../components';
import { finishedJobStatuses } from '../../models/Enums';

class FinishedJobs extends Component {
  constructor(props) {
    super(props);
    this.state = {
      jobs: [],
    };

    this.columns = [
      {
        Header: 'Name',
        accessor: 'name',
      },
      {
        Header: 'Owner',
        accessor: 'customerUserName',
      },
      {
        Header: 'Maker',
        accessor: 'maker',
      },
      {
        Header: 'Price',
        accessor: 'price',
      },
      {
        Header: 'Total Prints',
        accessor: 'prints',
      },
      {
        Header: 'Status',
        accessor: 'status',
      },
      {
        Header: 'Final Print Time',
        accessor: 'printTime',
      },
      {
        Header: 'Finished On',
        accessor: 'completedDate',
      },
    ];
  }

  componentDidMount() {
    this.subscription = JobApi.TrackMyJobs(1000).subscribe(jobs => {
      this.setState({ jobs: Array.isArray(jobs) ? jobs : [] });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    const { jobs } = this.state;

    const displayJobs = jobs
      .filter(job => {
        return finishedJobStatuses.includes(job.status);
      })
      .map(job => ({
        ...job,
        link: `/job/${job.id}`,
      }));

    return (
      <BodyCard>
        <ToggleableDisplay
          title="My Finished Jobs"
          data={displayJobs}
          cardType={CardTypes.JOB}
          columns={this.columns}
        />
      </BodyCard>
    );
  }
}

export default FinishedJobs;
