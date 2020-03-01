import React from 'react';
import { BodyCard, Table, SectionTitle } from '../../components';

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
      <SectionTitle title="My Printers" actions={actions} />
      <Table columns={columns} data={data} />
    </BodyCard>
  );
}

export default Devices;
