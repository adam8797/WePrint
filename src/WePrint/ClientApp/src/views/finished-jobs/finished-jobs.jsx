import React, { Component } from 'react';

import { BodyCard, JobGrid, SectionTitle, Table } from '../../components';

class FinishedJobs extends Component {
  constructor(props) {
    super(props);
    this.state = {
      showGrid: true,
    };

    this.jobs = [
      {
        name: 'DnD Minis',
        jobId: '1234',
        user: 'Emily',
        maker: 'Steve',
        image: 'http://placekitten.com/500/500',
        parts: '4',
        price: '$10',
        printTime: '4h',
        prints: '4',
        source: 'Custom',
        status: 'Completed',
        completedDate: '02/10/2020',
        link: '/job/1234',
      },
      {
        name: 'DnD Minis',
        jobId: '5678',
        user: 'Steve',
        maker: 'Emily',
        parts: '4',
        price: '$0',
        printTime: '4h',
        prints: '4',
        source: 'Thingiverse',
        externalId: '1234567',
        status: 'Completed',
        completedDate: '03/01/2020',
        link: '/job/1234',
      },
      {
        name: 'DnD Minis',
        jobId: '91011',
        user: 'Emily',
        maker: 'Mike',
        image: 'http://placekitten.com/250',
        parts: '4',
        price: '$5',
        printTime: '4h',
        prints: '4',
        source: 'Custom',
        status: 'Cancelled',
        completedDate: '-',
        link: '/job/1234',
      },
      {
        name: 'DnD Minis',
        jobId: '147295',
        user: 'Emily',
        maker: 'luigi',
        image: 'http://placekitten.com/420',
        parts: '4',
        price: '$50',
        printTime: '4h',
        prints: '4',
        source: 'Custom',
        status: 'Shipped',
        completedDate: '-',
        link: '/job/1234',
      },
    ];

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

  toggleGrid(showGrid) {
    this.setState({ showGrid: showGrid });
  }

  render() {
    let data;

    if (this.state.showGrid) {
      data = <JobGrid jobs={this.jobs} />;
    } else {
      data = <Table columns={this.columns} data={this.jobs} />;
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
