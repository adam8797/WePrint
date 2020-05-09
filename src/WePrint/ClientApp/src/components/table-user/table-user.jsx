import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import UserApi from '../../api/UserApi';
import './table-user.scss';

function TableUser({ userId }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const sub = UserApi.GetUser(userId).subscribe(setUser, err => {
      console.error('Error loading table user', userId, err);
    });
    return () => sub.unsubscribe();
  }, [userId]);

  return (
    <div className="table-user">
      <img
        src={UserApi.getAvatarUrl(userId)}
        alt={user ? `Avatar for ${user.username}` : 'Pledger Avatar'}
      />{' '}
      <span>{user && user.username}</span>
    </div>
  );
}

TableUser.propTypes = {
  userId: PropTypes.string.isRequired,
};

export default TableUser;
