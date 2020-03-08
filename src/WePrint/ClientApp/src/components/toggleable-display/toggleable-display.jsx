import React, { Component } from 'react';

import JobGrid from '../job-grid/job-grid';
import SectionTitle from '../section-title/section-title';
import Table from '../table/table';

class ToggleableDisplay extends Component {
  constructor(props) {
    super(props);
    this.state = {
      showGrid: true,
    };

    this.columns = [
      {
        Header: 'Name',
        accessor: 'name',
      },
      {
        Header: 'Owner',
        accessor: 'userName',
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
    this.setState({ showGrid });
  }

  render() {
    const { showGrid } = this.state;
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
      <div>
        <SectionTitle title={this.props.title} actions={actions} />
        {showGrid ? (
          <JobGrid jobs={this.props.data} />
        ) : (
          <Table columns={this.columns} data={this.props.data} />
        )}
      </div>
    );
  }
}

export default ToggleableDisplay;
