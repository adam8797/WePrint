import React from 'react';
import PropTypes from 'prop-types';
import UserApi from '../../api/UserApi';
import './table-user.scss';
import UserModel from '../../models/UserModel';

function TableUser({ user }) {
  if (!user) {
    return (
      <div className="table-user">
        <span>Unknown User</span>
      </div>
    );
  }

  let name = user.username;
  if (user.firstName) {
    name = user.firstName;
    if (user.lastName) {
      name += ` ${user.lastName}`;
    }
  }

  return (
    <div className="table-user">
      <img
        src={UserApi.getAvatarUrl(user.id)}
        alt={user ? `Avatar for ${name}` : 'Pledger Avatar'}
      />
      <span> {name}</span>
    </div>
  );
}

TableUser.propTypes = {
  user: PropTypes.shape(UserModel),
};

export default TableUser;
