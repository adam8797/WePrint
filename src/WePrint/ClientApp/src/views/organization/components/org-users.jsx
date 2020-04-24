import React from 'react';
import PropTypes from 'prop-types';

import UserApi from '../../../api/UserApi';
import UserModel from '../../../models/UserModel';
import './org-users.scss';

function OrgUsers(props) {
  const { users } = props;
  if (!users) {
    return <div>Loading users...</div>;
  }
  if (!users.length) {
    return <div>No users...</div>;
  }

  return (
    <>
      {users.map(user => (
        <div className="org-user" key={user.id}>
          <div className="org-user__info">
            <img
              className="org-user__icon"
              src={UserApi.getDetailRoute(user.id, 'avatar')}
              alt="User's Avatar"
            />
            <div className="org-user__name">
              {user.firstName} {user.lastName}
            </div>
          </div>
          <div className="org-user__bio">{user.bio || <i>No user bio provided</i>}</div>
        </div>
      ))}
    </>
  );
}

OrgUsers.propTypes = {
  users: PropTypes.arrayOf(PropTypes.shape(UserModel)),
};

export default OrgUsers;
