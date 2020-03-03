import React, { Component } from 'react';
import { JobApi } from '../../api/JobApi';

import { BodyCard, JobGrid, SectionTitle, Table } from '../../components';

class FinishedJobs extends Component {
  constructor(props) {
    super(props);
    this.state = {
      showGrid: true,
      jobs: [],
    };

    this.columns = [
      {
        Header: 'Name',
        accessor: 'name',
      },
      {
        Header: 'Owner',
        accessor: 'user',
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
      this.setState({ ...this.state, jobs });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  toggleGrid(showGrid) {
    this.setState({ showGrid: showGrid });
  }

  render() {
    let data;

    let jobs = this.state.jobs.filter(job => {
      if (job.status === 6 || job.status === 7 || job.status === 8) {
        job.user = job.customerId.split('ApplicationUsers-')[1];
        job.link = `/job/${job.id}`;
        return true;
      }
      return false;
    });

    if (this.state.showGrid) {
      data = <JobGrid jobs={jobs} />;
    } else {
      data = <Table columns={this.columns} data={jobs} />;
    }

    const actions = [
      {
        key: 'grid',
        icon: 'th',
        action: () => this.toggleGrid(true),
        selected: this.state.showGrid,
      },
      {
        key: 'list',
        icon: 'bars',
        action: () => this.toggleGrid(false),
        selected: !this.state.showGrid,
      },
    ];

    return (
      <BodyCard>
        <SectionTitle title="My Finished Jobs" actions={actions} />
        {data}
      </BodyCard>
    );
  }
}

export default FinishedJobs;
