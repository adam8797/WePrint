import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import moment from 'moment';
import ProjectApi from '../../../api/ProjectApi';
import {
  toastError,
  StatusView,
  Table,
  TableUser,
  Button,
  toastMessage,
} from '../../../components';
import { PledgeStatus } from '../../../models/Enums';

function ManagePledges() {
  const { projId } = useParams();

  const [pledges, setPledges] = useState(null);
  const [error, setError] = useState();

  function loadPledges() {
    ProjectApi.pledgesFor(projId)
      .getAll()
      .subscribe(setPledges, err => {
        console.error(err);
        setError(true);
        toastError('Error loading pledges');
      });
  }

  useEffect(loadPledges, [projId]);

  const rejectPledge = id => {
    const decision = window.confirm('Are you sure you want to reject this pledge?');
    if (decision) {
      ProjectApi.pledgesFor(projId)
        .setStatus(id, PledgeStatus.Canceled)
        .subscribe(
          () => {
            toastMessage('Pledge has been rejected');
            loadPledges();
          },
          err => {
            console.error(err);
            toastError('Failed to reject pledge, try again later');
          }
        );
    }
  };

  if (error) {
    return <StatusView text="Could not load pledges" icon={['far', 'frown']} />;
  }

  if (!pledges) {
    return <StatusView text="Loading Pledges" icon="sync" spin />;
  }

  const pledgeCols = [
    {
      Header: 'Date Pledged',
      accessor: 'created',
      Cell: ({ cell: { value } }) => moment(value).format('MM/DD/YYYY'),
    },
    {
      Header: 'Name',
      accessor: 'maker',
      Cell: data => {
        const { anonymous } = data.cell.row.original;
        return anonymous ? 'Anonymous' : <TableUser userId={data.cell.value} />;
      },
    },
    {
      Header: 'Amount Pledged',
      accessor: 'quantity',
    },
    {
      Header: 'Estimated Delivery',
      accessor: 'deliveryDate',
      Cell: ({ cell: { value } }) => moment(value).format('MM/DD/YYYY'),
    },
    {
      Header: 'Status',
      accessor: 'status',
    },
    {
      accessor: 'id',
      Cell: data => {
        const pledge = data.cell.row.original;
        return (
          <Button
            type={Button.Type.DANGER}
            icon="trash"
            onClick={() => rejectPledge(pledge.id)}
            disabled={pledge.status === PledgeStatus.Canceled}
          />
        );
      },
    },
  ];

  return (
    <div className="manage-pledges">
      <Table columns={pledgeCols} data={pledges || []} />
    </div>
  );
}

export default ManagePledges;
