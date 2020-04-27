import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import ProjectApi from '../../../api/ProjectApi';
import UserApi from '../../../api/UserApi';
import UpdateModel from '../../../models/ProjectUpdateModel';
import './updates-panel.scss';

function Update({ update }) {
  const { title, body, timestamp, postedBy } = update;

  const [poster, setPoster] = useState('');

  useEffect(() => {
    UserApi.GetUser(postedBy).subscribe(user => setPoster(user), console.error);
  }, [postedBy]);

  let name = 'unknown';
  if (poster) {
    if (!poster.firstName || !poster.lastName) {
      name = poster.firstName + poster.lastName;
    }
    name = poster.username;
  }

  return (
    <div className="update" key={update.id}>
      <div className="cube" />
      <div className="header">
        <div className="time">{moment(timestamp).fromNow()}</div>
        <div className="subject">
          <div className="title">{title}</div>
          <div className="author">by {name}</div>
        </div>
      </div>
      <div className="body">{body}</div>
    </div>
  );
}

Update.propTypes = {
  update: PropTypes.shape(UpdateModel),
};

function UpdatesPanel({ projId }) {
  const states = {
    LOADING: 'LOADING',
    ERROR: 'ERROR',
    DONE: 'DONE',
  };

  const [updates, setUpdates] = useState([]);
  const [dataState, setDataState] = useState(states.LOADING);

  useEffect(() => {
    ProjectApi.updatesFor(projId)
      .getAll()
      .subscribe(
        updateData => {
          setUpdates(updateData);
          setDataState(states.DONE);
        },
        err => {
          console.error(err);
          setDataState(states.ERROR);
        }
      );
  }, [projId, states.DONE, states.ERROR]);

  switch (dataState) {
    case states.ERROR:
      return <div className="updates-panel">Error Loading Updates</div>;
    case states.LOADING:
      return <div className="updates-panel">Loading Updates...</div>;
    case states.DONE:
      if (!updates.length) {
        return <div className="updates-panel">There are no updates yet</div>;
      }
      return (
        <div className="updates-panel">
          {updates.map(update => (
            <Update update={update} key={update.id} />
          ))}
        </div>
      );
    default:
      break;
  }
}

export default UpdatesPanel;
