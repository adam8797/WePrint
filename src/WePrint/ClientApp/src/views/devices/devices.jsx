import React from 'react';
import { BodyCard, Table } from '../../components';

function Devices() {
  const columns = [
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
  const data = [
    {
      name: 'Boring Printer',
      type: 'FDM',
      dimensions: '250 x 210 x 200 mm',
      model: 'Boring model',
    },
    {
      name: 'Fun Printer',
      type: 'SLA',
      dimensions: '180 x 180 x 180 mm',
      model: 'Fancy model',
    },
  ];
  const actions = [
    {
      text: 'Add Device',
      key: 'addDevice',
      action: () => {
        console.log('adding device');
      },
    },
  ];
  return (
    <BodyCard>
      <Table title="My Printers" columns={columns} data={data} actions={actions} />
    </BodyCard>
  );
}

export default Devices;
