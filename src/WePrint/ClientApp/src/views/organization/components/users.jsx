import React from 'react';
import Jdenticon from 'react-jdenticon';
import PropTypes from 'prop-types';

import UserApi from '../../../api/UserApi';

import './users.scss';

function Users(props) {
  const { users } = props;
  if (!users) {
    return <div>Loading users...</div>;
  }
  if (!users.length) {
    return <div>No users...</div>;
  }

  return (
    <div>
      {users.map(user => (
        <div className="user">
          <div className="user__info">
            {user.avatar ? (
              <img
                className="user__icon"
                src={UserApi.getDetailRoute(user.id, 'avatar')}
                alt="User Avatar"
              />
            ) : (
              <Jdenticon className="user__icon" value={user.username} size="75" />
            )}
            <div className="user__name">
              {user.firstName} {user.lastName}
            </div>
          </div>
          <div className="user__bio">{user.bio || <i>No user bio provided</i>}</div>
        </div>
      ))}
    </div>
  );
}

Users.propTypes = {
  users: PropTypes.arrayOf(
    PropTypes.objectOf(PropTypes.oneOfType([PropTypes.string, PropTypes.number, PropTypes.object]))
  ).isRequired,
};

export default Users;
