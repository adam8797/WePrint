import React, { Component } from 'react';
import PropTypes from 'prop-types';

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

  toggleGrid(showGrid) {
    this.setState({ showGrid });
  }

  render() {
    const { showGrid } = this.state;
    const { title, data } = this.props;
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
        <SectionTitle title={title} actions={actions} />
        {showGrid ? <JobGrid jobs={data} /> : <Table columns={this.columns} data={data} />}
      </div>
    );
  }
}

ToggleableDisplay.propTypes = {
  title: PropTypes.string.isRequired,
  data: PropTypes.arrayOf(
    PropTypes.objectOf(PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object]))
  ).isRequired,
};

export default ToggleableDisplay;
