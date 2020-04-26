import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import PrinterApi from '../../api/PrinterApi';
import { Button, BodyCard, Table } from '../../components';

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
        Header: 'Volume',
        accessor: 'dimensions',
      },
      {
        Header: 'Min Layer Height',
        accessor: 'layerMin',
        Cell: ({ cell: { value } }) => `${value} mm`,
      },
      {
        id: 'edit',
        accessor: 'id',
        // eslint-disable-next-line react/prop-types
        Cell: ({ cell: { value: printerId } }) => (
          <Button
            icon="pen"
            size={Button.Size.SMALL}
            type={Button.Type.PRIMARY}
            onClick={() => this.navToEditDevice(printerId)}
          />
        ),
      },
    ];

    this.actions = [
      {
        text: 'Add Device',
        key: 'addDevice',
        action: this.navToAddDevice,
      },
    ];
  }

  componentDidMount() {
    this.subscription = PrinterApi.trackAll(1000).subscribe(printers => {
      this.setState({ printers });
    }, console.error);
  }

  componentWillUnmount() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  navToAddDevice = () => {
    this.setState({ toAddDevice: true });
  };

  navToEditDevice = printerId => {
    this.setState({ toEditDevice: printerId });
  };

  render() {
    const { printers, toAddDevice, toEditDevice } = this.state;

    if (toAddDevice) {
      return <Redirect to="/edit-device" push />;
    }
    if (toEditDevice) {
      return <Redirect to={`/edit-device/${toEditDevice}`} push />;
    }

    const printersWDimensions = printers.map(p => {
      return { ...p, dimensions: `${p.xMax} x ${p.yMax} x ${p.zMax} mm` };
    });
    return (
      <BodyCard>
        <Table
          title="My Printers"
          columns={this.columns}
          data={printersWDimensions}
          actions={this.actions}
        />
      </BodyCard>
    );
  }
}

export default Devices;
