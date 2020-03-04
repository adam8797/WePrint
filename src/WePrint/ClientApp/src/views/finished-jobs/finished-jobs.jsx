import React, { Component } from 'react';
import JobApi from '../../api/JobApi';

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
      this.setState({ jobs });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  toggleGrid(showGrid) {
    this.setState({ showGrid });
  }

  render() {
    const { jobs, showGrid } = this.state;
    let data;

    const displayJobs = jobs
      .filter(job => {
        return job.status === 6 || job.status === 7 || job.status === 8;
      })
      .map(job => ({
        ...job,
        user: job.customerId.replace('ApplicationUsers-', ''),
        link: `/job/${job.id}`,
      }));

    if (showGrid) {
      data = <JobGrid jobs={displayJobs} />;
    } else {
      data = <Table columns={this.columns} data={displayJobs} />;
    }

    const actions = [
      {
        key: 'grid',
        icon: 'th',
        action: () => this.toggleGrid(true),
        selected: showGrid,
      },
      {
        key: 'list',
        icon: 'bars',
        action: () => this.toggleGrid(false),
        selected: !showGrid,
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
