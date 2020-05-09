import React, { useEffect, useState } from 'react';
import moment from 'moment';
import { useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { isEmpty } from 'lodash';
import ProjectApi from '../../../api/ProjectApi';
import {
  StatusView,
  toastError,
  WepInput,
  Table,
  WepTextarea,
  Button,
  toastMessage,
  TableUser,
  FormGroup,
} from '../../../components';
import './manage-updates.scss';

function ManageUpdates() {
  const { projId } = useParams();
  const { register, handleSubmit, errors, reset, setValue } = useForm();

  const [updates, setUpdates] = useState(null);
  const [updatingId, setUpdatingId] = useState(null);

  // TODO: careful, this will create errors if the component is unloaded before this is complete
  // because there is no unsubscribe method. Need to fix this but also provide a way to trigger
  // a load on submission of a new update
  function loadUpdates() {
    ProjectApi.updatesFor(projId)
      .getAll()
      .subscribe(setUpdates, err => {
        console.error(err);
        toastError('Error loading updates');
      });
  }

  useEffect(loadUpdates, [projId]);

  const editUpdate = update => {
    setUpdatingId(update.id);
    setValue([{ title: update.title }, { body: update.body }]);
  };

  const stopEditing = () => {
    setUpdatingId(null);
    reset();
  };

  const submitUpdate = form => {
    const { title, body } = form;

    if (updatingId) {
      // ProjectApi.updatesFor(projId)
      //   .add({ title, body })
      //   .subscribe(
      //     newUpdate => {
      //       console.log(newUpdate);
      //       toastMessage('Update Posted Successfully');
      //       loadUpdates();
      //       reset();
      //     },
      //     err => {
      //       console.error(err);
      //       toastError('There was an error submitting your update');
      //     }
      //   );
      toastMessage("Update should have happened, it's just broken");
      stopEditing();
    } else {
      ProjectApi.updatesFor(projId)
        .add({ title, body })
        .subscribe(
          newUpdate => {
            console.log(newUpdate);
            toastMessage('Update Posted Successfully');
            loadUpdates();
            reset();
          },
          err => {
            console.error(err);
            toastError('There was an error submitting your update');
          }
        );
    }
  };

  const formatDate = dateStr => {
    const time = moment(dateStr);
    return `${time.format('MM/DD/YY')} at ${time.format('HH:mm')}`;
  };
  const updatesCols = [
    {
      Header: 'Poster',
      accessor: 'postedBy',
      Cell: data => <TableUser userId={data.cell.value} />,
    },
    {
      Header: 'Posted On',
      accessor: 'timestamp',
      Cell: ({ cell: { value } }) => formatDate(value),
    },
    {
      Header: 'Edited On',
      accessor: 'editTimestamp',
      Cell: ({ cell }) => (cell.value ? formatDate(cell.value) : 'n/a'),
    },
    {
      Header: 'Title',
      accessor: 'title',
    },
    {
      accessor: 'id',
      Cell: data => (
        <Button
          type={Button.Type.PRIMARY}
          icon="pen"
          onClick={() => editUpdate(data.cell.row.original)}
        />
      ),
    },
  ];

  if (!updates) {
    return <StatusView text="Loading Updates" icon="sync" spin />;
  }

  return (
    <div className="manage-updates">
      <form className="manage-updates__new-form" onSubmit={handleSubmit(submitUpdate)}>
        <FormGroup title={updatingId ? 'Editing update' : 'New Update'}>
          <label htmlFor="title">Title*</label>
          <WepInput
            name="title"
            id="title"
            register={register({ required: true })}
            error={!!errors.title}
          />
          <label htmlFor="body">Body*</label>
          <WepTextarea
            name="body"
            id="body"
            register={register({ required: true })}
            error={!!errors.body}
          />
          {updatingId && (
            <Button type={Button.Type.DANGER} onClick={() => stopEditing()}>
              Cancel Editing
            </Button>
          )}
          <Button type={Button.Type.SUCCESS} htmlType="submit" disabled={!isEmpty(errors)}>
            {updatingId ? 'Save' : 'Post Update'}
          </Button>
        </FormGroup>
      </form>
      <div className="manage-updates__past">
        <Table columns={updatesCols} data={updates || []} />
      </div>
    </div>
  );
}

export default ManageUpdates;
