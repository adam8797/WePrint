import React, { Component } from 'react';
import PropTypes from 'prop-types';

import CardGrid from '../card-grid/card-grid';
import SectionTitle from '../section-title/section-title';
import Table from '../table/table';

class ToggleableDisplay extends Component {
  constructor(props) {
    super(props);
    this.state = {
      showGrid: true,
    };
  }

  toggleGrid(showGrid) {
    this.setState({ showGrid });
  }

  render() {
    const { showGrid } = this.state;
    const { title, data, cardType, columns } = this.props;
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
        {showGrid ? (
          <CardGrid cards={data} cardType={cardType} />
        ) : (
          <Table columns={columns} data={data} />
        )}
      </div>
    );
  }
}

ToggleableDisplay.propTypes = {
  title: PropTypes.string.isRequired,
  data: PropTypes.arrayOf(
    PropTypes.objectOf(
      PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object, PropTypes.array])
    )
  ).isRequired,
  cardType: PropTypes.string.isRequired,
  columns: PropTypes.arrayOf(
    PropTypes.objectOf(PropTypes.oneOfType([PropTypes.string, PropTypes.func]))
  ).isRequired,
};

export default ToggleableDisplay;
