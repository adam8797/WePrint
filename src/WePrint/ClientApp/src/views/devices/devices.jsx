import React, { Component } from 'react';
import { PrinterApi } from '../../api/PrinterApi';
import { BodyCard, Table } from '../../components';

class Devices extends Component {
  constructor(props) {
    super(props);

    this.state = {
      printers: [],
    };

    this.columns = [
      {
        Header: 'Name',
        accessor: 'name',
      },
      {
        Header: 'Type',
        accessor: 'type',
      },
      {
        Header: 'Dimensions',
        accessor: 'dimensions',
      },
      {
        Header: 'Model',
        accessor: 'model',
      },
    ];

    this.actions = [
      {
        text: 'Add Device',
        key: 'addDevice',
        action: () => {
          console.log('adding device');
        },
      },
    ];
  }

  componentDidMount() {
    this.subscription = PrinterApi.TrackMyPrinters(1000).subscribe(printers => {
      this.setState({ ...this.state, printers });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  render() {
    this.state.printers.map(p => (p.dimensions = `${p.XMax} x ${p.YMax} x ${p.ZMax} mm`));
    return (
      <BodyCard>
        <Table
          title="My Printers"
          columns={this.columns}
          data={this.state.printers}
          actions={this.actions}
        />
      </BodyCard>
    );
  }
}

export default Devices;
